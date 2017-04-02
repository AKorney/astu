#ifndef OCTAVELAYER_H
#define OCTAVELAYER_H

#include <memory>

#include "doublemat.h"

using namespace std;
struct DoG
{
    DoubleMat diff;
    double sigmaLow;
    double sigmaHigh;
};

class OctaveLayer
{
private:
    unique_ptr<DoubleMat> _image;
	double _sigmaLocal;
	double _sigmaGlobal;
public:
	OctaveLayer();
    OctaveLayer(const unique_ptr<DoubleMat>& image, const double sigmaLocal, const double sigmaGlobal);
	OctaveLayer(const OctaveLayer& other);
    OctaveLayer(OctaveLayer&& other) = default;
    OctaveLayer& operator=(OctaveLayer&& other) = default;

    const DoubleMat& GetImage() const;
    double GetSigmaGlobal() const {return _sigmaGlobal;}
    double GetSigmaLocal() const {return _sigmaLocal;}
};

#endif // !OCTAVELAYER_H
