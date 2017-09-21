#ifndef HOUGHTRANSFORMATIONHELPER_H
#define HOUGHTRANSFORMATIONHELPER_H
#include <set>
#include <vector>
#include <algorithm>

#include "descriptorsbuilder.h"
#include <gsl/gsl_linalg.h>
#include <gsl/gsl_blas.h>

using namespace std;
//struct HoughVoteInfo
//{
//    double value;
//    set<int> voters;
//};

struct ObjectPose
{
    int centerX, centerY;
    double alpha, scale;
};

class houghobjectsearch
{
private:
    const int xStep = 20, yStep = 20, sPower = 4, sCoeff = 2;
    const double aStep = M_PI/9;


    vector<pair<InterestingPoint,InterestingPoint>> MatchImages(InterestingPointsDetector detector, const CVImage &object, const CVImage &scene, DescriptorsBuilder builder);
    DoubleMat AffineMatrix(const vector<pair<InterestingPoint,InterestingPoint>> matches,
                           set<int> inliers);
public:
    vector<DoubleMat> FindPoses(const CVImage& scene, const CVImage& object);
    static ObjectPose Pose(const InterestingPoint& objectPoint, const InterestingPoint& scenePoint,
                           const int cx, const int cy);
};

#endif // HOUGHTRANSFORMATIONHELPER_H
