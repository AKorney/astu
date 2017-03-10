#include "interestingpointsdetector.h"

InterestingPointsDetector::InterestingPointsDetector(const DoubleMat &source)
    :_source(source)
{

}

InterestingPointsDetector::~InterestingPointsDetector()
{

}


vector<InterestingPoint> InterestingPointsDetector::FindInterestingPoints
(const int windowHalfSize, const double threshold,
 const int extractionRadius, const BorderType borderType) const
{
    vector<InterestingPoint> result;
    auto errors =
            CalculateDiffs(windowHalfSize, borderType);
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
