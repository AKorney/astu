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

class FeaturesCollector
{
public:

	static const map<QString, vector<Descriptor>> Features(const QString imagesDirectoryPath);
	static const vector<Descriptor> BuildBOWVocabulary(const map<QString, vector<Descriptor>>& features);
	static const map<int, set<QString>> BuildInvertFile(const map<QString, vector<Descriptor>>& featureMap,
		const vector<Descriptor>& vocabulary);
	static const vector<QString> RequestNNearest(const QString targetImagePath, const int count,
		const map<int, set<QString>>& invertFile, const vector<Descriptor>& vocabulary);
	static int WordIndex(const vector<Descriptor>& vocabulary, const Descriptor& target);
	FeaturesCollector();
	~FeaturesCollector();
};

