#include <QImage>

#include "mainwindow.h"
#include "ui_mainwindow.h"

#include "cvimage.h"

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
    QImage image = QImage();
    image.load("C:\\Users\\Alena\\Desktop\\Capture.PNG");
    image = image.convertToFormat(QImage::Format_RGB888);
    auto source = make_unique<CVImage>(image.width(), image.height(),
                                       image.bits());

}

void MainWindow::on_saveButton_clicked()
{

}


