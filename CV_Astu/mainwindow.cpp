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
    
    const auto sourceImage1 = ImageHelper::Load("C:\\Users\\Alena\\Pictures\\descr\\test1.png");
    const auto sourceImage2 = ImageHelper::Load("C:\\Users\\Alena\\Pictures\\descr\\test2.png");
    auto detector = InterestingPointsDetector(DetectionMethod::Harris);

    auto im1DM = sourceImage1.PrepareDoubleMat();
    auto im2DM = sourceImage2.PrepareDoubleMat();
    auto points1 = detector.FindInterestingPoints(im1DM
                                                 .Convolve(KernelBuilder::BuildGauss(1),
                                                           BorderType::Replicate),
                                                 3, 0.075);
    auto points2 = detector.FindInterestingPoints(im2DM
                                                 .Convolve(KernelBuilder::BuildGauss(1),
                                                           BorderType::Replicate),
                                                 3, 0.075);

    auto descriptorBuilder = DescriptorsBuilder();
    auto descriptors1 = descriptorBuilder.CalculateSimpleDescriptors
            (im1DM, points1);
    auto descriptors2 = descriptorBuilder.CalculateSimpleDescriptors
            (im2DM, points2);

    auto desc3 = descriptorBuilder.CalculateHistogramDesctiptors
            (im1DM, points1);
    auto desc4 = descriptorBuilder.CalculateHistogramDesctiptors
            (im2DM, points2);
    auto matches = DescriptorsBuilder::FindMatches(desc3, desc4);
    auto ip1 = ImageHelper::MarkInterestingPoints(sourceImage1, points1);
    ip1.save("C:\\Users\\Alena\\Pictures\\descr\\ip1.jpg");
    auto ip2 = ImageHelper::MarkInterestingPoints(sourceImage2, points2);
    ip2.save("C:\\Users\\Alena\\Pictures\\descr\\ip2.jpg");
    auto extended = ImageHelper::DrawMatches(sourceImage1, sourceImage2, matches);
    extended.save("C:\\Users\\Alena\\Pictures\\descr\\matches.jpg");
}




