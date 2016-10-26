// GAOpenMP.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <algorithm>
#include <omp.h>

#include "Graph.h"
#include "GeneticTaskSolver.h"
bool LessFitted(Organizm* left, Organizm *right)
{
	return left->fitness > right->fitness;
}
int main()
{

#pragma region task parameters
	int solversCount = 5;
	int graphSize = 50;
	double graphD = 0.3;

	int fullPopulationSize = 100;
	int fullCrossoverCount = 30;
	int subgraphsCount = 15;


	double pm = 0.04,
		pvm = 0.99,
		pv = 0.95;

	int fullGenCount = 15;
	int epochCount = 3;

	int topCount = 3;
#pragma endregion
	srand(NULL);
	Graph* gr = new Graph();
	gr->GenerateRandom(graphSize, graphD);

#pragma region Single task solver
	
	GeneticTaskSolver* solver = new GeneticTaskSolver(gr, fullPopulationSize, fullGenCount, subgraphsCount);
	solver->GeneratePopulation();
	solver->RunEvolution(fullGenCount, pm, pvm, pv);
#pragma endregion


#pragma region Island model



	vector<Organizm*> champs;

	vector<GeneticTaskSolver*> solvers;
	for (int i = 0; i < solversCount; i++)
	{
		solvers.push_back(new GeneticTaskSolver(gr, 
			fullPopulationSize/solversCount, 
			fullCrossoverCount/solversCount, 
			subgraphsCount));
	}

	omp_set_num_threads(solversCount);

	#pragma omp parallel for
	for (int i = 0; i < solversCount; i++)
	{
		solvers[i]->GeneratePopulation();
	}

	for (int g = 0; g < epochCount; g++)
	{
		
		//run partial evo
		#pragma omp parallel for
		for (int i = 0; i < solversCount; i++)
		{
			solvers[i]->RunEvolution(fullGenCount/epochCount, pm, pvm, pv);
		}
		//get best
		champs.clear();
		for (int i = 0; i < solversCount; i++)
		{
			champs.push_back(solvers[i]->GetBest()->Clone());
		}
		std::sort(champs.begin(), champs.end(), LessFitted);
		//inject best
		for (int i = 0; i < solversCount; i++)
		{
			if (champs[i % topCount] != solvers[i]->GetBest())
			{

				solvers[i]->Inject(champs[i % topCount]);
			}
			else
			{
				solvers[i]->Inject(champs[(i + 1) % topCount]);
			}
		}
	}
#pragma endregion
    return 0;
}

