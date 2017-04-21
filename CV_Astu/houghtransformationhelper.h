#ifndef HOUGHTRANSFORMATIONHELPER_H
#define HOUGHTRANSFORMATIONHELPER_H

#include <vector>

#include <descriptorsbuilder.h>

using namespace std;
struct HoughVoteInfo
{
    double value;
    vector<int> voters;
};

class HoughTransformationHelper
{
    static int xBins, yBins, aBins, sBins;
    static vector<HoughVoteInfo> HoughHypotheses(const vector<pair<InterestingPoint, InterestingPoint>> &mathces,
                                                 const double dxStep, const double dyStep,
                                                 const double daStep, const double dsStep);
    static const vector<pair<InterestingPoint, InterestingPoint>> BuildImageToObjectMatches
                                        (const CVImage &sourceImage, const CVImage &targetImage);
    HoughTransformationHelper();
public:
    static DoubleMat FindPoses(const CVImage& sourceImage, const CVImage& target);

};

#endif // HOUGHTRANSFORMATIONHELPER_H
