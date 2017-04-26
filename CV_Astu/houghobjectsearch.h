#ifndef HOUGHTRANSFORMATIONHELPER_H
#define HOUGHTRANSFORMATIONHELPER_H
#include <set>

#include <descriptorsbuilder.h>

using namespace std;
struct HoughVoteInfo
{
    double value;
    set<int> voters;
};

struct ObjectPose
{
    int centerX, centerY;
    double alpha, scale;
};

class houghobjectsearch
{
private:
    const int xStep = 20, yStep = 20, sPower = 4, sCoeff = 2;
    const double aStep = M_PI/6;


public:
    vector<ObjectPose> FindPoses(const CVImage& scene, const CVImage& object);
};

#endif // HOUGHTRANSFORMATIONHELPER_H
