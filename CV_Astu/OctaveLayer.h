#ifndef OCTAVELAYER_H
#define OCTAVELAYER_H

#include <memory>

#include "cvimage.h"

using namespace std;

class OctaveLayer
{
private:
	unique_ptr<CVImage> _image;
	double _sigmaLocal;
	double _sigmaGlobal;
public:
	OctaveLayer();
	OctaveLayer(const unique_ptr<CVImage>& image, const double sigmaLocal, const double sigmaGlobal);
	OctaveLayer(const OctaveLayer& other);
    OctaveLayer(OctaveLayer&& other) = default;
    OctaveLayer& operator=(OctaveLayer&& other) = default;

	CVImage& GetImage() const;
    double GetSigmaGlobal() const {return _sigmaGlobal;}
    double GetSigmaLocal() const {return _sigmaLocal;}
};

#endif // !OCTAVELAYER_H