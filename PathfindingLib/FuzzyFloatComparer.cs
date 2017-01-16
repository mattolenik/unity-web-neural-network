using System;
using System.Collections.Generic;

namespace PathfindingLib
{
    public class FuzzyFloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y)
        {
            return Math.Abs(x - y) < 0.01f;
        }

        public int GetHashCode(float obj)
        {
            throw new NotImplementedException();
        }
    }
}