#include "interestingpointsdetector.h"

InterestingPointsDetector::InterestingPointsDetector(const DetectionMethod method)

{
    switch (method) {
    case DetectionMethod::Moravec:
        _diffCalc =[&](const DoubleMat& src, const int ws, const BorderType bt)
            {return CalculateMoravecMap(src, ws, bt);};
        break;
    case DetectionMethod::Harris:
        _diffCalc =[&](const DoubleMat& src, const int ws, const BorderType bt)
            {return CalculateHarrisMap(src, ws, bt);};
        break;
    default:
        break;
    }
}

InterestingPointsDetector::~InterestingPointsDetector()
{

}
DoubleMat InterestingPointsDetector::CalculateHarrisMap
(const DoubleMat& source, const int windowHalfSize, const BorderType borderType) const
{
    const auto sobelX = source.Convolve(KernelBuilder::BuildSobelX(), borderType);
    const auto sobelY = source.Convolve(KernelBuilder::BuildSobelY(), borderType);
    double sigma = 1.0*windowHalfSize/3;
    const auto weights = KernelBuilder::BuildGauss(sigma);

    DoubleMat result(source.getWidth(), source.getHeight());
    for(int x = 0; x < source.getWidth(); x++)
    {
        for(int y = 0; y < source.getHeight(); y++)
        {
            double a=0, b=0, c=0;
            for(int u = -windowHalfSize; u<= windowHalfSize; u++)
            {
                for(int v = -windowHalfSize; v <= windowHalfSize; v++)
                {
                    double Ix = sobelX.get(x+u, y+v, borderType);
                    double Iy = sobelY.get(x+u, y+v, borderType);
                    double w = weights.get(u+windowHalfSize, v+windowHalfSize);
                    a += w * Ix*Ix;
                    b += w * Ix*Iy;
                    c += w * Iy*Iy;
                }
            }
            double d = sqrt((a-c)*(a-c) + 4*b*b);
            double l1, l2;
            l1 = abs((a+c+d)/2);
            l2 = abs((a+c-d)/2);
            result.set(min(l1,l2), x, y);
        }
    }
    return result;
}

double InterestingPointsDetector::CalculateHarrisValue(const DoubleMat &source,
const int windowHalfSize, const BorderType borderType, const int x, const int y) const
{
    double sigma = 1.0*windowHalfSize/3;
    const auto weights = KernelBuilder::BuildGauss(sigma);

    const auto sxKernel = KernelBuilder::BuildSobelX();
    const auto syKernel = KernelBuilder::BuildSobelY();
    double a=0, b=0, c=0;
    for(int u = -windowHalfSize; u<= windowHalfSize; u++)
    {
        for(int v = -windowHalfSize; v <= windowHalfSize; v++)
        {
            double Ix = source.ConvolveCell(sxKernel, borderType, x+u, y+v);
            //sobelX.get(x+u, y+v, borderType);
            double Iy = source.ConvolveCell(syKernel, borderType, x+u, y+v);
            //sobelY.get(x+u, y+v, borderType);
            double w = weights.get(u+windowHalfSize, v+windowHalfSize);
            a += w * Ix*Ix;
            b += w * Ix*Iy;
            c += w * Iy*Iy;
        }
    }
    double d = sqrt((a-c)*(a-c) + 4*b*b);
    double l1, l2;
    l1 = abs((a+c+d)/2);
    l2 = abs((a+c-d)/2);
    return min(l1,l2);
}

double InterestingPointsDetector::CalculateCxy
(const DoubleMat& source, const int x, const int y,
  const int windowHalfSize, const BorderType borderType) const
{
    double cxy = INFINITY;
    for(int dx = -1; dx <= 1; dx++)
    {
        for(int dy = -1; dy <= 1; dy++)
        {
            if(dx == 0 && dy == 0) continue;
            double sum = 0;
            for(int v = -windowHalfSize; v <= windowHalfSize; v++)
            {
                for(int u = -windowHalfSize; u<= windowHalfSize; u++)
                {
                    double diff = source.get(x+u, y+v, borderType)
                            - source.get(x+u+dx, y+v+dy, borderType);
                    sum += diff*diff;
                }
            }
            if(sum < cxy) cxy = sum;
        }
    }
    return cxy;
}

DoubleMat InterestingPointsDetector::CalculateMoravecMap
(const DoubleMat& source, const int windowHalfSize, const BorderType borderType) const
{
    DoubleMat result(source.getWidth(), source.getHeight());
    for(int x = 0; x < source.getWidth(); x++)
    {
        for(int y = 0; y < source.getWidth(); y++)
        {
            result.set(CalculateCxy(source, x, y, windowHalfSize, borderType)
                        , x, y);
        }
    }
    return result;
}

vector<InterestingPoint> InterestingPointsDetector::FindInterestingPoints
(const DoubleMat& source, const int windowHalfSize, const double threshold,
 const int extractionRadius, const BorderType borderType) const
{
    vector<InterestingPoint> result;

    auto errors = _diffCalc(source, windowHalfSize, borderType);
            //CalculateDiffs(windowHalfSize, borderType);
    for(int x = 0; x < errors.getWidth(); x++)
    {
        for(int y = 0; y < errors.getHeight(); y++)
        {
            if(errors.get(x,y, borderType) < threshold) continue;
            bool max = true;
            for(int px = - extractionRadius; px <= extractionRadius; px++)
            {
                for(int py = -extractionRadius; py <= extractionRadius; py++)
                {
                    if(errors.get(x+px, y+py, borderType) > errors.get(x,y, borderType))
                        max = false;
                }
            }

            if(max)
            {
                InterestingPoint  newPoint;
                newPoint.x = x;
                newPoint.y = y;
                newPoint.w = errors.get(x,y);
                result.emplace_back(newPoint);
            }
        }
    }
    return result;
}

vector<InterestingPoint> InterestingPointsDetector::ANMS
(const vector<InterestingPoint> source,
          const int maxCount, const int maxRadius)
{
    vector<InterestingPoint> suppressed;
    suppressed.assign(source.begin(), source.end());
    int r = 0;
    while (r < maxRadius
           && suppressed.size() > maxCount)
    {
        for(int i = 0; i < suppressed.size(); i++)
        {
            for(int j = i+1; j < suppressed.size(); j++)
            {
                int dx = suppressed[i].x - suppressed[j].x;
                int dy = suppressed[i].y - suppressed[j].y;
                double distance = sqrt(dx*dx + dy*dy);
                if(distance <= r &&  suppressed[j].w < suppressed[i].w * 0.9)
                {
                    suppressed.erase(suppressed.begin() + j);
                    --j;
                }
            }
        }
        ++r;
    }
    return suppressed;
}

vector<InterestingPoint> InterestingPointsDetector::FindBlobBasedPoints(const Pyramid &pyramid)
{
    vector<InterestingPoint> result;
    const auto blobs = pyramid.FindBlobs();

    for(auto& blob : blobs)
    {
        int octave = pyramid.GetOctaveAndLayer(blob.sigma).first;
        double harrisValue = CalculateHarrisValue(pyramid.GetNearestImage(blob.sigma),
                                                  3, BorderType::Replicate,
                                                  blob.x / pow(2, octave),
                                                  blob.y / pow(2, octave));
        if(harrisValue > 0.018)
        {
            InterestingPoint point;
            point.x = blob.x;
            point.y = blob.y;
            point.sigma = blob.sigma;
            point.w = harrisValue;

            result.emplace_back(point);
        }
    }
    return result;
}



