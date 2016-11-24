using System.Collections.Generic;

namespace PathfindingLib
{
    public class Genome
    {
        public List<double> Weights { get; private set; }

        public double Fitness { get; set; }

        public Genome()
        {
            Fitness = 0;
            Weights = new List<double>();
        }

        public Genome(List<double> weights, double fitness)
        {
            Weights = weights;
            Fitness = fitness;
        }
    }
}