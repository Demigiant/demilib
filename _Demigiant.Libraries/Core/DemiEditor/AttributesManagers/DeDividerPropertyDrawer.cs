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
        bool _stylesSet;
        GUIStyle _attributeStyle;

        DeDividerAttribute _attr { get { return (DeDividerAttribute)attribute; } }

        public override float GetHeight()
        {
            return _attr.height + _attr.marginTop + _attr.marginBottom;
        }

        public override void OnGUI(Rect position)
        {
            SetStyles(_attr);

            Rect pos = new Rect(position.x, position.y + _attr.marginTop, position.width, _attr.height);

            Color defBgColor = GUI.backgroundColor;
            GUI.backgroundColor = DeEditorUtils.HexToColor(_attr.hexColor);
            GUI.Label(pos, "", _attributeStyle);
            GUI.backgroundColor = defBgColor;
        }

        void SetStyles(DeDividerAttribute attr)
        {
            if (_stylesSet) return;

            _stylesSet = true;

            _attributeStyle = new GUIStyle(GUI.skin.label).Margin(0).Padding(0);
            _attributeStyle.Background(Texture2D.whiteTexture);
        }
    }
}