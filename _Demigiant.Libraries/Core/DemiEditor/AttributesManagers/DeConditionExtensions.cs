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
                switch (condition.stringConditionType) {
                case StringCondition.IsNot:
                    return targetProp.stringValue != condition.stringValue;
                default:
                    return targetProp.stringValue == condition.stringValue;
                }
            case DeCondition.ValueType.Float:
                if (targetProp.propertyType != SerializedPropertyType.Float) return true;
                switch (condition.floatConditionType) {
                case FloatCondition.IsNot:
                    return !Mathf.Approximately(targetProp.floatValue, condition.floatValue);
                case FloatCondition.GreaterThan:
                    return targetProp.floatValue > condition.floatValue;
                case FloatCondition.GreaterOrEqual:
                    return targetProp.floatValue >= condition.floatValue;
                case FloatCondition.LessThan:
                    return targetProp.floatValue < condition.floatValue;
                case FloatCondition.LessOrEqual:
                    return targetProp.floatValue <= condition.floatValue;
                default:
                    return Mathf.Approximately(targetProp.floatValue, condition.floatValue);
                }
            case DeCondition.ValueType.Int:
                if (targetProp.propertyType != SerializedPropertyType.Integer) return true;
                switch (condition.intConditionType) {
                case IntCondition.IsNot:
                    return targetProp.intValue != condition.intValue;
                case IntCondition.GreaterThan:
                    return targetProp.intValue > condition.intValue;
                case IntCondition.GreaterOrEqual:
                    return targetProp.intValue >= condition.intValue;
                case IntCondition.LessThan:
                    return targetProp.intValue < condition.intValue;
                case IntCondition.LessOrEqual:
                    return targetProp.intValue <= condition.intValue;
                default:
                    return targetProp.intValue == condition.intValue;
                }
            default: // Bool
                if (targetProp.propertyType != SerializedPropertyType.Boolean) return true;
                return targetProp.boolValue == condition.boolValue;
            }
        }

        #endregion
    }
}