#include "houghtransformationhelper.h"

#include <cassert>

int HoughTransformationHelper::xBins = 40;
int HoughTransformationHelper::yBins = 40;
int HoughTransformationHelper::aBins = 36;
int HoughTransformationHelper::sBins = 9;

HoughVoteInfo  voteSpace[40][40][36][9];

vector<HoughVoteInfo> HoughTransformationHelper::HoughHypotheses(const vector<pair<InterestingPoint, InterestingPoint> > &matches,
                                                                 const double dxStep, const double dyStep,
                                                                 const double daStep, const double dsStep)
{
    const double dSigmaMax = pow(2, sBins/2);
    const double dSigmaMin = 1.0/dSigmaMax;
    vector<HoughVoteInfo> result;

    //it's a kind of maggggiiiiiiccc


    for(int m = 0; m < matches.size(); m++)
    {
        const double dx = matches[m].first.x - matches[m].second.x;
        const int dxBinNumber = dx/dxStep + xBins/2;
        const int xDir = dx > dxStep*(dxBinNumber + 0.5) ? 1: -1;

        const double dy = matches[m].first.y - matches[m].second.y;
        const int dyBinNumber = dy/dyStep + yBins/2;
        const int yDir = dy > dyStep*(dyBinNumber + 0.5) ? 1: -1;

        const double da = matches[m].first.alpha - matches[m].second.alpha;
        const int aBinNumber = da/daStep;
        const double ac = daStep*(aBinNumber + 0.5);
        const int aDir = da > ac ? 1: -1;

        const double ds = matches[m].first.sigmaGlobal/matches[m].second.sigmaGlobal;
        const int sBinNumber = log(ds/dSigmaMin)/log(dsStep);
        const double sc = dSigmaMin * pow(2, sBinNumber) / sqrt(2);
        const int sDir = ds > sc ? 1: -1;
        double distance = abs(sc - sc * pow(dsStep, sDir));


        for(int xshift = 0; xshift < 2; xshift ++)
        {
            int currentX = dxBinNumber + xDir*xshift;
            if(currentX < 0 || currentX > xBins) continue;
            double wx = 1- abs(dx/dxStep - (currentX-xBins/2+0.5));
            assert(wx>=0);

            for(int yshift = 0; yshift < 2; yshift++)
            {
                int currentY = dyBinNumber + yDir*yshift;
                if(currentY < 0 || currentY > yBins) continue;
                double wy = 1 - abs(dy/dyStep - (currentY-yBins/2+0.5));
                assert(wy>=0);
                for(int ashift = 0; ashift < 2; ashift++)
                {
                    int currentA = aBinNumber + aDir*ashift;
                    double wa = 1 - abs(da/daStep - (currentA+0.5));
                    if(currentA < 0) currentA = aBins-1;
                    if(currentA >= aBins) currentA = 0;
                    assert(wa>=0);

                    for(int sshift = 0; sshift < 2; sshift++)
                    {
                        int currentS = sBinNumber + sDir*ashift;
                        if(currentS < 0 || currentS >=sBins) continue;
                        double currentCenter = sc * pow(dsStep, sDir*sshift);

                        double ws = 1 - abs(ds - currentCenter)/distance;


                        voteSpace[currentX][currentY][currentA][currentS]
                                .value += wx*wy*wa*ws;
                        voteSpace[currentX][currentY][currentA][currentS]
                                .voters.emplace_back(m);

                    }
                }
            }
        }



    }

    HoughVoteInfo maximal;
    maximal.value = 0;
    for(int x = 0; x< 20; x++)
        for(int y=0;y<20;y++)
            for(int a=0;a<36;a++)
                for(int s=0;s<9;s++)
                {
                    if(voteSpace[x][y][a][s].value > maximal.value)
                    {
                        maximal = voteSpace[x][y][a][s];
                    }
                }

    result.emplace_back(maximal);
    return result;
}

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

    const auto hypotheses = HoughHypotheses(matches,
                                            sourceImage.getWidth()/(xBins/2),
                                            sourceImage.getHeight()/(xBins/2),
                                            M_PI/18,
                                            2);
    //got something, need to improve accuracy using dlt;




    return DoubleMat();
}

