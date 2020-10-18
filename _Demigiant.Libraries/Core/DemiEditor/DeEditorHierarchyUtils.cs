// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/28 17:01
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorHierarchyUtils
    {
        static MethodInfo _miExpand;

        public static void ExpandGameObject(GameObject go)
        {
            if (_miExpand == null) {
                Type t = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor.dll");
                if (t == null) {
                    Debug.LogError("Couldn't find type \"UnityEditor.SceneHierarchyWindow,UnityEditor.dll\"");
                    return;
                }
                _miExpand = t.GetMethod("ExpandTreeViewItem", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_miExpand == null) _miExpand = t.GetMethod("SetExpandedRecursive", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (_miExpand == null) {
                    Debug.LogError("Couldn't find SceneHierarchyWindow \"ExpandTreeViewItem\" or \"SetExpandedRecursive\" methods");
                    return;
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2019) EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            else EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            var hierarchy = EditorWindow.focusedWindow;
            _miExpand.Invoke(hierarchy, new object[] { go.GetInstanceID(), true});
        }
    }
}