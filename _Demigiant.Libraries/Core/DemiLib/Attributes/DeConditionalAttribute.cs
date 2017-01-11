// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 13:59
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// <para>Shows/hides or enables/disables the property depending on the given conditions</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeConditionalAttribute : PropertyAttribute
    {
        internal DeCondition condition;
        internal ConditionalBehaviour behaviour;

        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (boolean)</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeConditionalAttribute(string propertyToCompare, bool value, ConditionalBehaviour behaviour = ConditionalBehaviour.Disable)
        {
            this.condition = new DeCondition(propertyToCompare, value);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (string)</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeConditionalAttribute(string propertyToCompare, string value, Condition conditionType = Condition.Is, ConditionalBehaviour behaviour = ConditionalBehaviour.Disable)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeConditionalAttribute(string propertyToCompare, float value, Condition conditionType = Condition.Is, ConditionalBehaviour behaviour = ConditionalBehaviour.Disable)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        /// <param name="behaviour">Behaviour in case condition is not met</param>
        public DeConditionalAttribute(string propertyToCompare, int value, Condition conditionType = Condition.Is, ConditionalBehaviour behaviour = ConditionalBehaviour.Disable)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
            this.behaviour = behaviour;
        }
    }
}