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
            GetWindow(typeof(DeHierarchyCustomizePanel), false, _Title);
            // win.position = new Rect(win.position.x, win.position.y, _src.winSize.x, _src.winSize.y);
            // win.maxSize = win.minSize = new Vector2(300, 400);
        }
		
        const string _Title = "DeHierarchy";
        static DeHierarchyData _src;
        static DeHierarchyComponent.HColor[] _hColors; // Stored as sorted colors
        static DeHierarchyComponent.SeparatorType[] _separatorTypes;
        static DeHierarchyComponent.IcoType[] _icoTypes;
        static Texture2D[] _icoTextures;
        static DeScrollView _scrollView;

        #region Unity and GUI Methods

        void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;
            // _hColors = (DeHierarchyComponent.HColor[])Enum.GetValues(typeof(DeHierarchyComponent.HColor));
            _hColors = new[] {
                DeHierarchyComponent.HColor.None,
                DeHierarchyComponent.HColor.Red, DeHierarchyComponent.HColor.Yellow, DeHierarchyComponent.HColor.Orange,
                DeHierarchyComponent.HColor.Green, DeHierarchyComponent.HColor.Blue, DeHierarchyComponent.HColor.Purple, DeHierarchyComponent.HColor.Pink,
                DeHierarchyComponent.HColor.BrightGrey, DeHierarchyComponent.HColor.DarkGrey, DeHierarchyComponent.HColor.Black, DeHierarchyComponent.HColor.White
            };
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
            if (Event.current.type == EventType.Layout) return;

            ConnectToData(true);
            DeGUI.BeginGUI();
            Styles.Init();

            Rect area = position.ResetXY();
            Rect scrollViewArea = area;

            if (_scrollView.fullContentArea.height > area.height) area = area.Shift(0, 0, -11, 0);
            _scrollView = DeGUI.BeginScrollView(scrollViewArea, _scrollView);
			
            Rect labelR = new Rect(2, 0, area.width, EditorGUIUtility.singleLineHeight);
            Rect icosR = labelR.Shift(0, labelR.height + 4, 0, 0).SetHeight(24);
            Rect colorsR = icosR.Shift(0, icosR.height + 4, 0, 0);
            Rect labelSeparators = colorsR.Shift(0, colorsR.height + 4, 0, 0).SetHeight(EditorGUIUtility.singleLineHeight);
            Rect separatorTypeR = labelSeparators.Shift(0, labelSeparators.height + 4, 0, 0).SetHeight(EditorGUIUtility.singleLineHeight);
            Rect separatorColorsR = separatorTypeR.Shift(0, separatorTypeR.height + 4, 0, 0).SetHeight(24);

            _scrollView.SetContentWidth(_hColors.Length * (colorsR.height + 1) + 3);

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

            // Extra evidences (preferences based)
            using (var check = new EditorGUI.ChangeCheckScope()) {
                Undo.RecordObject(_src, _Title);
                //
                Rect specialEvidencesArea = area.SetY(separatorColorsR.yMax + 5).SetHeight(0);
                _scrollView.SetContentHeight(specialEvidencesArea.yMax);
                // ► Toolbar
                Rect toolbarR = _scrollView.GetWideSingleLineRect(22f);
                using (new DeGUI.ColorScope(null, null, new DeSkinColor(0.9f, 0.1f))) {
                    GUI.DrawTexture(toolbarR, DeStylePalette.whiteSquare);
                }
                Rect toolbarContentR = toolbarR.Shift(0, 2, 0, -4);
                Rect btAddEvidenceR = toolbarContentR.HangToRightAndResize(toolbarR.xMax - toolbarR.height).Shift(-2, 0, 0, 0);
                GUI.Label(toolbarContentR.ShiftXAndResize(4).ShiftYAndResize(1), "Evidence By Component", DeGUI.styles.label.toolbar);
                if (GUI.Button(btAddEvidenceR, "+", DeGUI.styles.button.tool)) {
                    DeEditorUtils.Array.ExpandAndAdd(ref _src.extraEvidences, new DeHierarchyData.ExtraEvidenceData());
                    _src.totExtraEvidences = _src.extraEvidences.Length;
                    GUI.changed = true;
                }
                // ► Items
                if (_src.extraEvidences != null) {
                    for (int i = 0; i < _src.extraEvidences.Length; ++i) {
                        DeHierarchyData.ExtraEvidenceData item = _src.extraEvidences[i];
                        Rect itemR = _scrollView.GetWideSingleLineRect(18f);
                        Rect btDragR = itemR.SetWidth(itemR.height).Shift(0, 1, 0, 0);
                        Rect btDeleteR = itemR.HangToRightAndResize(itemR.xMax - itemR.height).Shift(-2, 0, 0, 0);
                        Rect tfR = new Rect(btDragR.xMax, itemR.y, btDeleteR.x - btDragR.xMax, itemR.height);
                        Rect modeR = tfR.HangToRightAndResize(tfR.width - 38);
                        Rect evColorR = modeR.Shift(-50, 1, 0, -2).SetWidth(50);
                        tfR.width -= evColorR.width + modeR.width;
                        GUI.Box(itemR, GUIContent.none, DeGUI.styles.box.sticky);
                        if (DeGUI.PressButton(btDragR, "≡", DeGUI.styles.button.tool)) {
                            DeGUIDrag.StartDrag(this, _src.extraEvidences, i);
                        }
                        item.componentClass = EditorGUI.DelayedTextField(tfR, item.componentClass);
                        item.evidenceMode = (DeHierarchyData.EvidenceMode)EditorGUI.EnumPopup(modeR, item.evidenceMode, EditorStyles.toolbarPopup);
                        item.color = EditorGUI.ColorField(evColorR, item.color);
                        if (DeGUI.ActiveButton(btDeleteR, new GUIContent("×"), DeGUI.styles.button.tool)) {
                            DeEditorUtils.Array.RemoveAtIndexAndContract(ref _src.extraEvidences, i);
                            _src.totExtraEvidences = _src.extraEvidences.Length;
                            GUI.changed = true;
                            i--;
                        }
                        if (DeGUIDrag.Drag(_src.extraEvidences, i, itemR).outcome == DeDragResultType.Accepted) GUI.changed = true;
                    }
                }
                if (check.changed) {
                    DeHierarchy.Refresh();
                    EditorApplication.RepaintHierarchyWindow();
                    EditorUtility.SetDirty(_src);
                }
            }

            DeGUI.EndScrollView();

        }

        #endregion

        #region Methods

        static void ConnectToData(bool createIfMissing)
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<DeHierarchyData>(DeHierarchy.ADBDataPath, createIfMissing, false);
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