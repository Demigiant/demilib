// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/21 12:22
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #
        /// </summary>
        public static string ToHex(this Color32 color)
        {
            return color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
        }

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