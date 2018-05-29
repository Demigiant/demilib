// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 17:05
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using System.IO;
using DG.DemiEditor;
using DG.DemiEditor.Internal;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    /// <summary>
    /// Customizes Project panel
    /// </summary>
    [InitializeOnLoad]
    public class DeProject
    {
        public const string ADBDataPath = "Assets/-DeProjectData.asset";

        static DeProjectData _src;

        static DeProject()
        {
            EditorApplication.projectWindowItemOnGUI -= ItemOnGUI;
            EditorApplication.projectWindowItemOnGUI += ItemOnGUI;
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        static void OnUndoRedo()
        {
            EditorApplication.RepaintProjectWindow();
        }

        #region GUI

        static void ItemOnGUI(string guid, Rect selectionRect)
        {
            if (Event.current.type != EventType.Repaint) return;

//            Debug.Log(selectionRect + " > " + AssetDatabase.GUIDToAssetPath(guid));
            if (_src == null) _src = ConnectToData(false);
            if (_src == null) return;

            DeGUI.BeginGUI();
            bool isLeftSide = selectionRect.x >= 15;
            bool isFirstItemOrAssets = selectionRect.x < 20;

            DeProjectData.CustomizedItem customizedItem = _src.GetItem(guid);
            if (customizedItem != null) {

                Texture2D icoFolder = DeStylePalette.proj_folder;
                Rect folderR = new Rect(selectionRect.x, selectionRect.y + 2, icoFolder.width, icoFolder.height);
                // Color
                if (customizedItem.hasColor) {
                    Color color = customizedItem.GetColor();
                    if (!isLeftSide) folderR.x = folderR.x + 3;
                    using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(folderR, icoFolder);
                }
                // Icon
                Texture2D icoTexture = customizedItem.GetIcon();
                if (icoTexture != null) {
                    bool isCustom = customizedItem.icoType == DeProjectData.IcoType.Custom;
                    Rect icoR = new Rect(0, 0, icoTexture.width, icoTexture.height);
                    if (isCustom) icoR = icoR.Fit(customizedItem.customIconMaxSize, customizedItem.customIconMaxSize);
                    Vector2 offset = customizedItem.GetIconOffset();
                    icoR.x = (int)(folderR.xMax - icoR.width + offset.x);
                    icoR.y = (int)(folderR.yMax - icoR.height + offset.y);
                    using (new DeGUI.ColorScope(null, null, customizedItem.GetIconColor())) GUI.DrawTexture(icoR, icoTexture);
                }
            }

            // Evidence
            if (isLeftSide && _src.evidenceType != DeProjectData.EvidenceType.None && !isFirstItemOrAssets) {
                const float columnStep = 14;
                const float minColumnX = 30;
                Rect r = new Rect(selectionRect.x - 14, selectionRect.y, 1, selectionRect.height);
                Color color = new Color(0.4078432f, 0.4078432f, 0.4078432f, 1f);
                bool firstElementDone = false;
                while (r.x > 20) {
                    float colorDepth = columnStep / (r.x - minColumnX + columnStep);
                    if (colorDepth < 0.1f) colorDepth = 0.1f;
                    color = color.SetAlpha(colorDepth);
                    Rect drawR = new Rect(r.x - 8, r.y - 7, r.width, r.height);
                    DeGUI.DrawColoredSquare(drawR, color); // vertical
                    drawR = new Rect(drawR.x + drawR.width, drawR.y + drawR.height - 1, 14 - drawR.width, 1);
                    if (firstElementDone) drawR.width = 2;
                    DeGUI.DrawColoredSquare(drawR, color);
                    firstElementDone = true;
                    r.x -= columnStep;
                }
            }
        }

        #endregion

        #region Internal Methods

        internal static void ResetSelections()
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < Selection.assetGUIDs.Length; ++i) {
                changed = _src.RemoveItemData(Selection.assetGUIDs[i]) || changed;
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        internal static bool CanCopyDataFromSelection()
        {
            if (Selection.assetGUIDs.Length == 0) return false;
            string guid = Selection.assetGUIDs[0];
            if (!IsProjectFolder(guid)) return false;
            _src = ConnectToData(true);
            return _src.GetItem(guid) != null;
        }

        internal static void CopyDataFromSelection()
        {
            if (Selection.assetGUIDs.Length == 0) return;
            string guid = Selection.assetGUIDs[0];
            if (!IsProjectFolder(guid)) return;
            _src = ConnectToData(true);
            DeProjectData.CustomizedItem item = _src.GetItem(guid);
            if (item != null) DeProjectClipboard.storedItem = item.Clone();
        }

        internal static void PasteDataToSelections()
        {
            if (!DeProjectClipboard.hasStoreData) return;
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < Selection.assetGUIDs.Length; ++i) {
                string guid = Selection.assetGUIDs[i];
                if (!IsProjectFolder(guid)) continue;
                changed = true;
                DeProjectData.CustomizedItem item = _src.StoreItem(guid);
                item.Paste(DeProjectClipboard.storedItem);
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        // Assumes at least one object is selected
        internal static void CustomizeSelections()
        {
            _src = ConnectToData(true);
            DeProjectCustomizeItemWindow.Items.Clear();
            for (int i = 0; i < Selection.assetGUIDs.Length; ++i) {
                string guid = Selection.assetGUIDs[i];
                if (!IsProjectFolder(guid)) continue;
                DeProjectCustomizeItemWindow.Items.Add(_src.StoreItem(guid));
            }
            if (DeProjectCustomizeItemWindow.Items.Count > 0) DeProjectCustomizeItemWindow.Open(_src);
        }

        // Assumes at least one object is selected
        internal static void SetColorForSelections(DeProjectData.HColor hColor)
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < Selection.assetGUIDs.Length; ++i) {
                string guid = Selection.assetGUIDs[i];
                if (!IsProjectFolder(guid)) continue;
                if (hColor == DeProjectData.HColor.None) {
                    DeProjectData.CustomizedItem item = _src.GetItem(guid);
                    if (item != null) {
                        if (!item.hasIcon) changed = _src.RemoveItemData(guid) || changed; // Remove reference since it has no customization
                        else {
                            changed = true;
                            _src.StoreItemColor(guid, hColor);
                        }
                    }
                } else {
                    changed = true;
                    _src.StoreItemColor(guid, hColor);
                }
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        // Assumes at least one object is selected
        internal static void SetIconForSelections(DeProjectData.IcoType icoType)
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < Selection.assetGUIDs.Length; ++i) {
                string guid = Selection.assetGUIDs[i];
                if (!IsProjectFolder(guid)) continue;
                if (icoType == DeProjectData.IcoType.None) {
                    DeProjectData.CustomizedItem item = _src.GetItem(guid);
                    if (item != null) {
                        if (!item.hasColor) changed = _src.RemoveItemData(guid) || changed; // Remove reference since it has no customization
                        else {
                            changed = true;
                            _src.StoreItemIcon(guid, icoType);
                        }
                    }
                } else {
                    changed = true;
                    _src.StoreItemIcon(guid, icoType);
                }
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        // Assumes at least one object is selected
        internal static void SetEvidence(DeProjectData.EvidenceType evidenceType)
        {
            _src = ConnectToData(true);
            if (_src.evidenceType == evidenceType) return;
            Undo.RecordObject(_src, "DeProject");
            _src.evidenceType = evidenceType;
            EditorUtility.SetDirty(_src);
        }

        #endregion

        #region Methods

        static bool IsProjectFolder(string assetGuid)
        {
            if (assetGuid == null) return false;
            string adbPath = AssetDatabase.GUIDToAssetPath(assetGuid);
            return !string.IsNullOrEmpty(adbPath) && Directory.Exists(DeEditorFileUtils.ADBPathToFullPath(adbPath));
        }

        static DeProjectData ConnectToData(bool createIfMissing)
        {
            return DeEditorPanelUtils.ConnectToSourceAsset<DeProjectData>(ADBDataPath, createIfMissing, false);
        }

        #endregion
    }
}