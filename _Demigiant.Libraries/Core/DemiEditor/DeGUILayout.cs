// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/27 11:09

using System;
using System.Collections;
using System.Reflection;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// GUILayout methods
    /// </summary>
    public static class DeGUILayout
    {
        static int _activePressButtonId = -1;
        static MethodInfo _miGradientField;

        #region Buttons

        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Color shade, string text, params GUILayoutOption[] options)
        { return ShadedButton(shade, new GUIContent(text, ""), null, options); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Color shade, string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return ShadedButton(shade, new GUIContent(text, ""), guiStyle, options); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Color shade, GUIContent content, params GUILayoutOption[] options)
        { return ShadedButton(shade, content, null, options); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Color shade, GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = shade;
            bool clicked = guiStyle == null ? GUILayout.Button(content, options) : GUILayout.Button(content, guiStyle, options);
            GUI.backgroundColor = prevBgColor;
            return clicked;
        }

        /// <summary>Colored button</summary>
        public static bool ColoredButton(Color shade, Color contentColor, string text, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, new GUIContent(text, ""), null, options); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Color shade, Color contentColor, string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, new GUIContent(text, ""), guiStyle, options); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Color shade, Color contentColor, GUIContent content, params GUILayoutOption[] options)
        { return ColoredButton(shade, contentColor, content, null, options); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Color shade, Color contentColor, GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = shade;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.def;
            bool clicked = GUILayout.Button(content, guiStyle.Clone(contentColor), options);
            GUI.backgroundColor = prevBgColor;
            return clicked;
        }

        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(string text, GUIStyle guiStyle, params GUILayoutOption[] options)
        { return PressButton(new GUIContent(text, ""), guiStyle, options); }
        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(GUIContent content, GUIStyle guiStyle, params GUILayoutOption[] options)
        {
            // NOTE: tried using RepeatButton, but doesn't work if used for dragging
            if (GUI.enabled && Event.current.type == EventType.MouseUp && _activePressButtonId != -1) {
                _activePressButtonId = -1;
//                GUIUtility.hotControl = -1; // This breaks the undo system
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }
            GUILayout.Button(content, guiStyle, options);
//            int controlId = GUIUtility.GetControlID(FocusType.Native);
            int controlId = DeEditorGUIUtils.GetLastControlId(); // Changed from prev while working on DeInspektor
            int hotControl = GUIUtility.hotControl;
            bool pressed = GUI.enabled && hotControl > 1 && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition);
            if (pressed) {
//                if (_activePressButtonId != controlId) {
                if (_activePressButtonId == -1 && _activePressButtonId != controlId) { // Modified while working on DeInspektor
                    GUIUtility.hotControl = controlId; // Remove control from other elements (added while working on DeInspektor)
                    _activePressButtonId = controlId;
                    return true;
                }
            }
            if (!pressed && hotControl < 1) _activePressButtonId = -1;
            return false;
        }

        /// <summary>Toolbar foldout button</summary>
        public static bool ToolbarFoldoutButton(bool toggled, string text = null, bool isLarge = false, bool stretchedLabel = false, Color? forceLabelColor = null)
        {
            GUIStyle style;
            if (isLarge) {
                style = string.IsNullOrEmpty(text)
                    ? toggled ? DeGUI.styles.button.toolLFoldoutOpen : DeGUI.styles.button.toolLFoldoutClosed
                    : toggled
                        ? stretchedLabel ? DeGUI.styles.button.toolLFoldoutOpenWStretchedLabel : DeGUI.styles.button.toolLFoldoutOpenWLabel
                        : stretchedLabel ? DeGUI.styles.button.toolLFoldoutClosedWStretchedLabel : DeGUI.styles.button.toolLFoldoutClosedWLabel;
            } else {
                style = string.IsNullOrEmpty(text)
                    ? toggled ? DeGUI.styles.button.toolFoldoutOpen : DeGUI.styles.button.toolFoldoutClosed
                    : toggled
                        ? stretchedLabel ? DeGUI.styles.button.toolFoldoutOpenWStretchedLabel : DeGUI.styles.button.toolFoldoutOpenWLabel
                        : stretchedLabel ? DeGUI.styles.button.toolFoldoutClosedWStretchedLabel : DeGUI.styles.button.toolFoldoutClosedWLabel;
            }
            if (forceLabelColor != null) style = style.Clone((Color)forceLabelColor);
            bool clicked = GUILayout.Button(text, style);
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
        public static bool ToggleButton(bool toggled, GUIContent content, DeColorPalette colorPalette, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            DeColorPalette cp = colorPalette ?? DeGUI.colors;
            return ToggleButton(toggled, content, cp.bg.toggleOff, cp.bg.toggleOn, cp.content.toggleOff, cp.content.toggleOn, guiStyle, options);
        }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, string text, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        { return ToggleButton(toggled, new GUIContent(text, ""), DeGUI.colors.bg.toggleOff, bgOnColor, DeGUI.colors.content.toggleOff, contentOnColor, guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, GUIContent content, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        { return ToggleButton(toggled, content, DeGUI.colors.bg.toggleOff, bgOnColor, DeGUI.colors.content.toggleOff, contentOnColor, guiStyle, options); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(bool toggled, GUIContent content, Color bgOffColor, Color bgOnColor, Color contentOffColor, Color contenOnColor, GUIStyle guiStyle = null, params GUILayoutOption[] options)
        {
            Color prevBgColor = GUI.backgroundColor;
            Color prevContentColor = GUI.contentColor;
            GUI.backgroundColor = toggled ? bgOnColor : bgOffColor;
            GUI.contentColor = toggled ? contenOnColor : contentOffColor;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.bBlankBorder;
            bool clicked = GUILayout.Button(content, guiStyle, options);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            DeGUI.SetGUIColors(prevBgColor, prevContentColor, null);
            return toggled;
        }

        #endregion

        #region Toolbars

        public class ToolbarScope : DeScope
        {
            public ToolbarScope(params GUILayoutOption[] options)
            { BeginToolbar(options); }
            public ToolbarScope(GUIStyle style, params GUILayoutOption[] options)
            { BeginToolbar(style, options); }
            public ToolbarScope(Color backgroundShade, GUIStyle style, params GUILayoutOption[] options)
            { BeginToolbar(backgroundShade, style, options); }

            protected override void CloseScope()
	        { EndToolbar(); }
        }

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

        public class VBoxScope : DeScope
        {
            public VBoxScope(GUIStyle style)
            { BeginVBox(style); }

            protected override void CloseScope()
	        { EndVBox(); }
        }

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

        /// <summary>Horizontal Divider</summary>
        public static void HorizontalDivider(Color? color = null, int height = 1, int topMargin = 5, int bottomMargin = 5)
        {
            GUILayout.Space(topMargin);
            Rect r = GUILayoutUtility.GetRect(0, height, GUILayout.ExpandWidth(true));
            DeGUI.DrawColoredSquare(r, color == null ? (Color)DeGUI.colors.bg.divider : (Color)color);
            GUILayout.Space(bottomMargin);
        }

        /// <summary>
        /// A text field that becomes editable only on double-click
        /// </summary>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextField(EditorWindow editorWindow, string id, string text, GUIStyle defaultStyle, GUIStyle editingStyle = null, params GUILayoutOption[] options)
        { return DoDoubleClickTextField(null, editorWindow, id, text, null, -1, defaultStyle, editingStyle, options); }
        /// <summary>
        /// A text field that becomes editable only on double-click
        /// </summary>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextField(Editor editor, string id, string text, GUIStyle defaultStyle, GUIStyle editingStyle = null, params GUILayoutOption[] options)
        { return DoDoubleClickTextField(editor, null, id, text, null, -1, defaultStyle, editingStyle, options); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        /// <returns></returns>
        public static string DoubleClickDraggableTextField(EditorWindow editorWindow, string id, string text, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle, GUIStyle editingStyle = null, params GUILayoutOption[] options)
        { return DoDoubleClickTextField(null, editorWindow, id, text, draggableList, draggedItemIndex, defaultStyle, editingStyle, options); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        /// <returns></returns>
        public static string DoubleClickDraggableTextField(Editor editor, string id, string text, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle, GUIStyle editingStyle = null, params GUILayoutOption[] options)
        { return DoDoubleClickTextField(editor, null, id, text, draggableList, draggedItemIndex, defaultStyle, editingStyle, options); }

        static string DoDoubleClickTextField(Editor editor, EditorWindow editorWindow, string id, string text, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle, GUIStyle editingStyle = null, params GUILayoutOption[] options)
        {
            Rect r = GUILayoutUtility.GetRect(new GUIContent(""), defaultStyle, options);
            return DeGUI.DoDoubleClickTextField(r, false, editor, editorWindow, id, text, draggableList, draggedItemIndex, defaultStyle, editingStyle);
        }

        /// <summary>
        /// Creates a Gradient field by using Unity 4.x hidden default one and Reflection.
        /// </summary>
        public static Gradient GradientField(string label, Gradient gradient, params GUILayoutOption[] options)
        {
            if (_miGradientField == null) _miGradientField = typeof(EditorGUILayout).GetMethod("GradientField", BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string), typeof(Gradient), typeof(GUILayoutOption[]) }, null);
            return _miGradientField.Invoke(null, new object[] { label, gradient, options }) as Gradient;
        }

        /// <summary>Scene field</summary>
        public static Object SceneField(string label, Object obj)
        {
            // Verify that obj is a SceneAsset (not recognized by compiler as a class, so we have to use the string representation)
            if (obj != null && !obj.ToString().EndsWith(".SceneAsset)")) obj = null;
            // Draw
            return EditorGUILayout.ObjectField(label, obj, typeof(Object), false);
        }

        #endregion
    }
}