// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 18:21
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    public static class DeConditionExtensions
    {
        #region Public Methods

        public static bool IsTrue(this DeCondition condition, SerializedProperty relatedProperty)
        {
            SerializedProperty targetProp = relatedProperty.serializedObject.FindProperty(condition.propertyToCompare);
            if (targetProp == null) return true;

            switch (condition.valueType) {
            case DeCondition.ValueType.String:
                if (targetProp.propertyType != SerializedPropertyType.String) return true;
                switch (condition.conditionType) {
                case Condition.IsNot:
                    return targetProp.stringValue != condition.stringValue;
                default:
                    return targetProp.stringValue == condition.stringValue;
                }
            case DeCondition.ValueType.Number:
                if (targetProp.propertyType != SerializedPropertyType.Float && targetProp.propertyType != SerializedPropertyType.Integer) return true;
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
            default: // Bool
                if (targetProp.propertyType != SerializedPropertyType.Boolean) return true;
                return targetProp.boolValue == condition.boolValue;
            }
        }

        #endregion
    }
}