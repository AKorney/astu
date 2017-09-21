#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <qfiledialog.h>

#include <opencv2/core/core.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "FeaturesCollector.h"


using namespace cv;

FeaturesMap fc;
Vocabulary voc;
FullIndex idx;

MainWindow::MainWindow(QWidget *parent) :
    QMainWindow(parent),
    ui(new Ui::MainWindow)
{
    ui->setupUi(this);

	
}

void MainWindow::run()
{
	fc = FeaturesCollector::Features(ui->lineEdit->text());
	voc = FeaturesCollector::BuildBOWVocabulary(fc);
	idx = FeaturesCollector::BuildFullIndex(fc, voc);	
}

MainWindow::~MainWindow()
{
    delete ui;
	/*/
	*/
}

void MainWindow::selectBasePath()
{
	ui->lineEdit->setText(QFileDialog::getExistingDirectory(0, ("Select Image Base Folder"), QDir::currentPath()));
}

void MainWindow::selectImage()
{
	QFileDialog imagePicker(this);
	imagePicker.setFileMode(QFileDialog::ExistingFile);
	imagePicker.setNameFilter(tr("Images (*.png *.jpg)"));
	if (imagePicker.exec())
	{
		auto fileName = imagePicker.selectedFiles()[0];
		ui->lineEdit_2->setText(fileName);
		QImage qImage;
		qImage.load(fileName);
		auto pixmap = QPixmap();
		pixmap.convertFromImage(qImage);
		QGraphicsScene *scene = new QGraphicsScene(this);
		scene->addPixmap(pixmap);
		ui->graphicsView->setScene(scene);
		ui->graphicsView->show();
	}
}

void MainWindow::search()
{
	


	const auto res = FeaturesCollector::RequestNNearest(ui->lineEdit_2->text(), 3, idx, voc);

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
