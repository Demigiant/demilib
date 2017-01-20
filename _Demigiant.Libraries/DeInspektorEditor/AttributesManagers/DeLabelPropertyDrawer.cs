// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/18 17:38
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeLabelAttribute))]
    public class DeLabelPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeLabelAttribute attr = (DeLabelAttribute)attribute;

            EditorGUI.PropertyField(position, property, new GUIContent(attr.customText, label.tooltip));
        }
    }
}