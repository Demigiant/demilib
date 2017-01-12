// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/24 18:27

using System.Collections;
using DG.DemiLib;
using DG.DemiLib.Core;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// Global Demigiant GUI manager. Call <see cref="DeGUI.BeginGUI"/> to initialize it inside GUI calls.
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
        /// <summary>TRUE if we're using the PRO skin</summary>
        public static readonly bool IsProSkin;
        public static Color defaultGUIColor, defaultGUIBackgroundColor, defaultGUIContentColor; // Set on Begin GUI
        static DeColorPalette _defaultColorPalette; // Default color palette if none selected
        static DeStylePalette _defaultStylePalette; // Default style palette if none selected
        static string _doubleClickTextFieldId; // ID of selected double click textField
        static int _activePressButtonId = -1;

        static DeGUI()
        {
            GUIUtils.isProSkin = IsProSkin = EditorGUIUtility.isProSkin;
        }

        #region Public GUI Methods

        /// <summary>
        /// Call this at the beginning of GUI methods
        /// </summary>
        /// <param name="guiColorPalette">Eventual <see cref="DeColorPalette"/> to use</param>
        /// <param name="guiStylePalette">Eventual <see cref="DeStylePalette"/> to use</param>
        public static void BeginGUI(DeColorPalette guiColorPalette = null, DeStylePalette guiStylePalette = null)
        {
            ChangePalette(guiColorPalette, guiStylePalette);
            defaultGUIColor = GUI.color;
            defaultGUIBackgroundColor = GUI.backgroundColor;
            defaultGUIContentColor = GUI.contentColor;
        }

        /// <summary>
        /// Exits the current event correctly, also taking care of eventual drag operations
        /// </summary>
        public static void ExitCurrentEvent()
        {
            DeGUIDrag.EndDrag(false);
            Event.current.Use();
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

        /// <summary>
        /// Resets the GUI colors to the default ones (only available if BeginGUI was called first)
        /// </summary>
        public static void ResetGUIColors(bool resetBackground = true, bool resetContent = true, bool resetMain = true)
        {
            if (resetBackground) GUI.backgroundColor = defaultGUIBackgroundColor;
            if (resetContent) GUI.contentColor = defaultGUIContentColor;
            if (resetMain) GUI.color = defaultGUIColor;
        }

        /// <summary>
        /// Sets the GUI colors to the given ones
        /// </summary>
        public static void SetGUIColors(Color? background, Color? content, Color? main)
        {
            if (background != null) GUI.backgroundColor = (Color)background;
            if (content != null) GUI.contentColor = (Color)content;
            if (main != null) GUI.color = (Color)main;
        }

        #endregion

        #region Public GUI Draw Methods

        #region Buttons

        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Rect rect, Color shade, string text)
        { return ShadedButton(rect, shade, new GUIContent(text, ""), null); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Rect rect, Color shade, string text, GUIStyle guiStyle)
        { return ShadedButton(rect, shade, new GUIContent(text, ""), guiStyle); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Rect rect, Color shade, GUIContent content)
        { return ShadedButton(rect, shade, content, null); }
        /// <summary>Shaded button</summary>
        public static bool ShadedButton(Rect rect, Color shade, GUIContent content, GUIStyle guiStyle)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = shade;
            bool clicked = guiStyle == null ? GUI.Button(rect, content) : GUI.Button(rect, content, guiStyle);
            GUI.backgroundColor = prevBgColor;
            return clicked;
        }

        /// <summary>Colored button</summary>
        public static bool ColoredButton(Rect rect, Color shade, Color contentColor, string text)
        { return ColoredButton(rect, shade, contentColor, new GUIContent(text, ""), null); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Rect rect, Color shade, Color contentColor, string text, GUIStyle guiStyle)
        { return ColoredButton(rect, shade, contentColor, new GUIContent(text, ""), guiStyle); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Rect rect, Color shade, Color contentColor, GUIContent content)
        { return ColoredButton(rect, shade, contentColor, content, null); }
        /// <summary>Colored button</summary>
        public static bool ColoredButton(Rect rect, Color shade, Color contentColor, GUIContent content, GUIStyle guiStyle)
        {
            Color prevBgColor = GUI.backgroundColor;
            GUI.backgroundColor = shade;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.def;
            bool clicked = GUI.Button(rect, content, guiStyle.Clone(contentColor));
            GUI.backgroundColor = prevBgColor;
            return clicked;
        }

        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(Rect rect, string text, GUIStyle guiStyle)
        { return PressButton(rect, new GUIContent(text, ""), guiStyle); }
        /// <summary>
        /// Draws a button that returns TRUE the first time it's pressed, instead than when its released.
        /// </summary>
        public static bool PressButton(Rect rect, GUIContent content, GUIStyle guiStyle)
        {
            // NOTE: tried using RepeatButton, but doesn't work if used for dragging
            GUI.Button(rect, content, guiStyle);
            int controlId = GUIUtility.GetControlID(FocusType.Native);
            int hotControl = GUIUtility.hotControl;
            bool pressed = _activePressButtonId == -1 && hotControl > 1 && rect.Contains(Event.current.mousePosition);
            if (pressed && _activePressButtonId != controlId) {
                _activePressButtonId = controlId;
                return true;
            }
            if (!pressed && hotControl < 1) _activePressButtonId = -1;
            return false;
        }

        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), null, null); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, GUIStyle guiStyle)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), null, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, DeColorPalette colorPalette, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), colorPalette, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content)
        { return ToggleButton(rect, toggled, content, null, null); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, GUIStyle guiStyle)
        { return ToggleButton(rect, toggled, content, null, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, DeColorPalette colorPalette, GUIStyle guiStyle = null)
        {
            DeColorPalette cp = colorPalette ?? DeGUI.colors;
            Color prevBgColor = GUI.backgroundColor;
            Color prevContentColor = GUI.contentColor;
            Color prevColor = GUI.color;
            GUI.backgroundColor = toggled ? cp.bg.toggleOn : cp.bg.toggleOff;
            GUI.contentColor = toggled ? cp.content.toggleOn : cp.content.toggleOff;
//            if (toggled) GUI.color = cp.content.toggleOn;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.bBlankBorder;
            bool clicked = GUI.Button(
                rect,
                content,
                guiStyle
            );
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            SetGUIColors(prevBgColor, prevContentColor, prevColor);
            return toggled;
        }

        #endregion

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

        #region Miscellaneous

        /// <summary>Box with style and color options</summary>
        public static void Box(Rect rect, Color color, GUIStyle style = null)
        {
            if (style == null) style = DeGUI.styles.box.def;
            Color orColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, "", style);
            GUI.backgroundColor = orColor;
        }

        /// <summary>Draws a colored square</summary>
        public static void DrawColoredSquare(Rect rect, Color color)
        {
            Color orColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, DeStylePalette.whiteSquare);
            GUI.color = orColor;
        }

        /// <summary>
        /// A text field that becomes editable only on double-click
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextField(Rect rect, EditorWindow editorWindow, string id, string text, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, null, editorWindow, id, text, -1, null, -1, defaultStyle, editingStyle); }
        /// <summary>
        /// A text field that becomes editable only on double-click
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextField(Rect rect, Editor editor, string id, string text, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, editor, null, id, text, -1, null, -1, defaultStyle, editingStyle); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="dragId">ID for this drag operation (must be the same for both this and Drag</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        /// <returns></returns>
        public static string DoubleClickDraggableTextField(Rect rect, EditorWindow editorWindow, string id, string text, int dragId, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, null, editorWindow, id, text, dragId, draggableList, draggedItemIndex, defaultStyle, editingStyle); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="dragId">ID for this drag operation (must be the same for both this and Drag</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        /// <returns></returns>
        public static string DoubleClickDraggableTextField(Rect rect, Editor editor, string id, string text, int dragId, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, editor, null, id, text, dragId, draggableList, draggedItemIndex, defaultStyle, editingStyle); }

        internal static string DoDoubleClickTextField(Rect rect, Editor editor, EditorWindow editorWindow, string id, string text, int dragId, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        {
            if (defaultStyle == null) defaultStyle = EditorStyles.label;
            if (editingStyle == null) editingStyle = EditorStyles.textField;
            GUI.SetNextControlName(id);
            if (_doubleClickTextFieldId != id && Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition)) {
                if (Event.current.clickCount == 2) {
                    // Activate edit mode
                    _doubleClickTextFieldId = id;
                    EditorGUI.FocusTextInControl(id);
                    DeGUI.ExitCurrentEvent();
                    if (editor != null) editor.Repaint();
                    else editorWindow.Repaint();
                } else if (draggableList != null) {
                    // Start drag
                    if (editor != null) DeGUIDrag.StartDrag(dragId, editor, draggableList, draggedItemIndex);
                    else DeGUIDrag.StartDrag(dragId, editorWindow, draggableList, draggedItemIndex);
                }
            }
            bool selected = _doubleClickTextFieldId == id && GUI.GetNameOfFocusedControl() == id;
            if (!selected) {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Arrow);
                if (GUI.GetNameOfFocusedControl() == id) GUIUtility.keyboardControl = 0;
            }
            text = EditorGUI.TextField(rect, text, selected ? editingStyle : defaultStyle);
            // End editing
            if (!selected) return text;
            if (
                Event.current.isKey && Event.current.keyCode == KeyCode.Return
                || Event.current.type == EventType.MouseDown && !rect.Contains(Event.current.mousePosition)
            ) {
                GUIUtility.keyboardControl = 0;
                _doubleClickTextFieldId = null;
                if (editor != null) editor.Repaint();
                else editorWindow.Repaint();
            }
            return text;
        }

        /// <summary>Divider</summary>
        public static void FlatDivider(Rect rect, Color? color = null)
        {
            Color prevBgColor = GUI.backgroundColor;
            if (color != null) GUI.backgroundColor = (Color)color;
            GUI.Box(rect, "", DeGUI.styles.box.flat);
            GUI.backgroundColor = prevBgColor;
        }

        #endregion

        #endregion
    }
}