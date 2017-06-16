// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/06/15 18:55
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code><para/>
    /// Shows a bigger preview image for the given sprite/texture.<para/>
    /// Extra properties which can be set directly:<para/>
    /// - emptyAlert<para/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeImagePreviewAttribute : PropertyAttribute
    {
        /// <summary>If TRUE marks this field if no image is present</summary>
        public bool emptyAlert;

        internal const float Height = 50;

        /// <summary>
        /// Shows a bigger preview image for the given sprite/texture.
        /// </summary>
        public DeImagePreviewAttribute() {}
    }
}