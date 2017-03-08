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
 const int extractionRadius, const BorderType borderType)
{
    //vector<InterestingPoint> result;
    _points.clear();
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
                    if(errors.get(x+px, y+py, borderType) > errors.get(x,y, borderType)) max = false;
                }
            }

            if(max)
            {
                InterestingPoint  newPoint;
                newPoint.x = x;
                newPoint.y = y;
                newPoint.w = errors.get(x,y);
                _points.emplace_back(newPoint);
            }
        }
    }
    return _points;
}

vector<InterestingPoint> InterestingPointsDetector::ANMS
(int maxCount) const
{
    vector<InterestingPoint> supressed;
    supressed.assign(_points.begin(), _points.end());
    int r = 0;
    while (r < max(_source.getWidth(), _source.getHeight())
           && supressed.size() > maxCount)
    {
        for(int i = 0; i < supressed.size(); i++)
        {
            for(int j = i+1; j < supressed.size(); j++)
            {
                int dx = supressed[i].x - supressed[j].x;
                int dy = supressed[i].y - supressed[j].y;
                double distance = sqrt(dx*dx + dy*dy);
                if(distance <= r &&  supressed[j].w < supressed[i].w * 0.9)
                {
                    supressed.erase(supressed.begin() + j);
                    --j;
                }
            }
        }
        ++r;
    }
    return supressed;
}
