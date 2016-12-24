using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace PathfindingLib
{
    public class Genome : IEquatable<Genome>, IEnumerable<double>
    {
        readonly List<double> weights;

        public double Fitness { get; set; }

        public int WeightCount => weights.Count;

        static readonly IEqualityComparer<double> fuzzy = new FuzzyDoubleComparer();

        public Genome()
        {
            Fitness = 0;
            weights = new List<double>();
        }

        public Genome(IEnumerable<double> weights, double fitness)
        {
            this.weights = weights.ToList();
            Fitness = fitness;
        }

        /// <summary>
        /// Import a previously exported genome
        /// </summary>
        /// <param name="data">previously exported genome</param>
        public static Genome Import(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                var result = serializer.Deserialize<Genome>(reader);
                return result;
            }
        }

        /// <summary>
        /// Export genome to BSON
        /// </summary>
        /// <returns>BSON byte array</returns>
        /// <remarks>BSON is more compact that .NET binary serialization</remarks>
        public byte[] Export()
        {
            using (var ms = new MemoryStream())
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, this);
                var data = ms.ToArray();
                return data;
            }
        }

        public double this[int key]
        {
            get { return weights[key]; }
            set { weights[key] = value; }
        }

        public bool Equals(Genome other)
        {
            return other != null && weights.SequenceEqual(other.weights, fuzzy);
        }

        public IEnumerator<double> GetEnumerator()
        {
            // Yielding out each value instead of returning weights.GetEnumerator()
            // ensures a new list is created, rather than referencing this one.
            foreach (var weight in weights)
            {
                yield return weight;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}