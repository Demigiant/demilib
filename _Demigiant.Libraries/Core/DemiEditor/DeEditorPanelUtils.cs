// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/09 12:36
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utilities for Editor Panels.
    /// </summary>
    public static class DeEditorPanelUtils
    {
        #region Public Methods

        /// <summary>
        /// Connects to a <see cref="ScriptableObject"/> asset.
        /// If the asset already exists at the given path, loads it and returns it.
        /// Otherwise, depending on the given parameters, either returns NULL or automatically creates it before loading and returning it.
        /// </summary>
        /// <typeparam name="T">Asset type</typeparam>
        /// <param name="adbFilePath">File path (relative to Unity's project folder)</param>
        /// <param name="createIfMissing">If TRUE and the requested asset doesn't exist, forces its creation</param>
        /// <param name="createFoldersIfMissing">If TRUE also creates the path folders if they don't exist</param>
        public static T ConnectToSourceAsset<T>(string adbFilePath, bool createIfMissing = false, bool createFoldersIfMissing = false) where T : ScriptableObject
        {
            if (!DeEditorFileUtils.AssetExists(adbFilePath)) {
                if (createIfMissing) CreateScriptableAsset<T>(adbFilePath, createFoldersIfMissing);
                else return null;
            }
            T source = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
            if (source == null) {
                // Source changed (or editor file was moved from outside of Unity): overwrite it
                CreateScriptableAsset<T>(adbFilePath, createFoldersIfMissing);
                source = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
            }
            return source;
        }

        #endregion

        #region Private Methods

        static void CreateScriptableAsset<T>(string adbFilePath, bool createFoldersIfMissing) where T : ScriptableObject
        {
            T data = ScriptableObject.CreateInstance<T>();
            if (createFoldersIfMissing) {
                string[] folders = adbFilePath.Split(DeEditorFileUtils.ADBPathSlash.ToCharArray()[0]);
                string path = "Assets";
                for (int i = 1; i < folders.Length - 1; ++i) {
                    string folder = folders[i];
                    if (!DeEditorFileUtils.AssetExists(path + DeEditorFileUtils.ADBPathSlash + folder)) AssetDatabase.CreateFolder(path, folder);
                    path = path + DeEditorFileUtils.ADBPathSlash + folder;
                }
            }
            AssetDatabase.CreateAsset(data, adbFilePath);
        }

        #endregion
    }
}