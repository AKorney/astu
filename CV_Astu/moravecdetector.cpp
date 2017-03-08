#include "moravecdetector.h"

MoravecDetector::MoravecDetector(const DoubleMat& source)
 : InterestingPointsDetector(source)
{

}

double MoravecDetector::CalculateCxy
(const int x, const int y, const int windowHalfSize,
 const BorderType borderType) const
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
                    double diff = _source.get(x+u, y+v, borderType)
                            - _source.get(x+u+dx, y+v+dy, borderType);
                    sum += diff*diff;
                }
            }
            if(sum < cxy) cxy = sum;
        }
    }
    return cxy;
}

DoubleMat MoravecDetector::CalculateErrors
    (const int windowHalfSize, const BorderType borderType) const
{
    DoubleMat result(_source.getWidth(), _source.getHeight());
    for(int x = 0; x < _source.getWidth(); x++)
    {
        for(int y = 0; y < _source.getWidth(); y++)
        {
            result.set(CalculateCxy(x,y,windowHalfSize, borderType)
                        , x, y);
        }
    }
    return result;
}
