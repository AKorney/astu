#include "houghtransformationhelper.h"

#include <cassert>



HoughTransformationHelper::HoughTransformationHelper()
{

}

const vector<pair<InterestingPoint, InterestingPoint> > HoughTransformationHelper::BuildImageToObjectMatches(const CVImage &sourceImage, const CVImage &targetImage)
{
    InterestingPointsDetector detector(DetectionMethod::Harris);
    const DescriptorsBuilder builder;
    const Pyramid sourcePyr(6, 3, 1.6, 0.5, sourceImage);
    const Pyramid targetPyr(6, 3, 1.6, 0.5, targetImage);

    const auto sourcePoints = detector.FindBlobBasedPoints(sourcePyr);
    const auto targetPoints = detector.FindBlobBasedPoints(targetPyr);

    const auto sourceDesc = builder.CalculateHistogramDesctiptors(sourcePyr, sourcePoints);
    const auto targetDesc = builder.CalculateHistogramDesctiptors(targetPyr, targetPoints);

    const auto matches = builder.FindMatches(sourceDesc, targetDesc);
    return matches;
}

DoubleMat HoughTransformationHelper::FindPoses(const CVImage &sourceImage, const CVImage &targetImage)
{
    const auto matches = BuildImageToObjectMatches(sourceImage, targetImage);





    return DoubleMat();
}

