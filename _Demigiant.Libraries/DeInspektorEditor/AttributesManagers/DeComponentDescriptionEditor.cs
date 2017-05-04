// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/12 20:28
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    public class DeComponentDescriptionEditor
    {
        DeComponentDescriptionAttribute _attr;

        static GUIStyle _attrStyle;

        public DeComponentDescriptionEditor(UnityEngine.Object target)
        {
            DeComponentDescriptionAttribute[] attrs = target.GetType().GetCustomAttributes(typeof(DeComponentDescriptionAttribute), true) as DeComponentDescriptionAttribute[];
            if (attrs != null && attrs.Length > 0) _attr = attrs[0];
        }

        public void Draw()
        {
            if (_attr == null) return;

            SetStyles(_attr);

            GUILayout.Space(-1);
            using (new GUILayout.HorizontalScope()) {
                GUILayout.Space(-13);
                Rect r = GUILayoutUtility.GetRect(new GUIContent(_attr.text), _attrStyle);
                r.width += 4;
                using (new DeGUI.ColorScope(new DeSkinColor(0.65f, 0.16f))) GUI.Box(r, _attr.text, _attrStyle);
            }
        }

        void SetStyles(DeComponentDescriptionAttribute attr)
        {
            if (_attrStyle != null) return;

            _attrStyle = new GUIStyle(GUI.skin.box).Add(TextAnchor.MiddleLeft, attr.fontSize, Format.RichText, new DeSkinColor(0.2f, 0.58f))
                .Padding(4, 4 + 4, 3, 4).Margin(0, 0, 0, 2).Background(DeStylePalette.whiteSquare_fadeOut_bt).StretchWidth();
        }
    }
}