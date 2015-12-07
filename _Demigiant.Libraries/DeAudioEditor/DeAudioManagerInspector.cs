// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:36
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Text;
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
        StringBuilder _runtimeStrb = new StringBuilder();

        #region Unity and GUI Methods

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
            _audioGroupsList.onAddCallback = list => {
                // Force volume to 1, max sources to -1 and recycle to true
                ReorderableList.defaultBehaviours.DoAddButton(list);
                int addedIndex = list.serializedProperty.arraySize - 1;
                SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(addedIndex);
                element.FindPropertyRelative("mixerGroup").objectReferenceValue = null;
                element.FindPropertyRelative("id").enumValueIndex = 0;
                element.FindPropertyRelative("fooVolume").floatValue = 1;
                element.FindPropertyRelative("maxSources").intValue = -1;
                element.FindPropertyRelative("recycle").boolValue = true;
            };
            _audioGroupsList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                if (_requiresDuplicateCheck && Event.current.type == EventType.Repaint) {
                    DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);
                    _requiresDuplicateCheck = false;
                }
                bool hasDuplicateGroupIds = _duplicateGroupIds.Count > 0;
                EditorGUIUtility.labelWidth = 86;
                float ry = rect.y + 2;
                DeAudioGroup item = _src.fooAudioGroups[index];
                GUI.color = item.mixerGroup == null ? Color.white : Color.green;
                AudioMixerGroup prevMixerGroup = item.mixerGroup;
                item.mixerGroup = EditorGUI.ObjectField(new Rect(rect.x, ry, rect.width, lineH), "MixerGroup", item.mixerGroup, typeof (AudioMixerGroup), false) as AudioMixerGroup;
                if (item.mixerGroup != prevMixerGroup) DeAudioEditorUtils.CheckForDuplicateAudioGroupIds(_src.fooAudioGroups, _duplicateGroupIds);
                GUI.color = Color.white;
                ry += lineH + listVSpacer;
                DeAudioGroupId prevId = item.id;
                GUI.color = hasDuplicateGroupIds && _duplicateGroupIds.Contains(item.id) ? Color.red : Color.white;
                item.id = (DeAudioGroupId) EditorGUI.EnumPopup(new Rect(rect.x, ry, rect.width, lineH), "Id (univocal)", item.id);
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
            };
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeAudioManager");
            DeGUI.BeginGUI();

            GUILayout.Label("v" + DeAudioManager.Version, EditorStyles.miniLabel.MarginBottom(0));
            if (Application.isPlaying) DrawRuntime();
            else DrawDefault();
        }

        void DrawDefault()
        {
            EditorGUIUtility.labelWidth = 86;
            if (_duplicateGroupIds.Count > 0) EditorGUILayout.HelpBox("Beware: some DeAudioGroups have the same ID, which is not permitted", MessageType.Error);

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
            if (Math.Abs(_src.fooGlobalVolume - prevGlobalVolume) > float.Epsilon) DeAudioManager.SetVolume(_src.fooGlobalVolume);
            GUILayout.EndVertical();
            // Group volumes
            GUILayout.Space(4);
            EditorGUIUtility.labelWidth = 101;
            GUILayout.Label("DeAudioGroups", DeGUI.styles.label.bold);
            // Groups with ids
            int len = _src.fooAudioGroups.Length;
            for (int i = 0; i < len; ++i) {
                // Group volumes
                DeAudioGroup g = _src.fooAudioGroups[i];
                float prevVolume = g.fooVolume;
                g.fooVolume = EditorGUILayout.Slider(g.id.ToString(), g.fooVolume, 0, 1);
                if (Math.Abs(g.fooVolume - prevVolume) > float.Epsilon) g.SetVolume(g.fooVolume);
                DrawRuntimeSourcesFor(g);
            }
            // Global group
            if (HasPlayingSources(DeAudioManager.globalGroup)) {
                GUILayout.Label("[GLOBAL]");
                DrawRuntimeSourcesFor(DeAudioManager.globalGroup);
            }
        }

        void DrawRuntimeSourcesFor(DeAudioGroup group)
        {
            Color volumeColor = new Color(0.1999999f, 1f, 0f, 1f);
            Color pitchColor = new Color(1f, 0.8482759f, 0f, 1f);
            Color timeColor = new Color(0f, 0.9172413f, 1f, 1f);
            int len = group.sources.Count;
            for (int i = 0; i < len; ++i) {
                // Sources volume
                DeAudioSource s = group.sources[i];
                if (s.isPlaying) {
                    _runtimeStrb.Length = 0;
                    _runtimeStrb.Append("└ ").Append(s.clip.name);
                    if (s.locked) _runtimeStrb.Append(" [LOCKED]");
                    if (s.loop) _runtimeStrb.Append(" [loop]");
                    GUILayout.Label(_runtimeStrb.ToString());
                    DrawRuntimeSourceBar(volumeColor, s.unscaledVolume);
                    DrawRuntimeSourceBar(pitchColor, s.pitch);
                    float elapsed = s.audioSource.time / s.clip.length;
                    if (elapsed > 1) elapsed = 1;
                    DrawRuntimeSourceBar(timeColor, elapsed);
                }
            }
        }

        void DrawRuntimeSourceBar(Color color, float percentage)
        {
            Rect timeRect = GUILayoutUtility.GetRect(0, 2, GUILayout.ExpandWidth(true));
            timeRect.x += 15;
            timeRect.y -= 1;
            timeRect.width -= 15;
            Color guiOrColor = GUI.color;
            GUI.color = Color.black;
            GUI.Box(timeRect, "", DeGUI.styles.box.flat);
            timeRect.width *= percentage;
            GUI.color = color;
            GUI.Box(timeRect, "", DeGUI.styles.box.flat);
            GUI.color = guiOrColor;
            GUILayout.Space(2);
        }

        #endregion

        #region Utils

        bool HasPlayingSources(DeAudioGroup group)
        {
            int len = group.sources.Count;
            for (int i = 0; i < len; ++i) {
                if (group.sources[i].isPlaying) return true;
            }
            return false;
        }

        #endregion
    }
}