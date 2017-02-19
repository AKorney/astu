#include "cvimageloader.h"

unique_ptr<CVImage> CVImageLoader::Load(QString filePath)
{
    auto qImage = QImage();
    qImage.load(filePath);
    qImage = qImage.convertToFormat(QImage::Format_RGB888);

    auto result = make_unique<CVImage>(qImage.bits(), qImage.width(), qImage.height());
    return move(result);
}

void CVImageLoader::Save(QString filePath, CVImage *source)
{
    auto qImage = QImage(source->GetImageData(), source->getWidth(), source->getHeight(),
                           QImage::Format_Grayscale8);
    qImage = qImage.convertToFormat(QImage::Format_ARGB32);
    qImage.save(filePath);
}

QImage CVImageLoader::CreateQImage(CVImage *source)
{
    auto qImage = QImage(source->GetImageData(), source->getWidth(), source->getHeight(),
                           QImage::Format_Grayscale8);
    return qImage;
}
