#pragma once
#include <stdlib.h>
#include <iostream>
#include <map>

#include "Vertex.h"
using namespace std;




class Graph
{
	double _summaryVertexWeight;
	map<int, Vertex*> _verticesList;
public:
	void AddVertex(int number, double weight);
	void AddEdge(int num_from, int num_to, double weight);
	double EvaluatePartitioning(int* partitionDescription, int partsCount);
	int GetVerticesCount();
	void GenerateRandom(int verticiesCount, double density);
	Graph();
	~Graph();
};