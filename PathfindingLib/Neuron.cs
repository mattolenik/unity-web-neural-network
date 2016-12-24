using System.Collections.Generic;
using System;

namespace PathfindingLib
{
    public class Neuron
    {
        public List<double> Weights { get; }

        public Neuron(int numInputs)
        {
            Weights = new List<double>(numInputs + 1);
            var rnd = new Random();
            for (var i = 0; i < numInputs + 1; i++)
            {
                Weights.Add(rnd.NextDouble(-1.0, 1.0));
            }
        }
    }
}