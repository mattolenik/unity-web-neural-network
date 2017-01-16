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
        public static float NextFloat(this Random rnd, float min, float max)
        {
            return rnd.NextFloat() * (max - min) + min;
        }

        /// <summary>
        /// Returns a random float between 0.0 and 1.0
        /// </summary>
        /// <param name="rnd">a Random instance</param>
        /// <returns>a random value between 0.0 and 1.0, inclusive</returns>
        public static float NextFloat(this Random rnd)
        {
            return (float)rnd.NextDouble();
        }

        /// <summary>
        /// Returns a random weight between -1.0 and 1.0
        /// </summary>
        /// <param name="rnd">a Random instance</param>
        /// <returns>a value between -1.0 and 1.0, inclusive</returns>
        public static float NextWeight(this Random rnd)
        {
            return rnd.NextFloat(-1.0f, 1.0f);
        }
    }
}
