#ifndef CVIMAGE_H
#define CVIMAGE_H

#include <QObject>
#include <math.h>
#include <memory>

using namespace std;

class CVImage
{
protected:
    int _width, _height;
    unique_ptr<uchar[]> _data;
public:
    CVImage();
    CVImage(const int width, const int height);
    CVImage(const int width, const int height, uchar * rgbData);
};

#endif // IMAGE_H
