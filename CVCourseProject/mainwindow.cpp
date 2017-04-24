#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <opencv2\features2d.hpp>

#include "FeaturesCollector.h"


using namespace cv;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

	const auto fc = FeaturesCollector::Features("C:\\Users\\Alena\\Pictures\\BOWTest");
	const auto voc = FeaturesCollector::BuildBOWVocabulary(fc);
	const auto idx = FeaturesCollector::BuildInvertFile(fc, voc);

	const auto res = FeaturesCollector::RequestNNearest("C:\\Users\\Alena\\Pictures\\testBow.jpg", 5, idx, voc);
}

MainWindow::~MainWindow()
{
    delete ui;
}
