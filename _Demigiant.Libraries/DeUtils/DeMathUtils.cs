// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/25 20:26
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeUtils
{
    public static class DeMathUtils
    {
        #region Public Methods

        /// <summary>
        /// Clamps an angle between the given values.
        /// </summary>
        /// Taken from wydoidoit's code on Unity Answers http://answers.unity3d.com/questions/659932/how-do-i-clamp-my-rotation.html
        public static float ClampAngle(float angle, float min, float max)
        {
            angle = Mathf.Repeat(angle, 360);
            min = Mathf.Repeat(min, 360);
            max = Mathf.Repeat(max, 360);
            bool inverse = false;
            float tmin = min;
            float tangle = angle;
            if (min > 180) {
                inverse = !inverse;
                tmin -= 180;
            }
            if (angle > 180) {
                inverse = !inverse;
                tangle -= 180;
            }
            bool result = !inverse ? tangle > tmin : tangle < tmin;
            if (!result) angle = min;

            inverse = false;
            tangle = angle;
            float tmax = max;
            if (angle > 180) {
                inverse = !inverse;
                tangle -= 180;
            }
            if (max > 180) {
                inverse = !inverse;
                tmax -= 180;
            }

            result = !inverse ? tangle < tmax : tangle > tmax;
            if (!result) angle = max;
            return angle;
        }

        /// <summary>
        /// Finds the eventual intersection points between a line and a circle, and returns the total number of intersections
        /// </summary>
        /// Uses code from Rod Stephens: http://csharphelper.com/blog/2014/09/determine-where-a-line-intersects-a-circle-in-c/
        public static int FindLineCircleIntersection(Vector2 circleCenter, float radius, Vector2 lineStartP, Vector2 lineEndP, out Vector2 intersection0, out Vector2 intersection1)
        {
            float dx = lineEndP.x - lineStartP.x;
            float dy = lineEndP.y - lineStartP.y;

            float a = dx * dx + dy * dy;
            float b = 2 * (dx * (lineStartP.x - circleCenter.x) + dy * (lineStartP.y - circleCenter.y));
            float c = (lineStartP.x - circleCenter.x) * (lineStartP.x - circleCenter.x) +
                      (lineStartP.y - circleCenter.y) * (lineStartP.y - circleCenter.y) -
                      radius * radius;

            float det = b * b - 4 * a * c;
            if (a <= 0.0000001f || det < 0) {
                // No real solutions.
                intersection0 = new Vector2(float.NaN, float.NaN);
                intersection1 = new Vector2(float.NaN, float.NaN);
                return 0;
            } else if (det > 0) {
                // Two solutions.
                float t = (-b + Mathf.Sqrt(det)) / (2 * a);
                intersection0 = new Vector2(lineStartP.x + t * dx, lineStartP.y + t * dy);
                t = (-b - Mathf.Sqrt(det)) / (2 * a);
                intersection1 = new Vector2(lineStartP.x + t * dx, lineStartP.y + t * dy);
                return 2;
            } else {
                // One solution.
                float t = -b / (2 * a);
                intersection0 = new Vector2(lineStartP.x + t * dx, lineStartP.y + t * dy);
                intersection1 = new Vector2(float.NaN, float.NaN);
                return 1;
            }
        }

        /// <summary>
        /// Finds the closest intersection point between a line and a circle, and returns TRUE if it's found, FALSE otherwise
        /// </summary>
        public static bool FindNearestLineCircleIntersection(Vector2 circleCenter, float radius, Vector2 lineStartP, Vector2 lineEndP, out Vector2 nearestIntersection)
        {
            Vector2 p0, p1;
            int res = FindLineCircleIntersection(circleCenter, radius, lineStartP, lineEndP, out p0, out p1);
            if (res == 0) {
                nearestIntersection = new Vector2(float.NaN, float.NaN);
                return false;
            } else {
                if (Mathf.Abs(p0.x - lineStartP.x) < Mathf.Abs(p1.x - lineStartP.x) || Mathf.Abs(p0.y - lineStartP.y) < Mathf.Abs(p1.y - lineStartP.y)) {
                    nearestIntersection = p0;
                } else nearestIntersection = p1;
                return true;
            }
        }

        /// <summary>
        /// Finds the clockwise vertices of a rectangle built around the given line with the given width, taking line rotation into account
        /// </summary>
        /// Uses code from Kromster: https://stackoverflow.com/questions/7854043/drawing-rectangle-between-two-points-with-arbitrary-width
        public static void LineToRectangle(Vector2 lineP0, Vector2 lineP1, float width, out Vector2 rp0, out Vector2 rp1, out Vector2 rp2, out Vector2 rp3)
        {
            float widthHalf = width * 0.5f;
            Vector2 v = new Vector2(lineP1.x - lineP0.x, lineP1.y - lineP0.y);
            Vector2 p = new Vector2(v.y, -v.x).normalized; // normalized perpendicular
            Vector2 pMult = new Vector2(p.x * widthHalf, p.y * widthHalf);
            rp0 = new Vector2(lineP0.x + pMult.x, lineP0.y + pMult.y);
            rp1 = new Vector2(lineP1.x + pMult.x, lineP1.y + pMult.y);
            rp2 = new Vector2(lineP1.x - pMult.x, lineP1.y - pMult.y);
            rp3 = new Vector2(lineP0.x - pMult.x, lineP0.y - pMult.y);
        }

        #endregion
    }
}