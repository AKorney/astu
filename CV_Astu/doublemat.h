/*
 * Core
 * Platform-independent
 * Mutable
 * NOT resizable
 *
 *
 */
#ifndef DOUBLEMAT_H
#define DOUBLEMAT_H

#include <stdlib.h>
#include <memory>

using namespace std;

class DoubleMat
{
private:
    int _width, _height;
    unique_ptr<double[]> _data;
    DoubleMat();
public:

    DoubleMat(int width, int height);
    DoubleMat(unsigned char * byteData, int width, int height);
    DoubleMat(const DoubleMat& source);

    double get(int x, int y);
    void set(double value, int x, int y);
};

#endif // DOUBLEMAT_H
