// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/23 13:08
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeEmptyAlertAttribute))]
    public class DeEmptyAlertPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Color currBgColor = GUI.backgroundColor;
            Color currContentColor = GUI.contentColor;
            bool alert = property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null
                         || property.propertyType == SerializedPropertyType.String && string.IsNullOrEmpty(property.stringValue);
            if (alert) {
                GUI.backgroundColor = Color.red;
                GUI.contentColor = DeGUI.colors.content.critical;
            }
            EditorGUI.PropertyField(position, property, label);
            GUI.backgroundColor = currBgColor;
            GUI.contentColor = currContentColor;
        }
    }
}