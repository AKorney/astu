#ifndef MORAVECDETECTOR_H
#define MORAVECDETECTOR_H
#include "interestingpointsdetector.h"

class MoravecDetector : public InterestingPointsDetector
{
private:
    double CalculateCxy(const int x, const int y,
                        const int windowHalfSize,
                        const BorderType borderType) const;
public:
    MoravecDetector(const DoubleMat& source);

    virtual DoubleMat CalculateErrors(const int windowHalfSize,
                               const BorderType borderType) const;
};

#endif // MORAVECDETECTOR_H
