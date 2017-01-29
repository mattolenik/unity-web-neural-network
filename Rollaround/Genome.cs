using System;

namespace Rollaround
{
    [Serializable]
    public struct Genome
    {
        public float[] Weights;

        public float Fitness;

        public int WeightCount => Weights.Length;

        public Genome(float[] weights, float fitness)
        {
            Weights = new float[weights.Length];
            Fitness = fitness;
            Array.Copy(weights, Weights, weights.Length);
        }

        public Genome(int weightCount)
        {
            Weights = new float[weightCount];
            Fitness = 0;
        }

        public float this[int key]
        {
            get { return Weights[key]; }
            set { Weights[key] = value; }
        }
    }
}