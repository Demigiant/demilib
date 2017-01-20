// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:13
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// Begins a conditional group that will be disabled if the given condition is not met.
    /// Must always be closed by a <see cref="DeEndDisabledAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeBeginDisabledAttribute : PropertyAttribute
    {
        internal DeCondition condition;

        /// <summary>
        /// Begins a conditional group that wil be disabled if the given condition is TRUE. Must always be closed by a <see cref="DeEndDisabledAttribute"/>
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (boolean)</param>
        public DeBeginDisabledAttribute(string propertyToCompare, bool value = false)
        {
            this.condition = new DeCondition(propertyToCompare, value);
        }
        /// <summary>
        /// Begins a conditional group that wil be disabled if the given condition is TRUE. Must always be closed by a <see cref="DeEndDisabledAttribute"/>
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (string)</param>
        /// <param name="conditionType">Condition type</param>
        public DeBeginDisabledAttribute(string propertyToCompare, string value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
        /// <summary>
        /// Begins a conditional group that wil be disabled if the given condition is TRUE. Must always be closed by a <see cref="DeEndDisabledAttribute"/>
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeBeginDisabledAttribute(string propertyToCompare, float value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
        /// <summary>
        /// Begins a conditional group that wil be disabled if the given condition is TRUE. Must always be closed by a <see cref="DeEndDisabledAttribute"/>
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeBeginDisabledAttribute(string propertyToCompare, int value, Condition conditionType = Condition.Is)
        {
            this.condition = new DeCondition(propertyToCompare, value, conditionType);
        }
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    /// <summary>
    /// Closes a disabled group
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeEndDisabledAttribute : PropertyAttribute {}
}