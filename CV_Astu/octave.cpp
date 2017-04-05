#include "octave.h"
#include <QString>
#include "imagehelper.h"
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
    _diffs.assign(other._diffs.begin(), other._diffs.end());
    _number = other._number;
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
        ImageHelper::Save("C:\\Users\\Alena\\Pictures\\CV_Pyr\\Oct"
                        + QString::number(_number)
                        + "_La" + QString::number(i)
                        + "_L"
                        + QString::number(layer.GetSigmaLocal())
                        + "_G"
                        + QString::number(layer.GetSigmaGlobal())
                        + ".jpg"
                        , CVImage(layer.GetImage()));
    }
    for(auto& diff : _diffs)
    {
        ImageHelper::Save("C:\\Users\\Alena\\Pictures\\CV_Pyr\\diff\\Oct"
                        + QString::number(_number)
                        + "_GD"
                        + QString::number(diff.GetSigmaGlobal())
                        + ".jpg"
                        , CVImage(diff.GetImage()));
    }
}

const OctaveLayer& Octave::GetLayerAt(const int index) const
{
    return _layers.at(index);
}

void Octave::BuildDiffs()
{
    for(int i=0; i<_layers.size()-1; i++)
    {
        OctaveLayer diff(make_unique<DoubleMat>(_layers[i+1].GetImage().Sub(_layers[i].GetImage()))
                , (_layers[i].GetSigmaLocal() + _layers[i+1].GetSigmaLocal())/2
                , (_layers[i].GetSigmaGlobal() +_layers[i+1].GetSigmaGlobal())/2);
        _diffs.emplace_back(diff);
    }
}

const OctaveLayer &Octave::GetDiffAt(const int index) const
{
    return _diffs.at(index);
}

