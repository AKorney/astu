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
	
	//auto image2 = image.get()->GaussianBlur(1.0, BorderType::Wrap);
	auto image2 = image->GaussianBlur(1.5, BorderType::Replicate);
	const auto qImage = CVImageLoader::CreateQImage(image2.get());
	auto pixmap = QPixmap();
	pixmap.convertFromImage(qImage);	
	QGraphicsScene *scene = new QGraphicsScene(this);
	scene->addPixmap(pixmap);
	ui->graphicsView->setScene(scene);	
	
	ui->graphicsView->show();
	
}




