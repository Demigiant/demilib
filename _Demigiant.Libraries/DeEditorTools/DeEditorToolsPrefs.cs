// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/02 18:41
// License Copyright (c) Daniele Giardini

using DG.DeEditorTools.SceneUISystem;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools
{
    public class DeEditorToolsPrefs
    {
        public static bool enableSceneContextMenu;
        const string _SavePrefix = "DeEditorTools_";
        const string _ID_EnableSceneContextMenu = _SavePrefix + "enableSceneContextMenu";

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
        }

        [PreferenceItem("DeEditorTools")]
        public static void PreferencesGUI()
        {
            DeGUI.BeginGUI();

            // SceneUI
            GUILayout.Label("v" + SceneUI.Version);
            using (new GUILayout.VerticalScope(GUI.skin.box)) {
                enableSceneContextMenu = DeGUILayout.ToggleButton(enableSceneContextMenu, "Enable Scene ContextMenu", GUILayout.Height(16));
                GUILayout.Label(
                    "When this is enabled, a context menu will be available in the Scene Panel with various features. Note though that enabling it will disable regular right-click (which by default is the same as middle-click, and shows the panning instrument).",
                    EditorStyles.wordWrappedMiniLabel
                );
            }

            if (GUI.changed) SaveAll();
        }

        static void SaveAll()
        {
            EditorPrefs.SetBool(_ID_EnableSceneContextMenu, enableSceneContextMenu);
        }
    }
}