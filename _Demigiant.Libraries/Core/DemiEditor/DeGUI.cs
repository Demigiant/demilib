// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/24 18:27

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.DemiEditor.Panels;
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
        enum ButtonState
        {
            Normal, Hover, Pressed
        }

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
        public static int defaultFontSize { get; private set; } // Set on Begin GUI
        public static bool usesInterFont { get; private set; } // Set on Begin GUI: new default font added in Unity 2019.3
        public static readonly GUIContent MixedValueLabel = new GUIContent("—");
        public static bool isMouseDown { get; private set; }

        static DeColorPalette _defaultColorPalette; // Default color palette if none selected
        static DeStylePalette _defaultStylePalette; // Default style palette if none selected
        static string _doubleClickTextFieldId; // ID of selected double click textField
        static int _activePressButtonId = -1;
        static readonly HashSet<int> _ActiveDownButtonsIds = new HashSet<int>();
        static int _pressFrame = -1;
        static bool _hasEditorPressUpdateActive = false;
        static readonly Dictionary<Rect, ButtonState> _ButtonRectToState = new Dictionary<Rect, ButtonState>();
        static MethodInfo _miDefaultPropertyField {
            get {
                if (_foo_miDefaultPropertyField == null) {
                    _foo_miDefaultPropertyField = typeof(EditorGUI).GetMethod("DefaultPropertyField", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return _foo_miDefaultPropertyField;
            }
        }
        static MethodInfo _foo_miDefaultPropertyField;
        static FieldInfo _fiChangedStack {
            get {
                if (_foo_fiChangedStack == null) {
                    _foo_fiChangedStack = typeof(EditorGUI).GetField("s_ChangedStack", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return _foo_fiChangedStack;
            }
        }
        static FieldInfo _foo_fiChangedStack;

        static DeGUI()
        {
            GUIUtils.isProSkin = IsProSkin = EditorGUIUtility.isProSkin;
            EditorApplication.update -= OnEditorPressUpdate;
            _hasEditorPressUpdateActive = false;
        }

        static void OnEditorPressUpdate()
        {
            if (GUIUtility.hotControl < 2) {
                _pressFrame = -1;
                EditorApplication.update -= OnEditorPressUpdate;
                _hasEditorPressUpdateActive = false;
                return;
            }
            _pressFrame++;
        }

        #region Public GUI Methods

        /// <summary>
        /// Call this at the beginning of GUI methods.
        /// Returns TRUE if the styles were initialized or re-initialized
        /// </summary>
        /// <param name="guiColorPalette">Eventual <see cref="DeColorPalette"/> to use</param>
        /// <param name="guiStylePalette">Eventual <see cref="DeStylePalette"/> to use</param>
        public static bool BeginGUI(DeColorPalette guiColorPalette = null, DeStylePalette guiStylePalette = null)
        {
            bool stylesReinitialized = ChangePalette(guiColorPalette, guiStylePalette);
            defaultGUIColor = GUI.color;
            defaultGUIBackgroundColor = GUI.backgroundColor;
            defaultGUIContentColor = GUI.contentColor;
            defaultFontSize = GUI.skin.label.fontSize;
            usesInterFont = DeUnityEditorVersion.Version >= 2019.3f && (GUI.skin.label.font == null);

            switch (Event.current.rawType) {
            case EventType.MouseDown:
                isMouseDown = true;
                break;
            case EventType.MouseUp:
                isMouseDown = false;
                break;
            }

            return stylesReinitialized;
        }

        /// <summary>
        /// Better implementation of GUI.BeginScrollView.
        /// Returns the modified scrollView struct.<para/>
        /// Must be closed by a DeGUI.<see cref="EndScrollView"/>.<para/>
        /// <pre><code>EXAMPLE
        /// Rect scrollViewArea = ...;
        /// Rect drawArea = scrollViewArea;
        /// // Decrease the full drawing area to exclude scrollbars if necessary
        /// if (_scrollView.fullContentArea.height > scrollViewArea.height) drawArea = drawArea.Shift(0, 0, -11, 0);
        /// // Begin scrollView
        /// _scrollView = DeGUI.BeginScrollView(scrollViewArea, _scrollView);
        /// // Increase scrollView area correctly (or directly set it with SetFullContentHeight
        /// _scrollView.IncreaseContentHeightBy(...)
        /// // End
        /// DeGUI.EndScrollView();
        /// </code></pre>
        /// </summary>
        /// <param name="scrollViewArea">Visible area used by the scrollView</param>
        /// <param name="scrollView"><see cref="DeScrollView"/> target. You'll need to set its size to the correct full content height
        /// (either within the Begin/ENd ScrollView calls or before them)</param>
        /// <param name="resetContentHeightToZero">If TRUE (default) resets <see cref="DeScrollView.fullContentArea"/>.height to 0
        /// after beginning the ScrollView</param>
        public static DeScrollView BeginScrollView(Rect scrollViewArea, DeScrollView scrollView, bool resetContentHeightToZero = true)
        {
            scrollView.Begin(scrollViewArea, resetContentHeightToZero);
            return scrollView;
        }
        /// <summary>
        /// Closes a DeGUI.<see cref="BeginScrollView"/> correctly
        /// </summary>
        public static void EndScrollView()
        {
            DeScrollView.Current().End();
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
        /// Removes focus from any GUI button/text/element that has focus
        /// </summary>
        public static void Deselect()
        {
            GUI.FocusControl(null);
        }

        /// <summary>
        /// Changes the active palettes to the given ones
        /// (or resets them to the default ones if NULL).
        /// Returns TRUE if the styles were initialized or re-initialized
        /// </summary>
        public static bool ChangePalette(DeColorPalette newColorPalette, DeStylePalette newStylePalette)
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

            return styles.Init();
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

        /// <summary>
        /// Opens a panel that previews the given texture (if not NULL)
        /// </summary>
        public static void ShowTexturePreview(Texture2D texture)
        {
            TexturePreviewWindow.Open(texture);
        }

        /// <summary>
        /// Gets either black or white, depending on the color that would be most visible on the given one
        /// </summary>
        public static Color GetVisibleContentColorOn(Color32 bgColor)
        {
            int lum = (int)Math.Sqrt(
                bgColor.r * bgColor.r * .299 +
                bgColor.g * bgColor.g * .587 +
                bgColor.b * bgColor.b * .114
            );
            return lum > 130 ? Color.black : Color.white;
        }
        /// <summary>
        /// Gets either black or white, depending on the color that would be most visible on the given one
        /// </summary>
        public static Color GetFastVisibleContentColorOn(Color32 bgColor)
        {
            // Old faster but obviously not as good method
            return bgColor.r + bgColor.g + bgColor.b > 1.5f ? Color.black : Color.white;
        }

        #endregion

        #region Methods

        static ButtonState GetButtonState(Rect r)
        {
            if (!_ButtonRectToState.ContainsKey(r)) _ButtonRectToState.Add(r, ButtonState.Normal);
            return _ButtonRectToState[r];
        }

        static void SetButtonState(Rect r, ButtonState state)
        {
            if (_ButtonRectToState.ContainsKey(r)) _ButtonRectToState[r] = state;
            else _ButtonRectToState.Add(r, state);
        }

        #endregion

        #region Scopes

        public class MixedValueScope : DeScope
        {
            readonly bool _prevMixedValue;
            public MixedValueScope(bool showMixedValue = true)
            {
                _prevMixedValue = EditorGUI.showMixedValue;
                EditorGUI.showMixedValue = showMixedValue;
            }
            protected override void CloseScope()
            {
                EditorGUI.showMixedValue = _prevMixedValue;
            }
        }

        public class LabelFieldWidthScope : DeScope
        {
            readonly float _prevLabelWidth;
            readonly float _prevFieldWidth;
            public LabelFieldWidthScope(float? labelWidth, float? fieldWidth = null)
            {
                _prevLabelWidth = EditorGUIUtility.labelWidth;
                _prevFieldWidth = EditorGUIUtility.fieldWidth;
                if (labelWidth != null) EditorGUIUtility.labelWidth = (float)labelWidth;
                if (fieldWidth != null) EditorGUIUtility.fieldWidth = (float)fieldWidth;
            }
            protected override void CloseScope()
            {
                EditorGUIUtility.labelWidth = _prevLabelWidth;
                EditorGUIUtility.fieldWidth = _prevFieldWidth;
            }
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

        /// <summary>
        /// Sets the GUI cursor color to the given ones
        /// </summary>
        public class CursorColorScope : DeScope
        {
            readonly Color _prevColor;
            public CursorColorScope(Color color)
            {
                _prevColor = GUI.skin.settings.cursorColor;
                GUI.skin.settings.cursorColor = color;
            }
            protected override void CloseScope()
            {
                GUI.skin.settings.cursorColor = _prevColor;
            }
        }

        /// <summary>
        /// Sets the GUI matrix to the given ones
        /// </summary>
        public class MatrixScope : DeScope
        {
            readonly Matrix4x4 _prevMatrix;
            public MatrixScope(Matrix4x4 matrix)
            {
                _prevMatrix = GUI.matrix;
                GUI.matrix = matrix;
            }
            protected override void CloseScope()
            {
                GUI.matrix = _prevMatrix;
            }
        }

        /// <summary>
        /// Wrapper to set serialized fields with multiple sources selected: automatically sets GUI to show mixed values when necessary
        /// and contains a fieldInfo <see cref="FieldInfo"/> which is set within the wrapper.<para/>
        /// Note that you must set the <see cref="value"/> property within the wrapper so that it's assigned correctly when closing the scope.
        /// </summary>
        public class MultiPropertyScope : DeScope
        {
            public readonly FieldInfo fieldInfo;
            public readonly bool hasMixedValue;
            public readonly bool isGenericSerializedProperty; // Not supported (usually UnityEvents)
            public object value;
            readonly IList _sources;
            readonly bool _prevShowMixedValue;
            readonly bool _isSerializedPropertyMode;
            readonly bool _requiresSpecialEndChangeCheck;
            readonly List<SerializedProperty> _fieldsAsSerializedProperties;

            /// <summary>Multi property scope</summary>
            /// <param name="fieldName">Name of the field so it can be found and set/get via Reflection</param>
            /// <param name="sources">List of the sources containing the given field</param>
            /// <param name="requiresSpecialEndChangeCheck">If TRUE validates EditorGUI.EndChangeCheck before calling it
            /// (fixes an issue which happens with advanced Undo usage in DOTween Timeline and ColorFields)</param>
            public MultiPropertyScope(
                string fieldName, IList sources, List<SerializedProperty> fieldsAsSerializedProperties = null,
                bool requiresSpecialEndChangeCheck = false
            ){
                _sources = sources;
                fieldInfo = GetFieldInfo(fieldName, sources);
                _prevShowMixedValue = EditorGUI.showMixedValue;
                _requiresSpecialEndChangeCheck = requiresSpecialEndChangeCheck;
                EditorGUI.BeginChangeCheck();
                EditorGUI.showMixedValue = hasMixedValue = HasMixedValue(fieldInfo, _sources);
                if (fieldsAsSerializedProperties != null) {
                    _isSerializedPropertyMode = true;
                    _fieldsAsSerializedProperties = fieldsAsSerializedProperties;
                    isGenericSerializedProperty = _fieldsAsSerializedProperties[0].propertyType == SerializedPropertyType.Generic;
                    for (int i = 0; i < _fieldsAsSerializedProperties.Count; ++i) _fieldsAsSerializedProperties[i].serializedObject.Update();
                }
            }

            protected override void CloseScope()
            {
                EditorGUI.showMixedValue = _prevShowMixedValue;
                if (_requiresSpecialEndChangeCheck) {
                    Stack<bool> changeStack = (Stack<bool>)_fiChangedStack.GetValue(null);
                    if (changeStack.Count > 0 && !EditorGUI.EndChangeCheck()) return;
                } else if (!EditorGUI.EndChangeCheck()) return;
                if (_isSerializedPropertyMode) {
                    if (isGenericSerializedProperty) {
                        // Update only first one that might have changed, ignore others
                        _fieldsAsSerializedProperties[0].serializedObject.ApplyModifiedProperties();
                    } else {
                        object value = null;
                        for (int i = 0; i < _fieldsAsSerializedProperties.Count; ++i) {
                            if (i == 0) value = _fieldsAsSerializedProperties[i].GetValue();
                            else _fieldsAsSerializedProperties[i].SetValue(value);
                            _fieldsAsSerializedProperties[i].serializedObject.ApplyModifiedProperties();
                        }
                    }
                } else {
                    try {
                        for (int i = 0; i < _sources.Count; ++i) fieldInfo.SetValue(_sources[i], value);
                    } catch {
                        // Means the value was only shown not set (like in case of toggle buttons that are set later by custom code)
                    }
                }
            }

            static FieldInfo GetFieldInfo(string fieldName, IList targets)
            {
                return targets[0].GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            }

            static bool HasMixedValue(FieldInfo fInfo, IList targets)
            {
                object defValue = null;
                for (int i = 0; i < targets.Count; ++i) {
                    if (i == 0) defValue = fInfo.GetValue(targets[i]);
                    else {
                        object value = fInfo.GetValue(targets[i]);
                        if (value == null) {
                            if (defValue != null) return true;
                        } else if (!value.Equals(defValue)) return true;
                    }
                }
                return false;
            }
        }

        #endregion

        #region Public GUI Draw Methods

        #region Buttons

        /// <summary>
        /// A button that triggers an immediate repaint when hovered/pressed/unhovered
        /// (which otherwise doesn't happen if you apply a background to the button's GUIStyle).<para/>
        /// Requires <see cref="EditorWindow.wantsMouseMove"/> to be activated.
        /// </summary>
        public static bool ActiveButton(Rect rect, GUIContent content, GUIStyle guiStyle = null)
        {
            ButtonState lastState = GetButtonState(rect);
            ButtonState currState = !rect.Contains(Event.current.mousePosition)
                ? ButtonState.Normal : isMouseDown ? ButtonState.Pressed : ButtonState.Hover;
            bool result = GUI.Button(rect, content, guiStyle);
            if (lastState != currState) {
                SetButtonState(rect, currState);
                DeEditorPanelUtils.RepaintCurrentEditor();
            }
            return result;
        }
        /// <summary>
        /// A button that triggers an immediate repaint when hovered/pressed/unhovered
        /// (which otherwise doesn't happen if you apply a background to the button's GUIStyle)
        /// and also assigns different GUI colors based on the button's state and the given one.<para/>
        /// Requires <see cref="EditorWindow.wantsMouseMove"/> to be activated.
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <param name="content">Content</param>
        /// <param name="onNormal">Default color</param>
        /// <param name="guiStyle">Style</param>
        public static bool ActiveButton(Rect rect, GUIContent content, Color onNormal, GUIStyle guiStyle)
        { return ActiveButton(rect, content, onNormal, null, null, guiStyle); }
        /// <summary>
        /// A button that triggers an immediate repaint when hovered/pressed/unhovered
        /// (which otherwise doesn't happen if you apply a background to the button's GUIStyle)
        /// and also assigns different GUI colors based on the button's state with options to eventually auto-generate them.<para/>
        /// Requires <see cref="EditorWindow.wantsMouseMove"/> to be activated.
        /// </summary>
        /// <param name="rect">Rect</param>
        /// <param name="content">Content</param>
        /// <param name="onNormal">Default color</param>
        /// <param name="onHover">Hover color (if NULL auto-generates it from the given one by making it brighter</param>
        /// <param name="onPressed">Pressed color (if NULL auto-generates it from the given one by making it even brighter</param>
        /// <param name="guiStyle">Style</param>
        public static bool ActiveButton(
            Rect rect, GUIContent content, Color onNormal, Color? onHover = null, Color? onPressed = null, GUIStyle guiStyle = null
        ){
            bool result;
            Color color = onNormal;
            ButtonState lastState = GetButtonState(rect);
            ButtonState currState = !rect.Contains(Event.current.mousePosition)
                ? ButtonState.Normal : isMouseDown ? ButtonState.Pressed : ButtonState.Hover;
            switch (currState) {
            case ButtonState.Hover:
                color = onHover != null ? (Color)onHover : onNormal.CloneAndChangeBrightness(1.15f);
                break;
            case ButtonState.Pressed:
                color = onPressed != null ? (Color)onPressed : onNormal.CloneAndChangeBrightness(1.3f);
                break;
            }
            using (new ColorScope(color)) result = GUI.Button(rect, content, guiStyle);
            if (lastState != currState) {
                SetButtonState(rect, currState);
                DeEditorPanelUtils.RepaintCurrentEditor();
            }
            return result;
        }

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

        /// <summary>Toolbar foldout button which allows clicking even on its label</summary>
        public static bool FoldoutButton(Rect rect, bool toggled, string text = null, bool isLarge = false, bool stretchedLabel = false)
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
            bool clicked = GUI.Button(rect, text, style);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
        }
        /// <summary>Foldout button + label (not intended to be used in toolbar) which allows click-to-foldout/foldin</summary>
        public static bool FoldoutLabel(Rect rect, bool toggled, GUIContent label = null)
        {
            bool hasLabel = label != null && !string.IsNullOrEmpty(label.text);
            GUIStyle style = !hasLabel
                ? toggled ? styles.button.toolFoldoutOpen : styles.button.toolFoldoutClosed
                : toggled
                    ? styles.button.foldoutOpenWLabel
                    : styles.button.foldoutClosedWLabel;
            bool clicked = GUI.Button(rect, hasLabel ? label : GUIContent.none, style);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
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
//                GUIUtility.hotControl = -1; // This breaks the undo system
                GUIUtility.hotControl = 0;
                Event.current.Use();
                _pressFrame = -1;
                _activePressButtonId = -1;
            }
            GUI.Button(rect, content, guiStyle);
//            int controlId = GUIUtility.GetControlID(FocusType.Native);
            int controlId = DeEditorGUIUtils.GetLastControlId(); // Changed from prev while working on DeInspektor
            int hotControl = GUIUtility.hotControl;
//            bool pressed = GUI.enabled && _activePressButtonId == -1 && hotControl > 1 && rect.Contains(Event.current.mousePosition);
            // Avoid detecting a press if mouse is already pressed and moves over button later
            bool mousePressed = GUI.enabled && _activePressButtonId == -1 && hotControl > 1;
            if (mousePressed && _pressFrame == -1) {
                _pressFrame++;
                if (!_hasEditorPressUpdateActive) {
                    _hasEditorPressUpdateActive = true;
                    EditorApplication.update -= OnEditorPressUpdate;
                    EditorApplication.update += OnEditorPressUpdate;
                }
            }
            bool pressedOverButton = mousePressed && _pressFrame < 2 && rect.Contains(Event.current.mousePosition);
            if (pressedOverButton) {
                if (_activePressButtonId == -1 && _activePressButtonId != controlId) {
                    // Modified while working on DeInspektor
                    GUIUtility.hotControl = controlId; // Remove control from other elements (added while working on DeInspektor)
                    _activePressButtonId = controlId;
                    return true;
                }
            }
            if (!pressedOverButton && hotControl < 1) _activePressButtonId = -1;
            return false;
        }

        /// <summary>
        /// Draws a button that returns TRUE the first time the mouse moves over it while the mouse button is pressed,
        /// even if it was pressed outside of the button first
        /// </summary>
        public static bool DownButton(Rect rect, string text, GUIStyle guiStyle)
        { return DownButton(rect, new GUIContent(text, ""), guiStyle); }
        /// <summary>
        /// Draws a button that returns TRUE the first time the mouse moves over it while the mouse button is pressed,
        /// even if it was pressed outside of the button first
        /// </summary>
        public static bool DownButton(Rect rect, GUIContent content, GUIStyle guiStyle)
        {
            if (GUI.enabled && Event.current.type == EventType.MouseUp && _ActiveDownButtonsIds.Count > 0) {
                _ActiveDownButtonsIds.Clear();
                GUIUtility.hotControl = 0;
                Event.current.Use();
            }
            GUI.Button(rect, content, guiStyle);
            int controlId = DeEditorGUIUtils.GetLastControlId(); // Changed from prev while working on DeInspektor
            int hotControl = GUIUtility.hotControl;
            bool mousePressed = GUI.enabled && controlId > 1 && hotControl > 1
                                && !_ActiveDownButtonsIds.Contains(controlId) && rect.Contains(Event.current.mousePosition)
                                // This line makes sure that if a non-down-button element is pressed before this nothing will happen
                                && (hotControl == controlId || _ActiveDownButtonsIds.Contains(hotControl));
            if (mousePressed) {
                GUIUtility.hotControl = controlId;
                _ActiveDownButtonsIds.Add(controlId);
                return true;
            }
            if (hotControl < 1) _ActiveDownButtonsIds.Clear();
            return false;
        }

        /// <summary>Toolbar foldout button</summary>
        public static bool ToolbarFoldoutButton(Rect rect, bool toggled, string text = null, bool isLarge = false, bool stretchedLabel = false, Color? forceLabelColor = null)
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
            bool clicked = GUI.Button(rect, text, style);
            if (clicked) {
                toggled = !toggled;
                GUI.changed = true;
            }
            return toggled;
        }

        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text)
        { return ToggleButton(rect, toggled, new GUIContent(text, "")); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content)
        { return ToggleButton(rect, toggled, content, null, null); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, GUIStyle guiStyle)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, GUIStyle guiStyle)
        { return ToggleButton(rect, toggled, content, null, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, DeColorPalette colorPalette, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), colorPalette, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, DeColorPalette colorPalette, GUIStyle guiStyle = null)
        {
            DeColorPalette cp = colorPalette ?? DeGUI.colors;
            return ToggleButton(rect, toggled, content, cp.bg.toggleOff, cp.bg.toggleOn, cp.content.toggleOff, cp.content.toggleOn, guiStyle);
        }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text, ""), bgOnColor, contentOnColor, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, Color bgOnColor, Color contentOnColor, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, content, DeGUI.colors.bg.toggleOff, bgOnColor, DeGUI.colors.content.toggleOff, contentOnColor, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, ToggleColors onColors, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text), onColors, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, ToggleColors onColors, GUIStyle guiStyle = null)
        {
            DeToggleColors.ColorCombination combo = DeGUI.colors.toggle.GetColors(ToggleColors.DefaultOff, onColors);
            return ToggleButton(rect, toggled, content, combo.offCols.bg, combo.onCols.bg, combo.offCols.content, combo.onCols.content, guiStyle);
        }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, string text, ToggleColors offColors, ToggleColors onColors, GUIStyle guiStyle = null)
        { return ToggleButton(rect, toggled, new GUIContent(text), offColors, onColors, guiStyle); }
        /// <summary>Button that can be toggled on and off</summary>
        public static bool ToggleButton(Rect rect, bool toggled, GUIContent content, ToggleColors offColors, ToggleColors onColors, GUIStyle guiStyle = null)
        {
            DeToggleColors.ColorCombination combo = DeGUI.colors.toggle.GetColors(offColors, onColors);
            return ToggleButton(rect, toggled, content, combo.offCols.bg, combo.onCols.bg, combo.offCols.content, combo.onCols.content, guiStyle);
        }
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
        /// <param name="scale">Eventual scale to apply to the grid</param>
        public static void BackgroundGrid(Rect area, Vector2 offset, Texture2D texture, float scale = 1)
        {
            if (Event.current.type != EventType.Repaint) return;

            int gridW = (int)(texture.width * scale);
            int gridH = (int)(texture.height * scale);
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
        /// <param name="scale">Eventual scale to apply to the grid</param>
        public static void BackgroundGrid(Rect area, Vector2 offset, bool forceDarkSkin = false, float scale = 1)
        {
            Texture2D gridImg = forceDarkSkin || IsProSkin ? DeStylePalette.grid_dark : DeStylePalette.grid_bright;
            BackgroundGrid(area, offset, gridImg, scale);
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
            _miDefaultPropertyField.Invoke(null, new object[] { position, property, label });
        }

        /// <summary>Draws a colored square</summary>
        public static void DrawColoredSquare(Rect rect, Color color)
        {
            Color orColor = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, DeStylePalette.whiteSquare);
            GUI.color = orColor;
        }

        /// <summary>Draws the given texture tiled within the given rect</summary>
        /// <param name="rect">Rect</param>
        /// <param name="texture">Texture</param>
        /// <param name="scale">Eventual scale to apply</param>
        /// <param name="color">If not NULL, colorizes the texture with this color</param>
        public static void DrawTiledTexture(Rect rect, Texture2D texture, float scale = 1, Color? color = null)
        {
            float w = texture.width * scale;
            float h = texture.height * scale;
            if (color == null) GUI.DrawTextureWithTexCoords(rect, texture, new Rect(0, 0, rect.width / w, rect.height / h));
            else {
                Color prev = GUI.color;
                GUI.color = (Color)color;
                GUI.DrawTextureWithTexCoords(rect, texture, new Rect(0, 0, rect.width / w, rect.height / h));
                GUI.color = prev;
            }
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
        { return DoDoubleClickTextField(rect, false, null, editorWindow, id, text, null, -1, defaultStyle, editingStyle); }
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
        { return DoDoubleClickTextField(rect, false, editor, null, id, text, null, -1, defaultStyle, editingStyle); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickDraggableTextField(Rect rect, EditorWindow editorWindow, string id, string text, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, false, null, editorWindow, id, text, draggableList, draggedItemIndex, defaultStyle, editingStyle); }
        /// <summary>
        /// A text field that becomes editable only on double-click and can also be dragged
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="draggableList">List containing the dragged item and all other relative draggable items</param>
        /// <param name="draggedItemIndex">DraggableList index of the item being dragged</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickDraggableTextField(Rect rect, Editor editor, string id, string text, IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, false, editor, null, id, text, draggableList, draggedItemIndex, defaultStyle, editingStyle); }

        /// <summary>
        /// A textArea that becomes editable only on double-click
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editorWindow">EditorWindow reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextArea(Rect rect, EditorWindow editorWindow, string id, string text, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, true, null, editorWindow, id, text, null, -1, defaultStyle, editingStyle); }
        /// <summary>
        /// A textArea that becomes editable only on double-click
        /// </summary>
        /// <param name="rect">Area</param>
        /// <param name="editor">Editor reference</param>
        /// <param name="id">A unique ID to use in order to determine if the text is selected or not</param>
        /// <param name="text">Text</param>
        /// <param name="defaultStyle">Style for default (non-editing mode) appearance</param>
        /// <param name="editingStyle">Style for editing mode</param>
        public static string DoubleClickTextArea(Rect rect, Editor editor, string id, string text, GUIStyle defaultStyle = null, GUIStyle editingStyle = null)
        { return DoDoubleClickTextField(rect, true, editor, null, id, text, null, -1, defaultStyle, editingStyle); }

        internal static string DoDoubleClickTextField(
            Rect rect, bool isTextArea, Editor editor, EditorWindow editorWindow, string id, string text,
            IList draggableList, int draggedItemIndex, GUIStyle defaultStyle = null, GUIStyle editingStyle = null
        ){
            if (defaultStyle == null) defaultStyle = EditorStyles.label;
            if (editingStyle == null) editingStyle = EditorStyles.textField;
            GUI.SetNextControlName(id);
            bool isMouseDown = Event.current.rawType == EventType.MouseDown;
            bool isMouseDownInside = isMouseDown && rect.Contains(Event.current.mousePosition);
            bool forceDeselect = isMouseDown && (_doubleClickTextFieldId == id && !isMouseDownInside || _doubleClickTextFieldId != id && isMouseDownInside);
            if (isMouseDownInside && _doubleClickTextFieldId != id) {
                if (Event.current.clickCount == 2) {
                    forceDeselect = false;
                    // Activate edit mode
                    _doubleClickTextFieldId = id;
                    EditorGUI.FocusTextInControl(id);
//                    DeGUI.ExitCurrentEvent();
                    if (editor != null) editor.Repaint();
                    else editorWindow.Repaint();
                } else if (draggableList != null) {
                    // Start drag
                    if (editor != null) DeGUIDrag.StartDrag(editor, draggableList, draggedItemIndex);
                    else DeGUIDrag.StartDrag(editorWindow, draggableList, draggedItemIndex);
                }
            }
//            bool selected = _doubleClickTextFieldId == id && GUI.GetNameOfFocusedControl() == id;
            bool selected = _doubleClickTextFieldId == id && (Event.current.type != EventType.Layout || GUI.GetNameOfFocusedControl() == id);
            if (!selected) {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Arrow);
                if (GUI.GetNameOfFocusedControl() == id) GUIUtility.keyboardControl = 0;
            }
            text = isTextArea
                ? EditorGUI.TextArea(rect, text, selected ? editingStyle : defaultStyle)
                : EditorGUI.TextField(rect, text, selected ? editingStyle : defaultStyle);
            // End editing
            if (forceDeselect) goto Deselect;
            if (!selected) return text;
            bool deselect = Event.current.type == EventType.MouseDown && !rect.Contains(Event.current.mousePosition);
            if (!deselect && Event.current.isKey) {
                deselect = !isTextArea && Event.current.keyCode == KeyCode.Return
                           || Event.current.control && Event.current.keyCode == KeyCode.Return
                           || Event.current.keyCode == KeyCode.Escape;
            }
            if (deselect) goto Deselect;
            else goto End;
        Deselect:
            GUIUtility.keyboardControl = 0;
            _doubleClickTextFieldId = null;
            if (editor != null) editor.Repaint();
            else editorWindow.Repaint();
        End:
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

        /// <summary>Draws a Vector3Field that can have single axes disabled</summary>
        public static Vector2 Vector2FieldAdvanced(
            Rect rect, GUIContent label, Vector2 value, bool xEnabled, bool yEnabled, bool lockAllToX, bool lockAllToY
        ){
            if (label.HasText()) rect = EditorGUI.PrefixLabel(rect, label);
            Rect axisR = rect.SetWidth((rect.width - 2) / 2);
            using (new LabelFieldWidthScope(10)) {
                for (int i = 0; i < 2; ++i) {
                    bool wasGuiEnabled = GUI.enabled;
                    switch (i) {
                    case 0:
                        GUI.enabled = wasGuiEnabled && xEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.x = EditorGUI.FloatField(axisR, "X", value.x);
                        if (EditorGUI.EndChangeCheck() && lockAllToX) value.y = value.x;
                        break;
                    case 1:
                        GUI.enabled = wasGuiEnabled && yEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.y = EditorGUI.FloatField(axisR, "Y", value.y);
                        if (EditorGUI.EndChangeCheck() && lockAllToY) value.x = value.y;
                        break;
                    }
                    GUI.enabled = wasGuiEnabled;
                    axisR = axisR.Shift(axisR.width + 2, 0, 0, 0);
                }
            }
            return value;
        }

        /// <summary>Draws a Vector3Field that can have single axes disabled</summary>
        public static Vector3 Vector3FieldAdvanced(
            Rect rect, GUIContent label, Vector3 value, bool xEnabled, bool yEnabled, bool zEnabled, bool lockAllToX, bool lockAllToY, bool lockAllToZ
        )
        {
            if (label.HasText()) rect = EditorGUI.PrefixLabel(rect, label);
            Rect axisR = rect.SetWidth((rect.width - 4) / 3);
            using (new LabelFieldWidthScope(10)) {
                for (int i = 0; i < 3; ++i) {
                    bool wasGuiEnabled = GUI.enabled;
                    switch (i) {
                    case 0:
                        GUI.enabled = wasGuiEnabled && xEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.x = EditorGUI.FloatField(axisR, "X", value.x);
                        if (EditorGUI.EndChangeCheck() && lockAllToX) value.y = value.z = value.x;
                        break;
                    case 1:
                        GUI.enabled = wasGuiEnabled && yEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.y = EditorGUI.FloatField(axisR, "Y", value.y);
                        if (EditorGUI.EndChangeCheck() && lockAllToY) value.x = value.z = value.y;
                        break;
                    case 2:
                        GUI.enabled = wasGuiEnabled && zEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.z = EditorGUI.FloatField(axisR, "Z", value.z);
                        if (EditorGUI.EndChangeCheck() && lockAllToZ) value.x = value.y = value.z;
                        break;
                    }
                    GUI.enabled = wasGuiEnabled;
                    axisR = axisR.Shift(axisR.width + 2, 0, 0, 0);
                }
            }
            return value;
        }

        /// <summary>Draws a Vector3Field that can have single axes disabled</summary>
        public static Vector4 Vector4FieldAdvanced(
            Rect rect, GUIContent label, Vector4 value, bool xEnabled, bool yEnabled, bool zEnabled, bool wEnabled,
            bool lockAllToX, bool lockAllToY, bool lockAllToZ, bool lockAllToW
        ){
            if (label.HasText()) rect = EditorGUI.PrefixLabel(rect, label);
            Rect axisR = rect.SetWidth((rect.width - 6) / 4);
            using (new LabelFieldWidthScope(10)) {
                for (int i = 0; i < 4; ++i) {
                    bool wasGuiEnabled = GUI.enabled;
                    switch (i) {
                    case 0:
                        GUI.enabled = wasGuiEnabled && xEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.x = EditorGUI.FloatField(axisR, "X", value.x);
                        if (EditorGUI.EndChangeCheck() && lockAllToX) value.y = value.z = value.w = value.x;
                        break;
                    case 1:
                        GUI.enabled = wasGuiEnabled && yEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.y = EditorGUI.FloatField(axisR, "Y", value.y);
                        if (EditorGUI.EndChangeCheck() && lockAllToY) value.x = value.z = value.w = value.y;
                        break;
                    case 2:
                        GUI.enabled = wasGuiEnabled && zEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.z = EditorGUI.FloatField(axisR, "Z", value.z);
                        if (EditorGUI.EndChangeCheck() && lockAllToZ) value.x = value.y = value.w = value.z;
                        break;
                    case 3:
                        GUI.enabled = wasGuiEnabled && wEnabled;
                        EditorGUI.BeginChangeCheck();
                        value.w = EditorGUI.FloatField(axisR, "W", value.w);
                        if (EditorGUI.EndChangeCheck() && lockAllToW) value.x = value.y = value.z = value.w;
                        break;
                    }
                    GUI.enabled = wasGuiEnabled;
                    axisR = axisR.Shift(axisR.width + 2, 0, 0, 0);
                }
            }
            return value;
        }

        #endregion

        #region MixedValue GUI

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiColorField(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources, null, true)) {
                mScope.value = EditorGUI.ColorField(rect, label, (Color)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiColorFieldAdvanced(Rect rect, GUIContent label, string fieldName, IList sources, bool alphaOnly)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources, null, true)) {
                if (alphaOnly) {
                    Color color = (Color)mScope.fieldInfo.GetValue(sources[0]);
                    color.a = EditorGUI.Slider(rect, label, ((Color)mScope.fieldInfo.GetValue(sources[0])).a, 0, 1);
                    mScope.value = color;
                } else {
                    mScope.value = EditorGUI.ColorField(rect, label, (Color)mScope.fieldInfo.GetValue(sources[0]));
                }
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiCurveField(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.CurveField(rect, label, (AnimationCurve)mScope.fieldInfo.GetValue(sources[0]));
                if (mScope.hasMixedValue) {
                    // Draw over curve field because Unity has no default way to show mixed values on them
                    DrawTiledTexture(rect.Contract(2), DeStylePalette.tileBars_empty, 0.25f, new Color(0.97f, 0.81f, 0.02f, 0.5f));
                }
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values. Supports using an int as an enum</summary>
        public static bool MultiEnumPopup<T>(Rect rect, GUIContent label, string fieldName, IList sources) where T : Enum
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                bool isIntMode = mScope.fieldInfo.FieldType == typeof(int);
                mScope.value = EditorGUI.EnumPopup(rect, label, (T)mScope.fieldInfo.GetValue(sources[0]));
                if (isIntMode) mScope.value = (int)mScope.value;
                return mScope.hasMixedValue;
            }
        }
        /// <summary>Returns TRUE if there's mixed values. Supports using an int as an enum</summary>
        public static bool MultiEnumPopup(Rect rect, GUIContent label, Type enumType, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                bool isIntMode = mScope.fieldInfo.FieldType == typeof(int);
                object parsed = Enum.Parse(enumType, mScope.fieldInfo.GetValue(sources[0]).ToString());
                mScope.value = EditorGUI.EnumPopup(rect, label, parsed as Enum);
                if (isIntMode) mScope.value = (int)mScope.value;
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiFloatField(Rect rect, GUIContent label, string fieldName, IList sources, float? min = null, float? max = null)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                EditorGUI.BeginChangeCheck();
                mScope.value = EditorGUI.FloatField(rect, label, (float)mScope.fieldInfo.GetValue(sources[0]));
                if (EditorGUI.EndChangeCheck()) {
                    if (min != null && (float)mScope.value < (float)min) mScope.value = min;
                    else if (max != null && (float)mScope.value > (float)max) mScope.value = max;
                }
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values. Supports also uint fields</summary>
        public static bool MultiIntField(Rect rect, GUIContent label, string fieldName, IList sources, int? min = null, int? max = null)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                bool isUintMode = mScope.fieldInfo.FieldType == typeof(uint);
                bool isFloatMode = !isUintMode && mScope.fieldInfo.FieldType == typeof(float);
                EditorGUI.BeginChangeCheck();
                mScope.value = EditorGUI.IntField(rect, label,
                    isUintMode
                        ? (int)(uint)mScope.fieldInfo.GetValue(sources[0])
                        : isFloatMode
                            ? (int)(float)mScope.fieldInfo.GetValue(sources[0])
                            : (int)mScope.fieldInfo.GetValue(sources[0])
                );
                if (EditorGUI.EndChangeCheck()) {
                    if (min != null && (int)mScope.value < (int)min) mScope.value = min;
                    else if (max != null && (int)mScope.value > (int)max) mScope.value = max;
                    if (isUintMode) {
                        if ((int)mScope.value < 0) mScope.value = (uint)0;
                        else mScope.value = (uint)(int)mScope.value;
                    } else if (isFloatMode) mScope.value = (float)(int)mScope.value;
                }
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiIntSlider(Rect rect, GUIContent label, string fieldName, IList sources, int min, int max)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.IntSlider(rect, label, (int)mScope.fieldInfo.GetValue(sources[0]), min, max);
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values. Auto-determines object type from the field's type</summary>
        public static bool MultiObjectField(Rect rect, GUIContent label, string fieldName, IList sources, bool allowSceneObjects)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.ObjectField(
                    rect, label, (Object)mScope.fieldInfo.GetValue(sources[0]), mScope.fieldInfo.FieldType, allowSceneObjects
                );
                return mScope.hasMixedValue;
            }
        }
        /// <summary>Returns TRUE if there's mixed values. Forces field to accept only objects of the given type</summary>
        public static bool MultiObjectField(Rect rect, GUIContent label, string fieldName, IList sources, Type objType, bool allowSceneObjects)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.ObjectField(
                    rect, label, (Object)mScope.fieldInfo.GetValue(sources[0]), objType, allowSceneObjects
                );
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiRectField(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.RectField(rect, label, (Rect)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiSlider(Rect rect, GUIContent label, string fieldName, IList sources, float min, float max)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.Slider(rect, label, (float)mScope.fieldInfo.GetValue(sources[0]), min, max);
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiTextArea(Rect rect, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.TextArea(rect, (string)mScope.fieldInfo.GetValue(sources[0]), EditorStyles.textArea);
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiTextField(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.TextField(rect, label, (string)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values. Supports passing int values as bool (1 = true, 0 = false)</summary>
        public static bool MultiToggleButton(Rect rect, GUIContent label, string fieldName, IList sources, GUIStyle guiStyle = null)
        { return MultiToggleButton(rect, null, label, fieldName, sources, null, null, null, null, guiStyle); }
        /// <summary>Returns TRUE if there's mixed values. Supports passing int values as bool (1 = true, 0 = false)</summary>
        public static bool MultiToggleButton(
            Rect rect, GUIContent label, string fieldName, IList sources,
            Color? bgOffColor, Color? bgOnColor = null, Color? contentOffColor = null, Color? contenOnColor = null,  GUIStyle guiStyle = null
        ){ return MultiToggleButton(rect, null, label, fieldName, sources, bgOffColor, bgOnColor, contentOffColor, contenOnColor, guiStyle); }
        /// <summary>Returns TRUE if there's mixed values. Supports passing int values as bool (1 = true, 0 = false)</summary>
        public static bool MultiToggleButton(
            Rect rect, bool? forceSetToggle, GUIContent label, string fieldName, IList sources,
            Color? bgOffColor, Color? bgOnColor = null, Color? contentOffColor = null, Color? contenOnColor = null,  GUIStyle guiStyle = null
        ){
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                bool intMode = mScope.fieldInfo.FieldType == typeof(int);
                mScope.value = ToggleButton(rect,
                    forceSetToggle == null
                        ? intMode
                            ? (int)mScope.fieldInfo.GetValue(sources[0]) == 1
                            : (bool)mScope.fieldInfo.GetValue(sources[0])
                        : (bool)forceSetToggle,
                    mScope.hasMixedValue ? MixedValueLabel : label,
                    bgOffColor == null ? (Color)colors.bg.toggleOff : (Color)bgOffColor,
                    bgOnColor == null ? (Color)colors.bg.toggleOn : (Color)bgOnColor,
                    contentOffColor == null ? (Color)colors.content.toggleOff : (Color)contentOffColor,
                    contenOnColor == null ? (Color)colors.content.toggleOn : (Color)contenOnColor,
                    guiStyle
                );
                if (intMode) mScope.value = (bool)mScope.value ? 1 : 0;
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values. Requires a SerializedProperty representation of each UnityEven field</summary>
        public static bool MultiUnityEvent(
            Rect rect, GUIContent label, string fieldName, IList sources, List<SerializedProperty> fieldsAsSerializedProperties
        ){
            using (var mScope = new MultiPropertyScope(fieldName, sources, fieldsAsSerializedProperties)) {
                EditorGUI.BeginDisabledGroup(mScope.hasMixedValue); // Unity events can't be set when multiple
                if (label.HasText()) EditorGUI.PropertyField(rect, fieldsAsSerializedProperties[0], label);
                else EditorGUI.PropertyField(rect, fieldsAsSerializedProperties[0]);
                EditorGUI.EndDisabledGroup();
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector2Field(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.Vector2Field(rect, label, (Vector2)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector3Field(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.Vector3Field(rect, label, (Vector3)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector4Field(Rect rect, GUIContent label, string fieldName, IList sources)
        {
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = EditorGUI.Vector4Field(rect, label.text, (Vector4)mScope.fieldInfo.GetValue(sources[0]));
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector2FieldAdvanced(
            Rect rect, GUIContent label, string fieldName, IList sources, bool xEnabled, bool yEnabled, bool lockAllToX, bool lockAllToY
        ){
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = Vector2FieldAdvanced(
                    rect, label, (Vector2)mScope.fieldInfo.GetValue(sources[0]), xEnabled, yEnabled, lockAllToX, lockAllToY
                );
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector3FieldAdvanced(
            Rect rect, GUIContent label, string fieldName, IList sources, bool xEnabled, bool yEnabled, bool zEnabled,
            bool lockAllToX, bool lockAllToY, bool lockAllToZ
        ){
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = Vector3FieldAdvanced(
                    rect, label, (Vector3)mScope.fieldInfo.GetValue(sources[0]), xEnabled, yEnabled, zEnabled, lockAllToX, lockAllToY, lockAllToZ
                );
                return mScope.hasMixedValue;
            }
        }

        /// <summary>Returns TRUE if there's mixed values</summary>
        public static bool MultiVector4FieldAdvanced(
            Rect rect, GUIContent label, string fieldName, IList sources, bool xEnabled, bool yEnabled, bool zEnabled, bool wEnabled,
            bool lockAllToX, bool lockAllToY, bool lockAllToZ, bool lockAllToW
        ){
            using (var mScope = new MultiPropertyScope(fieldName, sources)) {
                mScope.value = Vector4FieldAdvanced(
                    rect, label, (Vector4)mScope.fieldInfo.GetValue(sources[0]), xEnabled, yEnabled, zEnabled, wEnabled,
                    lockAllToX, lockAllToY, lockAllToZ, lockAllToW
                );
                return mScope.hasMixedValue;
            }
        }

        #endregion

        #endregion
    }
}