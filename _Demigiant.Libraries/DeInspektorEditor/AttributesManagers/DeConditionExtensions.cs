// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 18:21
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    public static class DeConditionExtensions
    {
        #region Public Methods

        public static bool IsTrue(this DeCondition condition, SerializedObject serializedObj)
        {
            SerializedProperty targetProp = serializedObj.FindProperty(condition.propertyToCompare);
            if (targetProp == null) return true;

            switch (targetProp.propertyType) {
            case SerializedPropertyType.Boolean:
                return targetProp.boolValue == condition.boolValue;
            case SerializedPropertyType.String:
                switch (condition.conditionType) {
                case Condition.IsNot:
                    return targetProp.stringValue != condition.stringValue;
                case Condition.IsNotNullOrEmpty:
                    return !string.IsNullOrEmpty(targetProp.stringValue);
                case Condition.IsNullOrEmpty:
                    return string.IsNullOrEmpty(targetProp.stringValue);
                default:
                    return targetProp.stringValue == condition.stringValue;
                }
            case SerializedPropertyType.Float:
            case SerializedPropertyType.Integer:
                float targetVal = targetProp.propertyType == SerializedPropertyType.Float ? targetProp.floatValue : targetProp.intValue;
                switch (condition.conditionType) {
                case Condition.IsNot:
                    return !Mathf.Approximately(targetVal, condition.numValue);
                case Condition.GreaterThan:
                    return targetVal > condition.numValue;
                case Condition.GreaterOrEqual:
                    return targetVal >= condition.numValue;
                case Condition.LessThan:
                    return targetVal < condition.numValue;
                case Condition.LessOrEqual:
                    return targetVal <= condition.numValue;
                default:
                    return Mathf.Approximately(targetVal, condition.numValue);
                }
            case SerializedPropertyType.ObjectReference:
                switch (condition.conditionType) {
                case Condition.IsNotNullOrEmpty:
                    return targetProp.objectReferenceValue != null;
                default:
                    return targetProp.objectReferenceValue == null;
                }
            case SerializedPropertyType.Enum:
                switch (condition.conditionType) {
                case Condition.Is:
                    return targetProp.enumValueIndex == (int)condition.numValue;
                default:
                    return targetProp.enumValueIndex != (int)condition.numValue;
                }
            }

            return true;
        }

        #endregion
    }
}