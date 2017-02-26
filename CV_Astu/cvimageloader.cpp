#include "cvimageloader.h"

unique_ptr<CVImage> CVImageLoader::Load(QString filePath)
{
    auto qImage = QImage();
    qImage.load(filePath);
	auto format = qImage.format();
	if (qImage.format() == QImage::Format_RGB888)
	{
		qImage = qImage.convertToFormat(QImage::Format_RGB888);

		auto result = make_unique<CVImage>(qImage.bits(), qImage.width(), qImage.height());
		return move(result);
	}
	else
	{
		auto bytes = make_unique<unsigned char[]>(qImage.height() * qImage.width() * 3);
		for (int y = 0; y < qImage.height(); y++)
		{
			for (int x = 0; x < qImage.width(); x++)
			{
				int index = (y * qImage.width() + x) * 3;
				bytes[index] = qRed(qImage.pixel(x, y));
				bytes[index + 1] = qGreen(qImage.pixel(x, y));
				bytes[index + 2] = qBlue(qImage.pixel(x, y));
			}
		}
		auto result = make_unique<CVImage>(bytes.get(), qImage.width(), qImage.height());
		return move(result);
	}
}

void CVImageLoader::Save(QString filePath, CVImage *source)
{

	auto qImage = make_unique<QImage>(source->getWidth(), source->getHeight(), QImage::Format_RGB32);
	for (int y = 0; y < source->getHeight(); y++) 
	{
		for (int x = 0; x < source->getWidth(); x++) 
		{
			int color = (int)(source->get(x, y));
			qImage->setPixel(x, y, qRgb(color, color, color));
		}
	}
    qImage->save(filePath);
}

QImage CVImageLoader::CreateQImage(CVImage *source)
{
    auto qImage = QImage(source->GetImageData(), source->getWidth(), source->getHeight(),
                           QImage::Format_Grayscale8);
    return qImage;
}
