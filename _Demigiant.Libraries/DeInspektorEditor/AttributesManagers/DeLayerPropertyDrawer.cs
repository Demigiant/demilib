// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/02/26 18:25
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeLayerAttribute))]
    public class DeLayerPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.intValue = EditorGUI.LayerField(position, label, property.intValue);
        }
    }
}