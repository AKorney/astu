#ifndef INTERESTINGPOINTSDETECTOR_H
#define INTERESTINGPOINTSDETECTOR_H
#include <doublemat.h>
#include <functional>
#include <vector>


#include "kernelbuilder.h"
#include "pyramid.h"
using namespace std;

enum class DetectionMethod { Moravec, Harris};



struct InterestingPoint
{
    double x,y;
    double w;
    int octave;
    double alpha;
    double sigmaLocal;
    double sigmaGlobal;
    double GlobalX() const {return x*pow(2, octave);}
    double GlobalY() const {return y*pow(2, octave);}
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
