#ifndef INTERESTINGPOINTSDETECTOR_H
#define INTERESTINGPOINTSDETECTOR_H
#include <doublemat.h>
#include <vector>

using namespace std;

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
protected:
   DoubleMat _source;
   //vector<InterestingPoint> FindPoints(const DoubleMat& errorData) const;
public:    
   InterestingPointsDetector(const DoubleMat& source);
   virtual DoubleMat CalculateDiffs(const int windowHalfSize,
                                      const BorderType borderType) const = 0;
   vector<InterestingPoint> FindInterestingPoints(const int windowHalfSize = 3,
                                                  const double threshold = 0.075,
                                                  const int extractionRadius = 3,
                                                  const BorderType borderType = BorderType::Replicate) const;
   virtual ~InterestingPointsDetector();


};

#endif // INTERESTINGPOINTSDETECTOR_H
