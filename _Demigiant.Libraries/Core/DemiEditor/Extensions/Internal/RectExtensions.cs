// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/23 12:41
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.Internal
{
    /// <summary>
    /// Replicates DeExtensions.RectExtensions for internal usage
    /// </summary>
    internal static class RectExtensions
    {
        /// <summary>
        /// Adds one rect into another, and returns the resulting a
        /// </summary>
        public static Rect Add(this Rect a, Rect b)
        {
            if (b.xMin < a.xMin) a.xMin = b.xMin;
            if (b.xMax > a.xMax) a.xMax = b.xMax;
            if (b.yMin < a.yMin) a.yMin = b.yMin;
            if (b.yMax > a.yMax) a.yMax = b.yMax;
            return a;
        }

        /// <summary>
        /// Returns a copy or the Rect expanded around its center by the given amount
        /// </summary>
        /// <param name="amount">Indicates how much to expand the rect on each size</param>
        public static Rect Expand(this Rect r, float amount)
        {
            float exSize = amount * 2;
            return r.Shift(-amount, -amount, exSize, exSize);
        }

        /// <summary>
        /// Returns a copy or the Rect contracted around its center by the given amount
        /// </summary>
        /// <param name="amount">Indicates how much to contract the rect on each size</param>
        public static Rect Contract(this Rect r, float amount)
        {
            float exSize = amount * 2;
            return r.Shift(amount, amount, -exSize, -exSize);
        }

        /// <summary>
        /// Returns a copy of the Rect resized so it fits proportionally within the given size limits
        /// </summary>
        /// <param name="w">Width to fit</param>
        /// <param name="h">Height to fit</param>
        /// <param name="shrinkOnly">If TRUE (default) only shrinks the rect if needed, if FALSE also enlarges it to fit</param>
        /// <returns></returns>
        public static Rect Fit(this Rect r, float w, float h, bool shrinkOnly = true)
        {
            if (shrinkOnly && r.width <= w && r.height <= h) return r;
            float wPerc = w / r.width;
            float hPerc = h / r.height;
            float perc = wPerc < hPerc ? wPerc : hPerc;
            r.width *= perc;
            r.height *= perc;
            return r;
        }

        /// <summary>
        /// Returns TRUE if the first rect includes the second one
        /// </summary>
        /// <param name="full">If TRUE, returns TRUE only if the second rect is fully included,
        /// otherwise just if some part of it is included</param>
        public static bool Includes(this Rect a, Rect b, bool full = true)
        {
            if (full) return a.xMin <= b.xMin && a.xMax > b.xMax && a.yMin <= b.yMin && a.yMax >= b.yMax;
            return b.xMax > a.xMin && b.xMin < a.xMax && b.yMax > a.yMin && b.yMin < a.yMax;
        }

        /// <summary>
        /// Returns a copy of the Rect with its X/Y coordinates set to 0
        /// </summary>
        public static Rect ResetXY(this Rect r)
        {
            r.x = r.y = 0;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its values shifted according the the given parameters
        /// </summary>
        public static Rect Shift(this Rect r, float x, float y, float width, float height)
        {
            return new Rect(r.x + x, r.y + y, r.width + width, r.height + height);
        }

        /// <summary>
        /// Returns a copy of the Rect with its X property set to the given value
        /// </summary>
        public static Rect SetX(this Rect r, float value)
        {
            r.x = value;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its Y property set to the given value
        /// </summary>
        public static Rect SetY(this Rect r, float value)
        {
            r.y = value;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its height property set to the given value
        /// </summary>
        public static Rect SetHeight(this Rect r, float value)
        {
            r.height = value;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its width property set to the given value
        /// </summary>
        public static Rect SetWidth(this Rect r, float value)
        {
            r.width = value;
            return r;
        }
    }
}