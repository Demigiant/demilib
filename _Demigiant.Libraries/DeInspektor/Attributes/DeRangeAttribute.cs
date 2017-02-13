// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/13 16:59
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code><para/>
    /// Shows a float/int value with a slider.
    /// Works like Unity's Range attribute, but has extra options for custom label, and also works with DemiLib's <code>Range</code> structs
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeRangeAttribute : PropertyAttribute
    {
        internal float min, max;
        internal string label;

        /// <summary>
        /// Shows a float/int value with a slider.
        /// Works like Unity's Range attribute, but has extra options for custom label, and also works with DemiLib's <code>Range</code> structs
        /// </summary>
        /// <param name="min">Min value</param>
        /// <param name="max">Max value</param>
        /// <param name="label">Override field label</param>
        public DeRangeAttribute(float min, float max, string label = null)
        {
            this.min = min;
            this.max = max;
            this.label = label;
        }
    }
}