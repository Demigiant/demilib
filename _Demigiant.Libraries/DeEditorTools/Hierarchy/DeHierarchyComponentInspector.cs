// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 15:08
// License Copyright (c) Daniele Giardini

using DG.DemiEditor;
using DG.DemiLib;
using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    [CustomEditor(typeof(DeHierarchyComponent))]
    public class DeHierarchyComponentInspector : Editor
    {
        static GUIStyle _infoStyle;

        #region Unity and GUI Methods

        public override void OnInspectorGUI()
        {
            SetStyles();

            using (new DeGUI.ColorScope(new DeSkinColor(0.1f))) {
                GUILayout.Label(
                    "This is an object created by <b>DeEditorTools' DeHierarchy</b>, in order to evidence Hierarchy elements with colors etc." +
                    "\n\n<b>It will not be included in your build</b>, and you can delete it at any time to remove all evidences from a given scene." +
                    "\n\nNote that <b>you can hide it</b>, by using Unity's Preferences > DeEditorTools, if you want.",
                    _infoStyle
                );
            }

            using (new EditorGUI.DisabledScope(true)) {
                DrawDefaultInspector();
            }
        }

        static void SetStyles()
        {
            if (_infoStyle != null) return;

            _infoStyle = DeGUI.styles.box.flat.Clone(new DeSkinColor(0.75f), TextAnchor.MiddleLeft, 11, Format.RichText).Padding(14);
        }

        #endregion
    }
}