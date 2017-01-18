// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 20:22
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// <para>Draws a toggle button instead of the usual checkbox, only works with boolean fields.</para>
    /// Extra properties which can be set directly:
    /// <code>offText, customLabel</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeToggleButtonAttribute : PropertyAttribute
    {
        /// <summary>
        /// Custom label. Used only if the <code>showLabel</code> parameter was set to TRUE
        /// </summary>
        public string customLabel;
        /// <summary>Optional text shown when the toggle is OFF</summary>
        public string offText;

        internal string text;
        internal DePosition position;
        internal bool showLabel = false;
        internal Color? bgOffColor, bgOnColor, labelOffColor, labelOnColor;

        /// <summary>
        /// Draws a toggle button instead of the usual checkbox (only works with booleans)
        /// </summary>
        /// <param name="text">Button label</param>
        /// <param name="showLabel">If TRUE also shows the property's label</param>
        /// <param name="bgOffColor">Background color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="bgOnColor">Background color (hex no #) when ON (leave NULL or empty to use default color)</param>
        /// <param name="labelOffColor">Lable color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="labelOnColor">Label color (hex no #) when ON (leave NULL or empty to use default color)</param>
        public DeToggleButtonAttribute(string text, bool showLabel, string bgOffColor = null, string bgOnColor = null, string labelOffColor = null, string labelOnColor = null)
        {
            this.text = text;
            this.position = showLabel ? DePosition.HDefault : DePosition.HExtended;
            this.showLabel = showLabel;
            if (!string.IsNullOrEmpty(bgOffColor)) this.bgOffColor = DeColorPalette.HexToColor(bgOffColor);
            if (!string.IsNullOrEmpty(bgOnColor)) this.bgOnColor = DeColorPalette.HexToColor(bgOnColor);
            if (!string.IsNullOrEmpty(labelOffColor)) this.labelOffColor = DeColorPalette.HexToColor(labelOffColor);
            if (!string.IsNullOrEmpty(labelOnColor)) this.labelOnColor = DeColorPalette.HexToColor(labelOnColor);
        }
        /// <summary>
        /// Draws a toggle button instead of the usual checkbox (only works with booleans)
        /// </summary>
        /// <param name="text">Button label</param>
        /// <param name="position"><see cref="DePosition"/> of the button relative to Inspector width</param>
        /// <param name="bgOffColor">Background color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="bgOnColor">Background color (hex no #) when ON (leave NULL or empty to use default color)</param>
        /// <param name="labelOffColor">Lable color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="labelOnColor">Label color (hex no #) when ON (leave NULL or empty to use default color)</param>
        public DeToggleButtonAttribute(string text, DePosition position = DePosition.HExtended, string bgOffColor = null, string bgOnColor = null, string labelOffColor = null, string labelOnColor = null)
        {
            this.text = text;
            this.position = position;
            if (!string.IsNullOrEmpty(bgOffColor)) this.bgOffColor = DeColorPalette.HexToColor(bgOffColor);
            if (!string.IsNullOrEmpty(bgOnColor)) this.bgOnColor = DeColorPalette.HexToColor(bgOnColor);
            if (!string.IsNullOrEmpty(labelOffColor)) this.labelOffColor = DeColorPalette.HexToColor(labelOffColor);
            if (!string.IsNullOrEmpty(labelOnColor)) this.labelOnColor = DeColorPalette.HexToColor(labelOnColor);
        }
        /// <summary>
        /// Draws a toggle button instead of the usual checkbox (only works with booleans)
        /// </summary>
        /// <param name="position"><see cref="DePosition"/> of the button relative to Inspector width</param>
        /// <param name="bgOffColor">Background color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="bgOnColor">Background color (hex no #) when ON (leave NULL or empty to use default color)</param>
        /// <param name="labelOffColor">Lable color (hex no #) when OFF (leave NULL or empty to use default color)</param>
        /// <param name="labelOnColor">Label color (hex no #) when ON (leave NULL or empty to use default color)</param>
        public DeToggleButtonAttribute(DePosition position = DePosition.HExtended, string bgOffColor = null, string bgOnColor = null, string labelOffColor = null, string labelOnColor = null)
        {
            this.position = position;
            if (!string.IsNullOrEmpty(bgOffColor)) this.bgOffColor = DeColorPalette.HexToColor(bgOffColor);
            if (!string.IsNullOrEmpty(bgOnColor)) this.bgOnColor = DeColorPalette.HexToColor(bgOnColor);
            if (!string.IsNullOrEmpty(labelOffColor)) this.labelOffColor = DeColorPalette.HexToColor(labelOffColor);
            if (!string.IsNullOrEmpty(labelOnColor)) this.labelOnColor = DeColorPalette.HexToColor(labelOnColor);
        }
    }
}