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
        public static void SetFormat(this Texture2D texture, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32)
        {
            if (texture.wrapMode == TextureWrapMode.Clamp) return;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter tImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            tImporter.textureType = TextureImporterType.GUI;
            tImporter.npotScale = TextureImporterNPOTScale.None;
            tImporter.filterMode = filterMode;
            tImporter.wrapMode = TextureWrapMode.Clamp;
            tImporter.maxTextureSize = maxTextureSize;
            tImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
            AssetDatabase.ImportAsset(path);
        }
    }
}