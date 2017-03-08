#ifndef HARRISDETECTOR_H
#define HARRISDETECTOR_H
#include "interestingpointsdetector.h"
#include "kernelbuilder.h"
class HarrisDetector : public InterestingPointsDetector
{
public:
    HarrisDetector(const DoubleMat& source);
    virtual DoubleMat CalculateDiffs(const int windowHalfSize,
                                      const BorderType borderType) const;
};

#endif // HARRISDETECTOR_H
