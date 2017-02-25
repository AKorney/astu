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
    static DoubleMat BuildSobelX();
    static DoubleMat BuildSobelY();

    static DoubleMat BuildGauss(double sigma);
	static DoubleMat BuildGaussX(double sigma);
	static DoubleMat BuildGaussY(double sigma);
};

#endif // KERNELBUILDER_H
