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
            if (_miExpand == null) _miExpand = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor.dll")
                                               .GetMethod("ExpandTreeViewItem", BindingFlags.Instance | BindingFlags.NonPublic);
            EditorApplication.ExecuteMenuItem("Window/Hierarchy");
            var hierarchy = EditorWindow.focusedWindow;
            _miExpand.Invoke(hierarchy, new object[] { go.GetInstanceID(), true});
        }
    }
}