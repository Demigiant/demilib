// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 18:30
// License Copyright (c) Daniele Giardini

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// Property attribute
    /// </summary>
    public class DeColoredLabelAttribute : PropertyAttribute
    {
        public string textColor, bgColor;

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