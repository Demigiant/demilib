// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 18:30
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// <para>Draws the label with the given color and background</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeColoredLabelAttribute : PropertyAttribute
    {
        internal string textColor, bgColor;

        /// <summary>
        /// Colors the prefix label for the following property
        /// </summary>
        /// <param name="textColor">Prefix label color</param>
        /// <param name="bgColor">Prefix label bg color</param>
        public DeColoredLabelAttribute(string textColor, string bgColor = null)
        {
            this.textColor = textColor;
            this.bgColor = bgColor;
        }
    }
}