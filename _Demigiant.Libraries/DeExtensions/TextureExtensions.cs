// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/02/02 13:54
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class TextureExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns the full Rect of this texture, with options for position and scale
        /// </summary>
        public static Rect GetRect(this Texture2D texture, float x = 0, float y = 0, float scale = 1)
        {
            return new Rect(x, y, texture.width * scale, texture.height * scale);
        }

        #endregion
    }
}