// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:30
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiEditor.Internal;
using DG.DemiLib;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    /// <summary>
    /// Relies on <see cref="DeHierarchyComponent"/> in Core library.
    /// </summary>
    [InitializeOnLoad]
    public class DeHierarchy
    {
        static DeHierarchyComponent _dehComponent;

        static Color _icoVisibilityOnColor, _icoVisibilityOffColor;
        static GUIStyle _evidenceStyle, _btVisibility, _btVisibilityOff;

        static DeHierarchy()
        {
            // Delay initialization so GUI is linked and drawn after other assets that modify the Hierarchy (like DeHierarchy)
            EditorApplication.delayCall += Init;
        }

        static void Init()
        {
            EditorApplication.delayCall -= Init;
            EditorApplication.hierarchyWindowChanged -= Refresh;
            EditorApplication.hierarchyWindowItemOnGUI -= ItemOnGUI;
            Undo.undoRedoPerformed -= UndoRedoPerformed;
            EditorApplication.hierarchyWindowChanged += Refresh;
            EditorApplication.hierarchyWindowItemOnGUI += ItemOnGUI;
            Undo.undoRedoPerformed += UndoRedoPerformed;
            Refresh();
        }

        #region GUI

        static void Refresh()
        {
            ConnectToDeHierarchyComponent(false, true);
            if (_dehComponent == null) return;

            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            // Delete customizedItems that refer to objects not in the scene anymore
            // Doesn't call Undo.RecordObject before the operation, because at this point it would mark the scene dirty even if no change is made
            List<int> missingIndexes = _dehComponent.MissingItemsIndexes();
            if (missingIndexes != null) {
                Undo.RecordObject(_dehComponent, "DeHierarchy");
                foreach (int missingIndex in missingIndexes) {
                    _dehComponent.customizedItems.RemoveAt(missingIndex);
                    EditorUtility.SetDirty(_dehComponent);
                }
            }
        }

        static void UndoRedoPerformed()
        {
            Refresh();
            EditorApplication.RepaintHierarchyWindow();
        }

        static void ItemOnGUI(int instanceID, Rect selectionRect)
        {
            bool hasDehComponent = _dehComponent != null;
            bool showVisibilityButton = DeEditorToolsPrefs.deHierarchy_showVisibilityButton;
            if (showVisibilityButton) {
                if (Event.current.type == EventType.Layout) return;
            } else if (!hasDehComponent || Event.current.type != EventType.Repaint) return;

            DeGUI.BeginGUI();
            SetStyles();

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            // Visibility button
            if (showVisibilityButton) {
                bool isActive = go.activeSelf;
                bool isActiveInHierarchy = go.activeInHierarchy;
                Texture2D ico = DeStylePalette.ico_visibility;
                Rect visibilityR = new Rect(selectionRect.xMax - ico.width - 2, (int)(selectionRect.center.y - (ico.height * 0.5f)), ico.width, ico.height);
                using (new DeGUI.ColorScope(null, null, isActiveInHierarchy ? _icoVisibilityOnColor : _icoVisibilityOffColor)) {
                    if (GUI.Button(visibilityR, "", isActive ? _btVisibility : _btVisibilityOff)) {
                        Undo.RecordObject(go, go.name + " Visibility");
                        go.SetActive(!isActive);
                        EditorUtility.SetDirty(go);
                    }
                }
            }

            if (!hasDehComponent) return;

            DeHierarchyComponent.CustomizedItem customizedItem = _dehComponent.GetItem(go);
            if (customizedItem == null) return;

            // Color
            Color color = customizedItem.GetColor();
            if (DeEditorToolsPrefs.deHierarchy_fadeEvidenceWhenInactive && !go.activeInHierarchy) color.a = 0.4f;
            // Icon
            if (DeEditorToolsPrefs.deHierarchy_showIco) {
                const int icoOffset = -30; // was -28
                const int icoSize = 28; // was 28
                const int icoSizeHalf = 14; // was 14
                Rect fullR = selectionRect;
                fullR.x += icoOffset;
                fullR.width += icoSize;
                if (!DeEditorToolsPrefs.deHierarchy_indentIco) {
                    // Full rect not indented
                    Transform t = go.transform;
                    while (t.parent != null) {
                        t = t.parent;
                        fullR.x -= icoSizeHalf;
                        fullR.width += icoSizeHalf;
                    }
                }
                Texture2D icoTexture;
                switch (customizedItem.icoType) {
                case DeHierarchyComponent.IcoType.Star:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_star_border : DeStylePalette.ico_star;
                    break;
                case DeHierarchyComponent.IcoType.Cog:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_cog_border : DeStylePalette.ico_cog;
                    break;
                case DeHierarchyComponent.IcoType.Comment:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_comment_border : DeStylePalette.ico_comment;
                    break;
                case DeHierarchyComponent.IcoType.UI:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_ui_border : DeStylePalette.ico_ui;
                    break;
                case DeHierarchyComponent.IcoType.Play:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_play_border : DeStylePalette.ico_play;
                    break;
                case DeHierarchyComponent.IcoType.Heart:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_heart_border : DeStylePalette.ico_heart;
                    break;
                case DeHierarchyComponent.IcoType.Skull:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_skull_border : DeStylePalette.ico_skull;
                    break;
                case DeHierarchyComponent.IcoType.Camera:
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.ico_camera_border : DeStylePalette.ico_camera;
                    break;
                default: // Dot
                    icoTexture = DeEditorToolsPrefs.deHierarchy_showIcoBorder ? DeStylePalette.whiteDot_darkBorder : DeStylePalette.whiteDot;
                    break;
                }
                Rect icoR = new Rect(fullR.x + 9 - icoTexture.width * 0.5f, fullR.y + 9 - icoTexture.height * 0.5f, icoTexture.width, icoTexture.height);
                using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(icoR, icoTexture);
            }
            // Border
            if (DeEditorToolsPrefs.deHierarchy_showBorder) {
                using (new DeGUI.ColorScope(color)) {
                    GUI.Label(selectionRect.Shift(-3, 1, 4, -1), "", _evidenceStyle);
                }
            }
            // Separator
            if (customizedItem.separatorType != DeHierarchyComponent.SeparatorType.None) {
                color = customizedItem.GetSeparatorColor();
                bool thick = DeEditorToolsPrefs.deHierarchy_thickSeparators;
                const int offsetToLBorder = -30; // Offset to reach left border
                const int extraSize = 60;
                Rect separatorR;
                switch (customizedItem.separatorType) {
                case DeHierarchyComponent.SeparatorType.Top:
                    separatorR = new Rect(selectionRect.x, selectionRect.y - (thick ? 1 : 0), selectionRect.width, (thick ? 3 : 1));
                    break;
                default:
                    separatorR = new Rect(selectionRect.x, selectionRect.y + selectionRect.height - 1, selectionRect.width, (thick ? 3 : 1));
                    break;
                }
                separatorR = separatorR.Shift(offsetToLBorder, 0, extraSize, 0);
                DeGUI.DrawColoredSquare(separatorR, color);
            }
        }

        static void SetStyles()
        {
            if (_evidenceStyle != null) return;

            _icoVisibilityOnColor = new DeSkinColor(DeGUI.IsProSkin ? 0.65f : 0.5f);
            _icoVisibilityOffColor = new DeSkinColor(DeGUI.IsProSkin ? 0.4f : 0.6f);

            _evidenceStyle = DeGUI.styles.button.bBlankBorder.Clone(TextAnchor.MiddleLeft).Background(DeStylePalette.squareBorderCurvedEmpty)
                .PaddingLeft(3).PaddingTop(2);
            _btVisibility = DeGUI.styles.button.flatWhite.Clone().Margin(0).Padding(0).Background(DeStylePalette.ico_visibility)
                .Width(DeStylePalette.ico_visibility.width).Height(DeStylePalette.ico_visibility.height);
            _btVisibilityOff = _btVisibility.Clone().Background(DeStylePalette.ico_visibility_off);
        }

        #endregion

        #region Public Methods

        public static void OnPreferencesRefresh(bool flagsChanged)
        {
            if (flagsChanged) {
                if (_dehComponent != null) SetDeHierarchyGOFlags(_dehComponent.gameObject);
                EditorApplication.DirtyHierarchyWindowSorting();
            } else EditorApplication.RepaintHierarchyWindow();
        }

        public static void SetColorFor(GameObject go, DeHierarchyComponent.HColor hColor)
        {
            Object[] currSelectedObjs = Selection.objects;
            Selection.activeTransform = go.transform;
            SetColorForSelections(hColor);
            Selection.objects = currSelectedObjs;
        }

        public static void SetIconFor(GameObject go, DeHierarchyComponent.IcoType icoType)
        {
            Object[] currSelectedObjs = Selection.objects;
            Selection.activeTransform = go.transform;
            SetIconForSelections(icoType);
            Selection.objects = currSelectedObjs;
        }

        #endregion

        #region Internal Methods

        // Assumes at least one object is selected
        internal static void ResetSelections()
        {
            ConnectToDeHierarchyComponent(false);
            if (_dehComponent == null) return;

            Undo.RecordObject(_dehComponent, "DeHierarchy");
            bool changed = false;
            foreach (GameObject go in Selection.gameObjects) {
                changed = _dehComponent.RemoveItemData(go) || changed;
            }
            if (_dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(_dehComponent.gameObject);
                _dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(_dehComponent);
        }

        // Assumes at least one object is selected
        internal static void ResetSeparatorsForSelections()
        {
            ConnectToDeHierarchyComponent(false);
            if (_dehComponent == null) return;

            Undo.RecordObject(_dehComponent, "DeHierarchy");
            bool changed = false;
            foreach (GameObject go in Selection.gameObjects) {
                changed = _dehComponent.ResetSeparator(go) || changed;
            }
            if (_dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(_dehComponent.gameObject);
                _dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(_dehComponent);
        }

        // Assumes at least one object is selected
        // Assumes it's never called with HColor.None
        internal static void SetColorForSelections(DeHierarchyComponent.HColor hColor)
        {
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(_dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                _dehComponent.StoreItemColor(go, hColor);
            }
        }

        // Assumes at least one object is selected
        internal static void SetIconForSelections(DeHierarchyComponent.IcoType icoType)
        {
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(_dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                _dehComponent.StoreItemIcon(go, icoType);
            }
        }

        // Assumes at least one object is selected
        // Assumes it's never called with HColor.None
        internal static void SetSeparatorForSelections(
            DeHierarchyComponent.SeparatorType separatorType, DeHierarchyComponent.HColor separatorHColor = DeHierarchyComponent.HColor.None
        ){
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(_dehComponent, "DeHierarchy");
            foreach (GameObject go in Selection.gameObjects) {
                _dehComponent.StoreItemSeparator(go, separatorType, separatorHColor);
            }
        }

        #endregion

        #region Methods

        static void ConnectToDeHierarchyComponent(bool createIfMissing, bool forceRefresh = false)
        {
            if (_dehComponent != null && !forceRefresh) return;

            _dehComponent = DeEditorToolsUtils.FindFirstComponentOfType<DeHierarchyComponent>();
            if (_dehComponent != null || !createIfMissing) {
                if (_dehComponent != null) SetDeHierarchyGOFlags(_dehComponent.gameObject);
                return;
            }

            GameObject go = new GameObject(":: DeHierarchy ::");
            SetDeHierarchyGOFlags(go);
            _dehComponent = go.AddComponent<DeHierarchyComponent>();
        }

        static void SetDeHierarchyGOFlags(GameObject deHierarchyGO)
        {
            if (DeEditorToolsPrefs.deHierarchy_hideObject) {
                if ((deHierarchyGO.hideFlags & (HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy)) == (HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy)) return;
                deHierarchyGO.hideFlags = HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy;
            } else {
                if ((deHierarchyGO.hideFlags | (HideFlags.DontSaveInBuild)) == (HideFlags.DontSaveInBuild)) return;
                deHierarchyGO.hideFlags = HideFlags.DontSaveInBuild;
            }
        }

        #endregion
    }
}