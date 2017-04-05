#ifndef INTERESTINGPOINTSDETECTOR_H
#define INTERESTINGPOINTSDETECTOR_H
#include <doublemat.h>
#include <functional>
#include <vector>


#include "kernelbuilder.h"
#include "pyramid.h"
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
    double alpha;
    double sigma;
};

class InterestingPointsDetector
{
private:

   function<DoubleMat(const DoubleMat&, const int, const BorderType)> _diffCalc;
   double CalculateCxy(const DoubleMat& source, const int x, const int y,
                       const int windowHalfSize,
                       const BorderType borderType) const;
   DoubleMat CalculateMoravecMap(const DoubleMat& source,
                                 const int windowHalfSize,
                                 const BorderType borderType) const;
   DoubleMat CalculateHarrisMap(const DoubleMat& source,
                                const int windowHalfSize,
                                const BorderType borderType) const;
   double CalculateHarrisValue(const DoubleMat& source, const int windowHalfSize,
                          const BorderType borderType, const int x, const int y) const;
   vector<BlobDescription> FindBlobs(const Pyramid& pyramid, const double diffThreshold) const;
   //DoGPointType GetDoGPointType(const int x,const int y,const int octave, const int diffIndex) const;
public:

   InterestingPointsDetector(const DetectionMethod method);

   vector<InterestingPoint> FindInterestingPoints(const DoubleMat& source,
                                                  const int windowHalfSize = 3,
                                                  const double threshold = 0.075,
                                                  const int extractionRadius = 3,
                                                  const BorderType borderType = BorderType::Replicate)
                                                  const;

   static vector<InterestingPoint> ANMS(const vector<InterestingPoint> source,
                                        const int maxCount, const int maxRadius);

   vector<InterestingPoint> FindBlobBasedPoints(const Pyramid& pyramid);

   virtual ~InterestingPointsDetector();


};

#endif // INTERESTINGPOINTSDETECTOR_H
