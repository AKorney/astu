#include "descriptorsbuilder.h"
#include <QTextStream>
#include "imagehelper.h"

DescriptorsBuilder::DescriptorsBuilder()
{

}

DoubleMat DescriptorsBuilder::CalculateGradients(const DoubleMat &source)
{
    BorderType border = BorderType::Replicate;
    const auto sobelX = source.Convolve(KernelBuilder::BuildSobelX(), border);
    const auto sobelY = source.Convolve(KernelBuilder::BuildSobelY(), border);

    DoubleMat gradients(source.getWidth(), source.getHeight());
    for (int i = 0; i < source.getWidth(); i++)
    {
        for (int j = 0; j < source.getHeight(); j++)
        {
            double value = sobelX.get(i, j, border) * sobelX.get(i, j, border)
                + sobelY.get(i, j, border) * sobelY.get(i, j, border);
            gradients.set(sqrt(value), i, j);
        }
    }
    return gradients;
}

Descriptor DescriptorsBuilder::CalculateSimpleDescriptor
(DoubleMat &gradients, const InterestingPoint point)
{
    Descriptor result;
    result.targetPoint = point;
    for(int valueIndex = 0; valueIndex < GRID_CELLS_COUNT; valueIndex ++)
    {
        result.localDescription.emplace_back(0);
    }
    const int xLeft = point.x - GRID_HALFSIZE;
    const int xRight = point.x + GRID_HALFSIZE + GRID_SIZE % 2;
    const int yTop = point.y - GRID_HALFSIZE;
    const int yBottom = point.y + GRID_HALFSIZE + GRID_SIZE % 2;

    //CVImageLoader::Save("C:\\Users\\Alena\\Pictures\\gradsinside.jpg", CVImage(gradients));

    for(int x = xLeft; x < xRight; x++)
    {
        for(int y = yTop; y < yBottom; y++)
        {
            int cellX = (x - point.x + GRID_HALFSIZE)/GRID_STEP;
            int cellY = (y - point.y + GRID_HALFSIZE)/GRID_STEP;
            int cell = cellY * (GRID_SIZE/GRID_STEP) + cellX;

            double value = gradients.get(x,y,BorderType::Replicate);
            result.localDescription.at(cell) += value/(GRID_STEP*GRID_STEP);
        }
    }
    return result;
}

vector<Descriptor> DescriptorsBuilder::CalculateSimpleDescriptors
(const DoubleMat &source, const vector<InterestingPoint> points)
{
    auto gradientValues = CalculateGradients(source);

    vector<Descriptor> descriptors;

    for (auto &point : points) {
       descriptors.emplace_back(CalculateSimpleDescriptor(gradientValues, point));
    }
    return descriptors;
}


vector<pair<Point,Point>> DescriptorsBuilder::FindMatches
(const vector<Descriptor> &first, const vector<Descriptor> &second)
{
    vector<pair<Point,Point>> result;
    DoubleMat distances(first.size(), second.size());
    for(int i =0; i<first.size(); i++)
    {
        for(int j =0; j<second.size(); j++)
        {
            distances.set(CalcDistance(first.at(i), second.at(j)),
                          i,j);
        }
    }

    for(int i = 0; i < first.size(); i++)
    {
        double bestDistanceForFirst = INFINITY;
        int bestMatchForFirst = -1;
        for(int j = 0; j < second.size(); j++)
        {
            if(distances.get(i,j) < bestDistanceForFirst)
            {
                bestDistanceForFirst = distances.get(i,j);
                bestMatchForFirst = j;
            }

        }
        double bestDistanceForNearest = INFINITY;
        int bestMatchForNearest = -1;
        for(int check = 0; check < first.size(); check++)
        {
            if(distances.get(check, bestMatchForFirst) < bestDistanceForNearest)
            {
                bestDistanceForNearest = distances.get(check, bestMatchForFirst);
                bestMatchForNearest = check;
            }
        }
        if(bestMatchForNearest == i)
        {
            //Descriptor secondDescriptor = second.at(bestDistanceForFirst);
            //Descriptor firstDescriptor = first.at(i);
            pair<Point,Point> match;

            match.first.x = first.at(i).targetPoint.x;
            match.first.y= first.at(i).targetPoint.y;

            match.second.x = second.at(bestMatchForFirst).targetPoint.x;
            match.second.y = second.at(bestMatchForFirst).targetPoint.y;

            result.emplace_back(match);
            QString debugInf = QString::number(i) +" and " +QString::number(bestMatchForFirst)
                    + " coords: " + QString::number(match.first.x) +":" + QString::number(match.first.y)
                    + " and: " + QString::number(match.second.x) +":" + QString::number(match.second.y);
            qDebug(debugInf.toStdString().c_str());
        }
    }
    return result;
}
double DescriptorsBuilder::CalcDistance(const Descriptor &left,const Descriptor &right)
{
    double distance = 0;
    for(int i=0; i<left.localDescription.size(); i++)
    {
        distance += (left.localDescription.at(i) - right.localDescription.at(i))
                * (left.localDescription.at(i) - right.localDescription.at(i));
    }
    return sqrt(distance);
}
