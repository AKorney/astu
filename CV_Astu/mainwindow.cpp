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


    auto points1 = detector.FindInterestingPoints(sourceImage1.PrepareDoubleMat()
                                                 .Convolve(KernelBuilder::BuildGauss(1),
                                                           BorderType::Replicate),
                                                 3, 0.075);
    auto points2 = detector.FindInterestingPoints(sourceImage2.PrepareDoubleMat()
                                                 .Convolve(KernelBuilder::BuildGauss(1),
                                                           BorderType::Replicate),
                                                 3, 0.075);

    auto descriptorBuilder = DescriptorsBuilder();
    auto descriptors1 = descriptorBuilder.CalculateSimpleDescriptors
            (sourceImage1.PrepareDoubleMat(), points1);
    auto descriptors2 = descriptorBuilder.CalculateSimpleDescriptors
            (sourceImage2.PrepareDoubleMat(), points2);
    auto matches = DescriptorsBuilder::FindMatches(descriptors1, descriptors2);
    auto extended = ImageHelper::DrawMatches(sourceImage1, sourceImage2, matches);
    extended.save("C:\\Users\\Alena\\Pictures\\descr\\matches.jpg");
}




