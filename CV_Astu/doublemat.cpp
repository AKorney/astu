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


double DoubleMat::get(const int x, const int y) const
{
    return _data[y * _width + x];
}

void DoubleMat::set(const double value,const int x, const int y)
{
    _data[y * _width + x] = value;
}

unique_ptr<double[]> DoubleMat::GetNormalizedData(const double newMin, const double newMax)
{
	auto result = make_unique<DoubleMat>(*this);
	auto minmax = std::minmax_element(_data.get(), _data.get() + _width*_height);
	double min = minmax.first[0];
	double max = minmax.second[0];

	std::transform(result.get()->_data.get(), result.get()->_data.get() + _width * _height, result.get()->_data.get(),
		[min, max, newMin, newMax](double value) -> double
				{
					return (newMax - newMin)*(value - min) / (max - min) + newMin;
				});
	return std::move(result.get()->_data);
}




