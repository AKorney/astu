#include "doublemat.h"

DoubleMat::DoubleMat()
    : _width(0), _height(0), _data(nullptr)
{

}

DoubleMat::DoubleMat(const int width, const int height)
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

DoubleMat::DoubleMat(const unique_ptr<unsigned char[]>& byteSource, const int width, const int height)
	: DoubleMat(width, height)
{
	transform(byteSource.get(), byteSource.get() + width * height, _data.get(),
		[](unsigned char sourceValue)-> double {return  sourceValue / 255.0; });
}

DoubleMat::DoubleMat(const double * doubleSource, const int width, const int height)
	: DoubleMat(width, height)
{
	copy(doubleSource, doubleSource + width * height, _data.get());
}

DoubleMat::DoubleMat(DoubleMat && other)
{
	_height = other._height;
	_width = other._width;
	_data = move(other._data);
}



DoubleMat & DoubleMat::operator=(DoubleMat && other)
{
	if (this != &other)
	{
		_height = other._height;
		_width = other._width;
		_data = move(other._data);
	}
	return *this;
}

int DoubleMat::getHeight() const
{
	return _height;
}

int DoubleMat::getWidth() const
{
	return _width;
}


//double DoubleMat::get(const int x, const int y) const
//{
//    return _data[y * _width + x];
//}

void DoubleMat::set(const double value,const int x, const int y)
{
    _data[y * _width + x] = value;
}

unique_ptr<double[]> DoubleMat::GetNormalizedData(const double newMin, const double newMax) const
{
	auto result = make_unique<DoubleMat>(*this);
	auto minmax = minmax_element(_data.get(), _data.get() + _width*_height);
	double min = minmax.first[0];
	double max = minmax.second[0];

	transform(result->_data.get(), result->_data.get() + _width * _height, result->_data.get(),
		[min, max, newMin, newMax](double value) -> double
				{
					return (newMax - newMin)*(value - min) / (max - min) + newMin;
				});
	return move(result->_data);
}

double DoubleMat::get(const int x, const int y, BorderType borderType) const
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

DoubleMat DoubleMat::Convolve(const DoubleMat & kernel, BorderType border) const
{
	DoubleMat resultMat = DoubleMat(_width, _height);
	for (int x = 0; x < _width; x++)
	{
		for (int y = 0; y < _height; y++)
		{
            double result = ConvolveCell(kernel, border, x,y);//0;
            //int kernelWidth = kernel.getWidth();
            //int kernelHeight = kernel.getHeight();
            //for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
            //{
            //	for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
            //	{
            //		result += get(x - kernelX + kernelWidth / 2, y - kernelY + kernelHeight / 2, border)
            //            * kernel.get(kernelX, kernelY, border);
            //	}
            //}
			resultMat.set(result, x, y);
		}
	}
    return resultMat;
}

DoubleMat DoubleMat::Sub(const DoubleMat &other) const
{
    DoubleMat result(_width, _height);
    transform(_data.get(), _data.get() + _width * _height, other._data.get(), result._data.get(), minus<double>());
    return result;
}

double DoubleMat::ConvolveCell(const DoubleMat &kernel, const BorderType border
                               , const int x, const int y) const
{
    double result = 0;
    int kernelWidth = kernel.getWidth();
    int kernelHeight = kernel.getHeight();
    for (int kernelX = 0; kernelX < kernelWidth; kernelX++)
    {
        for (int kernelY = 0; kernelY < kernelHeight; kernelY++)
        {
            result += get(x - kernelX + kernelWidth / 2, y - kernelY + kernelHeight / 2, border)
                * kernel.get(kernelX, kernelY, border);
        }
    }
    return result;
}


DoubleMat DoubleMat::ScaleDown() const
{
    DoubleMat result(_width/2, _height/2);
    for (int y = 0; y < result._height; y++)
    {
        for (int x = 0; x < result._width; x++)
        {
            auto value = (get(2 * x, 2 * y, BorderType::Replicate)
                + get(2 * x + 1, 2 * y,  BorderType::Replicate)
                + get(2 * x, 2 * y + 1,  BorderType::Replicate)
                + get(2 * x + 1, 2 * y + 1,  BorderType::Replicate))
                / 4;
            result._data[y * result._width + x] = value;
        }
    }
    return result;
}

