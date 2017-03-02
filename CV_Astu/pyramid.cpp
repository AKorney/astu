#include "pyramid.h"
#include <math.h>

#include "cvimageloader.h"


Pyramid::Pyramid()
{

}

CVImage Pyramid::BuildOctave(Octave& octave,
                             const CVImage& firstImage,
                             const int octaveNumber)
{
    CVImage result;
    auto current = firstImage;
    double sigmaLocal = _sigmaStart;
    double sigmaGlobal = _sigmaStart * pow(2, octaveNumber);

    octave.AddLayer(OctaveLayer(make_unique<CVImage>(current),
                                    sigmaLocal, sigmaGlobal));
    for(int i=1; i<_octaveSize + _overlapSize; i++)
    {
        current = move(current.GaussianSmoothing(_deltas[i-1],
                       BorderType::Replicate, true));
        if(i == _octaveSize)
        {
            result = CVImage(current.ScaleDown());
            //
            sigmaLocal = 2 * _sigmaStart;
            sigmaGlobal = _sigmaStart * pow(2, octaveNumber + 1);
        }
        else
        {
            sigmaLocal *= _k;
            sigmaGlobal *= _k;
        }
        octave.AddLayer(OctaveLayer(make_unique<CVImage>(current),
            sigmaLocal, sigmaGlobal));
    }
    return result;
}

void Pyramid::CalculateDeltas()
{
    _k = pow(2, 1.0 / _octaveSize);
    _deltas = make_unique<double[]>(_octaveSize + _overlapSize -1);
    double s = _sigmaStart;
    for(int i =0; i < _octaveSize + _overlapSize - 1; i++)
    {
        _deltas[i] = s * sqrt(_k*_k - 1);
        s*=_k;

    }
}

Pyramid::Pyramid(const int octavesCount, const int octaveSize,
                 const double sigmaStart, const double sigmaInput,
                 const CVImage &source)
    :  _octaveSize(octaveSize),
      _sigmaStart(sigmaStart), _sigmaInput(sigmaInput)
{
    int maxOctavesCount = log2(min(source.getHeight(),source.getWidth()));
    _octavesCount = min(octavesCount, maxOctavesCount);
	const double startDelta = sqrt(sigmaStart * sigmaStart 
		- sigmaInput * sigmaInput);
    CalculateDeltas();
    CVImage currentImage = source.GaussianSmoothing(startDelta,
                                                    BorderType::Replicate,
                                                    true);
	for (int octave = 0; octave < _octavesCount; octave++)
	{
        Octave currentOctave(octave);
        currentImage = move(BuildOctave(currentOctave, currentImage,
                                        octave));
        currentOctave.SaveAll();
        _octaves.emplace_back(currentOctave);
	}

}
