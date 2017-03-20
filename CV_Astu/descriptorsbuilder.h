#ifndef DECRIPTORSBUILDER_H
#define DECRIPTORSBUILDER_H
#define _USE_MATH_DEFINES

#include <vector>
#include <cmath>
#include <math.h>
#include <stdlib.h>

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
    const int G_ANGLES_COUNT = 8;
    const int GRID_HALFSIZE = GRID_SIZE/2;
    const int GRID_CELLS_COUNT = (GRID_SIZE * GRID_SIZE)/(GRID_STEP*GRID_STEP);
    const double G_ANGLE = 2 * M_PI / G_ANGLES_COUNT;

    DoubleMat CalculateGradients(const DoubleMat &xDrv, const DoubleMat &yDrv);
    DoubleMat CalculateGradientAngles(const DoubleMat& xDrv, const DoubleMat& yDrv);
    Descriptor CalculateSimpleDescriptor(DoubleMat& gradients,
                                         const InterestingPoint point);
    Descriptor CalculateHistogramDescriptor(DoubleMat& gradients, DoubleMat& angles,
                                         const InterestingPoint point);

public:

    DescriptorsBuilder();
    vector<Descriptor> CalculateSimpleDescriptors(const DoubleMat& source,
                                                  const vector<InterestingPoint> points);
    vector<Descriptor> CalculateHistogramDesctiptors(const DoubleMat& source,
                                                     const vector<InterestingPoint> points);
    static vector<pair<Point, Point>> FindMatches(const vector<Descriptor>& first,
                                                  const vector<Descriptor>& second);
    static double CalcDistance(const Descriptor& left, const Descriptor& right);
};

#endif // DECRIPTORSBUILDER_H
