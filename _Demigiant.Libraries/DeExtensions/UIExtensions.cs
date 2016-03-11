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

        #endregion

        #region RawImage

        public static void SetAlpha(this RawImage t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
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

        #endregion

        #region Text

        public static void SetAlpha(this Text t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        #endregion
    }
}