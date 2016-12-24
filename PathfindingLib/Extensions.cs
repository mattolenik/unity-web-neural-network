using System;

namespace PathfindingLib
{
    static class Extensions
    {
        /// <summary>
        /// Returns a random value between min and max
        /// </summary>
        /// <param name="rnd">a Random instance</param>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <returns>a value between min and max, inclusive</returns>
        public static double NextDouble(this Random rnd, double min, double max)
        {
            return rnd.NextDouble() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random weight between -1.0 and 1.0
        /// </summary>
        /// <param name="rnd">a Random instance</param>
        /// <returns>a value between -1.0 and 1.0, inclusive</returns>
        public static double NextWeight(this Random rnd)
        {
            return rnd.NextDouble(-1.0, 1.0);
        }
    }
}
