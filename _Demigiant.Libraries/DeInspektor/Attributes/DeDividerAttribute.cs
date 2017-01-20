// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/17 14:23
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Decorator</code>
    /// <para>Draws a divider in the Inspector</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeDividerAttribute : PropertyAttribute
    {
        internal int height;
        internal string hexColor;
        internal int marginTop, marginBottom;

        /// <summary>
        /// Draws a divider with the given size, color and margins
        /// </summary>
        /// <param name="height">Divider height</param>
        /// <param name="hexColor">Hex color (# optional)</param>
        /// <param name="marginTop">Top margin</param>
        /// <param name="marginBottom">Bottom margin</param>
        public DeDividerAttribute(int height = 2, string hexColor = "555555", int marginTop = 1, int marginBottom = 3)
        {
            this.height = height;
            this.marginTop = marginTop;
            this.marginBottom = marginBottom;
            this.hexColor = hexColor;
        }
    }
}