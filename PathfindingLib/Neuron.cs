using System.Collections.Generic;
using System.Text;
using UnityEngineInternal;
using Random = UnityEngine.Random;

namespace PathfindingLib
{
    public class Neuron
    {
        public List<double> Weights { get; private set; }

        public Neuron(int numInputs)
        {
            Weights = new List<double>(numInputs + 1);
            for (var i = 0; i < numInputs + 1; i++)
            {
                Weights.Add(Random.Range(-1.0f, 1.0f));
            }
        }
    }
}