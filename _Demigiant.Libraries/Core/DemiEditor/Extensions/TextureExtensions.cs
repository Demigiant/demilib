// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/05/01 01:46

using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Texture extensions
    /// </summary>
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

        /// <summary>
        /// Checks that the texture uses the correct import settings, and applies them if they're incorrect.
        /// </summary>
        public static void SetGUIFormat(this Texture2D texture, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32, TextureWrapMode wrapMode = TextureWrapMode.Clamp, int quality = 100)
        {
//            if (texture.wrapMode == wrapMode && texture.filterMode == filterMode && texture.width <= maxTextureSize) return;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            bool reimportRequired = tImporter.textureType != TextureImporterType.GUI
                || tImporter.npotScale != TextureImporterNPOTScale.None
                || tImporter.filterMode != filterMode
                || tImporter.wrapMode != wrapMode
                || tImporter.maxTextureSize != maxTextureSize
                || tImporter.textureFormat != 0 && tImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor
                || tImporter.compressionQuality != quality;
            if (!reimportRequired) return;

            tImporter.textureType = TextureImporterType.GUI;
            tImporter.npotScale = TextureImporterNPOTScale.None;
            tImporter.filterMode = filterMode;
            tImporter.wrapMode = wrapMode;
            tImporter.maxTextureSize = maxTextureSize;
            tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            tImporter.compressionQuality = quality;
            AssetDatabase.ImportAsset(path);
        }

        #endregion
    }
}