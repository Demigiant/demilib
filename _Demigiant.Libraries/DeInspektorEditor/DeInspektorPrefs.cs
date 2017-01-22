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
            SpecialFeatures
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
            mode = (Mode)EditorGUILayout.EnumPopup("Mode", mode);

            if (GUI.changed) SaveAll();
        }

        static void SaveAll()
        {
            EditorPrefs.SetInt(_ModeId, (int)mode);
        }
    }
}