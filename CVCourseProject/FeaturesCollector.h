#pragma once
//Qt
#include <QDir>
#include <qstring.h>

//cv
#include <opencv2\features2d.hpp>

//common
#include <map>
#include <set>
//local
#include <..\CV_Astu\descriptorsbuilder.h>
#include <..\CV_Astu\imagehelper.h>



using namespace std;
using namespace cv;

typedef map<QString, vector<Descriptor>> FeaturesMap;
typedef map<int, set<QString>> InvertedFile;
typedef vector<Descriptor> Vocabulary;
typedef map<QString, vector<double>> FrequencyHistogramsMap;

struct FullIndex
{
	InvertedFile invertedFile;
	FrequencyHistogramsMap histograms;
};


class FeaturesCollector
{
public:

	static const FeaturesMap Features(const QString imagesDirectoryPath);
	static const Vocabulary BuildBOWVocabulary(const FeaturesMap& features);
	static const FullIndex BuildFullIndex(const FeaturesMap& featureMap, const Vocabulary& vocabulary);
	static const vector<QString> RequestNNearest(const QString targetImagePath, const int count,
		const FullIndex& index, const Vocabulary& vocabulary);
	static int WordIndex(const Vocabulary& vocabulary, const Descriptor& target);
	FeaturesCollector();
	~FeaturesCollector();
};

