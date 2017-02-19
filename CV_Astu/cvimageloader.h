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
    static unique_ptr<CVImage> Load(QString filePath);
    static QImage CreateQImage(CVImage * source);
    static void Save(QString filePath, CVImage * source);
};

#endif // CVIMAGELOADER_H
