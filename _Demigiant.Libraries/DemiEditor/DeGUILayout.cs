// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/27 11:09

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// GUILayout methods
    /// </summary>
    public static class DeGUILayout
    {
        #region Buttons

        /// <summary>Button that can be toggled on and off</summary>
        public static bool ColoredButton(Color shade, Color contentColor, string text, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, new GUIContent(text, ""), null, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ColoredButton(Color shade, Color contentColor, string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, new GUIContent(text, ""), guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ColoredButton(Color shade, Color contentColor, GUIContent content, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, content, null, options); }
        /// <summary>Button that can be toggled on and off</summary>
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ColoredButton(Color shade, Color contentColor, GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = shade;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.def;
            bool clicked = GUILayout.Button(content, guiStyle.Clone(contentColor), options);
            GUI.backgroundColor = prevBgColor;
            return clicked;
        }

        /// <summary>Toolbar foldout button</summary>
        public static bool ToolbarFoldoutButton(bool toggled, string text = null)
        {
            bool clicked = GUILayout.Button(
                text,
                string.IsNullOrEmpty(text)
                    ? toggled ? DeGUI.styles.button.toolFoldoutOpen : DeGUI.styles.button.toolFoldoutClosed
                    : toggled ? DeGUI.styles.button.toolFoldoutOpenWLabel : DeGUI.styles.button.toolFoldoutClosedWLabel
            );
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
        }

        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, string text, params GUILayoutOption[] options)
        { return ToggleButton(toggled, new GUIContent(text, ""), null, null, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return ToggleButton(toggled, new GUIContent(text, ""), null, guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, string text, DeColorPalette colorPalette, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        { return ToggleButton(toggled, new GUIContent(text, ""), colorPalette, guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, GUIContent content, params GUILayoutOption[] options)
        { return ToggleButton(toggled, content, null, null, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return ToggleButton(toggled, content, null, guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, GUIContent content, DeColorPalette colorPalette, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            DeColorPalette cp = colorPalette ?? DeGUI.colors;
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = toggled ? cp.bg.toggleOn : cp.bg.toggleOff;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.def;
            bool clicked = GUILayout.Button(
                content,
                guiStyle.Clone(toggled ? cp.content.toggleOn : cp.content.toggleOff),
                options
            );
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            GUI.backgroundColor = prevBgColor;
            return toggled;
        }

        #endregion

        #region Toolbars

        /// <summary>Begins an horizontal toolbar layout</summary>
        public static void BeginToolbar(params GUILayoutOption[] options)
        { BeginToolbar(Color.white, null, options); }
        /// <summary>Begins an horizontal toolbar layout</summary>
        public static void BeginToolbar(GUIStyle style, params GUILayoutOption[] options)
        { BeginToolbar(Color.white, style, options); }
        /// <summary>Begins an horizontal toolbar layout</summary>
        public static void BeginToolbar(Color backgroundShade, params GUILayoutOption[] options)
        { BeginToolbar(backgroundShade, null, options); }
        /// <summary>Begins an horizontal toolbar layout</summary>
        public static void BeginToolbar(Color backgroundShade, GUIStyle style, params GUILayoutOption[] options)
        {
            GUI.backgroundColor = backgroundShade;
            GUILayout.BeginHorizontal(style ?? DeGUI.styles.toolbar.def, options);
            GUI.backgroundColor = Color.white;
        }
        /// <summary>Ends an horizontal toolbar layout</summary>
        public static void EndToolbar()
        {
            GUILayout.EndHorizontal();
        }

        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, params GUILayoutOption[] options)
        { Toolbar(text, Color.white, null, null, options); }
        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, GUIStyle toolbarStyle, params GUILayoutOption[] options)
        { Toolbar(text, Color.white, toolbarStyle, null, options); }
        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, GUIStyle toolbarStyle, GUIStyle labelStyle, params GUILayoutOption[] options)
        { Toolbar(text, Color.white, toolbarStyle, labelStyle, options); }
        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, Color backgroundShade, params GUILayoutOption[] options)
        { Toolbar(text, backgroundShade, null, null, options); }
        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, Color backgroundShade, GUIStyle toolbarStyle, params GUILayoutOption[] options)
        { Toolbar(text, backgroundShade, toolbarStyle, null, options); }
        /// <summary>A toolbar with a label</summary>
        public static void Toolbar(string text, Color backgroundShade, GUIStyle toolbarStyle, GUIStyle labelStyle, params GUILayoutOption[] options)
        {
            BeginToolbar(backgroundShade, toolbarStyle, options);
            if (labelStyle == null) labelStyle = DeGUI.styles.label.toolbar;
            GUILayout.Label(text, labelStyle);
            EndToolbar();
        }

        #endregion

        #region Miscellaneous

        /// <summary>Vertical box layout with style and color options</summary>
        public static void BeginVBox(GUIStyle style)
        { BeginVBox(null, style); }
        /// <summary>Vertical box layout with style and color options</summary>
        public static void BeginVBox(Color? color = null, GUIStyle style = null)
        {
            Color applyColor = color == null ? Color.white : (Color)color;
            if (style == null) style = DeGUI.styles.box.def;
            Color orColor = GUI.backgroundColor;
            GUI.backgroundColor = applyColor;
            GUILayout.BeginVertical(style);
            GUI.backgroundColor = orColor;
        }
        /// <summary>End vertical box layout</summary>
        public static void EndVBox()
        {
            GUILayout.EndVertical();
        }

        /// <summary>Divider</summary>
        public static void HorizontalDivider(Color? color = null, int height = 1, int topMargin = 5, int bottomMargin = 5)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = color == null ? (Color)DeGUI.colors.bg.divider : (Color)color;
            GUILayout.Space(topMargin);
            GUILayout.BeginHorizontal(DeGUI.styles.misc.line, GUILayout.Height(height));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(bottomMargin);
            GUI.backgroundColor = prevBgColor;
        }

        #endregion
    }
}