// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/18 12:44
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    /// <summary>
    /// Mainly used for structs extensions
    /// </summary>
    public static class MiscellaneousExtensions
    {
        #region Structs - Color

        /// <summary>
        /// Changes the alpha of this color and returns it
        /// </summary>
        public static Color SetAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }

        #endregion
    }
}