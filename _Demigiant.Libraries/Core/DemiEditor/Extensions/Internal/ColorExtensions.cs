// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/20 21:40
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.Internal
{
    internal static class ColorExtensions
    {
        /// <summary>
        /// Changes the alpha of this color and returns it
        /// </summary>
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}