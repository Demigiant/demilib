// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/15 18:37
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeUtils
{
    /// <summary>
    /// Utils for structs (since they can't directly work with extension methods)
    /// </summary>
    public static class DeRectUtils
    {
        #region Rect

        /// <summary>
        /// Modifies and returns the given Rect, with its values shifted according the the given parameters
        /// </summary>
        public static Rect Shift(Rect rect, float x, float y, float width, float height)
        {
            return new Rect(rect.x + x, rect.y + y, rect.width + width, rect.height + height);
        }

        /// <summary>
        /// Includes one rect into another, and returns the resulting a
        /// </summary>
        public static Rect Include(Rect a, Rect b)
        {
            if (b.xMin < a.xMin) a.xMin = b.xMin;
            if (b.xMax > a.xMax) a.xMax = b.xMax;
            if (b.yMin < a.yMin) a.yMin = b.yMin;
            if (b.yMax > a.yMax) a.yMax = b.yMax;
            return a;
        }

        #endregion
    }
}