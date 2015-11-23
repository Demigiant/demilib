// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:36
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Audio;

namespace DG.DeAudioEditor
{
    [CustomEditor(typeof (DeAudioManager))]
    public class DeAudioManagerInspector : Editor
    {
        DeAudioManager _src;
        ReorderableList _audioGroupsList;
        readonly List<DeAudioGroupId> _duplicateGroupIds = new List<DeAudioGroupId>();
        bool _requiresDuplicateCheck; // Used to check for duplicates during draw method, since during onChangedCallback it won't work correctly

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ UNITY METHODS

        void OnEnable()
        {
            _src = target as DeAudioManager;
            DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);

            float lineH = EditorGUIUtility.singleLineHeight;
            float listVSpacer = 2f;
            _audioGroupsList = new ReorderableList(serializedObject, serializedObject.FindProperty("fooAudioGroups"), true, true, true, true);
            _audioGroupsList.elementHeight = lineH * 5 + listVSpacer * 4 + 10;
            _audioGroupsList.drawHeaderCallback = rect => { GUI.Label(rect, "DeAudioGroups", DeGUI.styles.label.bold); };
            _audioGroupsList.onChangedCallback = list => { _requiresDuplicateCheck = true; };
            _audioGroupsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                if (_requiresDuplicateCheck && Event.current.type == EventType.Repaint) {
                    DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);
                    _requiresDuplicateCheck = false;
                }
                bool hasDuplicateGroupIds = _duplicateGroupIds.Count > 0;
                EditorGUIUtility.labelWidth = 86;
                float ry = rect.y + 2;
                DeAudioGroup item = _src.fooAudioGroups[index];
                GUI.color = item.mixerGroup == null ? Color.red : Color.green;
                AudioMixerGroup prevMixerGroup = item.mixerGroup;
                item.mixerGroup = EditorGUI.ObjectField(new Rect(rect.x, ry, rect.width, lineH), "MixerGroup", item.mixerGroup, typeof(AudioMixerGroup), false) as AudioMixerGroup;
                if (item.mixerGroup != prevMixerGroup) DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);
                GUI.color = Color.white;
                ry += lineH + listVSpacer;
                if (item.mixerGroup == null) {
                    EditorGUI.HelpBox(new Rect(rect.x, ry, rect.width, lineH * 3), "Add a MixerGroup in order to set more options", MessageType.Warning);
                } else {
                    DeAudioGroupId prevId = item.id;
                    GUI.color = hasDuplicateGroupIds && _duplicateGroupIds.Contains(item.id) ? Color.red : Color.white;
                    item.id = (DeAudioGroupId)EditorGUI.EnumPopup(new Rect(rect.x, ry, rect.width, lineH), "Id (univocal)", item.id);
                    GUI.color = Color.white;
                    if (item.id != prevId) DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);
                    ry += lineH + listVSpacer;
                    item.fooVolume = EditorGUI.Slider(new Rect(rect.x, ry, rect.width, lineH), "Volume", item.fooVolume, 0, 1);
                    ry += lineH + listVSpacer;
                    item.maxSources = EditorGUI.IntSlider(new Rect(rect.x, ry, rect.width, lineH), "Max Sources", item.maxSources, -1, 100);
                    ry += lineH + listVSpacer;
                    item.recycle = EditorGUI.Toggle(new Rect(rect.x, ry, 120, lineH), new GUIContent("       Recycle", "If toggled, when max sources are reached and no free one is available the oldest one will be reused. If untoggled the new audio won't play."), item.recycle);
                    EditorGUI.BeginDisabledGroup(item.maxSources == 0);
                    EditorGUIUtility.labelWidth = 76;
                    item.preallocate = EditorGUI.IntSlider(new Rect(rect.x + 120, ry, rect.width - 120, lineH), new GUIContent("Pre-allocate", "Allocates the given sources at startup."), item.preallocate, 0, item.maxSources >= 0 ? item.maxSources : 100);
                    EditorGUI.EndDisabledGroup();
                }
            };
        }

        override public void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeAudioManager");
            DeGUI.BeginGUI();

            if (Application.isPlaying) DrawRuntime();
            else DrawDefault();
        }

        void DrawDefault()
        {
            EditorGUIUtility.labelWidth = 86;
            GUILayout.Space(4);
            if (_duplicateGroupIds.Count > 0) EditorGUILayout.HelpBox("Beware: some DeAudioGroups have the same ID, which is not permitted", MessageType.Error);
            GUILayout.Space(4);

            DeGUILayout.BeginVBox();
            GUI.color = _src.fooAudioMixer == null ? Color.red : Color.white;
            _src.fooAudioMixer = EditorGUILayout.ObjectField("Audio Mixer", _src.fooAudioMixer, typeof(AudioMixer), false) as AudioMixer;
            GUI.color = Color.white;
            DeGUILayout.EndVBox();
            if (_src.fooAudioMixer == null) {
                EditorGUILayout.HelpBox("Add an AudioMixer to proceed to the next step", MessageType.Warning);
                return;
            }

            EditorGUIUtility.labelWidth = 96;
            _src.logInfo = DeGUILayout.ToggleButton(_src.logInfo, "Output Additional Info Logs");
            _src.fooGlobalVolume = EditorGUILayout.Slider("Global Volume", _src.fooGlobalVolume, 0, 1);

            // AUDIO GROUPS //

            serializedObject.Update();
            _audioGroupsList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        void DrawRuntime()
        {
            EditorGUILayout.HelpBox("Runtime Mode", MessageType.Info);

            // Global volume
            EditorGUIUtility.labelWidth = 96;
            GUILayout.BeginVertical(DeGUI.styles.box.def);
            float prevGlobalVolume = _src.fooGlobalVolume;
            _src.fooGlobalVolume = EditorGUILayout.Slider("Global Volume", _src.fooGlobalVolume, 0, 1);
            if (Math.Abs(_src.fooGlobalVolume - prevGlobalVolume) > float.Epsilon) DeAudioManager.SetGlobalVolume(_src.fooGlobalVolume);
            GUILayout.EndVertical();
            // Group volumes
            GUILayout.Space(4);
            EditorGUIUtility.labelWidth = 101;
            GUILayout.Label("DeAudioGroups Volumes", DeGUI.styles.label.bold);
            int len = _src.fooAudioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup g = _src.fooAudioGroups[i];
                float prevVolume = g.fooVolume;
                g.fooVolume = EditorGUILayout.Slider(g.id.ToString(), g.fooVolume, 0, 1);
                if (Math.Abs(g.fooVolume - prevVolume) > float.Epsilon) g.SetVolume(g.fooVolume);
            }
        }
    }
}