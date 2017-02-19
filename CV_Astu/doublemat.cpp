#include "doublemat.h"

DoubleMat::DoubleMat()
    : _width(0), _height(0), _data(nullptr)
{

}

DoubleMat::DoubleMat(int width, int height)
    : _width(width), _height(height)
{
    _data = make_unique<double[]>((size_t)_width * _height);
}

DoubleMat::DoubleMat(const DoubleMat &source)
 : DoubleMat(source._width, source._height)
{
    copy(source._data.get()
        ,source._data.get() + _width * _height
        ,_data.get());
}

DoubleMat(unsigned char *byteData, int width, int height)
    : DoubleMat(width, height)
{
    int elementsCount = width * height;
    for(int i = 0; i < elementsCount; i++)
    {
        _data[i] = byteData[i]/255.0;
    }
}

double DoubleMat::get(int x, int y)
{
    return _data[y * _width + x];
}

void DoubleMat::set(double value, int x, int y)
{
    _data[y * _width + x];
}
