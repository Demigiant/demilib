// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/12 19:39
// License Copyright (c) Daniele Giardini

using System;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Class attribute</code> (MonoBehaviour only)<para/>
    /// Shows a description for the given MonoBehaviour before any other field.<para/>
    /// Extra properties which can be set directly:<para/>
    /// - mode
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DeComponentDescriptionAttribute : Attribute
    {
        internal string text;

        /// <summary>
        /// Shows a description for this MonoBehaviour before any other field.
        /// </summary>
        /// <param name="text">Text</param>
        public DeComponentDescriptionAttribute(string text)
        {
            this.text = text;
        }
    }
}