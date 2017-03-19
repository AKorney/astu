#include "descriptorsbuilder.h"

DescriptorsBuilder::DescriptorsBuilder()
{

}

const DoubleMat DescriptorsBuilder::CalculateGradients(const DoubleMat &source)
{
    BorderType border = BorderType::Replicate;
    const auto sobelX = source.Convolve(KernelBuilder::BuildSobelX(), border);
    const auto sobelY = source.Convolve(KernelBuilder::BuildSobelY(), border);

    DoubleMat gradients(source.getWidth(), source.getHeight());
    for (int i = 0; i < source.getWidth(); i++)
    {
        for (int j = 0; j < source.getHeight(); j++)
        {
            double value = sobelX.get(i, j, border) * sobelY.get(i, j, border)
                + sobelY.get(i, j, border) * sobelY.get(i, j, border);
            gradients.set(sqrt(value), i, j);
        }
    }
    return gradients;
}

Descriptor DescriptorsBuilder::CalculateSimpleDescriptor
(const DoubleMat &gradients, const InterestingPoint point)
{
    Descriptor result;
    result.targetPoint = point;
    for(int valueIndex = 0; valueIndex < gridCellsCount; valueIndex ++)
    {
        result.localDescription.emplace_back(0);
    }
    const int xLeft = point.x - offset;
    const int xRight = point.x + offset + gridSize % 2;
    const int yTop = point.y - offset;
    const int yBottom = point.y + offset + gridSize % 2;

    for(int x = xLeft; x < xRight; x++)
    {
        for(int y = yTop; y < yBottom; y++)
        {
            int cellX = (x - point.x + offset)/gridStep;
            int cellY = (y - point.y + offset)/gridStep;
            int cell = cellY * (gridSize/gridStep) + cellX;


            result.localDescription.at(cell) += gradients.get(x,y,BorderType::Replicate)
                    /(gridStep*gridStep);
        }
    }
    return result;
}

vector<Descriptor> DescriptorsBuilder::CalculateSimpleDescriptors
(const DoubleMat &source, const vector<InterestingPoint> points)
{
    const auto gradientValues = CalculateGradients(source);

    vector<Descriptor> descriptors;

    for (auto &point : points) {
       descriptors.emplace_back(CalculateSimpleDescriptor(gradientValues, point));
    }
    return descriptors;
}
