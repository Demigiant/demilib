// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 13:36
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeHeaderAttribute))]
    public class DeHeaderPropertyDrawer : DecoratorDrawer
    {
        GUIStyle _attributeStyle;

        public override float GetHeight()
        {
            DeHeaderAttribute attr = (DeHeaderAttribute)attribute;
            return attr.fontSize + 5 + attr.marginTop + attr.marginBottom;
        }

        public override void OnGUI(Rect position)
        {
            DeHeaderAttribute attr = (DeHeaderAttribute)attribute;
            SetStyles(attr);

            Rect headerPos = position;
            headerPos.height = attr.fontSize + 5;
            headerPos.y += attr.marginTop;

            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeEditorUtils.HexToColor(attr.bgColor);
            GUI.Label(headerPos, attr.text, _attributeStyle);
            GUI.backgroundColor = defBgColor;
        }

        void SetStyles(DeHeaderAttribute attr)
        {
            if (_attributeStyle != null) return;

            _attributeStyle = new GUIStyle(GUI.skin.label).Add(attr.textAnchor, attr.fontStyle, attr.fontSize).PaddingRight(4);
            if (attr.textColor != null) _attributeStyle.Add(DeEditorUtils.HexToColor(attr.textColor));
            if (attr.bgColor != null) _attributeStyle.Background(Texture2D.whiteTexture);
        }
    }
}