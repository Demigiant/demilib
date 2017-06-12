// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 14:17
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeExtensions
{
    public static class VectorExtensions
    {
        #region Vector2

        /// <summary>
        /// Returns a copy of the vector with its X set to the given value.
        /// </summary>
        public static Vector2 SetX(this Vector2 v, float value)
        {
            v.x = value;
            return v;
        }
        /// <summary>
        /// Returns a copy of the vector with its Y set to the given value.
        /// </summary>
        public static Vector2 SetY(this Vector2 v, float value)
        {
            v.y = value;
            return v;
        }

        /// <summary>
        /// Returns a copy of the vector with its values rounded to integers, using Mathf.Round.
        /// </summary>
        public static Vector2 Round(this Vector2 v)
        {
            return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
        }

        /// <summary>
        /// Returns a copy of the vector with its values rounded to integers, using a fast int cast.
        /// </summary>
        public static Vector2 Floor(this Vector2 v)
        {
            return new Vector2((int)v.x, (int)v.y);
        }

        #endregion

        #region Vector3

        /// <summary>
        /// Returns a copy of the vector with its X set to the given value.
        /// </summary>
        public static Vector3 SetX(this Vector3 v, float value)
        {
            v.x = value;
            return v;
        }
        /// <summary>
        /// Returns a copy of the vector with its Y set to the given value.
        /// </summary>
        public static Vector3 SetY(this Vector3 v, float value)
        {
            v.y = value;
            return v;
        }
        /// <summary>
        /// Returns a copy of the vector with its Z set to the given value.
        /// </summary>
        public static Vector3 SetZ(this Vector3 v, float value)
        {
            v.z = value;
            return v;
        }

        #endregion
    }
}