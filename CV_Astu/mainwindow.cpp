#include <QImage>
#include <QPainter>
#include <qlabel.h>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"
#include "imagehelper.h"
#include "doublemat.h"
#include "pyramid.h"
#include "interestingpointsdetector.h"
#include "descriptorsbuilder.h"

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
    auto detector = InterestingPointsDetector(DetectionMethod::Harris);

    const auto sourceImage1 = ImageHelper::Load("C:\\Users\\Alena\\Pictures\\blob\\2.jpg");
    const auto pyr1 = Pyramid(5, 3, 1.6, 0.5, sourceImage1);
    const auto points1 = detector.FindBlobBasedPoints(pyr1);
    auto ip1 = ImageHelper::MarkInterestingPoints(sourceImage1, points1);
    ip1.save("C:\\Users\\Alena\\Pictures\\blob\\ip1.jpg");

    ///*
    const auto sourceImage2 = ImageHelper::Load("C:\\Users\\Alena\\Pictures\\blob\\22.jpg");
    const auto pyr2 = Pyramid(10, 3, 1.6, 0.5, sourceImage2);
    const auto points2 = detector.FindBlobBasedPoints(pyr2);
    auto ip2 = ImageHelper::MarkInterestingPoints(sourceImage2, points2);
    ip2.save("C:\\Users\\Alena\\Pictures\\blob\\ip2.jpg");


    auto sup1 = InterestingPointsDetector::ANMS(points1, 200, sourceImage1.getWidth());
    auto sup2 = InterestingPointsDetector::ANMS(points2, 200, sourceImage2.getWidth());
    auto descriptorBuilder = DescriptorsBuilder();


    auto desc3 = descriptorBuilder.CalculateHistogramDesctiptors(pyr1, sup1);
    auto desc4 = descriptorBuilder.CalculateHistogramDesctiptors(pyr2, sup2);

    auto matches = DescriptorsBuilder::FindMatches(desc3, desc4);


    auto extended = ImageHelper::DrawMatches(sourceImage1, sourceImage2, matches);
    extended.save("C:\\Users\\Alena\\Pictures\\blob\\matches.jpg");
    //*/
}




