#ifndef DECRIPTORSBUILDER_H
#define DECRIPTORSBUILDER_H

#include <vector>

#include "interestingpointsdetector.h"
#include "doublemat.h"
#include "kernelbuilder.h"


using namespace std;

struct Descriptor
{
    InterestingPoint targetPoint;
    vector<double> localDescription;
};

class DescriptorsBuilder
{
private:
    const int gridSize = 16;
    const int gridStep = 4;
    const int hgBinsCount = 8;
    const int offset = gridSize/2;
    const int gridCellsCount = (gridSize * gridSize)/(gridStep*gridStep);

    const DoubleMat CalculateGradients(const DoubleMat& source);
    Descriptor CalculateSimpleDescriptor(const DoubleMat& gradients,
                                         const InterestingPoint point);
public:

    DescriptorsBuilder();
    vector<Descriptor> CalculateSimpleDescriptors(const DoubleMat& source,
                                                  const vector<InterestingPoint> points);

};

#endif // DECRIPTORSBUILDER_H
