// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/17 13:20
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Decorator</code>
    /// <para>Draws an image in the inspector.</para>
    /// Extra properties which can be set directly:
    /// <code>marginTop</code>, <code>marginBottom</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeImageAttribute : PropertyAttribute
    {
        /// <summary>Top margin (default = 2)</summary>
        public int marginTop = 2;
        /// <summary>Bottom margin (default = 2)</summary>
        public int marginBottom = 2;

        internal string adbFilePath; // Relative to Assets folder
        internal float maxHeight = -1; // If < 0 uses full height, otherwise scales to fit
        internal float maxWidth = -1; // If < 0 uses full width, otherwise scales to fit

        /// <summary>
        /// Draws an image in the inspector, and fits it to max width/height if set higher than 0
        /// </summary>
        /// <param name="filePath">Image filepath, relative to Assets folder.<para>(example: "Images/myImg.png")</para></param>
        /// <param name="maxWidth">Max width (if lower than 0 ignores it)</param>
        /// <param name="maxHeight">Max height (if lower than 0 ignores it)</param>
        public DeImageAttribute(string filePath, float maxWidth = -1, float maxHeight = -1)
        {
            this.adbFilePath = "Assets/" + filePath;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        }
    }
}