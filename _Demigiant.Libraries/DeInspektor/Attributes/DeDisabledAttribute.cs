// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/16 17:28
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// Disables a field if the given condition is not met.
    /// Works like DeConditional but:
    /// <para>- Is a Decorator, so it can be combined with other attributes</para>- Only allows to disable a field, not to hide it
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeDisabledAttribute : PropertyAttribute
    {
        internal DeCondition condition;

        /// <summary>
        /// Disables the field if the given condition is TRUE.
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (boolean)</param>
        public DeDisabledAttribute(string propertyToCompare, bool value)
        {
            this.condition = new DeCondition(propertyToCompare, value);
        }
        /// <summary>
        /// Disables the field if the given condition is TRUE.
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (string)</param>
        /// <param name="conditionType">Condition type</param>
        public DeDisabledAttribute(string propertyToCompare, string value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
        /// <summary>
        /// Disables the field if the given condition is TRUE.
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeDisabledAttribute(string propertyToCompare, float value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
        /// <summary>
        /// Disables the field if the given condition is TRUE.
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeDisabledAttribute(string propertyToCompare, int value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
        /// <summary>
        /// Disables the field if the given condition is TRUE.
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="conditionType">Condition type</param>
        public DeDisabledAttribute(string propertyToCompare, Condition conditionType = Condition.IsNullOrEmpty)
        {
            this.condition = new DeCondition(propertyToCompare, conditionType);
        }
    }
}