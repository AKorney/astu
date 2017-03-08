#include "harrisdetector.h"

HarrisDetector::HarrisDetector(const DoubleMat& source)
 : InterestingPointsDetector(source)
{

}

DoubleMat HarrisDetector::CalculateDiffs
(const int windowHalfSize, const BorderType borderType) const
{
    const auto sobelX = _source.Convolve(KernelBuilder::BuildSobelX(), borderType);
    const auto sobelY = _source.Convolve(KernelBuilder::BuildSobelY(), borderType);
    double sigma = 1.0*windowHalfSize/3;
    const auto weights = KernelBuilder::BuildGauss(sigma);

    DoubleMat result(_source.getWidth(), _source.getHeight());
    for(int x = 0; x < _source.getWidth(); x++)
    {
        for(int y = 0; y < _source.getHeight(); y++)
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
