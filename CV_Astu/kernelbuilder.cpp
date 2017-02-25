#include "kernelbuilder.h"



DoubleMat KernelBuilder::BuildSobelX()
{
    const double _sobelX[9] = {1, 0, -1,
                               2, 0, -2,
                               1, 0, -1};
    return DoubleMat(_sobelX, 3, 3);
}

DoubleMat KernelBuilder::BuildSobelY()
{
    const double _sobelY[9] = { 1, 2, 1,
                                 0, 0, 0,
                                 -1, -2, -1};
    return DoubleMat(_sobelY, 3, 3);
}

DoubleMat KernelBuilder::BuildGauss(double sigma)
{
    int halfSize = (int)round(sigma*3);
    int fullSize = halfSize * 2 + 1;

	unique_ptr<double[]> data = make_unique<double[]>((size_t)fullSize * fullSize);

    for(int x = -halfSize; x <= halfSize; x++)
    {
        for(int y = -halfSize; y <= halfSize; y++)
        {
			data[(y + halfSize)*fullSize + x + halfSize] = exp(-(x*x + y*y) / (2 * sigma*sigma)) / (2 * M_PI * sigma * sigma);
        }
    }
	return DoubleMat(data.get(), fullSize, fullSize);
}

DoubleMat KernelBuilder::BuildGaussX(double sigma)
{
	int halfSize = (int)round(sigma * 3);
	int fullSize = halfSize * 2 + 1;

	unique_ptr<double[]> data = make_unique<double[]>((size_t)fullSize * fullSize);

	for (int x = -halfSize; x <= halfSize; x++)
	{		
		data[x + halfSize] = exp(-(x*x ) / (2 * sigma*sigma)) /(sqrt(2 * M_PI) * sigma);
	}
	return DoubleMat(data.get(), fullSize, 1);
}

DoubleMat KernelBuilder::BuildGaussY(double sigma)
{
	int halfSize = (int)round(sigma * 3);
	int fullSize = halfSize * 2 + 1;

	unique_ptr<double[]> data = make_unique<double[]>((size_t)fullSize * fullSize);

	for (int y = -halfSize; y <= halfSize; y++)
	{
		data[y + halfSize] = exp(-(y*y) / (2 * sigma*sigma)) / (sqrt(2 * M_PI) * sigma);
	}
	return DoubleMat(data.get(), 1, fullSize);
}
