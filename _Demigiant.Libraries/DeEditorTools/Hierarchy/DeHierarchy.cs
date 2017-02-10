// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:30
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
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

        static GUIStyle _evidenceStyle, _evidencePrefixStyle;

        static DeHierarchy()
        {
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
            if (Event.current.type != EventType.Repaint) return;
            if (_dehComponent == null) return;

            DeGUI.BeginGUI();
            SetStyles();

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            DeHierarchyComponent.CustomizedItem customizedItem = _dehComponent.GetItem(go);
            if (customizedItem == null) return;

            Color color = customizedItem.GetColor();
            if (DeEditorToolsPrefs.deHierarchy_fadeEvidenceWhenInactive && !go.activeInHierarchy) color.a = 0.4f;
            // Icon
            if (DeEditorToolsPrefs.deHierarchy_showIco) {
                Rect fullR = selectionRect;
                fullR.x -= 28;
                fullR.width += 28;
                if (!DeEditorToolsPrefs.deHierarchy_indentIco) {
                    // Full rect not indented
                    Transform t = go.transform;
                    while (t.parent != null) {
                        t = t.parent;
                        fullR.x -= 14;
                        fullR.width += 14;
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
                    GUI.Label(selectionRect, "", _evidenceStyle);
                }
            }
        }

        static void SetStyles()
        {
            if (_evidenceStyle != null) return;

            _evidenceStyle = DeGUI.styles.button.bBlankBorder.Clone(TextAnchor.MiddleLeft).Background(DeStylePalette.squareBorderCurvedEmpty)
                .PaddingLeft(3).PaddingTop(2);
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

        #endregion

        #region Internal Methods

        // Assumes at least one object is selected
        internal static void SetColorForSelections(DeHierarchyComponent.HColor hColor)
        {
            ConnectToDeHierarchyComponent(true);
            Undo.RecordObject(_dehComponent, "DeHierarchy");
            bool changed = false;
            foreach (GameObject go in Selection.gameObjects) {
                if (hColor == DeHierarchyComponent.HColor.None) {
                    changed = _dehComponent.RemoveItemData(go) || changed;
                } else {
                    changed = true;
                    _dehComponent.StoreItemColor(go, hColor);
                }
            }
            if (_dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(_dehComponent.gameObject);
                _dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(_dehComponent);
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