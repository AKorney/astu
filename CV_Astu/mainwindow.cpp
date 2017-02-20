#include <QImage>

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
    
    const auto image = CVImageLoader::Load("C:\\Users\\Alena\\Pictures\\Photo0415.jpg");
	auto mat = make_unique<DoubleMat>(3,3);
	mat.get()->set(1.0 / 9, 0, 0); mat.get()->set(1.0 / 9, 0, 1); mat.get()->set(1.0 / 9, 0, 2);
	mat.get()->set(1.0 / 9, 1, 0); mat.get()->set(1.0 / 9, 1, 1); mat.get()->set(1.0 / 9, 1, 2);
	mat.get()->set(1.0 / 9, 2, 0); mat.get()->set(1.0 / 9, 2, 1); mat.get()->set(1.0/9, 2, 2);
	auto image2 = image.get()->Sobel(BorderType::Constant);
	const auto qImage = CVImageLoader::CreateQImage(image2.get());
	auto pixmap = QPixmap();
	pixmap.convertFromImage(qImage);	
	QGraphicsScene *scene = new QGraphicsScene(this);
	scene->addPixmap(pixmap);
	ui->graphicsView->setScene(scene);	
	
	ui->graphicsView->show();
	
}




