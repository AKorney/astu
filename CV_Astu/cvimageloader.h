#ifndef CVIMAGELOADER_H
#define CVIMAGELOADER_H
#include <QImage>

#include <memory>

#include "cvimage.h"

class CVImageLoader
{

private:
    CVImageLoader() {}
public:
    static CVImage Load(QString filePath);
    static QImage CreateQImage(const CVImage &source);
	static void Save(const QString filePath, const CVImage & source);
};

#endif // CVIMAGELOADER_H
