#ifndef PYRAMID_H
#define PYRAMID_H

#include <vector>

#include "cvimage.h"
#include "Octave.h"
using namespace std;

enum class DoGPointType
{
    Minimal, Regular, Maximal
};
struct BlobDescription
{
    int x,y;
    double sigma;
    DoGPointType pointType;
};



class Pyramid
{
private:
    int _octavesCount, _octaveSize, _overlapSize = 3;
    double _sigmaStart, _sigmaInput;
    double _k;
    unique_ptr<double[]> _deltas;
    //vector<vector<OctaveLayer>> _octaves;
    vector<Octave> _octaves;
    Octave BuildOctave(const DoubleMat& firstImage,
                       const int octaveNumber);
    void CalculateDeltas();
    DoGPointType GetDoGPointType(const int x,const int y,const int octave, const int diffIndex) const;

public:
    Pyramid();
    Pyramid(const int octavesCount, const int octaveSize,
            const double sigmaStart, const double sigmaInput,
            const CVImage& source);
    unsigned char L(const int x, const int y, const double sigma) const;
    const DoubleMat& GetImageAt(const int octave, const int layer) const;
    const Octave& GetOctaveAt(const int octave) const;
    vector<BlobDescription> FindBlobs() const;
};

#endif // PYRAMID_H
