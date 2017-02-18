#include "cvimage.h"

CVImage::CVImage() : _width(0), _height(0), _data(nullptr)
{
}

CVImage::CVImage(const int width, const int height) : _width(width), _height(height)
{
    _data = make_unique<uchar[]>((size_t)_width * _height);
}


CVImage::CVImage(const int width, const int height, uchar * rgbData)
    : CVImage::CVImage(width, height)
{
    int imageSize = _width * _height;
    for(int i = 0; i < imageSize; i++)
    {
        int grey =  (0.299 * rgbData[i * 3] + 0.578 * rgbData[i*3+1] + 0.114 * rgbData[i*3+2]);
        _data[i] = (uchar)min(255, grey);
    }
}
