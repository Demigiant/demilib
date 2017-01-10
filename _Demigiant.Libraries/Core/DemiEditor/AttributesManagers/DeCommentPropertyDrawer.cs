// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:43
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeCommentAttribute))]
    public class DeCommentPropertyDrawer : DecoratorDrawer
    {
        float _positionW;

        bool _stylesSet;
        GUIStyle _attributeStyle;

        public override float GetHeight()
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;

            SetStyles(attr);
            float w = _positionW <= 0 ? EditorGUIUtility.currentViewWidth : _positionW;
            return _attributeStyle.CalcHeight(new GUIContent(attr.text), w) + attr.marginBottom;
        }

        public override void OnGUI(Rect position)
        {
            _positionW = position.width;
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            SetStyles(attr);

            Rect r = position;
            r.height -= attr.marginBottom;

            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeEditorUtils.HexToColor(attr.bgColor);
            GUI.Box(r, attr.text, _attributeStyle);
            GUI.backgroundColor = defBgColor;
        }

        void SetStyles(DeCommentAttribute attr)
        {
            if (_stylesSet) return;

            _stylesSet = true;

            _attributeStyle = new GUIStyle(GUI.skin.box).Add(TextAnchor.MiddleLeft, 9).Padding(4, 3, 2, 3);
            if (attr.textColor != null) {
                _attributeStyle.Add(DeEditorUtils.HexToColor(attr.textColor));
                _attributeStyle.Background(Texture2D.whiteTexture);
            } else _attributeStyle.Add(new DeSkinColor(0.35f, 0.55f));
        }
    }
}