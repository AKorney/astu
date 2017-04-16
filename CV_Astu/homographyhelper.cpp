#include "homographyhelper.h"



HomographyHelper::HomographyHelper()
{

}



DoubleMat HomographyHelper::RANSAC(const vector<pair<InterestingPoint, InterestingPoint>> matches)
{
    if(matches.size() < 4)
    {
        throw "Need at least 4 matches";
    }

    int bestInliers = 0;
    DoubleMat best(3,3);
    vector<int> inliersList;
    std::random_device rd;
    std::mt19937 g(rd());

    vector<int> indecies(matches.size());
    iota(indecies.begin(), indecies.end(), 0);


    gsl_matrix* A = gsl_matrix_alloc(8,9);
    gsl_matrix* ATA = gsl_matrix_alloc(9,9);

    gsl_matrix* V = gsl_matrix_alloc(9,9);
    gsl_matrix* H = gsl_matrix_alloc(3,3);

    gsl_vector* temp1 = gsl_vector_alloc(9);
    gsl_vector* temp2 = gsl_vector_alloc(9);


    for(int iteration = 0; iteration < iterationsCount; iteration++)
    {
        shuffle(indecies.begin(), indecies.end(), g);
        //prepare A

        for(int match = 0; match < 4; match++)
        {
            const auto pointR = matches[indecies[match]].second;
            const auto pointL = matches[indecies[match]].first;

            const double xR = pointR.x;
            const double yR = pointR.y;
            const double xL = pointL.x;
            const double yL = pointL.y;


            gsl_matrix_set(A, match*2, 0, xR);
            gsl_matrix_set(A, match*2, 1, yR);
            gsl_matrix_set(A, match*2, 2, 1);

            gsl_matrix_set(A, match*2, 3, 0);
            gsl_matrix_set(A, match*2, 4, 0);
            gsl_matrix_set(A, match*2, 5, 0);

            gsl_matrix_set(A, match*2, 6, -xL*xR);
            gsl_matrix_set(A, match*2, 7, -xL*yR);
            gsl_matrix_set(A, match*2, 8, -xL);


            gsl_matrix_set(A, match*2+1, 0, 0);
            gsl_matrix_set(A, match*2+1, 1, 0);
            gsl_matrix_set(A, match*2+1, 2, 0);

            gsl_matrix_set(A, match*2+1, 3, xR);
            gsl_matrix_set(A, match*2+1, 4, yR);
            gsl_matrix_set(A, match*2+1, 5, 1);

            gsl_matrix_set(A, match*2+1, 6, -yL*xR);
            gsl_matrix_set(A, match*2+1, 7, -yL*yR);
            gsl_matrix_set(A, match*2+1, 8, -yL);
        }

        gsl_blas_dgemm(CblasTrans, CblasNoTrans, 1.0,
                       A, A, 0.0, ATA);
        gsl_linalg_SV_decomp(ATA, V, temp1, temp2);

        auto hVec = gsl_matrix_column(V, 8);
        gsl_matrix_set(H,0,0,gsl_vector_get(&hVec.vector,0));
        gsl_matrix_set(H,0,1,gsl_vector_get(&hVec.vector,1));
        gsl_matrix_set(H,0,2,gsl_vector_get(&hVec.vector,2));

        gsl_matrix_set(H,1,0,gsl_vector_get(&hVec.vector,3));
        gsl_matrix_set(H,1,1,gsl_vector_get(&hVec.vector,4));
        gsl_matrix_set(H,1,2,gsl_vector_get(&hVec.vector,5));

        gsl_matrix_set(H,2,0,gsl_vector_get(&hVec.vector,6));
        gsl_matrix_set(H,2,1,gsl_vector_get(&hVec.vector,7));
        gsl_matrix_set(H,2,2,gsl_vector_get(&hVec.vector,8));
        //gsl_matrix_scale(&H.matrix, gsl_matrix_get(&H.matrix,2,2));


        int inliers = 0;
        vector<int> localInliers;
        double th = 6;
        for(int i=0; i<matches.size();i++)
        {
            const auto pointR = matches[i].second;
            const auto pointL = matches[i].first;
            double vR[3] = {pointR.x, pointR.y, 1.0};
            double vL[3];

            gsl_vector_view vecl = gsl_vector_view_array(vL, 3);
            gsl_vector_view vecr = gsl_vector_view_array(vR, 3);

            gsl_blas_dgemv(CblasNoTrans, 1.0, H,
                           &vecr.vector, 0.0, &vecl.vector);
            double scaleL = vL[2];
            //gsl_vector_scale(&vecr.vector, 1.0/gsl_vector_get(&vecr.vector, 2));
            double xl = vL[0];
            double yl = vL[1];
            double dist = hypot(pointL.x - xl/scaleL,
                                pointL.y - yl/scaleL);

            if(dist < th)
            {
                inliers++;
                localInliers.emplace_back(i);
            }
        }
        if(inliers > bestInliers)
        {
           bestInliers = inliers;
           inliersList.clear();
           inliersList.assign(localInliers.begin(), localInliers.end());
           //double scale = gsl_matrix_get(H, 2,2);
           //for(int i=0; i<3; i++)
           //    for(int j=0; j<3; j++)
           //        best.set(gsl_matrix_get(H, i, j)/scale, i,j);
        }
    }
    ///*
    gsl_matrix* ExtA = gsl_matrix_alloc(2*inliersList.size(), 9);
    gsl_matrix* ExtATA = gsl_matrix_alloc(9,9);
    for(int match = 0; match < inliersList.size(); match++)
    {
        const auto pointR = matches[inliersList[match]].second;
        const auto pointL = matches[inliersList[match]].first;

        const double xR = pointR.x;
        const double yR = pointR.y;
        const double xL = pointL.x;
        const double yL = pointL.y;


        gsl_matrix_set(ExtA, match*2, 0, xR);
        gsl_matrix_set(ExtA, match*2, 1, yR);
        gsl_matrix_set(ExtA, match*2, 2, 1);

        gsl_matrix_set(ExtA, match*2, 3, 0);
        gsl_matrix_set(ExtA, match*2, 4, 0);
        gsl_matrix_set(ExtA, match*2, 5, 0);

        gsl_matrix_set(ExtA, match*2, 6, -xL*xR);
        gsl_matrix_set(ExtA, match*2, 7, -xL*yR);
        gsl_matrix_set(ExtA, match*2, 8, -xL);


        gsl_matrix_set(ExtA, match*2+1, 0, 0);
        gsl_matrix_set(ExtA, match*2+1, 1, 0);
        gsl_matrix_set(ExtA, match*2+1, 2, 0);

        gsl_matrix_set(ExtA, match*2+1, 3, xR);
        gsl_matrix_set(ExtA, match*2+1, 4, yR);
        gsl_matrix_set(ExtA, match*2+1, 5, 1);

        gsl_matrix_set(ExtA, match*2+1, 6, -yL*xR);
        gsl_matrix_set(ExtA, match*2+1, 7, -yL*yR);
        gsl_matrix_set(ExtA, match*2+1, 8, -yL);
    }

    gsl_blas_dgemm(CblasTrans, CblasNoTrans, 1.0,
                   ExtA, ExtA, 0.0, ExtATA);
    gsl_linalg_SV_decomp(ExtATA, V, temp1, temp2);

    auto hV1 = gsl_matrix_column(V, 8);
    ///*
    gsl_matrix_set(H,0,0,gsl_vector_get(&hV1.vector,0));
    gsl_matrix_set(H,0,1,gsl_vector_get(&hV1.vector,1));
    gsl_matrix_set(H,0,2,gsl_vector_get(&hV1.vector,2));

    gsl_matrix_set(H,1,0,gsl_vector_get(&hV1.vector,3));
    gsl_matrix_set(H,1,1,gsl_vector_get(&hV1.vector,4));
    gsl_matrix_set(H,1,2,gsl_vector_get(&hV1.vector,5));

    gsl_matrix_set(H,2,0,gsl_vector_get(&hV1.vector,6));
    gsl_matrix_set(H,2,1,gsl_vector_get(&hV1.vector,7));
    gsl_matrix_set(H,2,2,gsl_vector_get(&hV1.vector,8));
    //
    //auto invH = gsl_matrix_alloc_from_matrix(H,0,0,3,3);

    //int s;
    //gsl_permutation * p = gsl_permutation_alloc(3);
    //gsl_linalg_LU_decomp(H, p, &s);
    //gsl_linalg_LU_invert(H, p, invH);


    double scale = gsl_vector_get(&hV1.vector, 8);
    for(int i=0; i<3; i++)
        for(int j=0; j<3; j++)
            best.set(gsl_matrix_get(H, i,j)/scale, i,j);
    //gsl_vector_get(&hV1.vector, j*3 + i)/scale, i,j);
    //*/
    return best;
}


