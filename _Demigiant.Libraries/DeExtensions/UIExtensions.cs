// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/27 18:11
// License Copyright (c) Daniele Giardini

using UnityEngine;
using UnityEngine.UI;

namespace DG.DeExtensions
{
    public static class UIExtensions
    {
        #region Image

        public static void SetAlpha(this Image t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        public static void SetColor(this Image t, Color color, float alpha)
        {
            color.a = alpha;
            t.color = color;
        }

        #endregion

        #region RawImage

        public static void SetAlpha(this RawImage t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        public static void SetColor(this RawImage t, Color color, float alpha)
        {
            color.a = alpha;
            t.color = color;
        }

        #endregion

        #region Graphic

        public static void SetAlpha(this Graphic t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        public static void SetColor(this Graphic t, Color color, float alpha)
        {
            color.a = alpha;
            t.color = color;
        }

        #endregion

        #region RectTransform

        public static void SetAnchoredPosX(this RectTransform t, float value)
        {
            Vector3 v = t.anchoredPosition;
            v.x = value;
            t.anchoredPosition = v;
        }
        public static void SetAnchoredPosY(this RectTransform t, float value)
        {
            Vector3 v = t.anchoredPosition;
            v.y = value;
            t.anchoredPosition = v;
        }

        /// <summary>
        /// Sets the given RectTransform to stretch to the exact borders of its parent
        /// </summary>
        public static void StretchToFill(this RectTransform rt)
        {
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.offsetMax = new Vector2(0, 0);
            rt.offsetMin = new Vector2(0, 0);
        }

        #endregion

        #region Text

        public static void SetAlpha(this Text t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        public static void SetColor(this Text t, Color color, float alpha)
        {
            color.a = alpha;
            t.color = color;
        }

        #endregion
    }
}