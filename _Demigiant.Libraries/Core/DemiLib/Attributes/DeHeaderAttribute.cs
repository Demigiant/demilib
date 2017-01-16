// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 13:29
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Decorator</code>
    /// <para>Draws a header in the inspector.</para>
    /// Extra properties which can be set directly:
    /// <code>marginTop</code>, <code>marginBottom</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeHeaderAttribute : PropertyAttribute
    {
        /// <summary>Top margin (default = 6)</summary>
        public int marginTop = 6;
        /// <summary>Bottom margin (default = 3)</summary>
        public int marginBottom = 3;

        internal string text;
        internal string textColor, bgColor;
        internal TextAnchor textAnchor = TextAnchor.MiddleLeft;
        internal FontStyle fontStyle;
        internal int fontSize;

        /// <summary>
        /// Draws a header with a color and background color
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="textColor">Color hex (no #) for text, leave NULL to use default</param>
        /// <param name="bgColor">Color hex (no #) for bg, leave NULL to use no bg color</param>
        /// <param name="fontStyle">Font style</param>
        /// <param name="fontSize">Font size</param>
        public DeHeaderAttribute(string text, string textColor = null, string bgColor = null, FontStyle fontStyle = FontStyle.Bold, int fontSize = 11)
        {
            this.text = text;
            this.textColor = textColor;
            this.bgColor = bgColor;
            this.fontStyle = fontStyle;
            this.fontSize = fontSize;
        }
        /// <summary>
        /// Draws a header with a color and background color
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="textAnchor">TextAnchor</param>
        /// <param name="textColor">Color hex (no #) for text, leave NULL to use default</param>
        /// <param name="bgColor">Color hex (no #) for bg, leave NULL to use no bg color</param>
        /// <param name="fontStyle">Font style</param>
        /// <param name="fontSize">Font size</param>
        public DeHeaderAttribute(string text, TextAnchor textAnchor, string textColor = null, string bgColor = null, FontStyle fontStyle = FontStyle.Bold, int fontSize = 11)
        {
            this.text = text;
            this.textColor = textColor;
            this.bgColor = bgColor;
            this.textAnchor = textAnchor;
            this.fontStyle = fontStyle;
            this.fontSize = fontSize;
        }
    }
}