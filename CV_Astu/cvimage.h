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


using namespace std;

#define min(a,b) (a < b ? a : b)

enum class BorderType { Constant, Replicate, Reflect, Wrap};


class CVImage
{
private:
    int _width, _height;
    unique_ptr<unsigned char[]> _data;

    CVImage();
    CVImage(const int width, const int height);	

	void Convolve(const unique_ptr<DoubleMat>& calculationBuffer, const unique_ptr<DoubleMat>& kernel, BorderType border);
public:

    CVImage(const unsigned char * rgb24Data, const int width, const int height);
    CVImage(const CVImage& source);
    CVImage(CVImage&& other);
	CVImage(const unique_ptr<DoubleMat>& doubleData);
    CVImage& operator=(CVImage&& other);

	unsigned char * GetImageData();

    int getHeight() { return _height; }
    int getWidth() { return _width; }

	unique_ptr<DoubleMat> PrepareDoubleMat();

	unique_ptr<CVImage> Convolve(const unique_ptr<DoubleMat>& kernel, BorderType border);

	unique_ptr<CVImage> SobelX(BorderType border);
	unique_ptr<CVImage> SobelY(BorderType border);
	unique_ptr<CVImage> Sobel(BorderType border);

    //unsigned char get(int x, int y);
	unsigned char get(int x, int y, BorderType borderType = BorderType::Constant);
};

#endif // IMAGE_H
