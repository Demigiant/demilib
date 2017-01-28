// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:43
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeCommentAttribute))]
    public class DeCommentPropertyDrawer : DecoratorDrawer
    {
        float _positionW;

        GUIStyle _attributeStyle;

        public override float GetHeight()
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            bool isVisible = attr.behaviour == ConditionalBehaviour.Disable || attr.condition.IsTrue(DeInspektor.I.serializedObject);
            if (!isVisible) return 0;

            SetStyles(attr);
            float w = _positionW < 2 ? EditorGUIUtility.currentViewWidth : _positionW;
            float h = _attributeStyle.CalcHeight(new GUIContent(attr.text), w) + attr.marginBottom;
            return h;
        }

        public override void OnGUI(Rect position)
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            bool isTrue = attr.condition.IsTrue(DeInspektor.I.serializedObject);
            if (!isTrue && attr.behaviour == ConditionalBehaviour.Hide) return;

            DeGUI.BeginGUI();
            if (Event.current.type == EventType.Repaint) _positionW = position.width;
            SetStyles(attr);

            Rect r = position;
            r.height -= attr.marginBottom;

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = isTrue && wasGUIEnabled;
            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeColorPalette.HexToColor(attr.bgColor);
            GUI.Box(r, attr.text, _attributeStyle);
            GUI.backgroundColor = defBgColor;
            GUI.enabled = wasGUIEnabled;
        }

        void SetStyles(DeCommentAttribute attr)
        {
            if (_attributeStyle != null) return;

            _attributeStyle = new GUIStyle(GUI.skin.box).Add(TextAnchor.MiddleLeft, attr.fontSize, Format.RichText).Padding(4, 3, 2, 3);
            if (attr.textColor != null) {
                _attributeStyle.Add(DeColorPalette.HexToColor(attr.textColor));
                _attributeStyle.Background(Texture2D.whiteTexture);
            } else _attributeStyle.Add(new DeSkinColor(0.35f, 0.58f));
        }
    }
}