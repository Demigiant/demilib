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
    /// - fontSize<para/>
    /// Normal properties which can be set directly:<para/>
    /// - textColor<para/>
    /// - bgColor<para/>
    /// - marginBottom
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeCommentAttribute : PropertyAttribute
    {
        /// <summary>Font size (default = 9)</summary>
        public int fontSize = 9;
        /// <summary>Text color</summary>
        public string textColor;
        /// <summary>Background color</summary>
        public string bgColor;
        /// <summary>Margin bottom</summary>
        public int marginBottom = 3;

        internal string text;
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

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ CONDITIONALS

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
        /// <summary>
        /// Shows a comment if the given condition is met.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (string)</param>
        /// <param name="conditionType">Condition type (required parameter in this case, to distinguish condition overload from main color settings)</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeCommentAttribute(string text, string propertyToCompare, string value, Condition conditionType, ConditionalBehaviour behaviour = ConditionalBehaviour.Hide)
        {
            this.text = text;
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows a comment if the given condition is met.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeCommentAttribute(string text, string propertyToCompare, float value, Condition conditionType = Condition.Is, ConditionalBehaviour behaviour = ConditionalBehaviour.Hide)
        {
            this.text = text;
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows a comment if the given condition is met.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (int)</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeCommentAttribute(string text, string propertyToCompare, int value, Condition conditionType = Condition.Is, ConditionalBehaviour behaviour = ConditionalBehaviour.Hide)
        {
            this.text = text;
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows a comment if the given condition is met.
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeCommentAttribute(string text, string propertyToCompare, Condition conditionType = Condition.IsNotNullOrEmpty, ConditionalBehaviour behaviour = ConditionalBehaviour.Hide)
        {
            this.text = text;
            this.condition = new DeCondition(propertyToCompare, conditionType);
            this.behaviour = behaviour;
        }
    }
}