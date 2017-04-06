#include "pyramid.h"
#include <math.h>

#include "imagehelper.h"


Pyramid::Pyramid()
{

}

Octave Pyramid::BuildOctave(const DoubleMat& firstImage,
                               const int octaveNumber)
{
    Octave octave(octaveNumber);
    auto current = firstImage;
    double sigmaLocal = _sigmaStart;
    double sigmaGlobal = _sigmaStart * pow(2, octaveNumber);

    octave.AddLayer(OctaveLayer(make_unique<DoubleMat>(current),
                                    sigmaLocal, sigmaGlobal));
    for(int i=1; i<_octaveSize + _overlapSize; i++)
    {
        current = move(current
                       .Convolve(KernelBuilder::BuildGaussX(_deltas[i-1]), BorderType::Replicate)
                       .Convolve(KernelBuilder::BuildGaussY(_deltas[i-1]), BorderType::Replicate));
        if(i == _octaveSize)
        {
            //result = DoubleMat(current.ScaleDown());
            //
            sigmaLocal = 2 * _sigmaStart;
            sigmaGlobal = _sigmaStart * pow(2, octaveNumber + 1);
        }
        else
        {
            sigmaLocal *= _k;
            sigmaGlobal *= _k;
        }
        octave.AddLayer(OctaveLayer(make_unique<DoubleMat>(current),
            sigmaLocal, sigmaGlobal));
    }
    octave.BuildDiffs();
    return octave;
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
    _sourceImage = source.PrepareDoubleMat();
    int maxOctavesCount = log2(min(source.getHeight(),source.getWidth()));
    _octavesCount = min(octavesCount, maxOctavesCount);
	const double startDelta = sqrt(sigmaStart * sigmaStart 
		- sigmaInput * sigmaInput);
    CalculateDeltas();
    DoubleMat currentImage = source.PrepareDoubleMat()
            .Convolve(KernelBuilder::BuildGaussX(startDelta), BorderType::Replicate)
            .Convolve(KernelBuilder::BuildGaussY(startDelta), BorderType::Replicate);

    for (int octave = 0; octave < _octavesCount; octave++)
	{
        Octave currentOctave = BuildOctave(currentImage, octave);
        //currentOctave.SaveAll();
        _octaves.emplace_back(currentOctave);
        if(_overlapSize > 0)
        {
            currentImage = currentOctave.GetLayerAt(_octaveSize)
                .GetImage().ScaleDown();
        }
        else
        {
            double delta = _sigmaStart * pow(_k, _octaveSize-1)
                    * sqrt(_k * _k - 1);
            currentImage = currentOctave.GetLayerAt(_octaveSize-1)
                    .GetImage()
                    .Convolve(KernelBuilder::BuildGaussX(delta), BorderType::Replicate)
                    .Convolve(KernelBuilder::BuildGaussY(delta), BorderType::Replicate)
                    .ScaleDown();
        }

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

const DoubleMat& Pyramid::GetImageAt(const int octave, const int layer) const
{
    return GetOctaveAt(octave).GetLayerAt(layer).GetImage();
}



const pair<int, int> Pyramid::GetOctaveAndLayer(double sigma) const
{
    pair<int, int> position;
    int globalPosition = log(sigma/_sigmaStart)/log(_k);

    if(globalPosition < 0)
    {
        position.first = 0;
        position.second = 0;
    }
    if(globalPosition >= _octavesCount*_octaveSize)
    {
        position.first = _octavesCount - 1;
        position.second =  _octaveSize - 1;
    }
    double diffLeft = abs(_sigmaStart * pow(_k, globalPosition) - sigma);
    double diffRight = abs(_sigmaStart * pow(_k, globalPosition+1) - sigma);
    if(diffRight < diffLeft) globalPosition++;

    position.first  = globalPosition / _octaveSize;
    position.second = globalPosition % _octaveSize;
    return position;
}

const DoubleMat &Pyramid::GetNearestImage(double sigma) const
{
    const auto pos = GetOctaveAndLayer(sigma);

    return GetImageAt(pos.first, pos.second);
}

