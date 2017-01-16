using System;

namespace PathfindingLib
{
    public class Neuron
    {
        public float[] Weights { get; }

        public Neuron(int numInputs, Random rnd)
        {
            // +1 for bias weight
            Weights = new float[numInputs + 1];
            for (var i = 0; i < numInputs + 1; i++)
            {
                Weights[i] = rnd.NextFloat(-1.0f, 1.0f);
            }
        }
    }
}