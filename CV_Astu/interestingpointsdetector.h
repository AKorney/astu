#ifndef INTERESTINGPOINTSDETECTOR_H
#define INTERESTINGPOINTSDETECTOR_H
#include <doublemat.h>
#include <functional>
#include <vector>


#include "kernelbuilder.h"
using namespace std;

enum class DetectionMethod { Moravec, Harris};

struct Point
{
    int x, y;
};

struct InterestingPoint
{
    int x, y;
    double w;
};

class InterestingPointsDetector
{
private:
   DoubleMat _source;
   function<DoubleMat(const int, const BorderType)> _diffCalc;
   double CalculateCxy(const int x, const int y,
                       const int windowHalfSize,
                       const BorderType borderType);// const;
   DoubleMat CalculateMoravecMap(const int windowHalfSize,
            const BorderType borderType);// const;
   DoubleMat CalculateHarrisMap(const int windowHalfSize,
            const BorderType borderType);// const;
public:

   InterestingPointsDetector(const DoubleMat& source, const DetectionMethod method);
   //virtual DoubleMat CalculateDiffs(const int windowHalfSize,
   //                                   const BorderType borderType) const = 0;
   vector<InterestingPoint> FindInterestingPoints(const int windowHalfSize = 3,
                                                  const double threshold = 0.075,
                                                  const int extractionRadius = 3,
                                                  const BorderType borderType = BorderType::Replicate);// const;

   static vector<InterestingPoint> ANMS(const vector<InterestingPoint> source,
                                        const int maxCount, const int maxRadius);
   virtual ~InterestingPointsDetector();


};

#endif // INTERESTINGPOINTSDETECTOR_H
