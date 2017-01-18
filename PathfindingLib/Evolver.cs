using System;
using System.Collections.Generic;
using System.Linq;

namespace PathfindingLib
{
    public class Evolver
    {
        public Genome[] Population { get; private set; }

        public int GenerationCount { get; private set; }

        readonly float mutationRate;
        readonly float crossoverRate;
        readonly float maxPerturbation = 0.3f;
        readonly int numEliteCopies;
        readonly int numElite;
        readonly Random rnd;

        public Evolver(
            int populationSize,
            float mutationRate,
            float crossoverRate,
            int numWeights,
            Genome[] initialGenes,
            int elitism = 4,
            int eliteCopies = 2,
            Random random = null)
        {
            this.mutationRate = mutationRate;
            this.crossoverRate = crossoverRate;
            numElite = elitism;
            numEliteCopies = eliteCopies;
            rnd = random ?? new Random();
            Population = new Genome[populationSize];
            for (var i = 0; i < initialGenes.Length; i++)
            {
                Population[i] = initialGenes[i];
                Population[i].Fitness = 0;
            }
            for (var i = initialGenes.Length; i < populationSize; i++)
            {
                var weights = new float[numWeights];
                for (var k = 0; k < weights.Length; k++)
                {
                    weights[k] = rnd.NextWeight();
                }
                Population[i] = new Genome(weights, 0);
            }
        }

        public void NewGeneration(IEnumerable<float> fitnesses)
        {
            using (var e = fitnesses.GetEnumerator())
            {
                foreach (var p in Population)
                {
                    e.MoveNext();
                    p.Fitness = e.Current;
                }
            }
            GenerationCount++;
            var best = Population.OrderByDescending(x => x.Fitness).Take(numElite).ToArray();
            var newPopulation = new List<Genome>(Population.Length);

            // Add in elitism by adding some copies of the fittest genomes.
            for (var i = 0; i < numEliteCopies; i++)
            {
                newPopulation.AddRange(best);
            }

            while (newPopulation.Count < Population.Length)
            {
                var parent1 = GetGenomeRoulette(Population);
                var parent2 = GetGenomeRoulette(Population);

                Crossover(parent1, parent2, out var offspring1, out var offspring2);

                offspring1 = Mutate(offspring1);
                offspring2 = Mutate(offspring2);

                newPopulation.Add(offspring1);
                newPopulation.Add(offspring2);
            }

            Population = newPopulation.ToArray();
        }

        Genome Mutate(Genome genome)
        {
            var weights = new float[genome.WeightCount];
            for (var i = 0; i < genome.WeightCount; i++)
            {
                var weight = genome[i];
                if (rnd.NextFloat() < mutationRate)
                {
                    weight += rnd.NextWeight() * maxPerturbation;
                }
                weights[i] = weight;
            }
            return new Genome(weights, 0);
        }

        void Crossover(Genome parent1, Genome parent2, out Genome offspring1, out Genome offspring2)
        {
            // If the ancestors are equivalent, or crossover rate not met, return ancestors as offspring
            if (rnd.NextFloat() > crossoverRate || parent1.Equals(parent2))
            {
                offspring1 = new Genome(parent1, 0);
                offspring2 = new Genome(parent2, 0);
                return;
            }

            // For now, parent weight counts are the same
            var crossoverPoint = rnd.Next(0, parent1.WeightCount - 1);

            var offspring1Genes = parent1.Take(crossoverPoint).Concat(parent2.Skip(crossoverPoint)).ToArray();
            offspring1 = new Genome(offspring1Genes, 0);

            var offspring2Genes = parent2.Take(crossoverPoint).Concat(parent1.Skip(crossoverPoint)).ToArray();
            offspring2 = new Genome(offspring2Genes, 0);
        }

        Genome GetGenomeRoulette(Genome[] population)
        {
            var slice = rnd.NextFloat() * population.Sum(x => x.Fitness);

            var fitnessSoFar = 0.0;

            foreach (var genome in population)
            {
                fitnessSoFar += genome.Fitness;

                if (fitnessSoFar >= slice)
                {
                    return genome;
                }
            }

            return population.Last();
        }
    }
}