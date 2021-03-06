#include "OctaveLayer.h"



OctaveLayer::OctaveLayer() : _sigmaLocal(0), _sigmaGlobal(0)
{
}

OctaveLayer::OctaveLayer(const unique_ptr<DoubleMat>& image, const double sigmaLocal, const double sigmaGlobal)
	: _sigmaLocal(sigmaLocal), _sigmaGlobal(sigmaGlobal)
{
    _image = make_unique<DoubleMat>(*image.get());

}

OctaveLayer::OctaveLayer(const OctaveLayer & other)
	: _sigmaLocal(other._sigmaLocal), _sigmaGlobal(other._sigmaGlobal)
{
    _image = make_unique<DoubleMat>(*other._image.get());
}




const DoubleMat& OctaveLayer::GetImage() const
{
	return *_image;
}


