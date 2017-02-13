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
        SerializedProperty _min, _max;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_min == null) {
                property.Next(true);
                _min = property.Copy();
                property.Next(true);
                _max = property.Copy();
            }

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            Rect positionMin = new Rect(position.x, position.y, position.width * 0.5f, position.height);
            Rect positionMax = new Rect(positionMin.xMax + 3, positionMin.y, positionMin.width - 3, positionMin.height);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            float defLabelW = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 28;
            GUIContent subLabel = new GUIContent("Min");
            EditorGUI.BeginProperty(positionMin, subLabel, _min);
            EditorGUI.BeginChangeCheck();
            float value = EditorGUI.FloatField(positionMin, subLabel, _min.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (value > _max.floatValue) value = _max.floatValue;
                _min.floatValue = value;
            }
            EditorGUI.EndProperty();
            //
            subLabel.text = "Max";
            EditorGUI.BeginProperty(positionMax, subLabel, _max);
            EditorGUI.BeginChangeCheck();
            value = EditorGUI.FloatField(positionMax, subLabel, _max.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (value < _max.floatValue) value = _min.floatValue;
                _max.floatValue = value;
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = defLabelW;

            EditorGUI.EndProperty();
        }
    }
}