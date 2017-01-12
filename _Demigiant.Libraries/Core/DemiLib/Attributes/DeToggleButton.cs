// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 20:22
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// Draws a toggle button instead of the usual checkbox, only works with boolean fields.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeToggleButton : PropertyAttribute
    {
        internal string text;
        internal DePosition position;

        /// <summary>
        /// Draws a toggle button instead of the usual checkbox (only works with booleans)
        /// </summary>
        /// <param name="text">Button label</param>
        /// <param name="position"><see cref="DePosition"/> of the button relative to Inspector width</param>
        public DeToggleButton(string text, DePosition position = DePosition.HDefault)
        {
            this.text = text;
            this.position = position;
        }
        /// <summary>
        /// Draws a toggle button instead of the usual checkbox (only works with booleans)
        /// </summary>
        /// <param name="position"><see cref="DePosition"/> of the button relative to Inspector width</param>
        public DeToggleButton(DePosition position = DePosition.HDefault)
        {
            this.position = position;
        }
    }
}