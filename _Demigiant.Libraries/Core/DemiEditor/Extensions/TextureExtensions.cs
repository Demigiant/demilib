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
        /// <summary>
        /// Checks that the texture uses the correct import settings, and applies them if they're incorrect.
        /// </summary>
        public static void SetGUIFormat(this Texture2D texture, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            if (texture.wrapMode == wrapMode && texture.filterMode == filterMode && texture.width <= maxTextureSize) return;

            Debug.Log("REIMPORT");
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            bool reimportRequired = tImporter.textureType != TextureImporterType.GUI
                || tImporter.npotScale != TextureImporterNPOTScale.None
                || tImporter.filterMode != filterMode
                || tImporter.wrapMode != wrapMode
                || tImporter.maxTextureSize != maxTextureSize
                || tImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor;
            if (!reimportRequired) return;

            tImporter.textureType = TextureImporterType.GUI;
            tImporter.npotScale = TextureImporterNPOTScale.None;
            tImporter.filterMode = filterMode;
            tImporter.wrapMode = wrapMode;
            tImporter.maxTextureSize = maxTextureSize;
            tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            AssetDatabase.ImportAsset(path);
        }
    }
}