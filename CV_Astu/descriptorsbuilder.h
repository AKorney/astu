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
    const int GRID_SIZE = 16;
    const int GRID_STEP = 4;
    const int HOG_ANGLES_COUNT = 8;
    const int GRID_HALFSIZE = GRID_SIZE/2;
    const int GRID_CELLS_COUNT = (GRID_SIZE * GRID_SIZE)/(GRID_STEP*GRID_STEP);

    DoubleMat CalculateGradients(const DoubleMat& source);
    Descriptor CalculateSimpleDescriptor(DoubleMat& gradients,
                                         const InterestingPoint point);

public:

    DescriptorsBuilder();
    vector<Descriptor> CalculateSimpleDescriptors(const DoubleMat& source,
                                                  const vector<InterestingPoint> points);
    static vector<pair<Point, Point>> FindMatches(const vector<Descriptor>& first,
                                                  const vector<Descriptor>& second);
    static double CalcDistance(const Descriptor& left, const Descriptor& right);
};

#endif // DECRIPTORSBUILDER_H
