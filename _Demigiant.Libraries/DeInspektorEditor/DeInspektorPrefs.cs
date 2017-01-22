// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/22 10:22
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor
{
    public static class DeInspektorPrefs
    {
        public enum Mode
        {
            Basic,
            Full
        }

        public static Mode mode;
        const string _ModeId = _SavePrefix + "mode";

        const string _SavePrefix = "DeInspektor_";

        static DeInspektorPrefs()
        {
            // Load preferences
            mode = (Mode)EditorPrefs.GetInt(_ModeId, 1);
        }

        [PreferenceItem("DeInspektor")]
        public static void PreferencesGUI()
        {
            GUILayout.Label("v" + DeInspektor.Version);
            GUILayout.BeginVertical(GUI.skin.box);
            mode = (Mode)EditorGUILayout.EnumPopup("Mode", mode);
            GUILayout.Space(-3);
            GUILayout.BeginHorizontal();
            switch (mode) {
            case Mode.Full:
                EditorGUILayout.HelpBox("Special layouts for:\n- lists/arrays", MessageType.Info);
                break;
            default:
                EditorGUILayout.HelpBox("Regular inspector, with only the hidden features necessary to show some DeAttributes", MessageType.Info);
                break;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            if (GUI.changed) SaveAll();
        }

        static void SaveAll()
        {
            EditorPrefs.SetInt(_ModeId, (int)mode);
        }
    }
}