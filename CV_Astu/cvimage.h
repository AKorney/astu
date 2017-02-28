#ifndef CVIMAGE_H
/*
 * Core image class
 * Platform-independent
 */
#define CVIMAGE_H

#include <stdlib.h>
#include <math.h>
#include <memory>
#include <algorithm>

#include "doublemat.h"
#include "kernelbuilder.h"

using namespace std;



class CVImage
{
private:
    int _width, _height;
    unique_ptr<unsigned char[]> _data;
    

public:

	CVImage();
	CVImage(const int width, const int height);
    CVImage(const unsigned char * rgb24Data, const int width, const int height);
    CVImage(const CVImage& source);
    CVImage(CVImage&& other);
	CVImage(const DoubleMat& doubleData);
    CVImage& operator=(CVImage&& other);

	unsigned char * GetImageData() const;

	int getHeight()  const;
	int getWidth() const;

	DoubleMat PrepareDoubleMat() const;

	CVImage Convolve(const DoubleMat& kernel, const BorderType border) const;

	CVImage SobelX(const BorderType border);
	CVImage SobelY(const BorderType border);
	CVImage Sobel(const BorderType border);

	CVImage GaussianSmoothing(const double sigma, const BorderType border, bool useAxisSeparation = false);

	CVImage ScaleDown() const;

	unsigned char get(const int x, const int y, const BorderType borderType = BorderType::Constant) const;
};

#endif // IMAGE_H
