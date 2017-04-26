#include "houghobjectsearch.h"

#include <cassert>




vector<ObjectPose> houghobjectsearch::FindPoses(const CVImage &scene, const CVImage &object)
{
    unique_ptr<HoughVoteInfo[]> votes;

    vector<ObjectPose> poses;
    const int xBins = scene.getWidth()/xStep + 1;
    const int yBins = scene.getHeight()/yStep + 1;
    const int aBins = 2 * M_PI/aStep;
    const int sBins = 2 * sPower + 1;
    const double sigmaMin = 1.0/pow(2,sPower);

    InterestingPointsDetector detector(DetectionMethod::Harris);
    DescriptorsBuilder builder;

    votes = make_unique<HoughVoteInfo[]>(xBins * yBins * aBins * sBins);

    const auto scenePyr = Pyramid(5, 3, 1.6, 0.5, scene);
    const auto scenePoints = detector.FindBlobBasedPoints(scenePyr);
    const auto sceneDescr = builder.CalculateHistogramDesctiptors(scenePyr, scenePoints);

    const auto objectPyr = Pyramid(5, 3, 1.6, 0.5, object);
    const auto objectPoints = detector.FindBlobBasedPoints(objectPyr);
    const auto objectDescr = builder.CalculateHistogramDesctiptors(objectPyr, objectPoints);

    const auto matches = DescriptorsBuilder::FindMatches(sceneDescr, objectDescr);


    const int objectCx = object.getWidth()/2;
    const int objectCy = object.getHeight()/2;
    const double baseAngle = 0;


    for(int m = 0; m < matches.size(); m++)
    {
        const auto& objectPoint = matches[m].second;
        const auto& scenePoint = matches[m].first;


        const double dx = objectPoint.GlobalX()-objectCx;
        const double dy = objectPoint.GlobalY()-objectCy;
        const double L = hypot(dx,dy);
        const double cda = atan2(dy,dx);
        const double cDev = objectPoint.alpha - cda;
        const double dDev = objectPoint.alpha - baseAngle;

        //scale
        const double ds = scenePoint.sigmaGlobal / objectPoint.sigmaGlobal;



        double objcda = scenePoint.alpha - cDev;

        //center + deviation
        double objdx = ds * L * sin(objcda);
        double objdy = ds * L * cos(objcda);
        double objddev = fmod(scenePoint.alpha - dDev + 2 *M_PI, 2*M_PI);

        const int CX = round(objdx + scenePoint.GlobalX());
        const int CY = round(objdy + scenePoint.GlobalY());
        const int xBinIndex = CX/xStep;
        const int yBinIndex = CY/yStep;
        const int aBinIndex = objddev/aStep;
        const int sBinIndex = log(ds/sigmaMin)/log(sCoeff);
        const double sc = sigmaMin * pow(2, sBinIndex) / sqrt(2);

        const int xDir = CX > (xBinIndex + 0.5)*xStep ? 1: -1;
        const int yDir = CY > (yBinIndex + 0.5)*yStep ? 1: -1;
        const int sDir = ds > sc ? 1 : -1;
        const int aDir = objddev > (aBinIndex + 0.5)*aStep ? 1: -1;

        double distance = abs(sc - sc * pow(sCoeff, sDir));

        for(int xshift = 0; xshift < 2; xshift ++)
        {
            int currentX = xBinIndex + xDir*xshift;
            if(currentX < 0 || currentX >= xBins) continue;
            double wx = 1 - abs(1.0*CX/xStep - (currentX + 0.5));
            assert(wx>=0);

            for(int yshift = 0; yshift < 2; yshift++)
            {
                int currentY = yBinIndex + yDir*yshift;
                if(currentY < 0 || currentY >= yBins) continue;
                double wy = 1 - abs(1.0*CY/yStep - (currentY + 0.5));
                assert(wy>=0);
                for(int ashift = 0; ashift < 2; ashift++)
                {
                    int currentA = aBinIndex + aDir*ashift;
                    double wa = 1 - abs(objddev/aStep - (currentA+0.5));
                    if(currentA < 0) currentA = aBins-1;
                    if(currentA >= aBins) currentA = 0;
                    assert(wa>=0);

                    for(int sshift = 0; sshift < 2; sshift++)
                    {
                        int currentS = sBinIndex + sDir*ashift;
                        if(currentS < 0 || currentS >=sBins) continue;
                        double currentCenter = sc * pow(sCoeff, sDir*sshift);

                        double ws = 1 - abs(ds - currentCenter)/distance;

                        double value = wx * wy * wa * ws;


                        int planeIndex = xBinIndex +
                                yBinIndex * xBins +
                                aBinIndex * xBins * yBins +
                                sBinIndex * xBins * yBins * aBins;

                        votes[planeIndex].value += value;
                        votes[planeIndex].voters.emplace(m);
                    }
                }
            }
        }
    }

    int maxIndex = -1;
    int maxValue = -1;
    for(int i = 0; i < xBins * yBins * aBins * sBins; i++)
    {
        if(votes[i].value > maxValue)
        {
            maxValue = votes[i].value;
            maxIndex = i;
        }
    }

    int sB = maxIndex / (xBins * yBins * aBins);
    int aB = (maxIndex % xBins * yBins * aBins) / (xBins * yBins);
    int yB = ((maxIndex % xBins * yBins * aBins) % (xBins * yBins)) / xBins;
    int xB = maxIndex % xBins;

    ObjectPose pose;
    pose.alpha = (aB + 0.5)*aStep;
    pose.scale = sigmaMin * pow(sCoeff, sB) * sqrt(2);
    pose.centerX = (xB + 0.5)*xStep;
    pose.centerY = (yB + 0.5)*yStep;


    poses.emplace_back(pose);

    return poses;
}
