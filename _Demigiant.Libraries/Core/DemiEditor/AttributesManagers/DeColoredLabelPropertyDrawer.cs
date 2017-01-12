// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 18:31
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeColoredLabelAttribute))]
    public class DeColoredLabelPropertyDrawer : PropertyDrawer
    {
        GUIStyle _attributeStyle;

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label)
        {
            DeColoredLabelAttribute attr = (DeColoredLabelAttribute)attribute;
            SetStyles(attr);

            // Evaluate rects
            Rect labelPos = position;
            labelPos.width = EditorGUIUtility.labelWidth;
            labelPos.width -= 2;

            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeEditorUtils.HexToColor(attr.bgColor);
            GUI.Label(labelPos, label.text, _attributeStyle);
            GUI.backgroundColor = defBgColor;

            // Draw property as usual
            EditorGUI.PropertyField(position, property, new GUIContent(" ", label.tooltip));
	    }

        void SetStyles(DeColoredLabelAttribute attr)
        {
            if (_attributeStyle != null) return;

            _attributeStyle = EditorStyles.label.Clone(DeEditorUtils.HexToColor(attr.textColor)).Padding(2, 2, 1, 0);
            if (attr.bgColor != null) _attributeStyle.Background(Texture2D.whiteTexture);
        }
    }
}