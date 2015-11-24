// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 16:30
// License Copyright (c) Daniele Giardini

using System;
using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DG.DeAudioEditor
{
    [CustomEditor(typeof (DeAudioCollection))]
    public class DeAudioCollectionInspector : Editor
    {
        DeAudioCollection _src;
        ReorderableList _dataList;
        AudioSource _previewSource { get { return ConnectToAudioSource(); } }

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as DeAudioCollection;
            ConnectToAudioSource();

            float lineH = EditorGUIUtility.singleLineHeight;
            float listVSpacer = 2f;
            _dataList = new ReorderableList(serializedObject, serializedObject.FindProperty("data"), true, true, true, true);
            _dataList.elementHeight = lineH * 2 + listVSpacer + 10;
            _dataList.drawHeaderCallback = rect => { GUI.Label(rect, "Data", DeGUI.styles.label.bold); };
            _dataList.onAddCallback = list => {
                // Force volume to 1 (otherwise set to 0 by list default) and audioClip to null
                ReorderableList.defaultBehaviours.DoAddButton(list);
                int addedIndex = list.serializedProperty.arraySize - 1;
                list.serializedProperty.GetArrayElementAtIndex(addedIndex).FindPropertyRelative("clip").objectReferenceValue = null;
                list.serializedProperty.GetArrayElementAtIndex(addedIndex).FindPropertyRelative("volume").floatValue = 1;
            };
            _dataList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                DeAudioClipData item = _src.data[index];
                EditorGUIUtility.labelWidth = 50;
                float ry = rect.y + 2;
                item.clip = EditorGUI.ObjectField(new Rect(rect.x, ry, rect.width, lineH), "Clip", item.clip, typeof (AudioClip), false) as AudioClip;
                ry += lineH + listVSpacer;
                float btW = 25;
                float prevVolume = item.volume;
                item.volume = EditorGUI.Slider(new Rect(rect.x, ry, rect.width - btW * 2 - 5, lineH), "Volume", item.volume, 0, 1);
                if (Math.Abs(item.volume - prevVolume) > float.Epsilon && _src.previewSource.isPlaying && _src.previewSource.clip == item.clip) _src.previewSource.volume = item.volume;
                if (GUI.Button(new Rect(rect.x + (rect.width - btW * 2), ry - 1, btW, lineH), "►", DeGUI.styles.button.tool)) Play(item);
                if (GUI.Button(new Rect(rect.x + (rect.width - btW), ry - 1, btW, lineH), "■", DeGUI.styles.button.tool)) Stop(item);
            };
        }

        override public void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeAudioCollection");
            DeGUI.BeginGUI();

            GUILayout.Space(4);

            serializedObject.Update();
            _dataList.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }

        #endregion

        #region Methods

        AudioSource ConnectToAudioSource()
        {
            if (_src.previewSource != null) return _src.previewSource;
            _src.previewSource = _src.GetComponent<AudioSource>();
            if (_src.previewSource != null) return _src.previewSource;
            _src.previewSource = _src.gameObject.AddComponent<AudioSource>();
            _previewSource.playOnAwake = false;
            EditorUtility.SetDirty(_src);
            return _src.previewSource;
        }

        void Play(DeAudioClipData data)
        {
            if (data.clip == null) return;
            _previewSource.clip = data.clip;
            _previewSource.volume = data.volume;
            _previewSource.Play();
        }

        void Stop(DeAudioClipData data)
        {
            _src.previewSource.Stop();
            _src.previewSource.clip = null;
        }

        #endregion
    }
}