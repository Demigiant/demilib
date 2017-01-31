// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/31 14:26
// License Copyright (c) Daniele Giardini

using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    public static class De2DPrefs
    {
        public static bool enableSceneContextMenu;
        const string _SavePrefix = "De2D_";
        const string _ID_EnableSceneContextMenu = _SavePrefix + "enableSceneContextMenu";

        static De2DPrefs()
        {
            // Load preferences
            enableSceneContextMenu = EditorPrefs.GetBool(_ID_EnableSceneContextMenu, true);
        }

        [PreferenceItem("De2D")]
        public static void PreferencesGUI()
        {
            DeGUI.BeginGUI();

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