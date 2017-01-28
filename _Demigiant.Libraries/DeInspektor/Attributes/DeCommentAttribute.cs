// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:41
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Decorator</code><para/>
    /// Shows a comment in the Inspector
    /// Extra properties which can be set directly:<para/>
    /// - fontSize
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeCommentAttribute : PropertyAttribute
    {
        /// <summary>Font size (default = 9)</summary>
        public int fontSize = 9;
        /// <summary>Margin bottom</summary>
        public int marginBottom = 3;

        internal string text;
        internal string textColor, bgColor;
        internal DeCondition condition;
        internal ConditionalBehaviour behaviour;

        /// <summary>
        /// Shows a comment.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="marginBottom">Bottom margin</param>
        public DeCommentAttribute(string text, int marginBottom = 3)
        {
            this.text = text;
            this.marginBottom = marginBottom;
        }

        /// <summary>
        /// Shows a comment.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="textColor">Text color</param>
        /// <param name="bgColor">Background color</param>
        /// <param name="marginBottom">Bottom margin</param>
        public DeCommentAttribute(string text, string textColor, string bgColor, int marginBottom = 3)
        {
            this.text = text;
            this.textColor = textColor;
            this.bgColor = bgColor;
            this.marginBottom = marginBottom;
        }

        /// <summary>
        /// Shows a comment if the given condition is met.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (boolean)</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeCommentAttribute(string text, string propertyToCompare, bool value, ConditionalBehaviour behaviour = ConditionalBehaviour.Hide)
        {
            this.text = text;
            this.condition = new DeCondition(propertyToCompare, value);
            this.behaviour = behaviour;
        }
    }
}