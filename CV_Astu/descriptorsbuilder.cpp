#include "descriptorsbuilder.h"
#include <QTextStream>
#include "imagehelper.h"
#include <cassert>
DescriptorsBuilder::DescriptorsBuilder()
{

}

DoubleMat DescriptorsBuilder::CalculateGradients(const DoubleMat &xDrv, const DoubleMat &yDrv) const
{
    BorderType border = BorderType::Replicate;


    DoubleMat gradients(xDrv.getWidth(), xDrv.getHeight());
    for (int i = 0; i < xDrv.getWidth(); i++)
    {
        for (int j = 0; j < xDrv.getHeight(); j++)
        {
            double value = xDrv.get(i, j, border) * xDrv.get(i, j, border)
                + yDrv.get(i, j, border) * yDrv.get(i, j, border);
            gradients.set(sqrt(value), i, j);
        }
    }

    return gradients;
}

DoubleMat DescriptorsBuilder::CalculateGradientAngles
(const DoubleMat &xDrv, const DoubleMat &yDrv) const
{
    BorderType border = BorderType::Replicate;


    DoubleMat angles(xDrv.getWidth(), xDrv.getHeight());
    for (int i = 0; i < xDrv.getWidth(); i++)
    {
        for (int j = 0; j < xDrv.getHeight(); j++)
        {
            double value = atan2(yDrv.get(i,j,border), xDrv.get(i,j,border));
            if(value < 0) value = 2 * M_PI + value;

            angles.set(value, i, j);
        }
    }
    return angles;
}

vector<double> DescriptorsBuilder::CalculateHistogram
(DoubleMat &gradients, DoubleMat &angles, const InterestingPoint &point,
 const int gridSize, const int gridStep, const int bins, const double sigma, const double alpha) const
{
    vector<double> result;
    const int cellsCount = (gridSize/gridStep) * (gridSize/gridStep);
    const double binAngle = 2 * M_PI / bins;
    const int gridHalfSize = gridSize/2;
    result.resize( cellsCount * bins, 0 );

    const double cosAlpha = cos(alpha);
    const double sinAlpha = sin(alpha);

    for(int dx = -gridHalfSize; dx < gridHalfSize; dx++)
    {
        for(int dy = -gridHalfSize; dy < gridHalfSize; dy++)
        {
            int rdx = floor(cosAlpha * dx + sinAlpha * dy);
            int rdy = floor(-sinAlpha * dx + cosAlpha * dy);
            if(rdx < -gridHalfSize || rdy < - gridHalfSize
                ||  rdx >= gridHalfSize || rdy >= gridHalfSize)
                continue;

            int cellX = (rdx + gridHalfSize)/gridStep;
            int cellY = (rdy + gridHalfSize)/gridStep;
            int cell = cellY * (gridSize/gridStep) + cellX;

            assert(cell >=0 && cell < cellsCount);

            int left, right;
            double cleft, cright;
            double phi = angles.get(dx + point.x, dy + point.y) - alpha;
            if(phi < 0) phi += 2*M_PI;
            int k = phi / binAngle ;

            if(phi > (k + 0.5) * binAngle)
            {
                left = k;
                right = k + 1;
                cleft = abs(phi - (k + 0.5) * binAngle)/binAngle;
                cright = 1 - cleft;
            }
            else
            {
                left = k - 1;
                right = k;
                cright = abs(phi - (k + 0.5) * binAngle)/binAngle;
                cleft = 1 - cright;
            }
            if(right >= bins) right = 0;
            if(left < 0) left = bins - 1;

            assert(cleft >=0 && cright >=0);

            double w = exp(-(dx*dx + dy*dy) / (2 * sigma*sigma))
                    / (2 * M_PI * sigma * sigma);
            double L = w * gradients.get(dx + point.x, dy + point.y);


            result.at(bins * cell + left)
                    += L * cright;
            result.at(bins * cell + right)
                    += L * cleft;

        }
    }
    double norm = CalculateNorm(result);

    for(int i=0; i<result.size(); i++)
    {
        result.at(i) = result.at(i)/norm;
    }
    return result;
}

Descriptor DescriptorsBuilder::CalculateHistogramDescriptor
(DoubleMat& gradients, DoubleMat& angles, const InterestingPoint &point, const double alpha) const
{
    double sigma = 2.0;
    Descriptor result;

    result.targetPoint.x = point.x;
    result.targetPoint.y = point.y;
    result.targetPoint.alpha = alpha;
    result.localDescription = CalculateHistogram(gradients, angles, point, GRID_SIZE, GRID_STEP,
                                                 BINS_COUNT, sigma, alpha);
    return result;
}

double DescriptorsBuilder::CalculateNorm(const vector<double>& histogram) const
{
    double sum = 0;
    for(double element: histogram)
    {
        sum += element*element;
    }
    return sqrt(sum);
}


