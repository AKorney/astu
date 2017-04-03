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
        currentOctave.SaveAll();
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

vector<BlobDescription> Pyramid::FindBlobs() const
{
    vector<BlobDescription> result;
    for(int octave = 0; octave < _octavesCount; octave++)
    {
        for(int diffIndex = 1; diffIndex < _octaveSize + _overlapSize - 2; diffIndex++)
        {
            const int width = GetImageAt(octave, 0).getWidth();
            const int height = GetImageAt(octave, 0).getHeight();
            for(int x=0; x<width; x++)
            {
                for(int y=0;y<height;y++)
                {
                    auto pointType = GetDoGPointType(x,y,octave, diffIndex);
                    switch(pointType)
                    {
                        case DoGPointType::Maximal:
                        {
                            BlobDescription blobMax;
                            blobMax.x = (x + 0.5)*pow(2, octave) - 0.5;
                            blobMax.y = (y + 0.5)*pow(2, octave) - 0.5;
                            blobMax.sigma = GetOctaveAt(octave).GetDiffAt(diffIndex).GetSigmaGlobal();
                            blobMax.pointType = DoGPointType::Maximal;
                            result.emplace_back(blobMax);
                            break;
                        }
                        case DoGPointType::Minimal:
                        {
                            BlobDescription blobMin;
                            blobMin.x = (x + 0.5)*pow(2, octave) - 0.5;
                            blobMin.y = (y + 0.5)*pow(2, octave) - 0.5;
                            blobMin.sigma = GetOctaveAt(octave).GetDiffAt(diffIndex).GetSigmaGlobal();
                            blobMin.pointType = DoGPointType::Minimal;
                            result.emplace_back(blobMin);
                            break;
                        }
                        default:
                            break;
                    }
                }
            }
        }
    }
    return result;
}

DoGPointType Pyramid::GetDoGPointType(const int x,const int y,const int octave, const int diffIndex) const
{
    BorderType border = BorderType::Replicate;

    bool minimal = true;
    bool maximal = true;
    double targetValue = GetOctaveAt(octave)
            .GetDiffAt(diffIndex)
            .GetImage().get(x, y, border);
    double old = _octaves[octave].GetLayerAt(diffIndex+1).GetImage().get(x,y, border)
            -_octaves[octave].GetLayerAt(diffIndex).GetImage().get(x,y, border);
    for(int dz=-1; dz<=1; dz++)
    {
        for(int dx=-1;dx<=1;dx++)
        {
            for(int dy=-1;dy<=1; dy++)
            {

                    if(dx || dy || dz)
                    {
                        double diff = GetOctaveAt(octave)
                                .GetDiffAt(diffIndex + dz)
                                .GetImage().get(x + dx, y + dy, border);
                        double oldDiff = GetImageAt(octave, diffIndex + dz + 1).get(x + dx, y + dy, border)
                                - GetImageAt(octave, diffIndex + dz).get(x + dx, y + dy, border);
                        if(diff <= targetValue) minimal = false;
                        if(diff >= targetValue) maximal = false;
                    }
                }

        }
    }
    if(maximal) return DoGPointType::Maximal;
    if(minimal) return DoGPointType::Minimal;
    return DoGPointType::Regular;
}
