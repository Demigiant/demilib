// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/17 14:26
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeDividerAttribute))]
    public class DeDividerPropertyDrawer : DecoratorDrawer
    {
        static GUIStyle _attributeStyle;

        public override float GetHeight()
        {
            DeDividerAttribute attr = (DeDividerAttribute)attribute;
            return attr.height + attr.marginTop + attr.marginBottom;
        }

        public override void OnGUI(Rect position)
        {
            DeDividerAttribute attr = (DeDividerAttribute)attribute;
            SetStyles();

            Rect pos = new Rect(position.x, position.y + attr.marginTop, position.width, attr.height);

            Color defBgColor = GUI.backgroundColor;
            GUI.backgroundColor = DeEditorUtils.HexToColor(attr.hexColor);
            GUI.Label(pos, "", _attributeStyle);
            GUI.backgroundColor = defBgColor;
        }

        static void SetStyles()
        {
            if (_attributeStyle != null) return;

            _attributeStyle = new GUIStyle(GUI.skin.label).Margin(0).Padding(0);
            _attributeStyle.Background(Texture2D.whiteTexture);
        }
    }
}