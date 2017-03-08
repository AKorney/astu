#include "cvimage.h"

CVImage::CVImage() : _width(0), _height(0), _data(nullptr)
{
}

CVImage::CVImage(const int width, const int height) : _width(width), _height(height)
{
    _data = make_unique<unsigned char[]>(_width * _height);
}


CVImage::CVImage(const CVImage & source)
	: CVImage(source._width, source._height)
{
	copy(source._data.get()
		,source._data.get() + _width * _height
		,_data.get());
}

CVImage::CVImage(const unsigned char * rgb24Data, const int width, const int height)
	: CVImage(width, height)
{
	int imageSize = _width * _height;
	for (int i = 0; i < imageSize; i++)
	{
		const unsigned char red = rgb24Data[i * 3];
		const unsigned char green = rgb24Data[i * 3 + 1];
		const unsigned char blue = rgb24Data[i * 3 + 2];

		_data[i] = (unsigned char) min(255.0, (0.299 * red + 0.587 * green + 0.114 * blue));
	}
}

unsigned char * CVImage::GetImageData() const
{
	/*unsigned char * data = new unsigned char[_width * _height];
	copy(_data.get()
		, _data.get() + _width * _height
		, data);*/
	return _data.get();
}

int CVImage::getHeight() const
{
	return _height;
}

int CVImage::getWidth() const
{
	return _width;
}

CVImage::CVImage(CVImage&& other)
    : CVImage()
{
    _height = other._height;
    _width = other._width;
    _data = move(other._data);
}

CVImage::CVImage(const DoubleMat& doubleData)
	:CVImage(doubleData.getWidth(), doubleData.getHeight())
{
	auto normalized = doubleData.GetNormalizedData(0, 255);
	transform(normalized.get(), normalized.get() + _width * _height, _data.get(),
		[](double sourceValue)-> unsigned char {return (unsigned char)sourceValue; });
}


CVImage& CVImage::operator=(CVImage&& other)
{
    if(this!=&other)
    {
        _height = other._height;
        _width = other._width;
        _data = move(other._data);
    }
    return *this;
}



DoubleMat CVImage::PrepareDoubleMat() const
{
	return DoubleMat(_data, _width, _height);
}

CVImage CVImage::Convolve(const DoubleMat& kernel, const BorderType border) const
{
	auto doubleMat = PrepareDoubleMat().Convolve(kernel, border);
	
	return CVImage(doubleMat);
}

CVImage CVImage::SobelX(const BorderType border)
{
	const auto kernel = KernelBuilder::BuildSobelX();
	return Convolve(kernel, border);
}

CVImage CVImage::SobelY(const BorderType border)
{
	const auto kernel = KernelBuilder::BuildSobelY();
	return Convolve(kernel, border);
}

CVImage CVImage::Sobel(const BorderType border)
{
	const auto kernelY = KernelBuilder::BuildSobelX();
	auto doubleMatY = PrepareDoubleMat().Convolve(kernelY, border);

	const auto kernelX = KernelBuilder::BuildSobelY();
	auto doubleMatX = PrepareDoubleMat().Convolve(kernelX, border);
	
	auto doubleMat = DoubleMat(_width, _height);

	for (int i = 0; i < _width; i++)
	{
		for (int j = 0; j < _height; j++)
		{
            double value = doubleMatX.get(i, j, border) * doubleMatX.get(i, j, border)
                + doubleMatY.get(i, j, border) * doubleMatY.get(i, j, border);
			doubleMat.set(sqrt(value), i, j);
		}
	}
	return CVImage(doubleMat);
}

CVImage CVImage::GaussianSmoothing(const double sigma, const BorderType border, bool useAxisSeparation) const
{
	if (useAxisSeparation)
	{
		const auto kernelX = KernelBuilder::BuildGaussX(sigma);
		const auto kernelY = KernelBuilder::BuildGaussY(sigma);
		return	CVImage(PrepareDoubleMat()
			.Convolve(kernelX, border)
			.Convolve(kernelY, border));		
	}
	else
	{
		const auto kernel = KernelBuilder::BuildGauss(sigma);
		return Convolve(kernel, border);
	}
}

CVImage CVImage::ScaleDown() const
{
	CVImage result = CVImage(_width/2, _height/2);
	for (int y = 0; y < result._height; y++)
	{		
		for (int x = 0; x < result._width; x++)
		{
			auto value = (get(2 * x, 2 * y)
				+ get(2 * x + 1, 2 * y)
				+ get(2 * x, 2 * y + 1)
				+ get(2 * x + 1, 2 * y + 1)) 
				/ 4;
			result._data[y * result._width + x] = value;				
		}
	}
	return result;
}

unsigned char CVImage::get(const int x, const int y, const BorderType borderType) const
{
	int effectiveX = x, effectiveY = y;
	switch (borderType)
	{
	case BorderType::Constant:
		if (x < 0 || y < 0 || x >= _width || y >= _height)
		{
			return 0;
		}
		else
		{
			return _data[y * _width + x];
		}

	case BorderType::Replicate:

		if (x < 0) effectiveX = 0;
		if (y < 0) effectiveY = 0;
		if (x >= _width) effectiveX = _width - 1;
		if (y >= _height) effectiveY = _height - 1;
		return _data[effectiveY * _width + effectiveX];
	case BorderType::Reflect:
		if (x < 0) effectiveX = -x;
		if (y < 0) effectiveY = -y;
		if (x >= _width) effectiveX = 2 * _width - 2 - x;
		if (y >= _height) effectiveY = 2 * _height - 2 - y;
		return _data[effectiveY * _width + effectiveX];
	case BorderType::Wrap:
		if (x < 0) effectiveX = _width - 1 + x;
		if (y < 0) effectiveY = _height - 1 + y;
		if (x >= _width) effectiveX = x - _width;
		if (y >= _height) effectiveY = y - _height;
		return _data[effectiveY * _width + effectiveX];
	default:
		break;
	}
}
