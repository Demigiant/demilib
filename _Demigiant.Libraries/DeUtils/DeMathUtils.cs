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
        /// Finds the point on a segment that is closest to the given p coordinates
        /// </summary>
        // Uses code from https://stackoverflow.com/questions/3120357/get-closest-point-to-a-line
        public static Vector2 FindClosestPointOnSegment(Vector2 p, Vector2 a, Vector2 b)
        {
            Vector2 ap = p - a; // Vector from A to P
            Vector2 ab = b - a; // Vector from A to B

            float magnitudeAB = ab.sqrMagnitude; // Magnitude of AB vector (it's length squared)
            float product_AB_AP = Vector2.Dot(ap, ab); // The DOT product of a_to_p and a_to_b
            float distance = product_AB_AP / magnitudeAB; // The normalized "distance" from a to your closest point

            if (distance < 0) return a;
            if (distance > 1)  return b;
            return a + ab * distance;
        }

        // Uses code from https://github.com/setchi/Unity-LineSegmentsIntersection/blob/master/Assets/LineSegmentIntersection/Scripts/Math2d.cs
        /// <summary>
        /// Finds the eventual point where two segments intersect.
        /// Returns FALSE if there is no intersection happening
        /// </summary>
        public static bool FindSegmentToSegmentIntersection(Vector2 a0, Vector2 a1, Vector2 b0, Vector2 b1, out Vector2 intersection)
        {
            intersection = Vector2.zero;

            float d = (a1.x - a0.x) * (b1.y - b0.y) - (a1.y - a0.y) * (b1.x - b0.x);
            if (d == 0.0f) return false;

            float u = ((b0.x - a0.x) * (b1.y - b0.y) - (b0.y - a0.y) * (b1.x - b0.x)) / d;
            float v = ((b0.x - a0.x) * (a1.y - a0.y) - (b0.y - a0.y) * (a1.x - a0.x)) / d;
            if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f) return false;

            intersection.x = a0.x + u * (a1.x - a0.x);
            intersection.y = a0.y + u * (a1.y - a0.y);
            return true;
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
        public static void LineToRectangle(Vector2 fromLineP, Vector2 toLineP, float width, out Vector2 fromP0, out Vector2 fromP1, out Vector2 toP2, out Vector2 toP3)
        {
            float widthHalf = width * 0.5f;
            Vector2 v = new Vector2(toLineP.x - fromLineP.x, toLineP.y - fromLineP.y);
            Vector2 p = new Vector2(v.y, -v.x).normalized; // normalized perpendicular
            Vector2 pMult = new Vector2(p.x * widthHalf, p.y * widthHalf);
            if (fromLineP.y < toLineP.y) pMult = new Vector2(-pMult.x, -pMult.y);
            fromP0 = new Vector2(fromLineP.x + pMult.x, fromLineP.y + pMult.y);
            fromP1 = new Vector2(fromLineP.x - pMult.x, fromLineP.y - pMult.y);
            toP2 = new Vector2(toLineP.x - pMult.x, toLineP.y - pMult.y);
            toP3 = new Vector2(toLineP.x + pMult.x, toLineP.y + pMult.y);
        }

        #endregion
    }
}