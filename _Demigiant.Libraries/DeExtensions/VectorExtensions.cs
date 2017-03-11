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
        /// Returns a copy of the vector with its values rounded to integers, using Mathf.Round.
        /// </summary>
        public static Vector2 Round(this Vector2 value)
        {
            return new Vector2(Mathf.Round(value.x), Mathf.Round(value.y));
        }

        /// <summary>
        /// Returns a copy of the vector with its values rounded to integers, using a fast int cast.
        /// </summary>
        public static Vector2 Floor(this Vector2 value)
        {
            return new Vector2((int)value.x, (int)value.y);
        }

        #endregion
    }
}