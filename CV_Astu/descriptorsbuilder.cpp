#include "descriptorsbuilder.h"
#include <QTextStream>
#include "imagehelper.h"
#include <cassert>
DescriptorsBuilder::DescriptorsBuilder()
{

}


vector<double> DescriptorsBuilder::CalculateHistogram
(const DoubleMat& source, const InterestingPoint &point,
 const double gridSize, const int gridLineCellsCount, const int bins, const double sigmaBase, const double alpha) const
{

    double cellSize = gridSize/gridLineCellsCount;

    const auto sxKernel = KernelBuilder::BuildSobelX();
    const auto syKernel = KernelBuilder::BuildSobelY();
    vector<double> result;
    const int cellsCount = gridLineCellsCount*gridLineCellsCount;//(gridSize/gridStep) * (gridSize/gridStep);
    const double binAngle = 2 * M_PI / bins;
    const int gridHalfSize = round(gridSize)/2;
    result.resize( cellsCount * bins, 0 );

    const double cosAlpha = cos(alpha);
    const double sinAlpha = sin(alpha);

    for(int dx = -gridHalfSize; dx < gridHalfSize; dx++)
    {
        for(int dy = -gridHalfSize; dy < gridHalfSize; dy++)
        {
            const double xDrv = source.ConvolveCell(sxKernel, BorderType::Replicate, dx + point.x, dy + point.y);
            const double yDrv = source.ConvolveCell(syKernel, BorderType::Replicate, dx + point.x, dy + point.y);

            const double gradValue = sqrt(xDrv * xDrv + yDrv * yDrv);
            const double gradAngle = fmod(atan2(yDrv, xDrv) + 2*M_PI, 2*M_PI);

            int rdx = floor(cosAlpha * dx + sinAlpha * dy);
            int rdy = floor(-sinAlpha * dx + cosAlpha * dy);
            if(rdx < -gridHalfSize || rdy < - gridHalfSize
                ||  rdx >= gridHalfSize || rdy >= gridHalfSize)
                continue;

            int cellX = (rdx + gridHalfSize)/cellSize;
            int cellY = (rdy + gridHalfSize)/cellSize;
            int cell = cellY * gridLineCellsCount + cellX;



            assert(cell >=0 && cell < cellsCount);

            int leftBin, rightBin;
            double cleft, cright;

            double phi = gradAngle - alpha;
            if(phi < 0) phi += 2*M_PI;

            rightBin = floor(phi/binAngle + 0.5);
            leftBin = rightBin - 1;
            cright = abs(rightBin + 0.5 - phi / binAngle);
            cleft = 1 - cright;

            if(rightBin >= bins) rightBin = 0;
            if(leftBin < 0) leftBin = bins - 1;

            assert(cleft >=0 && cright >=0);

            double w = exp(-(dx*dx + dy*dy) / (2 * sigmaBase*sigmaBase))
                    / (2 * M_PI * sigmaBase * sigmaBase);
            double L = w * gradValue;

            if(cellsCount == 1)
            {
                result.at(bins * cell + leftBin)
                    += L * cright;
                result.at(bins * cell + rightBin)
                    += L * cleft;
            }
            else
            {
                const int xDir = rdx > (cellX+0.5)*cellSize - gridHalfSize ? 1: -1;
                const int yDir = rdy > (cellY+0.5)*cellSize - gridHalfSize ? 1: -1;

                for(int xstep = 0; xstep < 2; xstep++)
                {
                    int currentCellX = cellX + xstep*xDir;
                    if(currentCellX < 0 || currentCellX >= gridLineCellsCount) continue;
                    double cx = (currentCellX+0.5)*cellSize-gridHalfSize;
                    double wx = 1 - abs(rdx-cx)/cellSize;
                    for(int ystep = 0; ystep < 2; ystep++)
                    {
                        int currentCellY = cellY + ystep*yDir;

                        if(currentCellY < 0 || currentCellY >= gridLineCellsCount) continue;
                        double cy = (currentCellY+0.5)*cellSize-gridHalfSize;
                        double wy = 1 - abs(rdy-cy)/cellSize;
                        int currentCell = currentCellY * gridLineCellsCount + currentCellX;


                        result.at(bins * currentCell + leftBin)
                            += L * cright * wx * wy;
                        result.at(bins * currentCell + rightBin)
                            += L * cleft * wx * wy;
                    }
                }
            }


        }
    }

    return result;
}

Descriptor DescriptorsBuilder::CalculateHistogramDescriptor
(const Pyramid& pyramid, const InterestingPoint &point, const double alpha) const
{
    double sigmaBase = 2.0;
    Descriptor result;
    double scale = point.sigmaLocal/pyramid.SigmaStart();
    const auto targetImagePosition = pyramid.GetOctaveAndLayer(point.sigmaGlobal);
    const auto& source = pyramid.GetImageAt(targetImagePosition.first, targetImagePosition.second);



    result.targetPoint.x = point.x;
    result.targetPoint.y = point.y;
    result.targetPoint.alpha = alpha;
    result.targetPoint.sigmaLocal = point.sigmaLocal;
    result.targetPoint.sigmaGlobal = point.sigmaGlobal;
    result.targetPoint.octave = point.octave;
    result.localDescription = CalculateHistogram(source, point, GRID_SIZE * scale, GRID_SIZE/GRID_STEP,
                                                 BINS_COUNT, sigmaBase * scale, alpha);

    double norm = sqrt(accumulate(result.localDescription.begin(), result.localDescription.end(), 0.0));
    transform(result.localDescription.begin(), result.localDescription.end(),
              result.localDescription.begin(), [norm] (double element)->double {return element/norm;});
    return result;
}

