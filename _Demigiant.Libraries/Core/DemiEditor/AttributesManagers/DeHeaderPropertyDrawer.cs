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
        const int _HeaderH = 16;
        const int _MarginTop = 6;
        const int _MarginBottom = 3;
        bool _stylesSet;
        GUIStyle _attributeStyle;

        public override float GetHeight()
        {
            return _HeaderH + _MarginTop + _MarginBottom;
        }

        public override void OnGUI(Rect position)
        {
            DeHeaderAttribute attr = (DeHeaderAttribute)attribute;
            SetStyles(attr);

            Rect headerPos = position;
            headerPos.height = _HeaderH;
            headerPos.y += _MarginTop;

            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeEditorUtils.HexToColor(attr.bgColor);
            GUI.Label(headerPos, attr.text, _attributeStyle);
            GUI.backgroundColor = defBgColor;
        }

        void SetStyles(DeHeaderAttribute attr)
        {
            if (_stylesSet) return;

            _stylesSet = true;

            _attributeStyle = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold, attr.textAnchor);
            if (attr.textColor != null) _attributeStyle.Add(DeEditorUtils.HexToColor(attr.textColor));
            if (attr.bgColor != null) _attributeStyle.Background(Texture2D.whiteTexture);
        }
    }
}