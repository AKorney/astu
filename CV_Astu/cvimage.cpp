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

unsigned char CVImage::get(int x, int y)
{

}
