#include <QImage>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"
#include "cvimageloader.h"

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
    
    const auto image = CVImageLoader::Load("C:\\Users\\Alena\\Pictures\\1.jpg");
	const auto qImage = CVImageLoader::CreateQImage(image.get());
	auto pixmap = QPixmap();
	pixmap.convertFromImage(qImage);	
	QGraphicsScene *scene = new QGraphicsScene(this);
	scene->addPixmap(pixmap);
	ui->graphicsView->setScene(scene);	
	
	ui->graphicsView->show();
	
}




