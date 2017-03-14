#include <QImage>
#include <QPainter>
#include <qlabel.h>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"
#include "cvimageloader.h"
#include "doublemat.h"
#include "pyramid.h"
#include "moravecdetector.h"
#include "harrisdetector.h"

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}



void MainWindow::on_openButton_clicked()
{
    
	const auto sourceImage = CVImageLoader::Load(ui->lineEdit->text());
    auto detector = InterestingPointsDetector(DetectionMethod::Harris);
    //auto diffs = detector.CalculateDiffs(3, BorderType::Replicate);
    //CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\Diffs.jpg", CVImage(diffs));
    auto points = detector.FindInterestingPoints(sourceImage.PrepareDoubleMat()
                                                 .Convolve(KernelBuilder::BuildGauss(1),
                                                           BorderType::Replicate),
                                                 3, 0.015);
    QImage qImage = CVImageLoader::CreateQImage(sourceImage);
    QPainter painter(&qImage);
    for (auto &point : points) {
        painter.setPen(QColor(255, 255, 0));
        painter.drawEllipse(point.x, point.y, 2, 2);
    }
    qImage.save("C:\\Users\\Alena\\Pictures\\pointsHarris.jpg");

    auto anmsResult = detector.ANMS(points, 250,
                                    max(sourceImage.getWidth(), sourceImage.getHeight()));
    QImage qImage2 = CVImageLoader::CreateQImage(sourceImage);
    QPainter painter2(&qImage2);
    for (auto &point : anmsResult) {
        painter2.setPen(QColor(255, 255, 0));
        painter2.drawEllipse(point.x, point.y, 2, 2);
    }
    qImage2.save("C:\\Users\\Alena\\Pictures\\anms.jpg");
}




