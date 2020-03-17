// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 17:05
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using System.IO;
using DG.DemiEditor;
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
        static readonly List<string> _tmpGUIDs = new List<string>();

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

        #region Public Methods

        /// <summary>
        /// Sets the color of a project folder with options to apply it to subfolders also
        /// </summary>
        /// <param name="color">Color to apply</param>
        /// <param name="absOrAdbFolderPath">Absolute or AssetDatabase path</param>
        /// <param name="alsoApplyToSubdirectories">If TRUE also applies the color to all subdirectories</param>
        public static void SetFolderColor(DeProjectData.HColor color, string absOrAdbFolderPath, bool alsoApplyToSubdirectories = false)
        {
            if (ValidateAndFillDirsList(absOrAdbFolderPath, alsoApplyToSubdirectories)) {
                SetColorForSelections(color, _tmpGUIDs);
            }
            _tmpGUIDs.Clear();
        }
        /// <summary>
        /// Sets the color of a project folder with options to apply it to subfolders also
        /// </summary>
        /// <param name="color">Color to apply</param>
        /// <param name="absOrAdbFolderPath">Absolute or AssetDatabase path</param>
        /// <param name="alsoApplyToSubdirectories">If TRUE also applies the color to all subdirectories</param>
        public static void SetFolderColor(Color color, string absOrAdbFolderPath, bool alsoApplyToSubdirectories = false)
        {
            if (ValidateAndFillDirsList(absOrAdbFolderPath, alsoApplyToSubdirectories)) {
                SetColorForSelections(DeProjectData.HColor.Custom, _tmpGUIDs, color);
            }
            _tmpGUIDs.Clear();
        }

        /// <summary>
        /// Sets the icon of a project folder with options to apply it to subfolders also
        /// </summary>
        /// <param name="icoType">Icon to apply</param>
        /// <param name="absOrAdbFolderPath">Absolute or AssetDatabase path</param>
        /// <param name="alsoApplyToSubdirectories">If TRUE also applies the color to all subdirectories</param>
        public static void SetFolderIcon(DeProjectData.IcoType icoType, string absOrAdbFolderPath, bool alsoApplyToSubdirectories = false)
        {
            if (ValidateAndFillDirsList(absOrAdbFolderPath, alsoApplyToSubdirectories)) {
                SetIconForSelections(icoType, _tmpGUIDs);
            }
            _tmpGUIDs.Clear();
        }

        #endregion

        #region Internal Methods

        internal static void ResetSelections(IList<string> assetGUIDs)
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < assetGUIDs.Count; ++i) {
                changed = _src.RemoveItemData(assetGUIDs[i]) || changed;
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        internal static bool CanCopyDataFromSelection(IList<string> assetGUIDs)
        {
            if (assetGUIDs.Count == 0) return false;
            string guid = assetGUIDs[0];
            if (!DeEditorFileUtils.IsProjectFolder(guid)) return false;
            _src = ConnectToData(true);
            return _src.GetItem(guid) != null;
        }

        internal static void CopyDataFromSelection(IList<string> assetGUIDs)
        {
            if (assetGUIDs.Count == 0) return;
            string guid = assetGUIDs[0];
            if (!DeEditorFileUtils.IsProjectFolder(guid)) return;
            _src = ConnectToData(true);
            DeProjectData.CustomizedItem item = _src.GetItem(guid);
            if (item != null) DeProjectClipboard.storedItem = item.Clone();
        }

        internal static void PasteDataToSelections(IList<string> assetGUIDs)
        {
            if (!DeProjectClipboard.hasStoreData) return;
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < assetGUIDs.Count; ++i) {
                string guid = assetGUIDs[i];
                if (!DeEditorFileUtils.IsProjectFolder(guid)) continue;
                changed = true;
                DeProjectData.CustomizedItem item = _src.StoreItem(guid);
                item.Paste(DeProjectClipboard.storedItem);
            }
            if (changed) EditorUtility.SetDirty(_src);
        }

        // Assumes at least one object is selected
        internal static void CustomizeSelections(IList<string> assetGUIDs)
        {
            _src = ConnectToData(true);
            DeProjectCustomizeItemWindow.Items.Clear();
            for (int i = 0; i < assetGUIDs.Count; ++i) {
                string guid = assetGUIDs[i];
                if (!DeEditorFileUtils.IsProjectFolder(guid)) continue;
                DeProjectCustomizeItemWindow.Items.Add(_src.StoreItem(guid));
            }
            if (DeProjectCustomizeItemWindow.Items.Count > 0) DeProjectCustomizeItemWindow.Open(_src);
        }

        // Assumes at least one object is selected
        internal static void SetColorForSelections(DeProjectData.HColor hColor, IList<string> assetGUIDs, Color? customColor = null)
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < assetGUIDs.Count; ++i) {
                string guid = assetGUIDs[i];
                if (!DeEditorFileUtils.IsProjectFolder(guid)) continue;
                if (hColor == DeProjectData.HColor.None) {
                    DeProjectData.CustomizedItem item = _src.GetItem(guid);
                    if (item != null) {
                        if (!item.hasIcon) changed = _src.RemoveItemData(guid) || changed; // Remove reference since it has no customization
                        else {
                            changed = true;
                            _src.StoreItemColor(guid, hColor, customColor);
                        }
                    }
                } else {
                    changed = true;
                    _src.StoreItemColor(guid, hColor, customColor);
                }
            }
            if (changed) {
                EditorUtility.SetDirty(_src);
                EditorApplication.RepaintProjectWindow();
            }
        }

        // Assumes at least one object is selected
        internal static void SetIconForSelections(DeProjectData.IcoType icoType, IList<string> assetGUIDs)
        {
            _src = ConnectToData(true);
            Undo.RecordObject(_src, "DeProject");
            bool changed = false;
            for (int i = 0; i < assetGUIDs.Count; ++i) {
                string guid = assetGUIDs[i];
                if (!DeEditorFileUtils.IsProjectFolder(guid)) continue;
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
            if (changed) {
                EditorUtility.SetDirty(_src);
                EditorApplication.RepaintProjectWindow();
            }
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

        static DeProjectData ConnectToData(bool createIfMissing)
        {
            return DeEditorPanelUtils.ConnectToSourceAsset<DeProjectData>(ADBDataPath, createIfMissing, false);
        }

        #endregion

        #region Helpers

        // Fills _tmpGUIDs with the GUIDs of the selected folders.
        // Returns FALSE if the given dir is invalid
        static bool ValidateAndFillDirsList(string absOrAdbFolderPath, bool includeSubdirectories)
        {
            string fullPath = absOrAdbFolderPath;
            if (!DeEditorFileUtils.IsFullPath(fullPath)) {
                if (DeEditorFileUtils.IsADBPath(absOrAdbFolderPath)) fullPath = DeEditorFileUtils.ADBPathToFullPath(absOrAdbFolderPath);
                else {
                    Debug.LogError(string.Format("Submitted path (\"{0}\") is neither a full nor ar and ADB valid path", absOrAdbFolderPath));
                    return false;
                }
            }
            if (!Directory.Exists(fullPath) || !fullPath.Contains(DeEditorFileUtils.projectPath)) {
                Debug.LogError(string.Format("Submitted path (\"{0}\") is not a valid path", absOrAdbFolderPath));
                return false;
            }
            _tmpGUIDs.Clear();
            _tmpGUIDs.Add(AssetDatabase.AssetPathToGUID(DeEditorFileUtils.FullPathToADBPath(fullPath)));
            if (includeSubdirectories) {
                string[] subdirs = Directory.GetDirectories(fullPath, "*", SearchOption.AllDirectories);
                foreach (string subdir in subdirs) {
                    string adbSubdir = AssetDatabase.AssetPathToGUID(DeEditorFileUtils.FullPathToADBPath(subdir));
                    _tmpGUIDs.Add(adbSubdir);
                }
            }
            return true;
        }

        #endregion
    }
}