double DescriptorsBuilder::CalculateNorm(const vector<double>& histogram) const
{
    double sum = 0;
    sum = accumulate(histogram.begin(), histogram.end(), 0);

    return sqrt(sum);
}

vector<double> DescriptorsBuilder::DescriptorOrientations
(const vector<double> &orientationHistogram) const
{
    vector<double> result;
    vector<pair<int,double>> validPeaks;
    double maxValue = *max_element(orientationHistogram.begin(), orientationHistogram.end());
    for(int i=0;i<orientationHistogram.size();i++)
    {
        if(orientationHistogram[i] < 0.8*maxValue) continue;
        int left = (i-1+orientationHistogram.size())%orientationHistogram.size();
        int right = (i+1)%orientationHistogram.size();
        if(orientationHistogram[i] > orientationHistogram[left]
                && orientationHistogram[i] > orientationHistogram[right])
        {
            validPeaks.emplace_back(i, orientationHistogram[i]);
        }
    }
    sort(validPeaks.begin(), validPeaks.end(), [=](auto& a, auto& b)
        {
            return a.second > b.second;
        });
    result.emplace_back(ORIENTATION_ANGLE_STEP * (validPeaks[0].first + 0.5 + PeakShift(orientationHistogram, validPeaks[0].first)));

    if(validPeaks.size() > 1)
    {
        result.emplace_back(ORIENTATION_ANGLE_STEP * (validPeaks[1].first + 0.5 + PeakShift(orientationHistogram, validPeaks[1].first)));
    }
    /*
    int first, second;

    first = 0;
    firstValue = orientationHistogram.at(first);

    if(orientationHistogram.at(0) > orientationHistogram.at(1))
    {
        first = 0; second = 1;
    }
    else
    {
        first = 1; second = 0;
    }
    firstValue = orientationHistogram.at(first);
    secondValue = orientationHistogram.at(second);
    for(int i = 2; i<orientationHistogram.size(); i++)
    {
        if(orientationHistogram[i] > firstValue)
        {
            secondValue = firstValue;
            second=first;
            firstValue = orientationHistogram[i];
            first = i;
            continue;
        }
        if(orientationHistogram[i] > secondValue)
        {
            secondValue = orientationHistogram[i];
            second = i;
        }
    }

    result.emplace_back(ORIENTATION_ANGLE_STEP * (first + 0.5 + PeakShift(orientationHistogram, first)));

    if(secondValue >= firstValue * 0.8 && abs(first-second) > 1 && abs(first-second) < ORIENTATION_BINS_COUNT - 1)
    {
        result.emplace_back(ORIENTATION_ANGLE_STEP * (second + 0.5 + PeakShift(orientationHistogram, second)));
    }
    */
    return result;
}

double DescriptorsBuilder::PeakShift(const vector<double> histogram, const int peakBinIndex) const
{
    //https://www.dsprelated.com/freebooks/sasp/Quadratic_Interpolation_Spectral_Peaks.html
    const int left = (peakBinIndex - 1 + histogram.size())%histogram.size();
    const int right = (peakBinIndex + 1)%histogram.size();
    assert(left >=0 && left < histogram.size());
    assert(right >=0 && right < histogram.size());
    const double alpha = histogram[left];
    const double betta = histogram[peakBinIndex];
    const double gamma = histogram[right];
    const double peakCorrection = 0.5*(alpha-gamma)/(alpha - 2*betta + gamma);
    assert(abs(peakCorrection)<=0.5);
    return peakCorrection;
}

vector<Descriptor> DescriptorsBuilder::CalculateHistogramDesctiptors
(const Pyramid& pyramid, const vector<InterestingPoint> points) const
{
    vector<Descriptor> descriptors;

    for (auto &point : points) {
        const auto targetImagePosition = pyramid.GetOctaveAndLayer(point.sigmaGlobal);
        const auto& source = pyramid.GetImageAt(targetImagePosition.first, targetImagePosition.second);

        double sigma = 1.5;
        double scale = point.sigmaLocal/pyramid.SigmaStart();
        const auto orientationHistogram = CalculateHistogram(source, point,
                                                 GRID_SIZE * scale, 1, ORIENTATION_BINS_COUNT,
                                                 sigma * scale);

        const auto orientations = DescriptorOrientations(orientationHistogram);
        for(double angle : orientations)
        {
            descriptors.emplace_back(CalculateHistogramDescriptor
                                     (pyramid, point, angle));

        }
    }
    return descriptors;
}

vector<pair<InterestingPoint,InterestingPoint>> DescriptorsBuilder::FindMatches
(const vector<Descriptor> &first, const vector<Descriptor> &second)
{
    vector<pair<InterestingPoint,InterestingPoint>> result;
    DoubleMat distances(first.size(), second.size());
    for(int i =0; i<first.size(); i++)
    {
        for(int j =0; j<second.size(); j++)
        {
            distances.set(CalcDistance(first[i], second[j]),
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
            pair<InterestingPoint,InterestingPoint> match;

            //match.first.x = first[i].targetPoint.x;
            //match.first.y = first[i].targetPoint.y;

            //match.second.x = second[firstBest].targetPoint.x;
            //match.second.y = second[firstBest].targetPoint.y;
            match.first = first[i].targetPoint;
            match.second = second[firstBest].targetPoint;

            double rotation = fmod(2*M_PI + first[i].targetPoint.alpha - second[firstBest].targetPoint.alpha, 2*M_PI);
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
