#ifndef CVIMAGE_H
/*
 * Core image class
 * Platform-independent
 */
#define CVIMAGE_H


#include <stdlib.h>
#include <math.h>
#include <memory>

using namespace std;

#define min(a,b) (a < b ? a : b)

struct DoubleMatrix
{

};


class CVImage
{
protected:
    int _width, _height;
    unique_ptr<unsigned char[]> _data;
public:
    CVImage();
	CVImage(const CVImage& source);
    CVImage(const int width, const int height);	
	CVImage(const unsigned char * rgb24Data, const int width, const int height);
    CVImage(CVImage&& other);
    CVImage& operator=(CVImage&& other);

	unsigned char * GetImageData();

    int getHeight() { return _height; }
    int getWidth() { return _width; }

    unsigned char get(int x, int y);

};

#endif // IMAGE_H
