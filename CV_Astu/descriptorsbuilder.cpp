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

Descriptor DescriptorsBuilder::CalculateSimpleDescriptor
(DoubleMat &gradients, const InterestingPoint point) const
{
    Descriptor result;
    result.targetPoint = point;
    //for(int valueIndex = 0; valueIndex < GRID_CELLS_COUNT; valueIndex ++)
    //{
    //    result.localDescription.emplace_back(0);
    //}
    result.localDescription.resize( GRID_CELLS_COUNT, 0 );

    const int xLeft = point.x - GRID_HALFSIZE;
    const int xRight = point.x + GRID_HALFSIZE + GRID_SIZE % 2;
    const int yTop = point.y - GRID_HALFSIZE;
    const int yBottom = point.y + GRID_HALFSIZE + GRID_SIZE % 2;

    //CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\gradsinside.jpg", CVImage(gradients));

    for(int x = xLeft; x < xRight; x++)
    {
        for(int y = yTop; y < yBottom; y++)
        {
            int cellX = (x - point.x + GRID_HALFSIZE)/GRID_STEP;
            int cellY = (y - point.y + GRID_HALFSIZE)/GRID_STEP;
            int cell = cellY * (GRID_SIZE/GRID_STEP) + cellX;

            double value = gradients.get(x,y,BorderType::Replicate);
            result.localDescription.at(cell) += value/(GRID_STEP*GRID_STEP);
        }
    }
    return result;
}

Descriptor DescriptorsBuilder::CalculateHistogramDescriptor
(DoubleMat& gradients, DoubleMat& angles, const InterestingPoint point) const
{
    double sigma = 2.0;
    const int startX = point.x - GRID_HALFSIZE;
    const int startY = point.y - GRID_HALFSIZE;

    Descriptor result;
    result.targetPoint = point;
    result.localDescription.resize( GRID_CELLS_COUNT * G_ANGLES_COUNT, 0 );
    for(int cell = 0; cell < GRID_CELLS_COUNT; cell++)
    {
        const int cellStartX = startX + GRID_STEP * (cell % (GRID_SIZE / GRID_STEP));
        const int cellStartY = startY + GRID_STEP * (cell / (GRID_SIZE / GRID_STEP));
        for(int x = cellStartX; x < cellStartX + GRID_STEP; x++)
        {
            for(int y = cellStartY; y < cellStartY + GRID_STEP; y++)
            {
                int left, right;
                double cleft, cright;
                double phi = angles.get(x,y);

                int k = phi / G_ANGLE;

                if(phi > k * G_ANGLE + 0.5 * G_ANGLE)
                {
                    left = k;
                    right = k + 1;
                    cleft = abs(phi - (k + 0.5) * G_ANGLE)/G_ANGLE;
                    cright = 1 - cleft;
                }
                else
                {
                    left = k - 1;
                    right = k;
                    cright = abs(phi - (k + 0.5) * G_ANGLE)/G_ANGLE;
                    cleft = 1 - cright;
                }
                if(right >= G_ANGLES_COUNT) right = 0;
                if(left < 0) left = G_ANGLES_COUNT - 1;

                assert(cleft >=0 && cright >=0);


                int dx = point.x - x;
                int dy = point.y - y;
                
                double w = exp(-(dx*dx + dy*dy) / (2 * sigma*sigma))
                        / (2 * M_PI * sigma * sigma);
                double L = w * gradients.get(x, y);


                result.localDescription.at(G_ANGLES_COUNT * cell + left)
                        += L * cright;
                result.localDescription.at(G_ANGLES_COUNT * cell + right)
                        += L * cleft;
            }
        }
    }
    //norm
    double norm = CalculateNorm(result);
    for(int i=0; i<result.localDescription.size(); i++)
    {
        result.localDescription.at(i) = result.localDescription.at(i)/norm;
    }
    return result;
}

double DescriptorsBuilder::CalculateNorm(const Descriptor &descriptor) const
{
    double sum = 0;
    for(double element: descriptor.localDescription)
    {
        sum += element*element;
    }
    return sqrt(sum);
}

vector<Descriptor> DescriptorsBuilder::CalculateSimpleDescriptors
(const DoubleMat &source, const vector<InterestingPoint> points) const
{
    const auto xDrv = source.Convolve(KernelBuilder::BuildSobelX(), BorderType::Replicate);
    const auto yDrv = source.Convolve(KernelBuilder::BuildSobelY(), BorderType::Replicate);
    auto gradientValues = CalculateGradients(xDrv, yDrv);

    vector<Descriptor> descriptors;

    for (auto &point : points) {
       descriptors.emplace_back(CalculateSimpleDescriptor(gradientValues, point));
    }
    return descriptors;
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
       descriptors.emplace_back(CalculateHistogramDescriptor(gradientValues, angleValues, point));
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
        double bestDistanceForFirst = INFINITY;
        int bestMatchForFirst = -1;
        for(int j = 0; j < second.size(); j++)
        {
            if(distances.get(i,j) < bestDistanceForFirst)
            {
                bestDistanceForFirst = distances.get(i,j);
                bestMatchForFirst = j;
            }

        }
        double bestDistanceForNearest = INFINITY;
        int bestMatchForNearest = -1;
        for(int check = 0; check < first.size(); check++)
        {
            if(distances.get(check, bestMatchForFirst) < bestDistanceForNearest)
            {
                bestDistanceForNearest = distances.get(check, bestMatchForFirst);
                bestMatchForNearest = check;
            }
        }
        if(bestMatchForNearest == i && bestDistanceForNearest < 0.001 * first.at(0).localDescription.size())
        {
            //Descriptor secondDescriptor = second.at(bestDistanceForFirst);
            //Descriptor firstDescriptor = first.at(i);
            pair<Point,Point> match;

            match.first.x = first.at(i).targetPoint.x;
            match.first.y= first.at(i).targetPoint.y;

            match.second.x = second.at(bestMatchForFirst).targetPoint.x;
            match.second.y = second.at(bestMatchForFirst).targetPoint.y;

            result.emplace_back(match);
            QString debugInf = QString::number(i) +" and " +QString::number(bestMatchForFirst)
                    + " coords: " + QString::number(match.first.x) +":" + QString::number(match.first.y)
                    + " and " + QString::number(match.second.x) +":" + QString::number(match.second.y)
                    + " L = " + QString::number(bestDistanceForFirst);
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
