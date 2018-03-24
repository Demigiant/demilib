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
        const int _ExtendedWidthIncrement = 17;
        float _positionW;

        GUIStyle _attrStyle;

        public override float GetHeight()
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            bool isVisible = attr.behaviour == ConditionalBehaviour.Disable || attr.condition.IsTrue(DeInspektor.I.serializedObject);
            if (!isVisible) return 0;

            SetStyles(attr);
            float w = _positionW < 2
                ? EditorGUIUtility.currentViewWidth - (attr.style == DeCommentStyle.TextInValueArea ? EditorGUIUtility.labelWidth - 2 : 0)
                : _positionW;
            if (attr.style == DeCommentStyle.BoxExtended) w += _ExtendedWidthIncrement;
            float h = _attrStyle.CalcHeight(new GUIContent(attr.text), w) + attr.marginBottom + 4;
            return h;
        }

        public override void OnGUI(Rect position)
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;
            bool isTrue = attr.condition.IsTrue(DeInspektor.I.serializedObject);
            if (!isTrue && attr.behaviour == ConditionalBehaviour.Hide) return;


            DeGUI.BeginGUI();
            float shiftX = attr.style == DeCommentStyle.TextInValueArea ? EditorGUIUtility.labelWidth - 2 : 0;

            if (Event.current.type == EventType.Repaint) _positionW = position.width - shiftX;
            SetStyles(attr);

            Rect r = position;
            r.x += shiftX;
            r.y += 3;
            r.width = _positionW - 1;
            if (attr.style == DeCommentStyle.BoxExtended) {
                r.x -= _ExtendedWidthIncrement - 4;
                r.width += _ExtendedWidthIncrement;
            }
            r.height -= attr.marginBottom + 3;
            if (attr.style == DeCommentStyle.WrapNextLine) {
                r.x -= 1;
                r.width += 2;
                r.height += attr.marginBottom + EditorGUIUtility.singleLineHeight - 1;
            }

            bool wasGUIEnabled = GUI.enabled;
            GUI.enabled = isTrue && wasGUIEnabled;
            Color defBgColor = GUI.backgroundColor;
            if (attr.bgColor != null) GUI.backgroundColor = DeColorPalette.HexToColor(attr.bgColor);
            else if (attr.style == DeCommentStyle.BoxExtended) GUI.backgroundColor = new Color(0, 0, 0, 0.2f);
            GUI.Box(r, attr.text, _attrStyle);
            GUI.backgroundColor = defBgColor;
            GUI.enabled = wasGUIEnabled;
        }

        void SetStyles(DeCommentAttribute attr)
        {
            if (_attrStyle != null) return;

            _attrStyle = new GUIStyle(DeGUI.styles.box.sticky).Add(attr.textAnchor, attr.fontSize, Format.RichText).Padding(3, 3, 2, 1)
                .Margin(0);
            if (attr.textColor != null) _attrStyle.Add(DeColorPalette.HexToColor(attr.textColor));
            else _attrStyle.Add(new DeSkinColor(0.35f, 0.58f));
            switch (attr.style) {
            case DeCommentStyle.TextOnly:
            case DeCommentStyle.TextInValueArea:
                _attrStyle.Background(null).Padding(2, 0, 0, 0);
                break;
            case DeCommentStyle.Box:
                if (attr.bgColor != null) _attrStyle.Background(DeStylePalette.squareBorder);
                break;
            case DeCommentStyle.BoxExtended:
                _attrStyle.Background(Texture2D.whiteTexture);
                break;
            }
        }
    }
}