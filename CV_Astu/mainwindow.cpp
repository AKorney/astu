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
#include "homographyhelper.h"

#include "houghobjectsearch.h"


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
   // const vector<pair<InterestingPoint,InterestingPoint>> vec();

    ///*
    auto detector = InterestingPointsDetector(DetectionMethod::Harris);
    QString leftPath = "C:\\Users\\Alena\\Pictures\\blob\\scene.jpg";
    QString rightPath = "C:\\Users\\Alena\\Pictures\\blob\\box.jpg";
    auto sourceImage1 = ImageHelper::Load(leftPath);
    auto sX = sourceImage1.SobelX(BorderType::Replicate);
    ImageHelper::Save("C:\\Users\\Alena\\Pictures\\sx.jpg",sX);
    auto sY = sourceImage1.SobelY(BorderType::Replicate);
    ImageHelper::Save("C:\\Users\\Alena\\Pictures\\sy.jpg", sY);
    auto s = sourceImage1.Sobel(BorderType::Replicate);
    ImageHelper::Save("C:\\Users\\Alena\\Pictures\\s.jpg", s);
    //const auto pyr1 = Pyramid(10, 3, 1.6, 0.5, sourceImage1);
    //const auto points1 = detector.FindBlobBasedPoints(pyr1);
    //auto ip1 = ImageHelper::MarkInterestingPoints(sourceImage1, points1);
    //ip1.save("C:\\Users\\Alena\\Pictures\\blob\\ip1.jpg");


    //const auto sourceImage2 = ImageHelper::Load(rightPath);
    //const auto pyr2 = Pyramid(10, 3, 1.6, 0.5, sourceImage2);
    //const auto points2 = detector.FindBlobBasedPoints(pyr2);
    //auto ip2 = ImageHelper::MarkInterestingPoints(sourceImage2, points2);
    //ip2.save("C:\\Users\\Alena\\Pictures\\blob\\ip2.jpg");
    //
    //houghobjectsearch search;
    //auto poses = search.FindPoses(sourceImage1, sourceImage2);
    //QImage scene;
    //scene.load(leftPath);
    //QImage object;
    //object.load(rightPath);
    //auto test = ImageHelper::DrawPoses(scene,object,poses);
    //test.save("C:\\Users\\Alena\\Pictures\\blob\\result.jpg");
/*
    auto sup1 = InterestingPointsDetector::ANMS(points1, 200, sourceImage1.getWidth());
    auto sup2 = InterestingPointsDetector::ANMS(points2, 200, sourceImage2.getWidth());
    auto descriptorBuilder = DescriptorsBuilder();


    auto desc3 = descriptorBuilder.CalculateHistogramDesctiptors(pyr1, sup1);
    auto desc4 = descriptorBuilder.CalculateHistogramDesctiptors(pyr2, sup2);

    auto matches = DescriptorsBuilder::FindMatches(desc3, desc4);
    auto extended = ImageHelper::DrawMatches(sourceImage1, sourceImage2, matches);
    extended.save("C:\\Users\\Alena\\Pictures\\blob\\matches.jpg");

    auto homography = HomographyHelper::RANSAC(matches);




    QImage leftOrigial, rightOriginal;
    leftOrigial.load(leftPath);
    rightOriginal.load(rightPath);
    auto stitched = ImageHelper::DrawStitching(
                leftOrigial,
                rightOriginal,
                homography);
    stitched.save("C:\\Users\\Alena\\Pictures\\blob\\stitch.jpg");

    //*/
}




