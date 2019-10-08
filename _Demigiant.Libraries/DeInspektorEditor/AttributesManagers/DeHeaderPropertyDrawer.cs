// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/16 13:36
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeHeaderAttribute), true)]
    public class DeHeaderPropertyDrawer : DecoratorDrawer
    {
        const int _DividerH = 2;
        const int _DividerHSpace = 2;
        GUIStyle _attributeStyle, _dividerStyle;

        public override float GetHeight()
        {
            DeHeaderAttribute attr = (DeHeaderAttribute)attribute;
            float h = attr.fontSize + 5 + attr.marginTop + attr.marginBottom;
            switch (attr.mode) {
            case DeHeaderAttribute.Mode.TopDivider:
            case DeHeaderAttribute.Mode.BottomDivider:
                h += _DividerH + _DividerHSpace;
                break;
            case DeHeaderAttribute.Mode.Dividers:
                h += _DividerH * 2 + _DividerHSpace * 2;
                break;
            }
            return h;
        }

        public override void OnGUI(Rect position)
        {
            DeGUI.BeginGUI();
            DeHeaderAttribute attr = (DeHeaderAttribute)attribute;
            SetStyles(attr);

            bool hasTopDivider = attr.mode == DeHeaderAttribute.Mode.TopDivider || attr.mode == DeHeaderAttribute.Mode.Dividers;
            bool hasBottomDivider = attr.mode == DeHeaderAttribute.Mode.BottomDivider || attr.mode == DeHeaderAttribute.Mode.Dividers;
            Rect topDividerR = position;
            Rect bottomDividerR = position;
            Rect headerR = position;
            headerR.height = attr.fontSize + 5;
            if (attr.mode != DeHeaderAttribute.Mode.Default) {
                if (hasTopDivider) {
                    topDividerR.height = _DividerH;
                    topDividerR.y += attr.marginTop;
                    headerR.y = topDividerR.yMax + _DividerHSpace;
                } else headerR.y += attr.marginTop;
                if (hasBottomDivider) {
                    bottomDividerR.height = _DividerH;
                    bottomDividerR.y = headerR.yMax + _DividerHSpace;
                }
            } else headerR.y += attr.marginTop;

            Color defBgColor = GUI.backgroundColor;
            GUI.backgroundColor = attr.bgColor != null
                ? DeColorPalette.HexToColor(attr.bgColor)
                : (Color)new DeSkinColor(0.45f, 0.05f);
            if (hasTopDivider) GUI.Label(topDividerR, "", _dividerStyle);
            GUI.Label(headerR, attr.text, _attributeStyle);
            if (hasBottomDivider) GUI.Label(bottomDividerR, "", _dividerStyle);
            GUI.backgroundColor = defBgColor;
        }

        void SetStyles(DeHeaderAttribute attr)
        {
            if (_attributeStyle != null) return;

            _attributeStyle = new GUIStyle(GUI.skin.label).Add(attr.textAnchor, attr.fontStyle, attr.fontSize).PaddingRight(4);
            if (attr.textColor != null) _attributeStyle.Add(DeColorPalette.HexToColor(attr.textColor));
            if (attr.bgColor != null) _attributeStyle.Background(Texture2D.whiteTexture);

            _dividerStyle = new GUIStyle(GUI.skin.label).Margin(0).Padding(0);
            _dividerStyle.Background(Texture2D.whiteTexture);
        }
    }
}