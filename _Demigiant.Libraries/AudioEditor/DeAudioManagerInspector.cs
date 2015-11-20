// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 21:21
// License Copyright (c) Daniele Giardini

using DG.Audio;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.AudioEditor
{
    [CustomEditor(typeof (DeAudioManager))]
    public class DeAudioManagerInspector : Editor
    {
        DeAudioManager _src;

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ UNITY METHODS

        void OnEnable()
        {
            _src = target as DeAudioManager;
        }

        override public void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeAudioManager");
            DeGUI.BeginGUI();

            EditorGUIUtility.labelWidth = 75;
            GUILayout.Space(8);

            DeGUILayout.Toolbar("Volumes", DeGUI.styles.toolbar.stickyTop);
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            _src.masterVolume = EditorGUILayout.Slider("Master", _src.masterVolume, 0, 1);
            _src.fxVolume = EditorGUILayout.Slider("FX", _src.fxVolume, 0, 1);
            _src.ambientVolume = EditorGUILayout.Slider("Ambient", _src.ambientVolume, 0, 1);
            _src.musicVolume = EditorGUILayout.Slider("Music", _src.musicVolume, 0, 1);
            DeGUILayout.EndVBox();

            DeGUILayout.Toolbar("Max allowed sources", DeGUI.styles.toolbar.stickyTop);
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            EditorGUILayout.HelpBox("Set -1 to allow for infinite sources", MessageType.Info);
            _src.maxFxSources = EditorGUILayout.IntSlider("FX", _src.maxFxSources, -1, 100);
            _src.maxAmbientSources = EditorGUILayout.IntSlider("Ambient", _src.maxAmbientSources, -1, 100);
            _src.maxMusicSources = EditorGUILayout.IntSlider("Music", _src.maxMusicSources, -1, 100);
            DeGUILayout.EndVBox();

            DeGUILayout.Toolbar("Other", DeGUI.styles.toolbar.stickyTop);
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            GUILayout.BeginHorizontal();
            _src.culling = DeGUILayout.ToggleButton(_src.culling, new GUIContent("Culling", "If toggled, when max sources are reached the oldest playing one will be reused. If untoggled the new audio won't play."));
            _src.logInfo = DeGUILayout.ToggleButton(_src.logInfo, "Log info");
            GUILayout.EndHorizontal();
            DeGUILayout.EndVBox();

            if (GUI.changed) EditorUtility.SetDirty(_src);
        }
    }
}