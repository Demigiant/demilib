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
    [CustomPropertyDrawer(typeof(DeCommentAttribute), true)]
    public class DeCommentPropertyDrawer : DecoratorDrawer
    {
        float _positionW;

        GUIStyle _boxStyle, _textOnlyStyle;

        public override float GetHeight()
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            bool isVisible = attr.behaviour == ConditionalBehaviour.Disable || attr.condition.IsTrue(DeInspektor.I.serializedObject);
            if (!isVisible) return 0;

            SetStyles(attr);
            float w = _positionW < 2 ? EditorGUIUtility.currentViewWidth : _positionW;
            float h = _boxStyle.CalcHeight(new GUIContent(attr.text), w) + attr.marginBottom;
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
            GUI.Box(r, attr.text, attr.style == DeCommentStyle.Box ? _boxStyle : _textOnlyStyle);
            GUI.backgroundColor = defBgColor;
            GUI.enabled = wasGUIEnabled;
        }

        void SetStyles(DeCommentAttribute attr)
        {
            if (_boxStyle != null) return;

            _boxStyle = new GUIStyle(GUI.skin.box).Add(TextAnchor.MiddleLeft, attr.fontSize, Format.RichText).Padding(4, 4, 3, 3);
            if (attr.textColor != null) _boxStyle.Add(DeColorPalette.HexToColor(attr.textColor));
            else _boxStyle.Add(new DeSkinColor(0.35f, 0.58f));
            if (attr.bgColor != null) _boxStyle.Background(DeStylePalette.squareBorderCurved);
            _textOnlyStyle = _boxStyle.Clone().Background(null).Padding(2, 0, 0, 0);
        }
    }
}