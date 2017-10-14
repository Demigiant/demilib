// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/13 17:42
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.PropertyDrawers
{
    [CustomPropertyDrawer(typeof(Range))]
    [CustomPropertyDrawer(typeof(IntRange))]
    public class RangePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            property.Next(true);
            SerializedProperty min = property.Copy();
            property.Next(true);
            SerializedProperty max = property.Copy();

            bool isIntRange = min.type == "int";
            float floatVal = 0;
            int intVal = 0;

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
            if (isIntRange) intVal = EditorGUI.IntField(positionMin, subLabel, min.intValue);
            else floatVal = EditorGUI.FloatField(positionMin, subLabel, min.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (isIntRange) {
                    if (intVal > max.intValue) intVal = max.intValue;
                    min.intValue = intVal;
                } else {
                    if (floatVal > max.floatValue) floatVal = max.floatValue;
                    min.floatValue = floatVal;
                }
            }
            EditorGUI.EndProperty();
            //
            subLabel.text = "Max";
            EditorGUI.BeginProperty(positionMax, subLabel, max);
            EditorGUI.BeginChangeCheck();
            if (isIntRange) intVal = EditorGUI.IntField(positionMax, subLabel, max.intValue);
            else floatVal = EditorGUI.FloatField(positionMax, subLabel, max.floatValue);
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                if (isIntRange) {
                    if (intVal < max.intValue) intVal = min.intValue;
                    max.intValue = intVal;
                } else {
                    if (floatVal < max.floatValue) floatVal = min.floatValue;
                    max.floatValue = floatVal;
                }
            }
            EditorGUI.EndProperty();
            EditorGUIUtility.labelWidth = defLabelW;

            EditorGUI.EndProperty();
        }
    }
}