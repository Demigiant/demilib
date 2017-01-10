// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 13:29
// License Copyright (c) Daniele Giardini

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// Decorator
    /// </summary>
    public class DeHeaderAttribute : PropertyAttribute
    {
        public string text;
        public string textColor, bgColor;
        public TextAnchor textAnchor = TextAnchor.MiddleLeft;
        public FontStyle fontStyle;
        public int fontSize;

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