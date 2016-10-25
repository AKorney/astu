#pragma once
#include <stdlib.h>
#include <iostream>
#include <map>

using namespace std;

class Vertex
{
	int _number;
	double _weight;
	map<int, double> _edges;

public:
	Vertex(int number, double weight);
	int GetNumber();
	double GetWeight();
	double GetEdgeWeight(int to);


	const map<int, double>&  GetEdges() const;

	void AddEdge(Vertex *to, double weight);
	bool operator<(const Vertex & op_right) const;

};

