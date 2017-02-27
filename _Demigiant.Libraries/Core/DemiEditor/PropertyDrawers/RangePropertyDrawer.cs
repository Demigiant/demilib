// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/13 17:42
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Range))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.Next(true);
            SerializedProperty min = property.Copy();
            property.Next(true);
            SerializedProperty max = property.Copy();

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            Rect positionMin = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            Rect positionMax = new Rect(positionMin.xMax + 3, positionMin.y, positionMin.width - 3, positionMin.height);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            float defLabelW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 28;
            GUIContent subLabel = new GUIContent("Min");
            EditorGUI.BeginProperty(positionMin, subLabel, min);
            EditorGUI.BeginChangeCheck();
            float value = EditorGUI.FloatField(positionMin, subLabel, min.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (value > max.floatValue) value = max.floatValue;
                min.floatValue = value;
            }
            EditorGUI.EndProperty();
            //
            subLabel.text = "Max";
            EditorGUI.BeginProperty(positionMax, subLabel, max);
            EditorGUI.BeginChangeCheck();
            value = EditorGUI.FloatField(positionMax, subLabel, max.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (value < max.floatValue) value = min.floatValue;
                max.floatValue = value;
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = defLabelW;

            EditorGUI.EndProperty();
        }
    }
}