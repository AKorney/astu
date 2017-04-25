#include "FeaturesCollector.h"

#include <qtextstream.h>
#include <opencv2\opencv.hpp>
#include <opencv2\core.hpp>

const map<QString, vector<Descriptor>> FeaturesCollector::Features(const QString imagesDirectoryPath)
{
	FeaturesMap featuresMap;

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

const Vocabulary FeaturesCollector::BuildBOWVocabulary(const FeaturesMap& features)
{
	Vocabulary vocabulary;

	Mat featuresMat;
	for (auto &feature : features)
	{
		for (const Descriptor &descr : feature.second)
		{
			Mat descriptorMat(descr.localDescription, true);
			descriptorMat = descriptorMat.t();
			featuresMat.push_back(descriptorMat);
		}
	}

		
	Mat fMat;
	featuresMat.convertTo(fMat, CV_32F);

	BOWKMeansTrainer trainer(500);
	trainer.add(fMat);
	
	Mat vocabF = trainer.cluster();
	vocabF.convertTo(vocabF, CV_64F);

	for (int i = 0; i < vocabF.rows; i++)
	{
		Descriptor word;
		vocabF.row(i).copyTo(word.localDescription);
		vocabulary.emplace_back(word);
	}

	return vocabulary;
}

const FullIndex FeaturesCollector::BuildFullIndex(const FeaturesMap& featureMap, const Vocabulary& vocabulary)
{
	InvertedFile index;
	FrequencyHistogramsMap histogramsMap;
	for (auto& imageFeatures : featureMap)
	{
		histogramsMap[imageFeatures.first].resize(vocabulary.size(), 0.0);
		int imageWordsCount = imageFeatures.second.size();

		for (auto& descriptor : imageFeatures.second)
		{
			int word = WordIndex(vocabulary, descriptor);
			//update inverted file			
			index[word].emplace(imageFeatures.first);
					
			//update raw histogram value
			histogramsMap[imageFeatures.first][word] += 1.0 / imageWordsCount;
		}
	}
	vector<double> idf;
	
	const int numOfDocs = featureMap.size();
	for (auto& wordInfo : index)
	{
		idf.emplace_back(1.0*numOfDocs / wordInfo.second.size());
	}

	for (auto& histogramInfo : histogramsMap)
	{
		for (int i = 0; i < histogramInfo.second.size(); i++)
		{
			histogramInfo.second[i] *= idf[i];
		}
	}

	return { index, histogramsMap };
}

const vector<QString> FeaturesCollector::RequestNNearest(const QString targetImagePath, const int count, 
	const FullIndex& index, const Vocabulary& vocabulary)
{
	InterestingPointsDetector detector(DetectionMethod::Harris);
	DescriptorsBuilder builder;

	const auto image = ImageHelper::Load(targetImagePath);
	const auto pyr = Pyramid(5, 3, 1.6, 0.5, image);
	const auto points = detector.FindBlobBasedPoints(pyr);
	const auto descriptors = builder.CalculateHistogramDesctiptors(pyr, points);

	map<QString, int> counterMap;
	vector<double> histogram;
	vector<double> idf;

	const int numOfDocs = index.histograms.size();
	for (auto& wordInfo : index.invertedFile)
	{
		idf.emplace_back(1.0*numOfDocs / wordInfo.second.size());
	}

	histogram.resize(vocabulary.size(), 0);

	for (auto& descr : descriptors)
	{
		int word = WordIndex(vocabulary, descr);
		
		const auto& files = index.invertedFile.at(word);
		for (auto& fileName : files)
		{
			if (counterMap.find(fileName) == counterMap.end())
			{
				counterMap[fileName] = 0;
			}
			counterMap[fileName]++;
		}
		histogram[word] += idf[word] / descriptors.size();
	}
	auto cmp = [](std::pair<QString, double> const & a, std::pair<QString, double> const & b)
	{
		return a.second != b.second ? a.second > b.second : a.first < b.first;
	};
	vector<pair<QString, double>> data;
	//collect all non-zero images into data vector
	for (auto& value : counterMap)
	{
		if (value.second > 0)
		{
			double simValue = 0;
			for (int i = 0; i < vocabulary.size(); i++)
			{
				simValue += histogram[i] * index.histograms.at(value.first)[i];
			}
			data.emplace_back(value.first, simValue);
		}
	}

	sort(data.begin(), data.end(), cmp);
	
	vector<QString> nearest;
	for (int i = 0; i < min(count, (int)data.size()); i++)
	{
		nearest.emplace_back(data[i].first);
	}
	return nearest;
}

int FeaturesCollector::WordIndex(const Vocabulary& vocabulary, const Descriptor & target)
{
	double minDistance = INFINITY;
	int nearestWordIndex;
	for (int w = 0; w < vocabulary.size(); w++)
	{
		double dist = DescriptorsBuilder::CalcDistance(vocabulary[w], target);
		if (dist < minDistance)
		{
			minDistance = dist;
			nearestWordIndex = w;
		}
	}
	return nearestWordIndex;
}



FeaturesCollector::FeaturesCollector()
{
}


FeaturesCollector::~FeaturesCollector()
{
}
