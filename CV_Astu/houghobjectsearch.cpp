#include "houghobjectsearch.h"
#include "imagehelper.h"
#include <cassert>




vector<pair<InterestingPoint,InterestingPoint>> houghobjectsearch::MatchImages(InterestingPointsDetector detector, const CVImage &object, const CVImage &scene, DescriptorsBuilder builder)
{
    const auto scenePyr = Pyramid(5, 3, 1.6, 0.5, scene);
    const auto scenePoints = detector.FindBlobBasedPoints(scenePyr);
    const auto sceneDescr = builder.CalculateHistogramDesctiptors(scenePyr, scenePoints);

    const auto objectPyr = Pyramid(5, 3, 1.6, 0.5, object);
    const auto objectPoints = detector.FindBlobBasedPoints(objectPyr);
    const auto objectDescr = builder.CalculateHistogramDesctiptors(objectPyr, objectPoints);

    const auto matches = DescriptorsBuilder::FindMatches(sceneDescr, objectDescr);

    return matches;
}

DoubleMat houghobjectsearch::AffineMatrix(const vector<pair<InterestingPoint, InterestingPoint> > matches, set<int> inliers)
{
    gsl_matrix* A = gsl_matrix_alloc(2*inliers.size(),6);
    gsl_matrix* ATA = gsl_matrix_alloc(6,6);
    gsl_matrix* ATAinv = gsl_matrix_alloc(6,6);

    gsl_vector* b = gsl_vector_alloc(2*inliers.size());
    gsl_vector* atb = gsl_vector_alloc(6);
    gsl_vector* t = gsl_vector_alloc(6);
    int i=0;
    for(int inlier: inliers)
    {
        const auto match = matches[inlier];

        gsl_matrix_set(A, 2*i, 0, match.second.GlobalX());
        gsl_matrix_set(A, 2*i, 1, match.second.GlobalY());
        gsl_matrix_set(A, 2*i, 2, 1.0);
        gsl_matrix_set(A, 2*i, 3, 0.0);
        gsl_matrix_set(A, 2*i, 4, 0.0);
        gsl_matrix_set(A, 2*i, 5, 0.0);

        gsl_matrix_set(A, 2*i+1, 0, 0.0);
        gsl_matrix_set(A, 2*i+1, 1, 0.0);
        gsl_matrix_set(A, 2*i+1, 2, 0.0);
        gsl_matrix_set(A, 2*i+1, 3, match.second.GlobalX());
        gsl_matrix_set(A, 2*i+1, 4, match.second.GlobalY());
        gsl_matrix_set(A, 2*i+1, 5, 1.0);


        gsl_vector_set(b, 2*i, match.first.GlobalX());
        gsl_vector_set(b, 2*i + 1, match.first.GlobalY());

        ++i;
    }

    gsl_blas_dgemm(CblasTrans, CblasNoTrans, 1.0,
                   A, A, 0.0, ATA);

    int s;
    gsl_permutation * p = gsl_permutation_alloc(6);
    gsl_linalg_LU_decomp(ATA, p, &s);
    gsl_linalg_LU_invert(ATA, p, ATAinv);

    gsl_blas_dgemv(CblasTrans, 1.0, A, b, 0.0, atb);

    gsl_blas_dgemv(CblasNoTrans, 1.0, ATAinv, atb, 0.0, t);

    DoubleMat transformMatrix(3,3);
    for(int i = 0; i<6; i++)
    {
        double value = gsl_vector_get(t,i);
        transformMatrix.set(value, i/3, i%3);
    }
    transformMatrix.set(0,2,0);
    transformMatrix.set(0,2,1);
    transformMatrix.set(1,2,2);


    gsl_matrix_free(A);
    gsl_matrix_free(ATA);
    gsl_matrix_free(ATAinv);

    gsl_vector_free(b);
    gsl_vector_free(atb);
    gsl_vector_free(t);


    return transformMatrix;
}

