#pragma once
#include <stdlib.h>
#include <set>
#include <random>
#include <limits.h>
#include "Graph.h"



struct Organizm
{
	int chromoSize;
	int *chromo;
	double fitness;
	Organizm(int size, int K)
	{
		chromoSize = size;
		chromo = new int[size];
		for (int i = 0; i < size; i++)
		{
			chromo[i] = rand() % K;
		}
		fitness = 0;
	}
	void Print()
	{
		for (int i = 0; i < chromoSize; i++)
		{
			//cout << chromo[i];
			//cout << " ";
		}
		//cout << endl;
	}
	bool operator<(Organizm r)
	{
		return fitness < r.fitness;
	}
	Organizm* Clone()
	{ 
		Organizm* clone = new Organizm(chromoSize);
		for (int i = 0; i < chromoSize; i++)
		{
			clone->chromo[i] = chromo[i];
		}
		clone->fitness = fitness;
		return clone;
	}
	~Organizm()
	{
		//delete [] chromo;
	}
private:
	Organizm(int size)
	{
		chromoSize = size;
		chromo = new int[size];
		
		fitness = 0;
	}
};
class GeneticTaskSolver
{
	int _crossoverCount,
		_populationSize, 
		_chromosomeVariability; // subgraphs count
	double _p_m, // mutation probability
		_p_vm,  // probability of after-mutation selection for better 
		_p_v;
	vector<Organizm*> _population;
	vector<Organizm*>::iterator _champion;
	vector<Organizm*>::iterator _outsider;
	Graph *_target;
	void RunCrossover();
	void RunMutation();
	void RunSelection();
	void CollectStats();

public:
	GeneticTaskSolver(Graph *target, int populationSize, int crossoverCount, int chromosomeVariability);

	void GeneratePopulation();
	void RunEvolution(int generationCount, double pm, double pvm, double pv);
	
	Organizm* GetBest();
	void Inject(Organizm *candidate);
	

	~GeneticTaskSolver();
};