vector<Descriptor> DescriptorsBuilder::CalculateHistogramDesctiptors
(const DoubleMat &source, const vector<InterestingPoint> points) const
{
    const auto xDrv = source.Convolve(KernelBuilder::BuildSobelX(), BorderType::Replicate);
    const auto yDrv = source.Convolve(KernelBuilder::BuildSobelY(), BorderType::Replicate);
    auto gradientValues = CalculateGradients(xDrv, yDrv);
    auto angleValues = CalculateGradientAngles(xDrv, yDrv);
    vector<Descriptor> descriptors;

    for (auto &point : points) {
        double sigma = 1.5;
        vector<double> hist = CalculateHistogram(gradientValues, angleValues, point,
                                                 GRID_SIZE, GRID_SIZE, ORIENTATION_BINS_COUNT,
                                                 sigma);
        /*
        hist.resize(ORIENTATION_BINS_COUNT, 0);

        const int xLeft = point.x - GRID_HALFSIZE;
        const int xRight = point.x + GRID_HALFSIZE + GRID_SIZE % 2;
        const int yTop = point.y - GRID_HALFSIZE;
        const int yBottom = point.y + GRID_HALFSIZE + GRID_SIZE % 2;
        for(int x = xLeft; x < xRight; x++)
        {
            for(int y = yTop; y < yBottom; y++)
            {
                int left, right;
                double cleft, cright;
                double phi = angleValues.get(x,y);

                int k = phi / ORIENTATION_ANGLE_STEP;

                if(phi > k * ORIENTATION_ANGLE_STEP + 0.5 * ORIENTATION_ANGLE_STEP)
                {
                    left = k;
                    right = k + 1;
                    cleft = abs(phi - (k + 0.5) * ORIENTATION_ANGLE_STEP)/ORIENTATION_ANGLE_STEP;
                    cright = 1 - cleft;
                }
                else
                {
                    left = k - 1;
                    right = k;
                    cright = abs(phi - (k + 0.5) * ORIENTATION_ANGLE_STEP)/ORIENTATION_ANGLE_STEP;
                    cleft = 1 - cright;
                }
                if(right >= ORIENTATION_BINS_COUNT) right = 0;
                if(left < 0) left = ORIENTATION_BINS_COUNT - 1;

                assert(cleft >=0 && cright >=0);

                int dx = point.x - x;
                int dy = point.y - y;
                double w = exp(-(dx*dx + dy*dy) / (2 * sigma*sigma))
                        / (2 * M_PI * sigma * sigma);
                double L = w * gradientValues.get(x, y);
                hist.at(left) += L * cright;
                hist.at(right) += L * cleft;

            }
        }
        */
        int first, second;
        double firstValue, secondValue;
        if(hist.at(0) > hist.at(1))
        {
            first = 0; second = 1;
        }
        else
        {
            first = 1; second = 0;
        }
        firstValue = hist.at(first);
        secondValue = hist.at(second);
        for(int i = 2; i<hist.size(); i++)
        {
            if(hist.at(i) > firstValue)
            {
                secondValue = firstValue;
                second=first;
                firstValue = hist.at(i);
                first = i;
                continue;
            }
            if(hist.at(i) > secondValue)
            {
                secondValue = hist.at(i);
                second = i;
            }
        }

        if(secondValue < firstValue * 0.8)
        {           
            descriptors.emplace_back(CalculateHistogramDescriptor
                                     (gradientValues, angleValues, point, ORIENTATION_ANGLE_STEP * (first + 0.5)));
        }
        else
        {           
            descriptors.emplace_back(CalculateHistogramDescriptor
                                     (gradientValues, angleValues, point, ORIENTATION_ANGLE_STEP * (first + 0.5)));

            descriptors.emplace_back(CalculateHistogramDescriptor
                                     (gradientValues, angleValues, point, ORIENTATION_ANGLE_STEP * (second + 0.5)));
        }
    }
    return descriptors;
}

vector<pair<Point,Point>> DescriptorsBuilder::FindMatches
(const vector<Descriptor> &first, const vector<Descriptor> &second)
{
    vector<pair<Point,Point>> result;
    DoubleMat distances(first.size(), second.size());
    for(int i =0; i<first.size(); i++)
    {
        for(int j =0; j<second.size(); j++)
        {
            distances.set(CalcDistance(first.at(i), second.at(j)),
                          i,j);
        }
    }

    for(int i = 0; i < first.size(); i++)
    {
        int firstBest, secondBest;
        double firstDistance, secondDistance;
        if(distances.get(i, 0) < distances.get(i,1))
        {
            firstBest = 0; secondBest = 1;
            firstDistance = distances.get(i,0);
            secondDistance = distances.get(i,1);
        }
        else
        {
            firstBest = 1; secondBest = 0;
            firstDistance = distances.get(i,1);
            secondDistance = distances.get(i,0);
        }
        for(int j = 2; j < second.size(); j++)
        {
            if(distances.get(i,j) < firstDistance)
            {
                secondBest = firstBest;
                secondDistance = firstDistance;
                firstBest = j;
                firstDistance = distances.get(i,j);
                continue;
            }
            if(distances.get(i,j) < secondDistance)
            {
                secondBest = j;
                secondDistance = distances.get(i,j);
            }
        }


        if(firstDistance/secondDistance < 0.8)
        {
            pair<Point,Point> match;

            match.first.x = first.at(i).targetPoint.x;
            match.first.y = first.at(i).targetPoint.y;

            match.second.x = second.at(firstBest).targetPoint.x;
            match.second.y = second.at(firstBest).targetPoint.y;

            double rotation = first.at(i).targetPoint.alpha - second.at(firstBest).targetPoint.alpha;
            result.emplace_back(match);
            QString debugInf = QString::number(i) +" and " +QString::number(firstBest)
                    + " values: " + QString::number(match.first.x) +":" + QString::number(match.first.y)
                    + " and " + QString::number(match.second.x) +":" + QString::number(match.second.y)
                    + " L = " + QString::number(firstDistance) + " rot=" + QString::number(rotation);
            qDebug(debugInf.toStdString().c_str());
        }
    }
    return result;
}
double DescriptorsBuilder::CalcDistance(const Descriptor &left,const Descriptor &right)
{
    double distance = 0;
    for(int i=0; i<left.localDescription.size(); i++)
    {
        distance += (left.localDescription.at(i) - right.localDescription.at(i))
                * (left.localDescription.at(i) - right.localDescription.at(i));
    }
    return sqrt(distance);
}
