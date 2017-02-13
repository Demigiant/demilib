// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/13 17:02
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeRangeAttribute))]
    public class DeRangePropertyDrawer : PropertyDrawer
    {
        SerializedProperty _min, _max; // Used in case of Range type

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DeGUI.BeginGUI();

            // Verify validity
            if (property.propertyType != SerializedPropertyType.Float && property.propertyType != SerializedPropertyType.Integer && property.type != "Range") {
                // Invalid
                position = EditorGUI.PrefixLabel(position, label);
                using (new DeGUI.ColorScope(null, DeGUI.colors.content.critical)) GUI.Label(position, "Invalid DeRange Attribute");
                return;
            }

            DeRangeAttribute attr = (DeRangeAttribute)attribute;

            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, new GUIContent(string.IsNullOrEmpty(attr.label) ? label.text : attr.label, label.tooltip));
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            switch (property.propertyType) {
            case SerializedPropertyType.Float:
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    float floatValue = EditorGUI.Slider(position, property.floatValue, attr.min, attr.max);
                    if (check.changed) {
                        GUI.changed = true;
                        property.floatValue = floatValue;
                    }
                }
                break;
            case SerializedPropertyType.Integer:
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    int intValue = EditorGUI.IntSlider(position, property.intValue, (int)attr.min, (int)attr.max);
                    if (check.changed) {
                        GUI.changed = true;
                        property.intValue = intValue;
                    }
                }
                break;
            default: // Range
                DrawRange(position, property, attr);
                break;
            }

            EditorGUI.EndProperty();
        }

        void DrawRange(Rect position, SerializedProperty property, DeRangeAttribute attr)
        {
            if (_min == null) {
                property.Next(true);
                _min = property.Copy();
                property.Next(true);
                _max = property.Copy();
            }

            const int minMaxSize = 40;
            Rect positionSlider = new Rect(position.x, position.y, position.width - minMaxSize * 2 - 2, position.height);
            Rect positionMin = new Rect(positionSlider.xMax + 2, positionSlider.y, minMaxSize - 1, positionSlider.height);
            Rect positionMax = new Rect(positionMin.xMax + 1, positionMin.y, minMaxSize - 1, positionMin.height);
            Color defColor = GUI.color;
            if (EditorGUI.showMixedValue) GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, 0.3f);
            using (var check = new EditorGUI.ChangeCheckScope()) {
                float min = _min.floatValue;
                float max = _max.floatValue;
                EditorGUI.MinMaxSlider(positionSlider, ref min, ref max, attr.min, attr.max);
                if (check.changed) {
                    _min.floatValue = min;
                    _max.floatValue = max;
                }
            }
            GUI.color = defColor;
            using (new EditorGUI.PropertyScope(positionMin, null, _min)) {
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    float value = EditorGUI.FloatField(positionMin, _min.floatValue);
                    if (check.changed) {
                        GUI.changed = true;
                        if (value > _max.floatValue) value = _max.floatValue;
                        else if (value < attr.min) value = attr.min;
                        _min.floatValue = value;
                    }
                }
            }
            using (new EditorGUI.PropertyScope(positionMax, null, _max)) {
                using (var check = new EditorGUI.ChangeCheckScope()) {
                    float value = EditorGUI.FloatField(positionMax, _max.floatValue);
                    if (check.changed) {
                        GUI.changed = true;
                        if (value < _min.floatValue) value = _min.floatValue;
                        else if (value > attr.max) value = attr.max;
                        _max.floatValue = value;
                    }
                }
            }
//            GUI.Label(position, string.Format("{0}-{1}", _min.floatValue, _max.floatValue), EditorStyles.centeredGreyMiniLabel);
        }
    }
}