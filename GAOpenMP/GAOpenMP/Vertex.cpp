#include "stdafx.h"
#include "Vertex.h"



Vertex::Vertex(int number, double weight)
{
	_number = number;
	_weight = weight;
}

int Vertex::GetNumber()
{
	return _number;
}

double Vertex::GetWeight()
{
	return _weight;
}

double Vertex::GetEdgeWeight(int to)
{
	if (_edges.find(to) == _edges.end())
	{
		return 0;
	}
	return _edges[to];
}

const map<int, double>& Vertex::GetEdges() const
{
	return _edges;
}

void Vertex::AddEdge(Vertex * to, double weight)
{
	_edges[to->GetNumber()] = weight;
}

bool Vertex::operator<(const Vertex & op_right) const
{
	return this->_number < op_right._number;
}

