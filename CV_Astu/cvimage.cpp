#include "cvimage.h"

CVImage::CVImage() : _width(0), _height(0), _data(nullptr)
{
}

CVImage::CVImage(const int width, const int height) : _width(width), _height(height)
{
    _data = make_unique<unsigned char[]>((size_t)_width * _height);
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
	:CVImage(doubleData.get()->getWidth(), doubleData.get()->getHeight())
{
	auto normalized = doubleData.get()->GetNormalizedData(0, 255);
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
	const auto doubleMat = make_unique<DoubleMat>(_width, _height);
	for (int x = 0; x < _width; x++)
	{
		for (int y = 0; y < _height; y++)
		{
			double result = 0;
			int kernelWidth = kernel.get()->getWidth();
			int kernelHeight = kernel.get()->getHeight();
			for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
			{
				for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
				{
					result += get(x - kernelX + kernelWidth / 2, y - kernelY + kernelHeight / 2, border)
						* kernel.get()->get(kernelX, kernelY);
				}
			}
			doubleMat.get()->set(result, x, y);
		}
	}
	return make_unique<CVImage>(doubleMat);
}

unsigned char CVImage::get(int x, int y, BorderType borderType)
{
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
		
	default:
		break;
	}
}
