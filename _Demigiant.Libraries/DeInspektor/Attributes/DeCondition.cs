// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 18:06
// License Copyright (c) Daniele Giardini

#pragma warning disable 1591
namespace DG.DeInspektor.Attributes
{
    public enum Condition
    {
        Is,
        IsNot,
        IsNullOrEmpty,
        IsNotNullOrEmpty,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual
    }

    public enum ConditionalBehaviour
    {
        Hide,
        Disable
    }

    public struct DeCondition
    {
        internal string propertyToCompare;
        internal bool boolValue;
        internal string stringValue;
        internal float numValue;
        internal Condition conditionType;

        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (boolean)</param>
        public DeCondition(string propertyToCompare, bool value) : this()
        {
            this.propertyToCompare = propertyToCompare;
            this.boolValue = value;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (string)</param>
        /// <param name="conditionType">Condition type</param>
        public DeCondition(string propertyToCompare, string value, Condition conditionType = Condition.Is) : this()
        {
            this.propertyToCompare = propertyToCompare;
            this.stringValue = value;
            this.conditionType = conditionType;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeCondition(string propertyToCompare, float value, Condition conditionType = Condition.Is) : this()
        {
            this.propertyToCompare = propertyToCompare;
            this.numValue = value;
            this.conditionType = conditionType;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="value">Property value to compare (float)</param>
        /// <param name="conditionType">Condition type</param>
        public DeCondition(string propertyToCompare, int value, Condition conditionType = Condition.Is) : this()
        {
            this.propertyToCompare = propertyToCompare;
            this.numValue = value;
            this.conditionType = conditionType;
        }
        /// <summary>
        /// Shows/enables the property only if the condition is met
        /// </summary>
        /// <param name="propertyToCompare">Name of the property to check for conditions</param>
        /// <param name="conditionType">Condition type</param>
        public DeCondition(string propertyToCompare, Condition conditionType = Condition.IsNotNullOrEmpty) : this()
        {
            this.propertyToCompare = propertyToCompare;
            this.conditionType = conditionType;
        }
    }
}