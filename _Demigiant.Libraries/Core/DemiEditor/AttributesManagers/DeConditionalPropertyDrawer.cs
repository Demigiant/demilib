// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 14:18
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeConditionalAttribute))]
    public class DeConditionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;
            return attr.behaviour == ConditionalBehaviour.Disable || IsTrue(property) ? base.GetPropertyHeight(property, label) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;
            bool isTrue = IsTrue(property);
            if (!isTrue && attr.behaviour == ConditionalBehaviour.Hide) return;

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = isTrue;
            EditorGUI.PrefixLabel (position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property, new GUIContent(" ", label.tooltip));
            GUI.enabled = wasGUIEnabled;
        }

        bool IsTrue(SerializedProperty property)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;

            SerializedProperty compP = property.serializedObject.FindProperty(attr.propertyToCompare);
            if (compP == null) return true;

            switch (attr.valueType) {
            case DeConditionalAttribute.ValueType.String:
                if (compP.propertyType != SerializedPropertyType.String) return true;
                switch (attr.stringConditionType) {
                case StringCondition.IsNot:
                    return compP.stringValue != attr.stringValue;
                default:
                    return compP.stringValue == attr.stringValue;
                }
            case DeConditionalAttribute.ValueType.Float:
                if (compP.propertyType != SerializedPropertyType.Float) return true;
                switch (attr.floatConditionType) {
                case FloatCondition.IsNot:
                    return !Mathf.Approximately(compP.floatValue, attr.floatValue);
                case FloatCondition.GreaterThan:
                    return compP.floatValue > attr.floatValue;
                case FloatCondition.GreaterOrEqual:
                    return compP.floatValue >= attr.floatValue;
                case FloatCondition.LessThan:
                    return compP.floatValue < attr.floatValue;
                case FloatCondition.LessOrEqual:
                    return compP.floatValue <= attr.floatValue;
                default:
                    return Mathf.Approximately(compP.floatValue, attr.floatValue);
                }
            case DeConditionalAttribute.ValueType.Int:
                if (compP.propertyType != SerializedPropertyType.Integer) return true;
                switch (attr.intConditionType) {
                case IntCondition.IsNot:
                    return compP.intValue != attr.intValue;
                case IntCondition.GreaterThan:
                    return compP.intValue > attr.intValue;
                case IntCondition.GreaterOrEqual:
                    return compP.intValue >= attr.intValue;
                case IntCondition.LessThan:
                    return compP.intValue < attr.intValue;
                case IntCondition.LessOrEqual:
                    return compP.intValue <= attr.intValue;
                default:
                    return compP.intValue == attr.intValue;
                }
            default: // Bool
                if (compP.propertyType != SerializedPropertyType.Boolean) return true;
                return compP.boolValue == attr.boolValue;
            }
        }
    }
}