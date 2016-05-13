// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/05/05 18:54
// License Copyright (c) Daniele Giardini
namespace DG.DeExtensions
{
    public static class NumberExtensions
    {
        /// <summary>Returns TRUE if the int is within the given range.</summary>
        /// <param name="min">Min</param>
        /// <param name="max">Max</param>
        /// <param name="inclusive">If TRUE min/max range values will be valid, otherwise not</param>
        /// <returns></returns>
        public static bool IsWithinRange(this int n, int min, int max, bool inclusive = true)
        {
            return inclusive ? n >= min && n <= max : n > min && n < max;
        }
    }
}