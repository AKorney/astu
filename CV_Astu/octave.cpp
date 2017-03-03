#include "octave.h"
#include <QString>
#include "cvimageloader.h"
Octave::Octave():
    _number(-1)
{

}

Octave::Octave(int number):
    _number(number)
{

}

Octave::Octave(const Octave &other)
{
    _layers.assign(other._layers.begin(), other._layers.end());
}

void Octave::AddLayer(const OctaveLayer& layer)
{
    _layers.emplace_back(layer);

}

void Octave::SaveAll()
{
    for(int i=0;i<_layers.size(); i++)
    {
        auto layer = _layers.at(i);
        CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Pyr\\Oct"
                        + QString::number(_number)
                        + "_La" + QString::number(i)
                        + "_L"
                        + QString::number(layer.GetSigmaLocal())
                        + "_G"
                        + QString::number(layer.GetSigmaGlobal())
                        + ".jpg"
                        , CVImage(layer.GetImage()));
    }
}

const OctaveLayer& Octave::GetLayerAt(const int index) const
{
    return _layers.at(index);
}

