// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/23 12:41
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Replicates DeExtensions.RectExtensions for internal usage
    /// </summary>
    public static class RectExtensions
    {
        #region Public Methods

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
        /// Returns a copy or the Rect expanded around its center by the given amount
        /// </summary>
        /// <param name="amountX">Indicates how much to expand the rect on each horizontal side</param>
        /// <param name="amountY">Indicates how much to expand the rect on each vertical side</param>
        public static Rect Expand(this Rect r, float amountX, float amountY)
        {
            return r.Shift(-amountX, -amountY, amountX * 2, amountY * 2);
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
        /// Returns a copy or the Rect contracted around its center by the given amount
        /// </summary>
        /// <param name="amountX">Indicates how much to contract the rect on each horizontal side</param>
        /// <param name="amountY">Indicates how much to contract the rect on each vertical side</param>
        public static Rect Contract(this Rect r, float amountX, float amountY)
        {
            return r.Shift(amountX, amountY, -amountX * 2, -amountY * 2);
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
        /// Returns TRUE if this rect intersects the given one, and also outputs the intersection area<para/>
        /// </summary>
        /// <param name="intersection">Intersection area</param>
        public static bool Intersects(this Rect a, Rect b, out Rect intersection)
        {
            intersection = new Rect();
            if (!a.Overlaps(b)) return false;

            float minX = a.x < b.x ? b.x : a.x;
            float maxX = a.xMax > b.xMax ? b.xMax : a.xMax;
            float minY = a.y < b.y ? b.y : a.y;
            float maxY = a.yMax > b.yMax ? b.yMax : a.yMax;
            intersection = new Rect(minX, minY, maxX - minX, maxY - minY);
            // Fix for Unity floating point imprecision where Overlaps returns TRUE in some cases when rects are simply adjacent
            const float epsilon = 0.001f;
            if (intersection.width < epsilon || intersection.height < epsilon) {
                intersection = new Rect();
                return false;
            }
            return true;
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
        /// Sets this rect to the left of the given x position, with options for margin and width resize
        /// </summary>
        /// <param name="margin">Distance between this rect and the given x position</param>
        /// <param name="resizeWidthTo">If greater than zero resizes this rect to the given size</param>
        public static Rect HangToLeftOf(this Rect r, float x, float margin = 0, float resizeWidthTo = -1)
        {
            if (resizeWidthTo > 0) r = r.SetWidth(resizeWidthTo);
            r.x = x - r.width - margin;
            return r;
        }

        /// <summary>
        /// Sets this rect to the right of the given x position and resizes it so that its xMax remains the same.
        /// </summary>
        /// <param name="margin">Distance between this rect and the given x position</param>
        /// <param name="extraSizeOffset">Extra offset to add to the resulting width</param>
        public static Rect HangToRightAndResize(this Rect r, float x, float margin = 0, float extraSizeOffset = 0)
        {
            float offset = x - r.x;
            r.width = r.width - offset - margin + extraSizeOffset;
            r.x = x + margin;
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
        /// Returns a copy of the Rect with its X value shifted by the given value
        /// </summary>
        public static Rect ShiftX(this Rect r, float x)
        {
            return new Rect(r.x + x, r.y, r.width, r.height);
        }

        /// <summary>
        /// Returns a copy of the Rect with its Y value shifted by the given value
        /// </summary>
        public static Rect ShiftY(this Rect r, float y)
        {
            return new Rect(r.x, r.y + y, r.width, r.height);
        }

        /// <summary>
        /// Returns a copy of the Rect with its x shifted by the given value and its width shrinked/expanded accordingly
        /// (so that the xMax value will stay the same as before)
        /// </summary>
        public static Rect ShiftXAndResize(this Rect r, float x)
        {
            return new Rect(r.x + x, r.y, r.width - x, r.height);
        }

        /// <summary>
        /// Returns a copy of the Rect with its y shifted by the given value and its height shrinked/expanded accordingly
        /// (so that the yMax value will stay the same as before)
        /// </summary>
        public static Rect ShiftYAndResize(this Rect r, float y)
        {
            return new Rect(r.x, r.y + y, r.width, r.height - y);
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

        /// <summary>
        /// Returns a copy of the Rect with its X,Y properties set so the rect center corresponds to the given values
        /// </summary>
        public static Rect SetCenter(this Rect r, float x, float y)
        {
            r.x = x - r.width * 0.5f;
            r.y = y - r.height * 0.5f;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its X property set so the rect X center corresponds to the given value
        /// </summary>
        public static Rect SetCenterX(this Rect r, float value)
        {
            r.x = value - r.width * 0.5f;
            return r;
        }

        /// <summary>
        /// Returns a copy of the Rect with its Y property set so the rect Y center corresponds to the given value
        /// </summary>
        public static Rect SetCenterY(this Rect r, float value)
        {
            r.y = value - r.height * 0.5f;
            return r;
        }

        #endregion
    }
}