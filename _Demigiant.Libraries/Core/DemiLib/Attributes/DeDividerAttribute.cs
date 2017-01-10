// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/17 14:23
// License Copyright (c) Daniele Giardini

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// Decorator
    /// </summary>
    public class DeDividerAttribute : PropertyAttribute
    {
        public int height;
        public string hexColor;
        public int marginTop, marginBottom;

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