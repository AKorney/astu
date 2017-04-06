#include "imagehelper.h"

CVImage ImageHelper::Load(QString filePath)
{
    auto qImage = QImage();
    qImage.load(filePath);
	auto format = qImage.format();
	if (qImage.format() == QImage::Format_RGB888)
	{
        auto result = CVImage(qImage.bits(), qImage.width(), qImage.height());
		return result;
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
		auto result = CVImage(bytes.get(), qImage.width(), qImage.height());
		return result;
	}
}

void ImageHelper::Save(const QString filePath, const CVImage &source)
{

	auto qImage = make_unique<QImage>(source.getWidth(), source.getHeight(), QImage::Format_RGB32);
	for (int y = 0; y < source.getHeight(); y++) 
	{
		for (int x = 0; x < source.getWidth(); x++) 
		{
			int color = (int)(source.get(x, y));
			qImage->setPixel(x, y, qRgb(color, color, color));
		}
	}
    qImage->save(filePath);
}

QImage ImageHelper::CreateQImage(const CVImage &source)
{
    auto qImage =  QImage(source.getWidth(), source.getHeight(), QImage::Format_RGB32);
    for (int y = 0; y < source.getHeight(); y++)
    {
        for (int x = 0; x < source.getWidth(); x++)
        {
            int color = (int)(source.get(x, y));
            qImage.setPixel(x, y, qRgb(color, color, color));
        }
    }
    return qImage;
}

QImage ImageHelper::MarkInterestingPoints
(const CVImage &source, vector<InterestingPoint> points)
{
    QImage qImage = ImageHelper::CreateQImage(source);
    QPainter painter(&qImage);
    for (auto &point : points) {
        painter.setPen(QColor(255, 255, 0));
        painter.drawEllipse(point.x, point.y, 2, 2);
    }
    return qImage;
}

QImage ImageHelper::DrawMatches
(const CVImage &left, const CVImage &right,  vector<pair<Point, Point> > matches)
{
     const int middle = 20;
     const int width = right.getWidth() + left.getWidth() + middle;
     const int height = max(left.getHeight(), right.getHeight());

     auto qImage =  QImage(width, height, QImage::Format_RGB32);
     for (int y = 0; y < left.getHeight(); y++)
     {
         for (int x = 0; x < left.getWidth(); x++)
         {
             int color = (int)(left.get(x, y));
             qImage.setPixel(x, y, qRgb(color, color, color));
         }
     }

     for (int y = 0; y < right.getHeight(); y++)
     {
         for (int x = 0; x < right.getWidth(); x++)
         {
             int color = (int)(right.get(x, y));
             qImage.setPixel(x + left.getWidth() + middle, y, qRgb(color, color, color));
         }
     }

     QPainter painter(&qImage);
     for(auto& match: matches)
     {
         //painter.setPen(QColor(255, 255, 0));
         painter.setPen(QColor(abs(rand()) % 256, abs(rand()) % 256, abs(rand()) % 256));
         painter.drawEllipse(match.first.x, match.first.y, 2, 2);
         painter.drawEllipse(match.second.x + left.getWidth() + middle, match.second.y, 2, 2);

         painter.drawLine(match.first.x, match.first.y,
                          match.second.x + left.getWidth() + middle, match.second.y);
     }
     return qImage;
}

QImage ImageHelper::DrawBlobs(const CVImage &source, vector<BlobDescription> blobs)
{
    QImage qImage = ImageHelper::CreateQImage(source);
    QPainter painter(&qImage);
    for (auto &blob : blobs) {
        painter.setPen(blob.pointType==DoGPointType::Maximal? QColor(0,255,0) : QColor(255,0,0));
        double r = sqrt(2)*blob.sigmaGlobal;
        painter.drawEllipse(QPoint(blob.x, blob.y), (int)r, (int)r);
    }
    return qImage;
}
