#include "kernelbuilder.h"



unique_ptr<DoubleMat> KernelBuilder::BuildSobelX()
{
    const double _sobelX[9] = { -1, 0, 1,
                                    -2, 0, 2,
                                    -1, 0, 1};
    return make_unique<DoubleMat>(_sobelX, 3, 3);
}

unique_ptr<DoubleMat> KernelBuilder::BuildSobelY()
{
    const double _sobelY[9] = { -1, -2, -1,
                                 0, 0, 0,
                                 1, 2, 1};
    return make_unique<DoubleMat>(_sobelY, 3, 3);
}

unique_ptr<DoubleMat> KernelBuilder::BuildGauss(double sigma)
{
    int halfSize = (int)round(sigma*3);
    int fullSize = halfSize * 2 + 1;

	unique_ptr<double[]> data = make_unique<double[]>((size_t)fullSize * fullSize);

    for(int x = -halfSize; x <= halfSize; x++)
    {
        for(int y = -halfSize; y <= halfSize; y++)
        {
			data[(y + halfSize)*fullSize + x + halfSize] = exp(-(x*x + y*y) / (2 * sigma*sigma)) / (2 * M_PI);
        }
    }
	return make_unique<DoubleMat>(data.get(), fullSize, fullSize);
}
