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
    const int BINS_COUNT = 8;
    const int GRID_HALFSIZE = GRID_SIZE/2;
    const int GRID_CELLS_COUNT = (GRID_SIZE * GRID_SIZE)/(GRID_STEP*GRID_STEP);
    const double G_ANGLE = 2 * M_PI / BINS_COUNT;
    const int ORIENTATION_BINS_COUNT = 36;
    const double ORIENTATION_ANGLE_STEP = 2 * M_PI / ORIENTATION_BINS_COUNT;

    /*
     * Что сделать?
     * 1. убрать предварительный рассчет градиентов и углов, считать по запросу в точке
     * 2. передавать вместо целевого изображения пирамиду, и используя методы доступа к
     * данным из пирамиды вычисление нужного слоя, находить целевую картинку и считать дескриптор
     * 3. Внести корректировки в вычисление сетки
     *
     *
     *
     */

    Descriptor CalculateHistogramDescriptor(const DoubleMat& source,
                                            const InterestingPoint &point, const double alpha) const;
    vector<double> CalculateHistogram(const DoubleMat& source,
                                      const InterestingPoint &point, const int gridSize, const int gridStep,
                                      const int bins, const double sigma, const double alpha = 0) const;
    double CalculateNorm(const vector<double>& histogram) const;
    vector<double> DescriptorOrientations( const vector<double>& anglesHistogram) const;
public:

    DescriptorsBuilder();

    vector<Descriptor> CalculateHistogramDesctiptors(const DoubleMat& source,
                                                     const vector<InterestingPoint> points) const;
    static vector<pair<Point, Point>> FindMatches(const vector<Descriptor>& first,
                                                  const vector<Descriptor>& second);
    static double CalcDistance(const Descriptor& left, const Descriptor& right);
};

#endif // DECRIPTORSBUILDER_H
