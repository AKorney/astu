#include "FeaturesCollector.h"

#include <qtextstream.h>
#include <opencv2\opencv.hpp>
#include <opencv2\core.hpp>

const map<QString, vector<Descriptor>> FeaturesCollector::Features(const QString imagesDirectoryPath)
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

const vector<Descriptor> FeaturesCollector::BuildBOWVocabulary(const map<QString, vector<Descriptor>>& features)
{
	vector<Descriptor> vocabulary;

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

	BOWKMeansTrainer trainer(200);
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

const map<int, set<QString>> FeaturesCollector::BuildInvertFile(const map<QString, vector<Descriptor>>& featureMap, const vector<Descriptor>& vocabulary)
{
	map<int, set<QString>> index;
	for (auto& imageFeatures : featureMap)
	{
		for (auto& descriptor : imageFeatures.second)
		{
			int word = WordIndex(vocabulary, descriptor);
			index[word].emplace(imageFeatures.first);
		}
	}
	return index;
}

const vector<QString> FeaturesCollector::RequestNNearest(const QString targetImagePath, const int count, const map<int, set<QString>>& invertFile, const vector<Descriptor>& vocabulary)
{
	InterestingPointsDetector detector(DetectionMethod::Harris);
	DescriptorsBuilder builder;

	const auto image = ImageHelper::Load(targetImagePath);
	const auto pyr = Pyramid(5, 3, 1.6, 0.5, image);
	const auto points = detector.FindBlobBasedPoints(pyr);
	const auto descriptors = builder.CalculateHistogramDesctiptors(pyr, points);

	map<QString, int> counterMap;
	for (auto& descr : descriptors)
	{
		int word = WordIndex(vocabulary, descr);
		
		const auto& files = invertFile.at(word);
		for (auto& fileName : files)
		{
			if (counterMap.find(fileName) == counterMap.end())
			{
				counterMap[fileName] = 0;
			}
			counterMap[fileName]++;
		}
	}
	auto cmp = [](std::pair<QString, int> const & a, std::pair<QString, int> const & b)
	{
		return a.second != b.second ? a.second > b.second : a.first < b.first;
	};
	vector<pair<QString, int>> data;
	transform(counterMap.begin(), counterMap.end(), back_inserter(data), [](auto p) { return p; });
	sort(data.begin(), data.end(), cmp);

	vector<QString> nearest;
	for (int i = 0; i < min(count, (int)data.size()); i++)
	{
		nearest.emplace_back(data[i].first);
	}
	return nearest;
}

int FeaturesCollector::WordIndex(const vector<Descriptor>& vocabulary, const Descriptor & target)
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
