// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/02 18:41
// License Copyright (c) Daniele Giardini

using DG.DeEditorTools.Hierarchy;
using DG.DeEditorTools.Scene;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools
{
    public class DeEditorToolsPrefs
    {
        const string _Version = "0.5.020";

        public static bool deScene_enableContextMenu;
        public static bool deHierarchy_hideObject;
        public static bool deHierarchy_showDot;
        public static bool deHierarchy_showDotBorder;
        public static bool deHierarchy_indentDot;
        public static bool deHierarchy_showBorder;

        const string _SavePrefix = "DeEditorTools_";
        const string _ID_DeScene_EnableContextMenu = _SavePrefix + "deScene_enableContextMenu";
        const string _ID_DeHierarchy_HideObject = _SavePrefix + "deHierarchy_hideObject";
        const string _ID_DeHierarchy_ShowDot = _SavePrefix + "deHierarchy_showDot";
        const string _ID_DeHierarchy_ShowDotBorder = _SavePrefix + "deHierarchy_showDotBorder";
        const string _ID_DeHierarchy_IndentDot = _SavePrefix + "deHierarchy_indentDot";
        const string _ID_DeHierarchy_ShowBorder = _SavePrefix + "deHierarchy_showBorder";

        // Elements to remove from EditorPrefs (possible leftovers from previous versions)
        static readonly string[] _PrefsToDelete = new [] {
           "De2D_enableSceneContextMenu",
           "DeEditorTools_enableSceneContextMenu",
           "DeEditorTools_hideDeHierarchyObject",
        };

        static DeEditorToolsPrefs()
        {
            // Delete leftover preferences
            foreach (string s in _PrefsToDelete) {
                if (EditorPrefs.HasKey(s)) EditorPrefs.DeleteKey(s);
            }
            // Load preferences
            deScene_enableContextMenu = EditorPrefs.GetBool(_ID_DeScene_EnableContextMenu, true);
            deHierarchy_hideObject = EditorPrefs.GetBool(_ID_DeHierarchy_HideObject, false);
            deHierarchy_showDot = EditorPrefs.GetBool(_ID_DeHierarchy_ShowDot, true);
            deHierarchy_showDotBorder = EditorPrefs.GetBool(_ID_DeHierarchy_ShowDotBorder, false);
            deHierarchy_indentDot = EditorPrefs.GetBool(_ID_DeHierarchy_IndentDot, false);
            deHierarchy_showBorder = EditorPrefs.GetBool(_ID_DeHierarchy_ShowBorder, true);
        }

        [PreferenceItem("DeEditorTools")]
        public static void PreferencesGUI()
        {
            DeGUI.BeginGUI();

            GUILayout.Label("v" + _Version);

            // DeScene
            GUILayout.Label("DeScene", EditorStyles.largeLabel);
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                deScene_enableContextMenu = DeGUILayout.ToggleButton(deScene_enableContextMenu, "Enable Scene ContextMenu", GUILayout.Height(16));
                GUILayout.Label(
                    "When this is enabled, a context menu will be available in the Scene Panel with various features. Note though that enabling it will disable regular right-click (which by default is the same as middle-click, and shows the panning instrument).",
                    EditorStyles.wordWrappedMiniLabel
                );
            }

            // DeHierarchy
            GUILayout.Space(4);
            GUILayout.Label("DeHierarchy", EditorStyles.largeLabel);
            bool hierarchyChanged = false;
            bool flagsChanged = false;
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginChangeCheck();
                deHierarchy_hideObject = DeGUILayout.ToggleButton(deHierarchy_hideObject, "Hide DeHierarchy GameObject", GUILayout.Height(16));
                flagsChanged = EditorGUI.EndChangeCheck();
                deHierarchy_showDot = EditorGUILayout.Toggle("Show Colored Dot", deHierarchy_showDot);
                using (new EditorGUI.DisabledScope(!deHierarchy_showDot)) {
                    deHierarchy_indentDot = EditorGUILayout.Toggle(" └ Indent Dot", deHierarchy_indentDot);
                    deHierarchy_showDotBorder = EditorGUILayout.Toggle(" └ Show Dot Outline", deHierarchy_showDotBorder);
                }
                deHierarchy_showBorder = EditorGUILayout.Toggle("Show Colored Border", deHierarchy_showBorder);
                hierarchyChanged = EditorGUI.EndChangeCheck();
            }

            if (GUI.changed) SaveAll();

            if (hierarchyChanged || flagsChanged) DeHierarchy.OnPreferencesRefresh(flagsChanged);
        }

        static void SaveAll()
        {
            EditorPrefs.SetBool(_ID_DeScene_EnableContextMenu, deScene_enableContextMenu);
            EditorPrefs.SetBool(_ID_DeHierarchy_HideObject, deHierarchy_hideObject);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowDot, deHierarchy_showDot);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowDotBorder, deHierarchy_showDotBorder);
            EditorPrefs.SetBool(_ID_DeHierarchy_IndentDot, deHierarchy_indentDot);
            EditorPrefs.SetBool(_ID_DeHierarchy_ShowBorder, deHierarchy_showBorder);
        }
    }
}