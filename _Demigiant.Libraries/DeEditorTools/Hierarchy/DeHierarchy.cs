// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:30
// License Copyright (c) Daniele Giardini

using DG.DemiEditor;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
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

            // Delete customizedItems that refer to objects not in the scene anymore
            Undo.RecordObject(_dehComponent, "DeHierarchy");
            if (_dehComponent.RemoveMissingItems()) EditorUtility.SetDirty(_dehComponent);
        }

        static void UndoRedoPerformed()
        {
            Refresh();
            EditorApplication.RepaintHierarchyWindow();
        }

        static void ItemOnGUI(int instanceID, Rect selectionRect)
        {
            if (_dehComponent == null) return;

            DeGUI.BeginGUI();
            SetStyles();

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            DeHierarchyComponent.CustomizedItem customizedItem = _dehComponent.GetItem(go);
            if (customizedItem == null) return;

            Color color = customizedItem.GetColor();
            // Dot
            if (DeEditorToolsPrefs.deHierarchy_showDot) {
                Rect fullR = selectionRect;
                fullR.x -= 28;
                fullR.width += 28;
                Rect indentendFullR = fullR;
                Transform t = go.transform;
                while (t.parent != null) {
                    t = t.parent;
                    fullR.x -= 14;
                    fullR.width += 14;
                }
                Rect dotR = DeEditorToolsPrefs.deHierarchy_indentDot
                    ? new Rect(indentendFullR.x + 5, indentendFullR.y + 4, 8, 8)
                    : new Rect(fullR.x + 5, fullR.y + 4, 8, 8);
                using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(dotR, DeStylePalette.whiteDot);
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
                    _dehComponent.StoreItemData(go, hColor);
                }
            }
            if (_dehComponent.customizedItems.Count == 0) {
                Undo.DestroyObjectImmediate(_dehComponent.gameObject);
                _dehComponent = null;
            } else if (changed) EditorUtility.SetDirty(_dehComponent);
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
            deHierarchyGO.hideFlags = DeEditorToolsPrefs.deHierarchy_hideObject
                ? HideFlags.DontSaveInBuild | HideFlags.HideInHierarchy
                : HideFlags.DontSaveInBuild;
        }

        #endregion
    }
}