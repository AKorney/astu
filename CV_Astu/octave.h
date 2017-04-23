#ifndef OCTAVE_H
#define OCTAVE_H

#include <vector>

#include "OctaveLayer.h"



using namespace std;
class Octave
{
private:
    vector<OctaveLayer> _layers;
    vector<OctaveLayer> _diffs;
    int _number;
public:
    Octave();
    Octave(int number);
    Octave(const Octave& other);
    void AddLayer(const OctaveLayer& layer);
    void SaveAll();
    const OctaveLayer& GetLayerAt(const int index) const;
    void BuildDiffs();
    const OctaveLayer& GetDiffAt(const int index) const;
};

#endif // OCTAVE_H
