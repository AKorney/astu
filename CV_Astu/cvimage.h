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

    int getHeight() { return _height; }
    int getWidth() { return _width; }

	DoubleMat PrepareDoubleMat();

	CVImage Convolve(const DoubleMat& kernel, BorderType border);

	CVImage SobelX(BorderType border);
	CVImage SobelY(BorderType border);
	CVImage Sobel(BorderType border);

	CVImage GaussianSmoothing(const double sigma, BorderType border, bool useAxisSeparation = false);


	unsigned char get(const int x, const int y, BorderType borderType = BorderType::Constant);
};

#endif // IMAGE_H
