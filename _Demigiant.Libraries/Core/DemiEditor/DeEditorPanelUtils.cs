// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/09 12:36
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utilities for Editor Panels.
    /// </summary>
    public static class DeEditorPanelUtils
    {
        static Dictionary<EditorWindow, GUIContent> _winTitleContentByEditor;
        static FieldInfo _fi_editorWindowParent;
        static MethodInfo _miRepaintCurrentEditor;

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
        public static T ConnectToSourceAsset<T>(
            string adbFilePath, bool createIfMissing = false, bool createFoldersIfMissing = false
        ) where T : ScriptableObject
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

        /// <summary>
        /// Check if the <see cref="ScriptableObject"/> at the given path exists and eventually if it's available
        /// </summary>
        /// <param name="adbFilePath">File path (relative to Unity's project folder)</param>
        /// <param name="checkIfAvailable">If TRUE also check if the file is available
        /// (file can be unavailable if it was deleted outside Unity, or if Unity is just starting)</param>
        /// <returns></returns>
        public static bool SourceAssetExists<T>(string adbFilePath, bool checkIfAvailable = true) where T : ScriptableObject
        {
            if (!DeEditorFileUtils.AssetExists(adbFilePath)) return false;
            if (!checkIfAvailable) return true;
            T source = (T)AssetDatabase.LoadAssetAtPath(adbFilePath, typeof(T));
            return source != null;
        }

        /// <summary>
        /// Returns TRUE if the given <see cref="EditorWindow"/> is dockable, FALSE if instead it's a utility window
        /// </summary>
        /// <param name="editor"></param>
        /// <returns></returns>
        public static bool IsDockableWindow(EditorWindow editor)
        {
            if (_fi_editorWindowParent == null) {
                _fi_editorWindowParent = editor.GetType().GetField("m_Parent", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            object parent = _fi_editorWindowParent.GetValue(editor);
            if (parent == null) {
                Debug.LogError("DeEditorPanelUtils.IsDockableWindow > parent is NULL, you should call this after the first GUI call happened");
                return false;
            }
            return parent.GetType().ToString() == "UnityEditor.DockArea";
        }

        /// <summary>
        /// Sets the icon and title of an editor window. Works with older versions of Unity, where the titleContent property wasn't available.
        /// </summary>
        /// <param name="editor">Reference to the editor panel whose icon to set</param>
        /// <param name="icon">Icon to apply</param>
        /// <param name="title">Title. If NULL doesn't change it</param>
        public static void SetWindowTitle(EditorWindow editor, Texture icon, string title = null)
        {
            GUIContent titleContent;
            if (_winTitleContentByEditor == null) _winTitleContentByEditor = new Dictionary<EditorWindow, GUIContent>();
            if (_winTitleContentByEditor.ContainsKey(editor)) {
                titleContent = _winTitleContentByEditor[editor];
                if (titleContent != null) {
                    if (titleContent.image != icon) titleContent.image = icon;
                    if (title != null && titleContent.text != title) titleContent.text = title;
                    return;
                }
                _winTitleContentByEditor.Remove(editor);
            }
            titleContent = GetWinTitleContent(editor);
            if (titleContent != null) {
                if (titleContent.image != icon) titleContent.image = icon;
                if (title != null && titleContent.text != title) titleContent.text = title;
                _winTitleContentByEditor.Add(editor, titleContent);
            }
        }

        /// <summary>
        /// Repaints the currently focues editor
        /// </summary>
        public static void RepaintCurrentEditor()
        {
            if (_miRepaintCurrentEditor == null) {
                _miRepaintCurrentEditor = typeof(EditorGUIUtility).GetMethod("RepaintCurrentWindow", BindingFlags.Static | BindingFlags.NonPublic);
            }
            _miRepaintCurrentEditor.Invoke(null, null);
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

        static GUIContent GetWinTitleContent(EditorWindow editor)
        {
            const BindingFlags bFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            PropertyInfo p = typeof(EditorWindow).GetProperty("cachedTitleContent", bFlags);
            if (p == null) return null;
            return p.GetValue(editor, null) as GUIContent;
        }

        #endregion
    }
}