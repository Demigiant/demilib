// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/25 21:50
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeUtils
{
    public static class DeVector3Utils
    {
        /// <summary>
        /// Returns the Y angle in degrees of the given vector (0 to 180, negative if on left, positive if on right), based on world coordinates.
        /// </summary>
        public static float AngleY(Vector3 v)
        {
            return AngleY(Vector3.forward, v);
        }

        /// <summary>
        /// Returns the Y angle in degrees between from and to (0 to 180, negative if to is on left, positive if to is on right).
        /// </summary>
        public static float AngleY(Vector3 from, Vector3 to)
        {
            Vector2 a = new Vector2(from.x, from.z).normalized;
            Vector2 b = new Vector2(to.x, to.z).normalized;
            float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(a, b), -1f, 1f)) * 57.29578f;
            float cross = b.x * a.y - b.y * a.x;
            if (cross < 0) angle = -angle;
            return angle;
        }
    }
}