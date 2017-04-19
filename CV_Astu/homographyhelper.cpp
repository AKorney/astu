#include "homographyhelper.h"
#include <cassert>


void HomographyHelper::FillMatrixA(const vector<pair<InterestingPoint, InterestingPoint>> &matches, const vector<int> &indicesMap, gsl_matrix *a)
{
    const int equationsCount = a->size1;
    const int pairsCount = equationsCount/2;
    assert(pairsCount <= indicesMap.size());
    for(int match=0; match<pairsCount; match++)
    {
        const auto pointR = matches[indicesMap[match]].second;
        const auto pointL = matches[indicesMap[match]].first;

        const double xR = pointR.x;
        const double yR = pointR.y;
        const double xL = pointL.x;
        const double yL = pointL.y;


        gsl_matrix_set(a, match*2, 0, xR);
        gsl_matrix_set(a, match*2, 1, yR);
        gsl_matrix_set(a, match*2, 2, 1);

        gsl_matrix_set(a, match*2, 3, 0);
        gsl_matrix_set(a, match*2, 4, 0);
        gsl_matrix_set(a, match*2, 5, 0);

        gsl_matrix_set(a, match*2, 6, -xL*xR);
        gsl_matrix_set(a, match*2, 7, -xL*yR);
        gsl_matrix_set(a, match*2, 8, -xL);


        gsl_matrix_set(a, match*2+1, 0, 0);
        gsl_matrix_set(a, match*2+1, 1, 0);
        gsl_matrix_set(a, match*2+1, 2, 0);

        gsl_matrix_set(a, match*2+1, 3, xR);
        gsl_matrix_set(a, match*2+1, 4, yR);
        gsl_matrix_set(a, match*2+1, 5, 1);

        gsl_matrix_set(a, match*2+1, 6, -yL*xR);
        gsl_matrix_set(a, match*2+1, 7, -yL*yR);
        gsl_matrix_set(a, match*2+1, 8, -yL);
    }
}

HomographyHelper::HomographyHelper()
{

}



void HomographyHelper::FillMatrixH(gsl_matrix* H, gsl_vector_view hVec)
{
    gsl_matrix_set(H,0,0,gsl_vector_get(&hVec.vector,0));
    gsl_matrix_set(H,0,1,gsl_vector_get(&hVec.vector,1));
    gsl_matrix_set(H,0,2,gsl_vector_get(&hVec.vector,2));

    gsl_matrix_set(H,1,0,gsl_vector_get(&hVec.vector,3));
    gsl_matrix_set(H,1,1,gsl_vector_get(&hVec.vector,4));
    gsl_matrix_set(H,1,2,gsl_vector_get(&hVec.vector,5));

    gsl_matrix_set(H,2,0,gsl_vector_get(&hVec.vector,6));
    gsl_matrix_set(H,2,1,gsl_vector_get(&hVec.vector,7));
    gsl_matrix_set(H,2,2,gsl_vector_get(&hVec.vector,8));
}

void HomographyHelper::DLT(gsl_matrix *A, gsl_matrix *ATA, gsl_matrix *H, gsl_matrix *V, gsl_vector *temp1, gsl_vector *temp2)
{

    gsl_blas_dgemm(CblasTrans, CblasNoTrans, 1.0,
                   A, A, 0.0, ATA);
    gsl_linalg_SV_decomp(ATA, V, temp1, temp2);

    auto hVec = gsl_matrix_column(V, 8);
    FillMatrixH(H, hVec);
}

vector<int> HomographyHelper::GetInliers(gsl_matrix* H, const vector<pair<InterestingPoint, InterestingPoint>>& matches)
{
    vector<int> localInliers;
    double th = 5;
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

        double xl = vL[0];
        double yl = vL[1];
        double dist = hypot(pointL.x - xl/scaleL,
                            pointL.y - yl/scaleL);

        if(dist < th)
        {
            localInliers.emplace_back(i);
        }
    }
    return localInliers;
}

DoubleMat HomographyHelper::RANSAC(const vector<pair<InterestingPoint, InterestingPoint>>& matches)
{
    if(matches.size() < 4)
    {
        throw "Need at least 4 matches";
    }

    DoubleMat best(3,3);
    vector<int> inliersList;
    std::random_device rd;
    std::mt19937 g(rd());

    vector<int> indices(matches.size());
    iota(indices.begin(), indices.end(), 0);


    gsl_matrix* A = gsl_matrix_alloc(8,9);
    gsl_matrix* ATA = gsl_matrix_alloc(9,9);

    gsl_matrix* V = gsl_matrix_alloc(9,9);
    gsl_matrix* H = gsl_matrix_alloc(3,3);

    gsl_vector* temp1 = gsl_vector_alloc(9);
    gsl_vector* temp2 = gsl_vector_alloc(9);


    for(int iteration = 0; iteration < iterationsCount; iteration++)
    {
        shuffle(indices.begin(), indices.end(), g);
        FillMatrixA(matches, indices, A);
        DLT(A, ATA, H, V, temp1, temp2);

        vector<int> localInliers = GetInliers(H, matches);
        if(localInliers.size() > inliersList.size())
        {
           inliersList.clear();
           inliersList.assign(localInliers.begin(), localInliers.end());
        }
    }

    gsl_matrix* ExtA = gsl_matrix_alloc(2*inliersList.size(), 9);
    gsl_matrix* ExtATA = gsl_matrix_alloc(9,9);

    FillMatrixA(matches, inliersList, ExtA);
    DLT(ExtA, ExtATA, H, V, temp1, temp2);

    double scale = gsl_matrix_get(H,2,2);
    for(int i=0; i<3; i++)
        for(int j=0; j<3; j++)
            best.set(gsl_matrix_get(H, i, j)/scale, i,j);
    gsl_matrix_free(A);
    gsl_matrix_free(ATA);
    gsl_matrix_free(V);
    gsl_matrix_free(ExtA);
    gsl_matrix_free(ExtATA);
    gsl_matrix_free(H);

    gsl_vector_free(temp1);
    gsl_vector_free(temp2);
    return best;
}


