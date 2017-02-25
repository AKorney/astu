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

enum class BorderType { Constant, Replicate, Reflect, Wrap };

class DoubleMat
{
private:
    int _width, _height;
    unique_ptr<double[]> _data;

public:
	DoubleMat();
    DoubleMat(const int width,const int height);
    DoubleMat(const DoubleMat& source);
	DoubleMat(const unique_ptr<unsigned char[]>& byteSource, const int width, const int height);
    DoubleMat(const double * doubleSource, const int width, const int height);
	DoubleMat(DoubleMat&& other); 
	DoubleMat& operator=(DoubleMat&& other);
	int getHeight() const;
	int getWidth() const;

    double get(const int x, const int y) const;
    void set(const double value, const int x, const int y);

	unique_ptr<double[]> GetNormalizedData(double newMin, double newMax) const;
	double get(const int x, const int y, BorderType borderType = BorderType::Constant) const;


	DoubleMat Convolve(const DoubleMat& kernel, BorderType border);
};


#endif // DOUBLEMAT_H
