#ifndef PYRAMID_H
#define PYRAMID_H

#include <vector>

#include "cvimage.h"
#include "OctaveLayer.h"
using namespace std;
class Pyramid
{
private:
    int _octavesCount, _octaveSize;
    double _sigmaStart, _sigmaInput;
    double _k;

    vector<vector<OctaveLayer>> _octaves;
public:
    Pyramid();
    Pyramid(const int octavesCount, const int octaveSize,
            const double sigmaStart, const double sigmaInput,
            const CVImage& source);
};

#endif // PYRAMID_H
