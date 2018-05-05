// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 17:05
// License Copyright (c) Daniele Giardini

using System;
using System.IO;
using DG.DemiEditor;
using DG.DemiEditor.Internal;
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

            if (_src == null) _src = ConnectToData(false);
            if (_src == null) return;

            DeProjectData.CustomizedItem customizedItem = _src.GetItem(guid);
            if (customizedItem == null) return;

            DeGUI.BeginGUI();
            bool isLeftSide = selectionRect.x >= 16;

            // Color
            Color color = customizedItem.GetColor();
            Texture2D icoFolder = DeStylePalette.proj_folder;
            Rect folderR = new Rect(selectionRect.x, selectionRect.y + 2, icoFolder.width, icoFolder.height);
            if (!isLeftSide) folderR.x = folderR.x + 3;
            using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(folderR, icoFolder);
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

        #endregion

        #region Internal Methods

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

        internal static void PastDataToSelections()
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
                    changed = _src.RemoveItemData(guid) || changed;
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
                changed = true;
                _src.StoreItemIcon(guid, icoType);
            }
            if (changed) EditorUtility.SetDirty(_src);
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