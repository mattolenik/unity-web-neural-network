using System;

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
            if (fitnesses.Length != Population.Length)
            {
                throw new ArgumentOutOfRangeException("fitnesses", "Fitness array must be same length as population");
            }
            GenerationCount++;
            var i = 0;
            var k = 0;

            // Assign fitness values to genomes
            for (i = 0; i < fitnesses.Length; i++)
            {
                Population[i].Fitness = fitnesses[i];
            }

            var newPopulation = new Genome[Population.Length];

            // Add in elitism by adding copies of the fittest genomes.
            var best = FindBest(numElite);

            // Remove lowest performers
            var numCulled = 0;
            while (i++ < Population.Length &&
                   k++ < best.Length &&
                   numCulled < numEliteCopies * best.Length)
            {
                if(Population[i].Fitness < 1)
                {
                    Population[i] = Population[best[i]];
                    numCulled++;
                }
            }

            // Fill numEliteCopies * best.Length instances
            var index = 0;
            for (i = 0; i < numEliteCopies; i++)
            {
                for (k = 0; k < best.Length; k++)
                {
                    var x = i * best.Length + k;
                    newPopulation[x] = Population[best[k]];
                    index++;
                }
            }
            // Continue with previous index until full
            while (index < Population.Length)
            {
                var parent1 = GetRouletteIndex(Population);
                var parent2 = GetRouletteIndex(Population);

                Crossover(parent1, parent2, out var offspring1, out var offspring2);

                Mutate(ref offspring1);
                Mutate(ref offspring2);

                newPopulation[index++] = offspring1;
                newPopulation[index++] = offspring2;
            }

            Population = newPopulation;
        }
        
        /// <summary>
        /// Finds the best n performers in Population
        /// </summary>
        /// <param name="topN">number of top performers</param>
        /// <returns>an array of indexes for Population</returns>
        int[] FindBest(int topN)
        {
            var best = new int[topN];
            var bestFitnesses = new float[topN];
            var picked = new bool[Population.Length];
            for (var i = 0; i < best.Length; i++)
            {
                // For each slot of best[], loop through Population
                // and assign the top value. Store that index in the bool
                // array, and use that to skip previously found values.
                var id = 0;
                for (var k = 0; k < Population.Length; k++)
                {
                    if (!picked[k] && Population[k].Fitness > bestFitnesses[i])
                    {
                        best[i] = k;
                        bestFitnesses[i] = Population[k].Fitness;
                        // Track the last index k
                        id = k;
                    }
                }
                picked[id] = true;
            }
            return best;
        }

        void Mutate(ref Genome genome)
        {
            for (var i = 0; i < genome.WeightCount; i++)
            {
                if (rnd.NextFloat() < mutationRate)
                {
                    genome[i] += rnd.NextWeight() * maxPerturbation;
                }
            }
        }

        /// <summary>
        /// Populates two new offspring from parents
        /// </summary>
        /// <param name="parent1">Population array index of parent1</param>
        /// <param name="parent2">Population array index of parent2</param>
        /// <param name="offspring1">new child offspring</param>
        /// <param name="offspring2">new child offspring</param>
        void Crossover(int parent1, int parent2, out Genome offspring1, out Genome offspring2)
        {
            // If the ancestors are equivalent, or crossover rate not met, return ancestors as offspring
            if (rnd.NextFloat() > crossoverRate || ReferenceEquals(parent1, parent2))
            {
                offspring1 = Population[parent1];
                offspring2 = Population[parent2];
                return;
            }

            var wc = Population[parent1].WeightCount;
            if(wc != Population[parent2].WeightCount)
            {
                throw new ArgumentOutOfRangeException("parent2", "Parent genomes are expected to be of same length");
            }

            // For now, parent weight counts are the same
            var crossoverPoint = rnd.Next(0, wc - 1);

            var offspring1Genes = new float[wc];
            var offspring2Genes = new float[wc];
            offspring1 = new Genome(wc);
            offspring2 = new Genome(wc);

            for (var i = 0; i < crossoverPoint; i++)
            {

                offspring1[i] = Population[parent1][i];
                offspring2[i] = Population[parent2][i];
            }

            for (var i = crossoverPoint; i < wc; i++)
            {
                offspring1[i] = Population[parent2][i];
                offspring2[i] = Population[parent1][i];
            }
        }

        /// <summary>
        /// Returns an array index for selected Genome in population
        /// </summary>
        /// <param name="population">Population to sample</param>
        /// <returns>Array index for selected genome</returns>
        int GetRouletteIndex(Genome[] population)
        {
            var sum = 0f;
            for (var i = 0; i < population.Length; i++)
            {
                sum += population[i].Fitness;
            }
            var slice = rnd.NextFloat() * sum;

            var fitnessSoFar = 0.0;

            for(var i = 0; i < Population.Length; i++)
            {
                fitnessSoFar += Population[i].Fitness;

                if (fitnessSoFar >= slice)
                {
                    return i;
                }
            }

            return population.Length - 1;
        }
    }
}