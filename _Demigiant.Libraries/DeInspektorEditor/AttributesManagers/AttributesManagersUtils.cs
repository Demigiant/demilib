// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 20:28
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    public static class AttributesManagersUtils
    {
        

        #region Public Methods

        public static Rect AdaptRectToDePosition(bool isDecorator, Rect r, DePosition position, float lineH, float marginBottom = 0)
        {
            r.height = lineH;
            float labelW = EditorGUIUtility.labelWidth;
            switch (position) {
            case DePosition.HDefault:
                r.width -= labelW;
                r.x += labelW;
                break;
            case DePosition.HHalfLeft:
            case DePosition.HHalfRight:
                r.width = r.width * 0.5f;
                r.x += position == DePosition.HHalfLeft ? 0 : r.width;
                if (position != DePosition.HHalfLeft) {
                    r.y -= lineH + marginBottom;
                    if (!isDecorator) r.y -= 2;
                }
                break;
            case DePosition.HThirdLeft:
            case DePosition.HThirdMiddle:
            case DePosition.HThirdRight:
                r.width = r.width * 0.333f;
                r.x += position == DePosition.HThirdLeft
                    ? 0
                    : position == DePosition.HThirdMiddle
                    ? r.width
                    : r.width * 2;
                if (position != DePosition.HThirdLeft) {
                    r.y -= lineH + marginBottom;
                    if (!isDecorator) r.y -= 2;
                }
                break;
            }
            return r;
        }
        
        #endregion
    }
}