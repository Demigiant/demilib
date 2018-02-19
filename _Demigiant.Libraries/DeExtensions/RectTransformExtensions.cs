// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/02/18 18:05
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class RectTransformExtensions
    {
        #region Public Methods

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
    }
}