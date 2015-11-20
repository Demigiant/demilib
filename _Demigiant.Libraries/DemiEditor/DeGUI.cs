// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/24 18:27

using System;
using DG.DemiLib;
using DG.DemiLib.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// Global Demigiant GUI manager
    /// </summary>
    public static class DeGUI
    {
        /// <summary>
        /// Default color palette
        /// </summary>
        public static DeColorPalette colors;
        /// <summary>
        /// Default style palette
        /// </summary>
        public static DeStylePalette styles;
        static DeColorPalette _defaultColorPalette; // Default color palette if none selected
        static DeStylePalette _defaultStylePalette; // Default style palette if none selected

        /// <summary>
        /// TRUE if we're using the PRO skin
        /// </summary>
        public static readonly bool IsProSkin;

        static DeGUI()
        {
            GUIUtils.isProSkin = IsProSkin = EditorGUIUtility.isProSkin;
        }

        #region Public GUI Methods

        /// <summary>
        /// Call this at the beginning of GUI methods
        /// </summary>
        /// <param name="guiColorPalette"></param>
        public static void BeginGUI(DeColorPalette guiColorPalette = null, DeStylePalette guiStylePalette = null)
        {
            ChangePalette(guiColorPalette, guiStylePalette);
        }

        /// <summary>
        /// Changes the active palettes to the given ones
        /// (or resets them to the default ones if NULL)
        /// </summary>
        public static void ChangePalette(DeColorPalette newColorPalette, DeStylePalette newStylePalette)
        {
            if (newColorPalette != null) colors = newColorPalette;
            else {
                if (_defaultColorPalette == null) _defaultColorPalette = new DeColorPalette();
                colors = _defaultColorPalette;
            }

            if (newStylePalette != null) styles = newStylePalette;
            else {
                if (_defaultStylePalette == null) _defaultStylePalette = new DeStylePalette();
                styles = _defaultStylePalette;
            }

            styles.Init();
        }

        #endregion

        #region Public GUI Draw Methods

        #region ObjectFields

        /// <summary>Scene field</summary>
        public static Object SceneField(Rect rect, string label, Object obj)
        {
            // Verify that obj is a SceneAsset (not recognized by compiler as a class, so we have to use the string representation)
            if (obj != null && !obj.ToString().EndsWith(".SceneAsset)")) obj = null;
            // Draw
            return EditorGUI.ObjectField(rect, label, obj, typeof(Object), false);
        }

        #endregion

        #endregion
    }
}