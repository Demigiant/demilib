// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 14:18
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeConditionalAttribute))]
    public class DeConditionalPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;
            return attr.behaviour == ConditionalBehaviour.Disable || attr.condition.IsTrue(property.serializedObject) ? base.GetPropertyHeight(property, label) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;
            bool isTrue = attr.condition.IsTrue(property.serializedObject);
            if (!isTrue && attr.behaviour == ConditionalBehaviour.Hide) return;

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = isTrue && wasGUIEnabled;
            if (attr.customLabel != null) label.text = attr.customLabel;
            EditorGUI.PropertyField(position, property, label);
            GUI.enabled = wasGUIEnabled;
        }
    }
}