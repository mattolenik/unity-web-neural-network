using System;

namespace Rollaround
{
    [Serializable]
    public class Genome
    {
        public float[] Weights;

        public float Fitness { get; set; }

        public int WeightCount => Weights.Length;

        public Genome(float[] weights, float fitness)
        {
            Weights = new float[weights.Length];
            Fitness = fitness;
            Array.Copy(weights, Weights, weights.Length);
        }

        public float this[int key]
        {
            get { return Weights[key]; }
            set { Weights[key] = value; }
        }
    }
}