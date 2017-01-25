using System;
using System.Collections.Generic;

namespace Rollaround
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

        public Evolver(int populationSize, float mutationRate, float crossoverRate, int numWeights, Genome[] initialGenes, int elitism = 4, int eliteCopies = 2, Random random = null)
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

        public void NewGeneration(float[] fitnesses)
        {
            GenerationCount++;

            // Assign fitness values to genomes
            for (var i = 0; i < fitnesses.Length; i++)
            {
                Population[i].Fitness = fitnesses[i];
            }

            var newPopulation = new Genome[Population.Length];

            // Add in elitism by adding copies of the fittest genomes.
            var best = FindBest(numElite);

            // Fill numEliteCopies * best.Length instances
            var index = 0;
            for (var i = 0; i < numEliteCopies; i++)
            {
                for (var k = 0; k < best.Length; k++)
                {
                    var x = i * best.Length + k;
                    newPopulation[x] = best[k];
                    index++;
                }
            }
            // Continue with previous index until full
            while (index < Population.Length)
            {
                var parent1 = GetGenomeRoulette(Population);
                var parent2 = GetGenomeRoulette(Population);

                Crossover(parent1, parent2, out var offspring1, out var offspring2);

                offspring1 = Mutate(offspring1);
                offspring2 = Mutate(offspring2);

                newPopulation[index++] = offspring1;
                newPopulation[index++] = offspring2;
            }

            Population = newPopulation;
        }
        
        Genome[] FindBest(int topN)
        {
            var best = new Genome[topN];
            var picked = new bool[Population.Length];
            for (var i = 0; i < best.Length; i++)
            {
                // For each slot of best[], loop through Population
                // and assign the top value. Store that index in the bool
                // array, and use that to skip previously found values.
                var id = 0;
                for (var k = 0; k < Population.Length; k++)
                {
                    if (!picked[k] && (best[i] == null || Population[k].Fitness > best[i].Fitness))
                    {
                        best[i] = Population[k];
                        // Track the last index k
                        id = k;
                    }
                }
                picked[id] = true;
            }
            return best;
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
            if (rnd.NextFloat() > crossoverRate || ReferenceEquals(parent1, parent2))
            {
                offspring1 = new Genome(parent1.Weights, 0);
                offspring2 = new Genome(parent2.Weights, 0);
                return;
            }

            // For now, parent weight counts are the same
            var crossoverPoint = rnd.Next(0, parent1.WeightCount - 1);

            var offspring1Genes = new float[parent1.WeightCount];
            var offspring2Genes = new float[parent2.WeightCount];

            for (var i = 0; i < crossoverPoint; i++)
            {

                offspring1Genes[i] = parent1[i];
                offspring2Genes[i] = parent2[i];
            }

            for (var i = crossoverPoint; i < parent1.WeightCount; i++)
            {
                offspring1Genes[i] = parent2[i];
                offspring2Genes[i] = parent1[i];
            }
            offspring1 = new Genome(offspring1Genes, 0);
            offspring2 = new Genome(offspring2Genes, 0);
        }

        Genome GetGenomeRoulette(Genome[] population)
        {
            var sum = 0f;
            for (var i = 0; i < population.Length; i++)
            {
                sum += population[i].Fitness;
            }
            var slice = rnd.NextFloat() * sum;

            var fitnessSoFar = 0.0;

            foreach (var genome in population)
            {
                fitnessSoFar += genome.Fitness;

                if (fitnessSoFar >= slice)
                {
                    return genome;
                }
            }

            return population[population.Length - 1];
        }
    }
}