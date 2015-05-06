// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/24 18:27

using DG.DemiLib;
using DG.DemiLib.Core;
using UnityEditor;

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
        public static readonly bool isProSkin;

        static DeGUI()
        {
            GUIUtils.isProSkin = isProSkin = EditorGUIUtility.isProSkin;
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
    }
}