#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "FeaturesCollector.h"


using namespace cv;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

	const auto fc = FeaturesCollector::Features("C:\\Users\\Alena\\Pictures\\640");
	const auto voc = FeaturesCollector::BuildBOWVocabulary(fc);
	const auto idx = FeaturesCollector::BuildFullIndex(fc, voc);

	const auto res = FeaturesCollector::RequestNNearest("C:\\Users\\Alena\\Pictures\\testIm\\2.jpg", 3, idx, voc);

	for (int i = 0; i < res.size(); i++)
	{
		Mat image;
		image = imread(res[i].toStdString(), CV_LOAD_IMAGE_COLOR);   // Read the file

		if (image.data)                              // Check for invalid input
		{
			QString wn = QString::number(i) + "//" + QString::number(res.size());
			namedWindow(wn.toStdString(), WINDOW_AUTOSIZE);// Create a window for display.
			imshow(wn.toStdString(), image);
		}
	}
}

MainWindow::~MainWindow()
{
    delete ui;
}
