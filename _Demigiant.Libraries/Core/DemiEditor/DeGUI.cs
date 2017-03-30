// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/24 18:27

using System.Collections;
using System.Reflection;
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
        static MethodInfo _defaultPropertyFieldMInfo;

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
//            Event.current.Use();
            Event.current.type = EventType.Used;
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

        public class ColorScope : DeScope
        {
            readonly Color _prevBackground;
            readonly Color _prevContent;
            readonly Color _prevMain;

            public ColorScope(Color? background, Color? content = null, Color? main = null)
            {
                _prevBackground = GUI.backgroundColor;
                _prevContent = GUI.contentColor;
                _prevMain = GUI.color;
                if (background != null) GUI.backgroundColor = (Color)background;
                if (content != null) GUI.contentColor = (Color)content;
                if (main != null) GUI.color = (Color)main;
            }

            protected override void CloseScope()
            {
                GUI.backgroundColor = _prevBackground;
                GUI.contentColor = _prevContent;
                GUI.color = _prevMain;
            }
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
            if (GUI.enabled && Event.current.type == EventType.MouseUp && _activePressButtonId != -1) {
                _activePressButtonId = GUIUtility.hotControl = -1;
                Event.current.Use();
            }
            GUI.Button(rect, content, guiStyle);
//            int controlId = GUIUtility.GetControlID(FocusType.Native);
            int controlId = DeEditorGUIUtils.GetLastControlId(); // Changed from prev while working on DeInspektor
            int hotControl = GUIUtility.hotControl;
            bool pressed = GUI.enabled && _activePressButtonId == -1 && hotControl > 1 && rect.Contains(Event.current.mousePosition);
            if (pressed) {
                if (_activePressButtonId == -1 && _activePressButtonId != controlId) {
                    // Modified while working on DeInspektor
                    GUIUtility.hotControl = controlId; // Remove control from other elements (added while working on DeInspektor)
                    _activePressButtonId = controlId;
                    return true;
                }
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
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, DeColorPalette colorPalette, GUIStyle guiStyle = null)
        {
            DeColorPalette cp = colorPalette ?? DeGUI.colors;
            return ToggleButton(rect, toggled, content, cp.bg.toggleOff, cp.bg.toggleOn, cp.content.toggleOff, cp.content.toggleOn, guiStyle);
        }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), DeGUI.colors.bg.toggleOff, bgOnColor, DeGUI.colors.content.toggleOff, contentOnColor, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, content, DeGUI.colors.bg.toggleOff, bgOnColor, DeGUI.colors.content.toggleOff, contentOnColor, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, Color bgOffColor, Color bgOnColor, Color contentOffColor, Color contenOnColor, GUIStyle guiStyle = null)
        {
            Color prevBgColor = GUI.backgroundColor;
            Color prevContentColor = GUI.contentColor;
            GUI.backgroundColor = toggled ? bgOnColor : bgOffColor;
            GUI.contentColor = toggled ? contenOnColor : contentOffColor;
            if (guiStyle == null) guiStyle = DeGUI.styles.button.bBlankBorder;
            bool clicked = GUI.Button(rect, content, guiStyle);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            SetGUIColors(prevBgColor, prevContentColor, null);
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

        /// <summary>
        /// Draws a background grid using the given grid texture
        /// </summary>
        /// <param name="area">Area rect</param>
        /// <param name="offset">Offset from 0, 0 position (used when area has been dragged)</param>
        /// <param name="texture">Texture to use for the grid</param>
        public static void BackgroundGrid(Rect area, Vector2 offset, Texture2D texture)
        {
            if (Event.current.type != EventType.Repaint) return;

            int gridW = texture.width;
            int gridH = texture.height;
            int shiftX = (int)(gridW - offset.x % gridW);
            if (shiftX < 0) shiftX = gridW + shiftX;
            int shiftY = (int)(gridH - offset.y % gridH);
            if (shiftY < 0) shiftY = gridH + shiftY;
            Rect bgArea = new Rect(area.x - shiftX, area.yMax, area.width + shiftX, -(area.height + shiftY)); // Inverted becasue tiled texture otherwise would start from BL instead of TL
            GUI.DrawTextureWithTexCoords(bgArea, texture, new Rect(0, 0, bgArea.width / gridW, bgArea.height / gridH));
        }
        /// <summary>
        /// Draws a background grid using default grid textures
        /// </summary>
        /// <param name="area">Area rect</param>
        /// <param name="offset">Offset from 0, 0 position (used when area has been dragged)</param>
        /// <param name="forceDarkSkin">If TRUE forces a dark skin, otherwise uses a skin that fits with the current Unity's one</param>
        public static void BackgroundGrid(Rect area, Vector2 offset, bool forceDarkSkin = false)
        {
            Texture2D gridImg = forceDarkSkin || IsProSkin ? DeStylePalette.grid_dark : DeStylePalette.grid_bright;
            BackgroundGrid(area, offset, gridImg);
        }

        /// <summary>Box with style and color options</summary>
        public static void Box(Rect rect, Color color, GUIStyle style = null)
        {
            if (style == null) style = DeGUI.styles.box.def;
            Color orColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            GUI.Box(rect, "", style);
            GUI.backgroundColor = orColor;
        }

        /// <summary>
        /// Can be used instead of EditorGUI.PropertyField, to draw a serializedProperty without its attributes
        /// (very useful in case you want to use this from within a PropertyDrawer for that same property,
        /// since otherwise bad infinite loops might happen)
        /// </summary>
        public static void DefaultPropertyField(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_defaultPropertyFieldMInfo == null) _defaultPropertyFieldMInfo = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);
            _defaultPropertyFieldMInfo.Invoke(null, new object[] { position, property, label });
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