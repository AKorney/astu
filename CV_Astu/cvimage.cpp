#include "cvimage.h"

CVImage::CVImage() : _width(0), _height(0), _data(nullptr)
{
}

CVImage::CVImage(const int width, const int height) : _width(width), _height(height)
{
    _data = make_unique<unsigned char[]>((size_t)_width * _height);
}

void CVImage::Convolve(unique_ptr<DoubleMat>& calculationBuffer, const unique_ptr<DoubleMat>& kernel, BorderType border)
{
	for (int x = 0; x < _width; x++)
	{
		for (int y = 0; y < _height; y++)
		{
			double result = 0;
			int kernelWidth = kernel->getWidth();
			int kernelHeight = kernel->getHeight();
			for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
			{
				for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
				{
					result += get(x - kernelX + kernelWidth / 2, y - kernelY + kernelHeight / 2, border)
						* kernel->get(kernelX, kernelY);
				}
			}
			calculationBuffer->set(result, x, y);
		}
	}
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

unsigned char * CVImage::GetImageData()
{
	unsigned char * data = new unsigned char[_width * _height];
	copy(_data.get()
		, _data.get() + _width * _height
		, data);
	return data;
}


CVImage::CVImage(CVImage&& other)
    : CVImage()
{
    _height = other._height;
    _width = other._width;
    _data = move(other._data);
}

CVImage::CVImage(const unique_ptr<DoubleMat>& doubleData)
	:CVImage(doubleData->getWidth(), doubleData->getHeight())
{
	auto normalized = doubleData->GetNormalizedData(0, 255);
	std::transform(normalized.get(), normalized.get() + _width * _height, _data.get(),
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

//unsigned char CVImage::get(int x, int y)
//{
//    return _data[y * _width + x];
//}

unique_ptr<DoubleMat> CVImage::PrepareDoubleMat()
{
	return make_unique<DoubleMat>(_data, _width, _height);
}

unique_ptr<CVImage> CVImage::Convolve(const unique_ptr<DoubleMat>& kernel, BorderType border)
{
	auto doubleMat = make_unique<DoubleMat>(_width, _height);
	Convolve(doubleMat, kernel, border);
	return make_unique<CVImage>(move(doubleMat));
}

unique_ptr<CVImage> CVImage::SobelX(BorderType border)
{
	const auto kernel = KernelBuilder::BuildSobelX();
	return Convolve(kernel, border);
}

unique_ptr<CVImage> CVImage::SobelY(BorderType border)
{
	const auto kernel = KernelBuilder::BuildSobelY();
	return Convolve(kernel, border);
}

unique_ptr<CVImage> CVImage::Sobel(BorderType border)
{
	const auto kernelY = KernelBuilder::BuildSobelX();
	auto doubleMatY = make_unique<DoubleMat>(_width, _height);
	Convolve(doubleMatY, kernelY, border);

	const auto kernelX = KernelBuilder::BuildSobelY();
	auto doubleMatX = make_unique<DoubleMat>(_width, _height);
	Convolve(doubleMatX, kernelX, border);
	
	const auto doubleMat = make_unique<DoubleMat>(_width, _height);
	for (int i = 0; i < _width; i++)
	{
		for (int j = 0; j < _height; j++)
		{
			double value = doubleMatX->get(i, j) * doubleMatX->get(i, j) + doubleMatY->get(i, j) * doubleMatY->get(i, j);
			doubleMat->set(sqrt(value), i, j);
		}
	}
	return make_unique<CVImage>(doubleMat);
}

unique_ptr<CVImage> CVImage::GaussianBlur(const double sigma, BorderType border)
{
	const auto kernel = KernelBuilder::BuildGauss(sigma);
	auto result = Convolve(kernel, border);
	return move(result);
}

unsigned char CVImage::get(const int x, const int y, BorderType borderType)
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
