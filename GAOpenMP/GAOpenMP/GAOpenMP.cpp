// GAOpenMP.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <algorithm>
#include "Graph.h"
#include "GeneticTaskSolver.h"
bool LessFitted(Organizm* left, Organizm *right)
{
	return left->fitness > right->fitness;
}
int main()
{
	Graph* gr = new Graph();
	int solversCount = 5;
	//gr->AddVertex(0, 0.2);
	//gr->AddVertex(1, 0.7);
	//gr->AddVertex(2, 0.3);
	//gr->AddEdge(0, 1, 1.);
	//gr->AddEdge(1, 2, 2.);

	//int *part = new int[3];
	//part[0] = 0;
	//part[1] = 1;
	//part[2] = 2;

	//gr->EvaluatePartitioning(part, 3);

	gr->GenerateRandom(50, 0.3);
	GeneticTaskSolver* solver = new GeneticTaskSolver(gr, 100, 40, 8);
	solver->GeneratePopulation();
	solver->RunEvolution(15, 0.04, 0.99, 0.95);



	vector<Organizm*> champs;

	vector<GeneticTaskSolver*> solvers;
	for (int i = 0; i < solversCount; i++)
	{
		solvers.push_back(new GeneticTaskSolver(gr, 20, 8, 8));
	}

	for (int i = 0; i < solversCount; i++)
	{
		solvers[i]->GeneratePopulation();
	}

	for (int g = 0; g < 3; g++)
	{

		//run partial evo
		for (int i = 0; i < solversCount; i++)
		{
			solvers[i]->RunEvolution(5, 0.04, 0.99, 0.95);
		}
		//get best
		champs.clear();
		for (int i = 0; i < solversCount; i++)
		{
			champs.push_back(solvers[i]->GetBest());
		}
		std::sort(champs.begin(), champs.end(), LessFitted);
		//inject best
		for (int i = 0; i < solversCount; i++)
		{
			solvers[i]->Inject(champs[i%3]);
		}
	}

    return 0;
}

