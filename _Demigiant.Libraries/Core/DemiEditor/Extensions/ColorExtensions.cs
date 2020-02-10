// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/20 21:40
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor
{
    public static class ColorExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns a new color equal to the given one with changed brightness
        /// </summary>
        /// <param name="color">Color to evaluate</param>
        /// <param name="brightnessFactor">Brightness factor (multiplied by current brightness)</param>
        /// <param name="alpha">If set applies this alpha value</param>
        public static Color CloneAndChangeBrightness(this Color color, float brightnessFactor, float? alpha = null)
        {
            float h, s, v;
            RGBToHSV(color, out h, out s, out v);
            v *= brightnessFactor;
            if (v < 0) v = 0;
            else if (v > 1) v = 1;
            Color result = HSVToRGB(h, s, v);
            if (alpha != null) result.a = (float)alpha;
            return result;
        }

        /// <summary>
        /// Returns a new color equal to the given one with changed saturation
        /// </summary>
        /// <param name="color">Color to evaluate</param>
        /// <param name="saturationFactor">Saturation factor (multiplied by current brightness)</param>
        /// <param name="alpha">If set applies this alpha value</param>
        public static Color CloneAndChangeSaturation(this Color color, float saturationFactor, float? alpha = null)
        {
            float h, s, v;
            RGBToHSV(color, out h, out s, out v);
            s *= saturationFactor;
            if (s < 0) s = 0;
            else if (s > 1) s = 1;
            Color result = HSVToRGB(h, s, v);
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
        /// Returns a new color equal to the given one with changed alpha
        /// </summary>
        public static Color CloneAndSetAlpha(this Color color, float alpha)
        {
            Color result = new Color(color.r, color.g, color.b, alpha);
            return result;
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

        #endregion

        #region Methods

        #region Internal Implementations for older Unity Versions

        static void RGBToHSV(Color rgbColor, out float H, out float S, out float V)
        {
            if (rgbColor.b > rgbColor.g && rgbColor.b > rgbColor.r) RGBToHSVHelper(4f, rgbColor.b, rgbColor.r, rgbColor.g, out H, out S, out V);
            else if (rgbColor.g > rgbColor.r) RGBToHSVHelper(2f, rgbColor.g, rgbColor.b, rgbColor.r, out H, out S, out V);
            else RGBToHSVHelper(0f, rgbColor.r, rgbColor.g, rgbColor.b, out H, out S, out V);
        }

        static void RGBToHSVHelper(float offset, float dominantcolor, float colorone, float colortwo, out float H, out float S, out float V)
        {
            V = dominantcolor;
            if (V != 0f) {
                float num;
                num = ((!(colorone > colortwo)) ? colorone : colortwo);
                float num2 = V - num;
                if (num2 != 0f) {
                    S = num2 / V;
                    H = offset + (colorone - colortwo) / num2;
                } else {
                    S = 0f;
                    H = offset + (colorone - colortwo);
                }
                H /= 6f;
                if (H < 0f) H += 1f;
            } else {
                S = 0f;
                H = 0f;
            }
        }

        static Color HSVToRGB(float H, float S, float V, bool hdr = true)
        {
            Color white = Color.white;
            if (S == 0f) {
                white.r = V;
                white.g = V;
                white.b = V;
            } else if (V == 0f) {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
            } else {
                white.r = 0f;
                white.g = 0f;
                white.b = 0f;
                float num = H * 6f;
                int num2 = (int)Mathf.Floor(num);
                float num3 = num - (float)num2;
                float num4 = V * (1f - S);
                float num5 = V * (1f - S * num3);
                float num6 = V * (1f - S * (1f - num3));
                switch (num2) {
                case 0:
                    white.r = V;
                    white.g = num6;
                    white.b = num4;
                    break;
                case 1:
                    white.r = num5;
                    white.g = V;
                    white.b = num4;
                    break;
                case 2:
                    white.r = num4;
                    white.g = V;
                    white.b = num6;
                    break;
                case 3:
                    white.r = num4;
                    white.g = num5;
                    white.b = V;
                    break;
                case 4:
                    white.r = num6;
                    white.g = num4;
                    white.b = V;
                    break;
                case 5:
                    white.r = V;
                    white.g = num4;
                    white.b = num5;
                    break;
                case 6:
                    white.r = V;
                    white.g = num6;
                    white.b = num4;
                    break;
                case -1:
                    white.r = V;
                    white.g = num4;
                    white.b = num5;
                    break;
                }
                if (!hdr) {
                    white.r = Mathf.Clamp(white.r, 0f, 1f);
                    white.g = Mathf.Clamp(white.g, 0f, 1f);
                    white.b = Mathf.Clamp(white.b, 0f, 1f);
                }
            }
            return white;
        }

        #endregion

        #endregion
    }
}