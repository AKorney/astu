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

unsigned char Pyramid::L(const int x, const int y, const double sigma) const
{
    int globalPosition = log(sigma/_sigmaStart)/log(_k);
    if(globalPosition < 0)
    {
        return GetImageAt(0,0).get(x,y);
    }
    if(globalPosition >= _octavesCount*_octaveSize)
    {
        return GetImageAt(_octavesCount-1, _octaveSize - 1)
                .get(x / pow(2, _octavesCount-1), y / pow(2, _octavesCount-1));
    }
    double diffLeft = abs(_sigmaStart * pow(_k, globalPosition) - sigma);
    double diffRight = abs(_sigmaStart * pow(_k, globalPosition+1) - sigma);
    if(diffRight < diffLeft) globalPosition++;

    const int targetOctave = globalPosition / _octaveSize;
    const int targetLayer = globalPosition % _octaveSize;

    const int targetX = x / pow(2, targetOctave);
    const int targetY = y / pow (2, targetOctave);

    return GetImageAt(targetOctave, targetLayer).get(targetX, targetY);
}

const Octave& Pyramid::GetOctaveAt(const int octave) const
{
    return _octaves.at(octave);
}

const CVImage& Pyramid::GetImageAt(const int octave, const int layer) const
{
    return GetOctaveAt(octave).GetLayerAt(layer).GetImage();
}
/*    const int octave = globalPosition/_octaveSize;
    const int leftLevel = globalPosition % _octaveSize;
    const int rightLevel = leftLevel + 1;



    const double diffLeft =  abs(_octaves.at(octave).GetLayerAt(leftLevel).GetSigmaGlobal()
                                 - sigma);
    const double diffRight =  abs(_octaves.at(octave).GetLayerAt(rightLevel).GetSigmaGlobal()
                                 - sigma);
    const auto targetLevel = diffLeft < diffRight
            ? _octaves.at(octave).GetLayerAt(leftLevel)
            : _octaves.at(octave).GetLayerAt(rightLevel);

    const int targetX = x / pow(2, octave);
    const int targetY = y / pow (2, octave);

    return targetLevel.GetImage().get(targetX, targetY);

 *
 */
