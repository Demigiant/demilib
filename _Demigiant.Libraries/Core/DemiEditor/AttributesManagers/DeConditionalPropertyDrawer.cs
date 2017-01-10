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
            return attr.behaviour == ConditionalBehaviour.Disable || attr.condition.IsTrue(property) ? base.GetPropertyHeight(property, label) : 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeConditionalAttribute attr = (DeConditionalAttribute)attribute;
            bool isTrue = attr.condition.IsTrue(property);
            if (!isTrue && attr.behaviour == ConditionalBehaviour.Hide) return;

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = isTrue;
            EditorGUI.PrefixLabel (position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.PropertyField(position, property, new GUIContent(" ", label.tooltip));
            GUI.enabled = wasGUIEnabled;
        }
    }
}