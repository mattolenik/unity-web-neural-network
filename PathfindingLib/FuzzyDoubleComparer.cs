using System;
using System.Collections.Generic;

namespace PathfindingLib
{
    public class FuzzyDoubleComparer : IEqualityComparer<double>
    {
        public bool Equals(double x, double y)
        {
            return Math.Abs(x - y) < 0.01;
        }

        public int GetHashCode(double obj)
        {
            throw new NotImplementedException();
        }
    }
}