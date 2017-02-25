#include <QImage>
#include <qlabel.h>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"
#include "cvimageloader.h"
#include "doublemat.h"

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
	
	auto sobelResult = sourceImage->Sobel(BorderType::Reflect);
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
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\CV_Tests\\CUSTOM.jpg", &customResult);
	
}




