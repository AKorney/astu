#ifndef HOMOGRAPHYHELPER_H
#define HOMOGRAPHYHELPER_H
#include <gsl/gsl_linalg.h>
#include <gsl/gsl_blas.h>
#include <algorithm>
#include <numeric>
#include <random>

#include "interestingpointsdetector.h"

using namespace std;

class HomographyHelper
{
    static const int iterationsCount = 400;
    static void FillMatrixA(const vector<pair<InterestingPoint, InterestingPoint>> &mathces,
                           const vector<int> &indeciesMap, gsl_matrix* a);
public:
    HomographyHelper();
    static DoubleMat RANSAC(const vector<pair<InterestingPoint, InterestingPoint>> mathces);
};

#endif // HOMOGRAPHYHELPER_H
