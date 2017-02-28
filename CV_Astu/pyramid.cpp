#include "pyramid.h"
#include <math.h>

#include "cvimageloader.h"


Pyramid::Pyramid()
{

}

Pyramid::Pyramid(const int octavesCount, const int octaveSize,
                 const double sigmaStart, const double sigmaInput,
                 const CVImage &source)
    : _octavesCount(octavesCount), _octaveSize(octaveSize),
      _sigmaStart(sigmaStart), _sigmaInput(sigmaInput)
{
	_k = pow(2, 1.0 / _octaveSize);
	const double startDelta = sqrt(sigmaStart * sigmaStart 
		- sigmaInput * sigmaInput);

	//first image of first octave
	CVImage currentImage = source.GaussianSmoothing(startDelta, BorderType::Replicate, true);

	double sigmaGlobal, sigmaLocal;
	sigmaGlobal = _sigmaStart;
	for (int octave = 0; octave < _octavesCount; octave++)
	{
		sigmaLocal = sigmaStart;

        auto currentOctave = vector<OctaveLayer>();
        currentOctave.emplace_back(OctaveLayer(make_unique<CVImage>(currentImage),
        	sigmaLocal, sigmaGlobal));
		
		CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\temp.jpg", currentImage);
		for (int layer = 1; layer <= octaveSize; layer++)
		{			
			double delta = sigmaLocal * sqrt(_k*_k - 1);

            currentImage = move(currentImage.GaussianSmoothing(delta, BorderType::Replicate, true));
			CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\temp.jpg", currentImage);
			sigmaLocal = layer == octaveSize
				? 2 * _sigmaStart
				: sigmaLocal * _k;
			sigmaGlobal = layer == octaveSize
				? pow(2, octave + 1) * _sigmaStart
				: sigmaGlobal *_k;

            currentOctave.emplace_back(OctaveLayer(make_unique<CVImage>(currentImage),
            	sigmaLocal, sigmaGlobal));
		}
        _octaves.emplace_back(currentOctave);
        currentImage = move(currentImage.ScaleDown());
	}

}
