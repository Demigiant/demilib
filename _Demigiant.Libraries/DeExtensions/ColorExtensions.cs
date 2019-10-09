// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/21 12:22
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns a new color equal to the given one with changed brightness
        /// </summary>
        /// <param name="color">Color to evaluate</param>
        /// <param name="brightnessFactor">Brightness factor (multiplied by current brightness)</param>
        /// <param name="alpha">If set applies this alpha value</param>
        public static Color ChangeBrightness(this Color color, float brightnessFactor, float? alpha = null)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            v *= brightnessFactor;
            if (v < 0) v = 0;
            else if (v > 1) v = 1;
            Color result = Color.HSVToRGB(h, s, v);
            if (alpha != null) result.a = (float)alpha;
            return result;
        }

        /// <summary>
        /// Returns a new color equal to the given one with changed saturation
        /// </summary>
        /// <param name="color">Color to evaluate</param>
        /// <param name="saturationFactor">Saturation factor (multiplied by current brightness)</param>
        /// <param name="alpha">If set applies this alpha value</param>
        public static Color ChangeSaturation(this Color color, float saturationFactor, float? alpha = null)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            s *= saturationFactor;
            if (s < 0) s = 0;
            else if (s > 1) s = 1;
            Color result = Color.HSVToRGB(h, s, v);
            if (alpha != null) result.a = (float)alpha;
            return result;
        }

        /// <summary>
        /// Changes the alpha of this color and returns it
        /// </summary>
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        /// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #
        /// </summary>
        /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(this Color32 color, bool includeAlpha = false)
        {
            string result = color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
            if (includeAlpha) result += color.a.ToString("x2");
            return result;
        }

        /// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #
        /// </summary>
        /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(this Color color, bool includeAlpha = false)
        {
            return ToHex((Color32)color, includeAlpha);
        }
    }
}