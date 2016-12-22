using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace PathfindingLib
{
    public class Evolver
    {
        public List<Genome> Population { get; private set; }

        public double AverageFitness { get; private set; }

        public double BestFitness { get; private set; }

        public int GenerationCount { get; private set; }

        int genomeLength;

        double totalFitness;

        double worstFitness;

        Genome fittestGenome;

        double mutationRate = 0.2;

        double crossoverRate = 0.7;

        double maxPerturbation = 0.3;

        int numEliteCopies = 1;

        int numElite = 4;

        static readonly IEqualityComparer<double> fuzzyComparer = new FuzzyDoubleComparer();

        void Mutate(ref List<double> genome)
        {
            for (int i = 0; i < genome.Count; i++)
            {
                if (Random.value < mutationRate)
                {
                    genome[i] += Random.Range(-1f, 1f) * maxPerturbation;
                }
            }
        }

        void Crossover(List<double> parent1, List<double> parent2, out List<double> offspring1, out List<double> offspring2)
        {
            // If the ancestors are equivalent, or crossover rate not met, return ancestors as offspring
            if (Random.value > crossoverRate || parent1.SequenceEqual(parent2, fuzzyComparer))
            {
                offspring1 = parent1.ToList();
                offspring2 = parent2.ToList();
                return;
            }

            var crossoverPoint = Random.Range(0, genomeLength - 1);
            offspring1 = new List<double>();
            offspring2 = new List<double>();

            for (var i = 0; i < crossoverPoint; i++)
            {
                offspring1.Add(parent1[i]);
                offspring2.Add(parent2[i]);
            }

            for (var i = crossoverPoint; i < parent1.Count; i++)
            {
                offspring1.Add(parent2[i]);
                offspring2.Add(parent1[i]);
            }
        }

        Genome GetGenomeRoulette()
        {
            var slice = Random.value * totalFitness;

            var fitnessSoFar = 0.0;

            foreach (var genome in Population)
            {
                fitnessSoFar += genome.Fitness;

                if (fitnessSoFar >= slice)
                {
                    return genome;
                }
            }

            return null;
        }

        void GrabNBest(int nBest, int numCopies, List<Genome> pop)
        {
            while (nBest-- > 0)
            {
                for (var i = 0; i < numCopies; i++)
                {
                    pop.Add(Population[Population.Count - 1 - nBest]);
                }
            }
        }

        void CalculateScores()
        {
            totalFitness = 0;
            var highestSoFar = 0.0;
            var lowestSoFar = double.MaxValue;

            foreach (var genome in Population)
            {
                if (genome.Fitness > highestSoFar)
                {
                    highestSoFar = genome.Fitness;
                    fittestGenome = genome;
                    BestFitness = highestSoFar;
                }

                if (genome.Fitness < lowestSoFar)
                {
                    lowestSoFar = genome.Fitness;
                    worstFitness = lowestSoFar;
                }

                totalFitness += genome.Fitness;
            }

            AverageFitness = totalFitness / Population.Count;
        }

        void Reset()
        {
            totalFitness = 0;
            BestFitness = 0;
            worstFitness = double.MaxValue;
            AverageFitness = 0;
        }

        public Evolver(int populationSize, double mutationRate, double crossoverRate, int numWeights)
        {
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            genomeLength = numWeights;
            Population = new List<Genome>(populationSize);
            for (var i = 0; i < populationSize; i++)
            {
                var genome = new Genome();
                Population.Add(genome);
                for (var k = 0; k < genomeLength; k++)
                {
                    genome.Weights.Add(Random.Range(-1f, 1f));
                }
            }
        }

        public List<Genome> Epoch(List<Genome> oldPopulation)
        {
            Population = oldPopulation.ToList();
            GenerationCount++;
            Reset();
            Population.Sort((x, y) => x.Fitness.CompareTo(y.Fitness));
            CalculateScores();

            var newPopulation = new List<Genome>();

            // Add in elitism by adding some copies of the fittest genomes.
            // Must be an even number for roulette sampling to work.
            if (numEliteCopies * numElite % 2 == 0)
            {
                GrabNBest(numElite, numEliteCopies, newPopulation);
            }

            while (newPopulation.Count < Population.Count)
            {
                var parent1 = GetGenomeRoulette();
                var parent2 = GetGenomeRoulette();

                Crossover(parent1.Weights, parent2.Weights, out var offspring1, out var offspring2);

                Mutate(ref offspring1);
                Mutate(ref offspring2);

                newPopulation.Add(new Genome(offspring1, 0));
                newPopulation.Add(new Genome(offspring2, 0));
            }

            Population = newPopulation;

            return Population;
        }
    }
}