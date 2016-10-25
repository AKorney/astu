#include "stdafx.h"



#include "GeneticTaskSolver.h"




GeneticTaskSolver::GeneticTaskSolver(Graph * target, int populationSize, int crossoverCount, int chromosomeVariability)
{
	_target = target;
	_populationSize = populationSize;
	_crossoverCount = crossoverCount;
	_chromosomeVariability = chromosomeVariability;

}

void GeneticTaskSolver::GeneratePopulation()
{
	int graphSize = _target->GetVerticesCount();
	for (int i = 0; i < _populationSize; i++)
	{
		Organizm *org = new Organizm(graphSize, _chromosomeVariability);
		for (int chrom = 0; chrom < graphSize; chrom++)
		{
			org->chromo[chrom] = rand() % _chromosomeVariability;
		}
		_population.push_back(org);
	}
}

void GeneticTaskSolver::RunEvolution(int generationCount, double pm, double pvm, double pv)
{
	_p_m = pm;
	_p_v = pv;
	_p_vm = pvm;
	for (int g = 0; g < generationCount; g++)
	{
		RunCrossover();
		RunMutation();
		RunSelection();
		CollectStats();
	}
}

Organizm * GeneticTaskSolver::GetBest()
{
	return *_champion;
}

void GeneticTaskSolver::Inject(Organizm * candidate)
{
	//if (candidate->fitness > (*_champion)->fitness)
	//{
		_population.erase(_outsider);
		_population.push_back(candidate);
	//}
}

void GeneticTaskSolver::CollectStats()
{
	double fitMax = -std::numeric_limits<double>::max();
	double fitMin = std::numeric_limits<double>::max();
	for (vector<Organizm*>::iterator i = _population.begin();
		i != _population.end();
		i++)
	{
		if ((*i)->fitness > fitMax)
		{
			fitMax = (*i)->fitness;
			_champion = i;			
		}
		if ((*i)->fitness < fitMin)
		{
			fitMin = (*i)->fitness;
			_outsider = i;
		}
	}
}

void GeneticTaskSolver::RunCrossover()
{
	bool *crossoverFlags = new bool[_populationSize];
	int parentA, parentB;
	for (int cross = 0; cross < _crossoverCount; cross++)
	{
		do
		{
			parentA = rand() % _populationSize;
		} while (!crossoverFlags[parentA]);
		crossoverFlags[parentA] = false;

		do
		{
			parentB = rand() % _populationSize;
		} while (!crossoverFlags[parentB]);
		crossoverFlags[parentB] = false;

		Organizm *childA, *childB;
		childA = _population[parentA]->Clone();
		childB = _population[parentB]->Clone();

		
		cout << "Parent A\n";
		_population[parentA]->Print();
		cout << "Parent B\n";
		_population[parentB]->Print();



		int crossoverPosition = rand() % (childA->chromoSize - 2) + 1;

		for (int i = crossoverPosition; i < childA->chromoSize; i++)
		{
			int buffer = childA->chromo[i];
			childA->chromo[i] = childB->chromo[i];
			childB->chromo[i] = buffer;
		}
		_population.push_back(childA);
		_population.push_back(childB);

		cout << "Child A\n";
		childA->Print();
		cout << "Child B\n";
		childB->Print();

	}
}

void GeneticTaskSolver::RunMutation()
{
	for (vector<Organizm*>::iterator pop_iter = _population.begin();
		pop_iter != _population.end();
		pop_iter++)
	{
		double f = (double)rand() / RAND_MAX;
		if (f < _p_m)
		{
			cout << "Try to mutate:\n";

			Organizm* clone = (*pop_iter)->Clone();
			clone->Print();
			for (int i = 0; i < clone->chromoSize; i++)
			{
				int oldValue = clone->chromo[i];
				int newValue;
				do
				{
					newValue = rand() % _chromosomeVariability;
				} while (newValue == oldValue);
				clone->chromo[i] = newValue;
				//int buffer = clone->chromo[i];
				//clone->chromo[i] = clone->chromo[clone->chromoSize - i - 1];
				//clone->chromo[clone->chromoSize - i - 1] = buffer;
			}
			(*pop_iter)->fitness = _target->EvaluatePartitioning((*pop_iter)->chromo, _chromosomeVariability);
			clone->fitness = _target->EvaluatePartitioning(clone->chromo, _chromosomeVariability);
			Organizm *best = (*pop_iter)->fitness > clone->fitness
				? *pop_iter
				: clone;
			Organizm *worse = (*pop_iter)->fitness < clone->fitness
				? *pop_iter
				: clone;
			f = (double)rand() / RAND_MAX;
			
			cout << "New:\n";
			clone->Print();

			if (f < _p_vm)
			{
				cout << "Best chosen\n";
				*pop_iter = best;
				//delete worse;
			}
			else
			{
				cout << "Worse chosen\n";
				*pop_iter = worse;
				//delete best;
			}
		}
	}
}

void GeneticTaskSolver::RunSelection()
{
	for (vector<Organizm*>::iterator i = _population.begin();
		i != _population.end();
		i++)
	{
			(*i)->fitness = _target->EvaluatePartitioning((*i)->chromo, _chromosomeVariability);
	}
	set<Organizm*> done;
	int selectionCount = _crossoverCount * 2;
	for (int i = 0; i < selectionCount; i++)
	{
		Organizm* forceA, *forceB;

		int indexA;
		do
		{
			indexA = rand() % _population.size();
			forceA = _population[indexA];
		} while (done.find(forceA) != done.end());
		done.insert(forceA);

		int indexB;
		do
		{
			indexB = rand() % _population.size();
			forceB = _population[indexB];
		} while (indexB == indexA 
			|| done.find(forceB) != done.end());
		done.insert(forceB);

		//forceA->fitness = _target->EvaluatePartitioning(forceA->chromo, _chromosomeVariability);
		//forceB->fitness = _target->EvaluatePartitioning(forceB->chromo, _chromosomeVariability);

		double f = (double)rand() / RAND_MAX;

		int best = forceA->fitness > forceB->fitness
			? indexA
			: indexB;

		int worse = forceA->fitness < forceB->fitness
			? indexA
			: indexB;

		if (f < _p_v)
		{
			//Organizm *o = _population[worse];
			_population.erase(_population.begin() + worse);
			//delete o;
		}
		else
		{
			//Organizm *o = _population[best];
			_population.erase(_population.begin() + best);
			//delete o;
		}

	}
	
	done.clear();
}

GeneticTaskSolver::~GeneticTaskSolver()
{
}
