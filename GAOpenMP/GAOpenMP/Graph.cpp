#include "stdafx.h"
#include "Graph.h"


void Graph::AddVertex(int number, double weight)
{
	if (_verticesList.find(number) != _verticesList.end())
	{
		cout << "Sorry, vertex was already added";
		return;
	}
	Vertex* vertex = new Vertex(number, weight);
	_summaryVertexWeight += weight;
	_verticesList[number] = vertex;
}

void Graph::AddEdge(int num_from, int num_to, double weight)
{
	_verticesList[num_from]->AddEdge(_verticesList[num_to], weight);
	_verticesList[num_to]->AddEdge(_verticesList[num_from], weight);
}
double Graph::EvaluatePartitioning(int * partitionDescription, int partsCount)
{
	double * partWeights = new double[partsCount];
	double  averageSubWeight = _summaryVertexWeight / partsCount;
	for (int i = 0; i < partsCount; i++)
	{
		partWeights[i] = 0.0;
		
	}
	int vc = _verticesList.size();

	//partWeights contains calculated weights of subgraphs 
	for (int i = 0; i < vc; i++)
	{
		partWeights[partitionDescription[i]] +=
			_verticesList.at(i)->GetWeight();
		
	}



	//calculating penalty for vertex weights of subraphs
	double sqSum = 0.0;
	//double sumDev = 0.0;
	for (int i = 0; i < partsCount; i++)
	{
		//sumDev += abs(partWeights[i] - averageSubWeight );
		//for (int j = i + 1; j < partsCount; j++)
		//{
		sqSum += pow((partWeights[i] - averageSubWeight), 2);
		//}
	}


	//calculating penalty for border edges weight
	double borderWeight = 0;

	for (map<int, Vertex*>::iterator itr = _verticesList.begin();
		itr != _verticesList.end();
		itr++)
	{
		Vertex* vertexFrom = itr->second;
		map<int, double> edges = vertexFrom->GetEdges();
		for (map<int, double>::iterator edgesItr = edges.begin();
			edgesItr != edges.end();
			edgesItr++)
		{
			int to = edgesItr->first;
			double weight = edgesItr->second;
			if (partitionDescription[vertexFrom->GetNumber()]
				!= partitionDescription[to])
			{
				borderWeight += weight / 2;
			}
		}
	}


	double fitness = -sqSum - borderWeight;
	//double fitness = -sumDev - log10(borderWeight);
	//delete[] partWeights;
	return fitness;
}
int Graph::GetVerticesCount()
{
	return _verticesList.size();
}
void Graph::GenerateRandom(int verticiesCount, double density)
{
	for (int i = 0; i < verticiesCount; i++)
	{
		double w = 30 * (double)rand() / RAND_MAX;
		this->AddVertex(i, w);
	}
	for (int i = 0; i < verticiesCount; i++)
	{
		for (int j = i+1; j < verticiesCount; j++)
		{
			double needToAdd = (double)rand() / RAND_MAX;
			if (needToAdd < density)
			{
				this->AddEdge(i, j, 3 * (double)rand() / RAND_MAX);
			}
		}
	}
}
Graph::Graph()
{
	_summaryVertexWeight = 0;
}


Graph::~Graph()
{
	for (map<int, Vertex*>::iterator i = _verticesList.begin();
		i != _verticesList.end();
		i++)
	{
		//delete i->second;
	}
}

