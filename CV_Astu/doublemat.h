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
#include <algorithm>

using namespace std;

class DoubleMat
{
private:
    int _width, _height;
    unique_ptr<double[]> _data;
    DoubleMat();
public:

    DoubleMat(const int width,const int height);
    DoubleMat(const DoubleMat& source);
	DoubleMat(const unique_ptr<unsigned char[]>& byteSource, const int width, const int height);
	int getHeight() { return _height; }
	int getWidth() { return _width; }

    double get(const int x, const int y) const;
    void set(const double value, const int x, const int y);

	unique_ptr<double[]> GetNormalizedData(double newMin, double newMax);
};

#endif // DOUBLEMAT_H
