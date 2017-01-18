// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 20:26
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeToggleButtonAttribute))]
    public class DeToggleButtonPropertyDrawer : PropertyDrawer
    {
        const int _LineH = 16;
        static GUIStyle _prefabOverrideStyle;
        static GUIStyle _labelOverrideStyle;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean) return base.GetPropertyHeight(property, label);

            DeToggleButtonAttribute attr = (DeToggleButtonAttribute)attribute;
            switch (attr.position) {
            case DePosition.HHalfRight:
            case DePosition.HThirdMiddle:
            case DePosition.HThirdRight:
                return -2f;
            default:
                return _LineH;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Boolean) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            DeGUI.BeginGUI();
            SetStyles();

            DeToggleButtonAttribute attr = (DeToggleButtonAttribute)attribute;

            Rect r = AttributesManagersUtils.AdaptRectToDePosition(false, position, attr.position, _LineH);

            EditorGUI.BeginProperty(position, label, property);
            // Eventual label
            if (attr.showLabel) {
                GUI.Label(
                    position,
                    string.IsNullOrEmpty(attr.customLabel) ? label : new GUIContent(attr.customLabel, label.tooltip),
                    property.prefabOverride ? _labelOverrideStyle : GUI.skin.label
                );
            }
            // Toggle button
            bool requiresCustomOffText = !property.boolValue && !string.IsNullOrEmpty(attr.offText);
            bool requiresCustomText = !requiresCustomOffText && !string.IsNullOrEmpty(attr.text);
            if (requiresCustomText || requiresCustomOffText) {
                label.text = requiresCustomText ? attr.text : attr.offText;
            }
            EditorGUI.BeginChangeCheck();
            Color prevColor = GUI.color;
            if (property.hasMultipleDifferentValues) GUI.color = new Color(0.4930259f, 0.625f, 0.4503677f, 1f);
            GUIStyle style = property.prefabOverride ? _prefabOverrideStyle : DeGUI.styles.button.bBlankBorder;
            bool toggled = DeGUI.ToggleButton(r, property.boolValue, label,
                attr.bgOffColor ?? DeGUI.colors.bg.toggleOff,
                attr.bgOnColor ?? DeGUI.colors.bg.toggleOn,
                attr.labelOffColor ?? DeGUI.colors.content.toggleOff,
                attr.labelOnColor ?? DeGUI.colors.content.toggleOn,
                style
            );
            GUI.color = prevColor;
            if (EditorGUI.EndChangeCheck()) {
                GUI.changed = true;
                property.boolValue = toggled;
            }
            EditorGUI.EndProperty();
        }

        static void SetStyles()
        {
            if (_prefabOverrideStyle != null) return;

            _prefabOverrideStyle = DeGUI.styles.button.bBlankBorder.Clone(FontStyle.Bold);
            _labelOverrideStyle = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold);
        }
    }
}