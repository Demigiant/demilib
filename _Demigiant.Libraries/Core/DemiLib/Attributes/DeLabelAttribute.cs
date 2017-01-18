// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/18 17:36
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// <para>Writes custom text as the property label</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeLabelAttribute : PropertyAttribute
    {
        internal string customText;

        /// <summary>
        /// Writes custem text as label, instead of using the regular nicified property name
        /// </summary>
        /// <param name="customText">Label</param>
        public DeLabelAttribute(string customText)
        {
            this.customText = customText;
        }
    }
}