vector<DoubleMat> houghobjectsearch::FindPoses(const CVImage &scene, const CVImage &object)
{
    unique_ptr<double[]> votes;

    vector<pair<int,int>> voteMap;

    vector<DoubleMat> poses;
    const double xMin = -scene.getWidth()/2;
    const double yMin = -scene.getHeight()/2;
    const int xBins = 2*scene.getWidth()/xStep;
    const int yBins = 2*scene.getHeight()/yStep;
    const int aBins = 2 * M_PI/aStep;
    const int sBins = 2 * sPower + 1;
    const double sigmaMin = 1.0/pow(2,sPower);

    InterestingPointsDetector detector(DetectionMethod::Harris);
    DescriptorsBuilder builder;

    votes = make_unique<double[]>(xBins * yBins * aBins * sBins);
    fill_n(votes.get(), xBins * yBins * aBins * sBins, 0.0);

    const auto matches = MatchImages(detector, object, scene, builder);

    auto mI = ImageHelper::DrawMatches(scene, object, matches);
    mI.save("C:\\Users\\Alena\\Pictures\\blob\\m.jpg");
    const int objectCx = object.getWidth()/2;
    const int objectCy = object.getHeight()/2;



    for(int m = 0; m < matches.size(); m++)
    {
        const auto& objectPoint = matches[m].second;
        const auto& scenePoint = matches[m].first;

        const auto pose = Pose(objectPoint, scenePoint, objectCx, objectCy);

        const int xBinIndex = (pose.centerX-xMin)/xStep;
        const int yBinIndex = (pose.centerY-yMin)/yStep;
        const int aBinIndex = pose.alpha/aStep;
        const int sBinIndex = log(pose.scale/sigmaMin)/log(sCoeff);
        const double sc = sigmaMin * pow(2, sBinIndex) / sqrt(2);


        const int xDir = pose.centerX > (xBinIndex + 0.5)*xStep+xMin ? 1: -1;
        const int yDir = pose.centerY > (yBinIndex + 0.5)*yStep+yMin ? 1: -1;
        const int sDir = pose.scale > sc ? 1 : -1;
        const int aDir = pose.alpha > (aBinIndex + 0.5)*aStep ? 1: -1;

        double distance = abs(sc - sc * pow(sCoeff, sDir));

        for(int xshift = 0; xshift < 2; xshift ++)
        {
            int currentX = xBinIndex + xDir*xshift;
            if(currentX < 0 || currentX >= xBins) continue;
            double wx = 1 - abs(1.0*(pose.centerX-xMin)/xStep - (currentX + 0.5));
            assert(wx>=0);

            for(int yshift = 0; yshift < 2; yshift++)
            {
                int currentY = yBinIndex + yDir*yshift;
                if(currentY < 0 || currentY >= yBins) continue;
                double wy = 1 - abs(1.0*(pose.centerY-yMin)/yStep - (currentY + 0.5));
                assert(wy>=0);
                for(int ashift = 0; ashift < 2; ashift++)
                {
                    int currentA = aBinIndex + aDir*ashift;
                    double wa = 1 - abs(pose.alpha/aStep - (currentA+0.5));
                    if(currentA < 0) currentA = aBins-1;
                    if(currentA >= aBins) currentA = 0;
                    assert(wa>=0);

                    for(int sshift = 0; sshift < 2; sshift++)
                    {
                        int currentS = sBinIndex + sDir*ashift;
                        if(currentS < 0 || currentS >=sBins) continue;
                        double currentCenter = sc * pow(sCoeff, sDir*sshift);

                        double ws = 1 - abs(pose.scale - currentCenter)/distance;

                        double value = wx * wy * wa * ws;


                        int planeIndex = xBinIndex +
                                yBinIndex * xBins +
                                aBinIndex * xBins * yBins +
                                sBinIndex * xBins * yBins * aBins;

                        votes[planeIndex] += value;
                        //votes[planeIndex].voters.emplace(m);
                        voteMap.emplace_back(pair<int,int>(m, planeIndex));
                    }
                }
            }
        }
    }

    int maxIndex = -1;
    int maxValue = -1;
    for(int i = 0; i < xBins * yBins * aBins * sBins; i++)
    {
        if(votes[i] > maxValue)
        {
            maxValue = votes[i];
            maxIndex = i;
        }
    }
    set<int> voters;
    for(int m = 0; m < voteMap.size(); m++)
    {
        if(voteMap[m].second == maxIndex)
        {
            voters.emplace(voteMap[m].first);
        }
    }
    if(voters.size() >=3)
    {
        auto matrix = AffineMatrix(matches, voters);
        poses.emplace_back(matrix);
    }
    return poses;
}

ObjectPose houghobjectsearch::Pose(const InterestingPoint &objectPoint, const InterestingPoint &scenePoint, const int objectCx, const int objectCy)
{
    ObjectPose pose;


    const double dx = objectPoint.GlobalX()-objectCx;
    const double dy = objectPoint.GlobalY()-objectCy;
    const double L = hypot(dx,dy);
    double phi = scenePoint.alpha - objectPoint.alpha + atan2(dy,dx);

    pose.scale = scenePoint.sigmaGlobal / objectPoint.sigmaGlobal;
    pose.centerX = round(scenePoint.GlobalX() - pose.scale * L * cos(phi));
    pose.centerY = round(scenePoint.GlobalY() - pose.scale * L * sin(phi));
    pose.alpha = fmod(scenePoint.alpha - objectPoint.alpha + 2 *M_PI, 2*M_PI);

    return pose;
}
