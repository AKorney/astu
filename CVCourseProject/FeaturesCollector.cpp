#include "FeaturesCollector.h"


#include <qtextstream.h>
#include <opencv2\opencv.hpp>
#include <opencv2\core.hpp>

map<QString, vector<Descriptor>> FeaturesCollector::Features(const QString imagesDirectoryPath)
{
	map<QString, vector<Descriptor>> featuresMap;

	QDir directory(imagesDirectoryPath);
	QStringList filters;
	filters << "*.png" << "*.jpg" << "*.bmp";
	const auto imageFileInfos = directory.entryInfoList(filters, QDir::Files | QDir::NoDotAndDotDot);

	InterestingPointsDetector detector(DetectionMethod::Harris);
	DescriptorsBuilder builder;

	for (auto& fileInfo : imageFileInfos)
	{
		const auto image = ImageHelper::Load(fileInfo.absoluteFilePath());
		const auto pyr = Pyramid(5, 3, 1.6, 0.5, image);
		const auto points = detector.FindBlobBasedPoints(pyr);
		const auto descriptors = builder.CalculateHistogramDesctiptors(pyr, points);
		featuresMap[fileInfo.absoluteFilePath()] = descriptors;
	}
	return featuresMap;
}

vector<Descriptor> FeaturesCollector::BuildBOWVocabulary(const map<QString, vector<Descriptor>>& features)
{
	vector<Descriptor> vocabulary;

	Mat featuresMat;// (0, 128, CV_32F);
	for (auto &feature : features)
	{
		for (const Descriptor &descr : feature.second)
		{
			Mat descriptorMat(descr.localDescription, true);
			descriptorMat = descriptorMat.t();
			featuresMat.push_back(descriptorMat);
		}
	}

	int dictionarySize = 2;

	FileStorage fs("featuresCombinedF.yml", FileStorage::WRITE);
	fs << "featuresMat" << featuresMat;
	fs.release();

	Mat fMat;
	featuresMat.convertTo(fMat, CV_32F);

	BOWKMeansTrainer trainer(150);
	trainer.add(fMat);
	
	Mat vocabF = trainer.cluster();
	FileStorage fs2("vocabF.yml", FileStorage::WRITE);
	fs2 << "vocabF" << vocabF;
	fs2.release();

	vocabF.convertTo(vocabF, CV_64F);
	FileStorage fs3("vocab.yml", FileStorage::WRITE);
	fs3 << "vocab" << vocabF;
	fs3.release();

	for (int i = 0; i < vocabF.rows; i++)
	{
		Descriptor word;
		vocabF.row(i).copyTo(word.localDescription);
		vocabulary.emplace_back(word);
	}

	return vocabulary;
}

FeaturesCollector::FeaturesCollector()
{
}


FeaturesCollector::~FeaturesCollector()
{
}
