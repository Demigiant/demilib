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
        const string _Version = "0.5.001";
        public static bool enableSceneContextMenu;
        public static bool hideDeHierarchyObject;
        const string _SavePrefix = "DeEditorTools_";
        const string _ID_EnableSceneContextMenu = _SavePrefix + "enableSceneContextMenu";
        const string _ID_HideDeHierarchyObject = _SavePrefix + "hideDeHierarchyObject";

        // Elements to remove from EditorPrefs (possible leftovers from previous versions)
        static readonly string[] _PrefsToDelete = new [] {
           "De2D_enableSceneContextMenu" 
        };

        static DeEditorToolsPrefs()
        {
            // Delete leftover preferences
            foreach (string s in _PrefsToDelete) {
                if (EditorPrefs.HasKey(s)) EditorPrefs.DeleteKey(s);
            }
            // Load preferences
            enableSceneContextMenu = EditorPrefs.GetBool(_ID_EnableSceneContextMenu, true);
            hideDeHierarchyObject = EditorPrefs.GetBool(_ID_HideDeHierarchyObject, false);
        }

        [PreferenceItem("DeEditorTools")]
        public static void PreferencesGUI()
        {
            DeGUI.BeginGUI();

            GUILayout.Label("v" + _Version);
            GUILayout.Space(4);

            // DeScene
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                enableSceneContextMenu = DeGUILayout.ToggleButton(enableSceneContextMenu, "Enable Scene ContextMenu", GUILayout.Height(16));
                GUILayout.Label(
                    "When this is enabled, a context menu will be available in the Scene Panel with various features. Note though that enabling it will disable regular right-click (which by default is the same as middle-click, and shows the panning instrument).",
                    EditorStyles.wordWrappedMiniLabel
                );
            }

            // DeHierarchy
            bool deHierarchyChanged = false;
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                EditorGUI.BeginChangeCheck();
                hideDeHierarchyObject = DeGUILayout.ToggleButton(hideDeHierarchyObject, "Hide DeHierarchy object", GUILayout.Height(16));
                GUILayout.Label(
                    "Toggling this will hide the DeHierarchy object in scenes.",
                    EditorStyles.wordWrappedMiniLabel
                );
                deHierarchyChanged = EditorGUI.EndChangeCheck();
            }

            if (GUI.changed) SaveAll();

            if (deHierarchyChanged) DeHierarchy.OnPreferencesRefresh();
        }

        static void SaveAll()
        {
            EditorPrefs.SetBool(_ID_EnableSceneContextMenu, enableSceneContextMenu);
            EditorPrefs.SetBool(_ID_HideDeHierarchyObject, hideDeHierarchyObject);
        }
    }
}