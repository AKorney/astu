#ifndef KERNELBUILDER_H
#define KERNELBUILDER_H
#define _USE_MATH_DEFINES 

#include <memory>
#include <stdlib.h>
#include <math.h>
#include <cmath>

#include <doublemat.h>
using namespace  std;

class KernelBuilder
{
private:
    KernelBuilder(){}

public:
    static unique_ptr<DoubleMat> BuildSobelX();
    static unique_ptr<DoubleMat> BuildSobelY();

    static unique_ptr<DoubleMat> BuildGauss(double sigma);
};

#endif // KERNELBUILDER_H
