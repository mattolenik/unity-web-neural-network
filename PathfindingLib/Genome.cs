using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PathfindingLib
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Genome : IEquatable<Genome>, IEnumerable<float>
    {
        [JsonProperty]
        float[] weights;

        public float Fitness { get; set; }

        public int WeightCount => weights.Length;

        static readonly IEqualityComparer<float> Fuzzy = new FuzzyFloatComparer();

        public Genome(float[] weights, float fitness)
        {
            this.weights = weights;
            Fitness = fitness;
        }

        public float this[int key]
        {
            get { return weights[key]; }
            set { weights[key] = value; }
        }

        public bool Equals(Genome other)
        {
            return other != null && weights.SequenceEqual(other.weights, Fuzzy);
        }

        public IEnumerator<float> GetEnumerator()
        {
            // Force a copy of values
            foreach (var weight in weights)
            {
                yield return weight;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static implicit operator float[](Genome g)
        {
            // Force a copy
            return g.weights.ToArray();
        }
    }
}