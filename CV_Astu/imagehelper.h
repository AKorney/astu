#ifndef CVIMAGELOADER_H
#define CVIMAGELOADER_H
#include <QImage>
#include <QPainter>
#include <memory>

#include "cvimage.h"
#include "interestingpointsdetector.h"
#include "pyramid.h"

class ImageHelper
{
/*
 *     QImage qImage = ImageHelper::CreateQImage(sourceImage);
    QPainter painter(&qImage);
    for (auto &point : points) {
        painter.setPen(QColor(255, 255, 0));
        painter.drawEllipse(point.x, point.y, 2, 2);
    }
 */
private:
    ImageHelper() {}
public:
    static CVImage Load(QString filePath);
    static QImage CreateQImage(const CVImage &source);
	static void Save(const QString filePath, const CVImage & source);
    static QImage MarkInterestingPoints(const CVImage& source, vector<InterestingPoint> points);
    static QImage DrawMatches(const CVImage& left, const CVImage& right,
                              vector <pair<InterestingPoint,InterestingPoint>> matches);
    static QImage DrawBlobs(const CVImage& source, vector<InterestingPoint> blobs);
};

#endif // CVIMAGELOADER_H
