// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/13 17:29
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib
{
    /// <summary>
    /// A serializable struct including a min and a max float value
    /// </summary>
    [Serializable]
    public struct Range
    {
        /// <summary>Min value</summary>
        public float min;
        /// <summary>Max value</summary>
        public float max;

        /// <summary>
        /// Creates a new Range
        /// </summary>
        public Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        /// <summary>
        /// Returns a random value within this range (min/max included)
        /// </summary>
        public float RandomWithin()
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// Returns a value within the range at the given 0-1 (clamped) percentage
        /// </summary>
        public float Evaluate(float atPercentage)
        {
            if (atPercentage > 1) atPercentage = 1;
            else if (atPercentage < 0) atPercentage = 0;
            return min + (max - min) * atPercentage;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return "(" + min + "/" + max + ")";
        }
        
        public static bool operator==(Range a, Range b)
        {
            return Mathf.Approximately(a.min, b.min) && Mathf.Approximately(a.max, b.max);
        }

        public static bool operator !=(Range a, Range b)
        {
            return !(a == b);
        }
    }
}