#include "pyramid.h"
#include <math.h>

#include "cvimageloader.h"


Pyramid::Pyramid()
{

}

CVImage Pyramid::BuildOctave(vector<OctaveLayer>& octave,
                             const CVImage& firstImage,
                             const int octaveNumber)
{
    CVImage result;
    auto current = firstImage;
    double sigmaLocal = _sigmaStart;
    double sigmaGlobal = _sigmaStart * pow(2, octaveNumber);

    octave.emplace_back(OctaveLayer(make_unique<CVImage>(current),
                                    sigmaLocal, sigmaGlobal));
     CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\temp.jpg", current);
    for(int i=1; i<_octaveSize + _overlapSize; i++)
    {
        current = move(current.GaussianSmoothing(_deltas[i-1], BorderType::Replicate, true));
        CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\temp.jpg", current);
        sigmaLocal = i == _octaveSize
            ? 2 * _sigmaStart
            : sigmaLocal * _k;
        sigmaGlobal = i == _octaveSize
            ? pow(2, i + 1) * _sigmaStart
            : sigmaGlobal *_k;
        if(i == _octaveSize)
        {
            result = CVImage(current.ScaleDown());
        }
        octave.emplace_back(OctaveLayer(make_unique<CVImage>(current),
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
    : _octavesCount(octavesCount), _octaveSize(octaveSize),
      _sigmaStart(sigmaStart), _sigmaInput(sigmaInput)
{

	const double startDelta = sqrt(sigmaStart * sigmaStart 
		- sigmaInput * sigmaInput);
    CalculateDeltas();
	//first image of first octave
	CVImage currentImage = source.GaussianSmoothing(startDelta, BorderType::Replicate, true);
    CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\temp.jpg", currentImage);
	for (int octave = 0; octave < _octavesCount; octave++)
	{
        auto currentOctave = vector<OctaveLayer>();
        currentImage = move(BuildOctave(currentOctave, currentImage, octave));
	}

}
