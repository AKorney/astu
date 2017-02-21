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
	auto sobelPixmap = QPixmap();
	sobelPixmap.convertFromImage(CVImageLoader::CreateQImage(sobelResult.get()));

	auto sobelScene = new QGraphicsScene(this);
	sobelScene->addPixmap(sobelPixmap);
	ui->graphicsView->setScene(sobelScene);
	ui->graphicsView->show();


	auto gaussianResult = sourceImage->GaussianBlur(1.5, BorderType::Reflect, true);
	auto gaussianPixmap = QPixmap();
	gaussianPixmap.convertFromImage(CVImageLoader::CreateQImage(gaussianResult.get()));

	auto gaussianScene = new QGraphicsScene(this);
	gaussianScene->addPixmap(gaussianPixmap);
	ui->graphicsView_2->setScene(gaussianScene);
	ui->graphicsView_2->show();

	double customKernelData[9] = { -2, -1, 0, -1, 1, 1, 0, 1, 2 };
	auto customKernel = make_unique<DoubleMat>(customKernelData, 3, 3);

	auto customResult = sourceImage->Convolve(customKernel, BorderType::Reflect);
	auto customPixmap = QPixmap();
	customPixmap.convertFromImage(CVImageLoader::CreateQImage(customResult.get()));

	auto customScene = new QGraphicsScene(this);
	customScene->addPixmap(customPixmap);
	ui->graphicsView_3->setScene(customScene);
	ui->graphicsView_3->show();

	//auto image2 = image.get()->GaussianBlur(1.0, BorderType::Wrap);
	/*auto image2 = image->GaussianBlur(1.5, BorderType::Replicate, false);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\Photo0415_sqGauss.jpg", image2.get());
	auto image3 = image->GaussianBlur(1.5, BorderType::Replicate, true);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\Photo0415_sepGauss.jpg", image3.get());

	double customKernelData[9] = { -2, -1, 0, -1, 1, 1, 0, 1, 2 };
	auto customKernel = make_unique<DoubleMat>(customKernelData, 3, 3);
	auto image4 = image->Convolve(customKernel, BorderType::Replicate);
	CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\Photo0415_Custom.jpg", image4.get());
	
	const auto qImage = CVImageLoader::CreateQImage(image2.get());
	auto pixmap = QPixmap();
	pixmap.convertFromImage(qImage);	
	QGraphicsScene *scene = new QGraphicsScene(this);
	scene->addPixmap(pixmap);
	ui->graphicsView->setScene(scene);	
	
	ui->graphicsView->show();
	
*/
}




