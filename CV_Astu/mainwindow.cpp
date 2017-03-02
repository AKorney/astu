#include <QImage>
#include <qlabel.h>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"
#include "cvimageloader.h"
#include "doublemat.h"
#include "pyramid.h"

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
	const auto pyr = Pyramid(3, 2, 1, 0.5, sourceImage);
    pyr.L(100,100, 2.5);
	/*
	const double imsource[16] = {1.0, 0.0, 1.0, 0.0,
								 0.0, 0.0, 0.0, 0.0,
								 0.0, 0.0, 0.0, 0.0,
								 0.0, 0.0, 0.0, 0.0 };
	auto mat = DoubleMat(imsource, 4, 4);
	const auto sourceImage = make_unique<CVImage>(mat);
	//*/
	/*CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\source.jpg", sourceImage);
	auto scaled = sourceImage.ScaleDown();
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\scaled.jpg", scaled);


	const auto testImage = CVImageLoader::Load("C:\\Users\\Alena\\Pictures\\CV_Tests\\scaled.jpg");
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\scaledrrewr.jpg", testImage);*/

	/*auto sobelResult = sourceImage->Sobel(BorderType::Reflect);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\SOBEL.jpg", &sobelResult);

	auto sobelXResult = sourceImage->SobelX(BorderType::Reflect);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\SOBEL_X.jpg", &sobelXResult);

	auto sobelYResult = sourceImage->SobelY(BorderType::Reflect);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\SOBEL_Y.jpg", &sobelYResult);


	auto gaussianResult = sourceImage->GaussianSmoothing(1.5, BorderType::Reflect, true);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\GAUSS.jpg", &gaussianResult);
	

	double customKernelData[9] = { -2, -1, 0, -1, 1, 1, 0, 1, 2 };
	auto customKernel = make_unique<DoubleMat>(customKernelData, 3, 3);

	auto customResult = sourceImage->Convolve(*customKernel.get(), BorderType::Reflect);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\CUSTOM.jpg", &customResult);*/
	
}




