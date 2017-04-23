#pragma once
//Qt
#include <QDir>
#include <qstring.h>

//cv
#include <opencv2\features2d.hpp>

//common
#include <map>
//local
#include <..\CV_Astu\descriptorsbuilder.h>
#include <..\CV_Astu\imagehelper.h>



using namespace std;
using namespace cv;

class FeaturesCollector
{
public:

	static map<QString, vector<Descriptor>> Features(const QString imagesDirectoryPath);
	static vector<Descriptor> BuildBOWVocabulary(const map<QString, vector<Descriptor>>& features);
	FeaturesCollector();
	~FeaturesCollector();
};

