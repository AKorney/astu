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
    auto detector = HarrisDetector(sourceImage.PrepareDoubleMat());
    auto diffs = detector.CalculateDiffs(3, BorderType::Replicate);
    CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\Diffs.jpg", CVImage(diffs));
    auto points = (&detector)->FindInterestingPoints(3);
    QImage qImage = CVImageLoader::CreateQImage(sourceImage);
    QPainter painter(&qImage);
    for (auto &point : points) {
        painter.setPen(QColor(255, 255, 0));
        painter.drawEllipse(point.x, point.y, 2, 2);
    }

    qImage.save("C:\\Users\\Alena\\Pictures\\IPDTEST.jpg");
}




