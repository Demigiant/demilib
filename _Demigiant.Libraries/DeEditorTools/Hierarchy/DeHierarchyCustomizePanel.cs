// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/01/15

using System;
using System.Reflection;
using DG.DemiEditor;
using DG.DemiLib;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    class DeHierarchyCustomizePanel : EditorWindow
    {
        public static void ShowWindow()
        {
            EditorWindow win = GetWindow(typeof(DeHierarchyCustomizePanel), false, _Title);
            win.maxSize = win.minSize = new Vector2(300, 150);
        }
		
        const string _Title = "DeHierarchy";
        static DeHierarchyComponent.HColor[] _hColors;
        static DeHierarchyComponent.SeparatorType[] _separatorTypes;
        static DeHierarchyComponent.IcoType[] _icoTypes;
        static Texture2D[] _icoTextures;

        #region Unity and GUI Methods

        void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;
            _hColors = (DeHierarchyComponent.HColor[])Enum.GetValues(typeof(DeHierarchyComponent.HColor));
            _separatorTypes = (DeHierarchyComponent.SeparatorType[])Enum.GetValues(typeof(DeHierarchyComponent.SeparatorType));
            _icoTypes = (DeHierarchyComponent.IcoType[])Enum.GetValues(typeof(DeHierarchyComponent.IcoType));
            _icoTextures = new Texture2D[_icoTypes.Length];
            for (int i = 0; i < _icoTypes.Length; ++i) {
                DeHierarchyComponent.IcoType icoType = _icoTypes[i];
                switch (icoType) {
                case DeHierarchyComponent.IcoType.Dot:
                    _icoTextures[i] = DeStylePalette.whiteDot;
                    break;
                default:
                    _icoTextures[i] = (Texture2D)typeof(DeStylePalette)
                        .GetProperty("ico_" + icoType.ToString().ToLowerInvariant(), BindingFlags.Static | BindingFlags.Public)
                        .GetValue(null, null);
                    break;
                }
            }
            Selection.selectionChanged += Repaint;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
            Selection.selectionChanged -= Repaint;
        }

        void OnHierarchyChange()
        {
            Repaint();
        }

        void OnGUI()
        {
            DeGUI.BeginGUI();
            Styles.Init();
			
            Rect labelR = new Rect(2, 0, position.width, EditorGUIUtility.singleLineHeight);
            Rect icosR = labelR.Shift(0, labelR.height + 4, 0, 0).SetHeight(24);
            Rect colorsR = icosR.Shift(0, icosR.height + 4, 0, 0);
            Rect labelSeparators = colorsR.Shift(0, colorsR.height + 4, 0, 0).SetHeight(EditorGUIUtility.singleLineHeight);
            Rect separatorTypeR = labelSeparators.Shift(0, labelSeparators.height + 4, 0, 0).SetHeight(EditorGUIUtility.singleLineHeight);
            Rect separatorColorsR = separatorTypeR.Shift(0, separatorTypeR.height + 4, 0, 0).SetHeight(24);

            GUI.Label(labelR, string.Format("{0} gameObjects selected", Selection.gameObjects.Length));
            // Icons
            Rect icoR = icosR.SetWidth(icosR.height);
            using (new DeGUI.ColorScope(new DeSkinColor(0.5f, 0.4f))) {
                for (int i = -1; i < _icoTextures.Length; ++i) {
                    if (i == -1) {
                        if (GUI.Button(icoR, DeStylePalette.ico_delete, Styles.bt)) {
                            DeHierarchy.ResetSelections();
                        }
                    } else {
                        if (GUI.Button(icoR, _icoTextures[i], Styles.bt)) DeHierarchy.SetIconForSelections(_icoTypes[i]);
                    }
                    icoR = icoR.Shift(icoR.width + 1, 0, 0, 0);
                }
            }
            // Colors
            Rect colorR = colorsR.SetWidth(colorsR.height);
            for (int i = 0; i < _hColors.Length; ++i) {
                bool noColor = _hColors[i] == DeHierarchyComponent.HColor.None;
                using (new DeGUI.ColorScope(null, null, noColor ? new DeSkinColor(0.5f, 0.4f) : new DeSkinColor(1)))
                using (new DeGUI.ColorScope(DeHierarchyComponent.GetColor(_hColors[i]))) {
                    if (GUI.Button(colorR, "", noColor ? Styles.btNoColor : Styles.btColor)) DeHierarchy.SetColorForSelections(_hColors[i]);
                }
                colorR = colorR.Shift(colorR.width + 1, 0, 0, 0);
            }
            // Separator
            GUI.Label(labelSeparators, "Separators");
            Rect separatorR = separatorTypeR.SetWidth(80);
            for (int i = 0; i < _separatorTypes.Length; ++i) {
                if (GUI.Button(separatorR, _separatorTypes[i].ToString(), Styles.bt)) {
                    DeHierarchy.SetSeparatorForSelections(_separatorTypes[i]);
                }
                separatorR = separatorR.Shift(separatorR.width + 1, 0, 0, 0);
            }
            Rect separatorColorR = separatorColorsR.SetWidth(separatorColorsR.height);
            for (int i = 0; i < _hColors.Length; ++i) {
                bool noColor = _hColors[i] == DeHierarchyComponent.HColor.None;
                using (new DeGUI.ColorScope(null, null, noColor ? new DeSkinColor(0.5f, 0.4f) : new DeSkinColor(1)))
                using (new DeGUI.ColorScope(DeHierarchyComponent.GetColor(_hColors[i]))) {
                    if (GUI.Button(separatorColorR, "", noColor ? Styles.btNoColor : Styles.btColor)) {
                        DeHierarchy.SetSeparatorForSelections(null, _hColors[i]);
                    }
                }
                separatorColorR = separatorColorR.Shift(separatorColorR.width + 1, 0, 0, 0);
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;
            public static GUIStyle bt;
            public static GUIStyle btColor;
            public static GUIStyle btNoColor;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                bt = DeGUI.styles.button.bBlankBorder.Clone().Background(DeStylePalette.squareBorderCurvedEmpty);
                btColor = DeGUI.styles.button.flatWhite.Clone();
                btNoColor = DeGUI.styles.button.flatWhite.Clone().Background(DeStylePalette.squareCornersEmpty02);
            }
        }
    }
}