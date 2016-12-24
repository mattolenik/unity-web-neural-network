using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingLib
{
    public class Evolver
    {
        public List<Genome> Population { get; private set; }

        public int GenerationCount { get; private set; }

        readonly double mutationRate;

        readonly double crossoverRate;

        double maxPerturbation = 0.3;

        int numEliteCopies = 1;

        int numElite = 2;

        readonly Random rnd;

        Genome Mutate(Genome genome)
        {
            var weights = new List<double>(genome.WeightCount);
            for (var i = 0; i < genome.WeightCount; i++)
            {
                var weight = genome[i];
                if (rnd.NextDouble() < mutationRate)
                {
                    weight += rnd.NextWeight() * maxPerturbation;
                }
                weights.Add(weight);
            }
            return new Genome(weights, 0);
        }

        void Crossover(Genome parent1, Genome parent2, out Genome offspring1, out Genome offspring2)
        {
            // If the ancestors are equivalent, or crossover rate not met, return ancestors as offspring
            if (rnd.NextDouble() > crossoverRate || parent1.Equals(parent2))
            {
                offspring1 = new Genome(parent1, 0);
                offspring2 = new Genome(parent2, 0);
                return;
            }

            // For now, parent weight counts are the same
            var crossoverPoint = rnd.Next(0, parent1.WeightCount - 1);

            var offspring1Genes = parent1.Take(crossoverPoint).Concat(parent2.Skip(crossoverPoint));
            offspring1 = new Genome(offspring1Genes, 0);

            var offspring2Genes = parent2.Take(crossoverPoint).Concat(parent1.Skip(crossoverPoint));
            offspring2 = new Genome(offspring2Genes, 0);
        }

        Genome GetGenomeRoulette(List<Genome> population)
        {
            var slice = rnd.NextDouble() * population.Sum(x => x.Fitness);

            var fitnessSoFar = 0.0;

            foreach (var genome in population)
            {
                fitnessSoFar += genome.Fitness;

                if (fitnessSoFar >= slice)
                {
                    return genome;
                }
            }

            return null;
        }

        public Evolver(int populationSize, double mutationRate, double crossoverRate, int numWeights)
        {
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            rnd = new Random();
            Population = new List<Genome>(populationSize);
            for (var i = 0; i < populationSize; i++)
            {
                var weights = Enumerable.Range(0, numWeights).Select(x => rnd.NextWeight());
                var genome = new Genome(weights, 0);
                Population.Add(genome);
            }
        }

        public void NewGeneration()
        {
            GenerationCount++;
            var sorted = Population.OrderByDescending(x => x.Fitness).ToList();
            var newPopulation = new List<Genome>(Population.Count);

            // Add in elitism by adding some copies of the fittest genomes.
            var best = sorted.Take(numElite).ToArray();
            for (var i = 0; i < numEliteCopies; i++)
            {
                newPopulation.AddRange(best);
            }

            while (newPopulation.Count < Population.Count)
            {
                var parent1 = GetGenomeRoulette(Population);
                var parent2 = GetGenomeRoulette(Population);

                Crossover(parent1, parent2, out var offspring1, out var offspring2);

                offspring1 = Mutate(offspring1);
                offspring2 = Mutate(offspring2);

                newPopulation.Add(offspring1);
                newPopulation.Add(offspring2);
            }

            Population = newPopulation;
        }

        /// <summary>
        /// Import a single genome as the basis for this population
        /// </summary>
        /// <param name="genome">the Genome to import</param>
        public void Import(Genome genome)
        {
            var popSize = Population.Count;
            Population.Clear();
            for (var i = 0; i < popSize; i++)
            {
                Population.Add(genome);
            }
        }
    }
}