// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 18:26
// License Copyright (c) Daniele Giardini

using System;
using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeAudioEditor
{
    [CustomPropertyDrawer(typeof (DeAudioClipData))]
    public class DeAudioClipDataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            DeGUI.BeginGUI();
            int orIndent = EditorGUI.indentLevel;

            float lineH = EditorGUIUtility.singleLineHeight;
            float btW = 22;
            float btLoopW = 44;
            Rect clipRect = new Rect(position.x, position.y, position.width, lineH);
            Rect volumeRect = new Rect(position.x, position.y + lineH + 1, position.width - btW * 2 - 4, lineH);
            Rect btPlayRect = new Rect(position.x + (position.width - btW * 2), volumeRect.y - 1, btW, lineH);
            Rect btStopRect = new Rect(position.x + (position.width - btW), volumeRect.y - 1, btW, lineH);
            Rect pitchRect = new Rect(position.x, volumeRect.y + lineH, position.width - btLoopW - 4, lineH);
            Rect btLoopRect = new Rect(position.x + (position.width - btLoopW), pitchRect.y, btLoopW, lineH);

            // Clip
            EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("clip"), label);
            // Volume
            SerializedProperty volumeProp = property.FindPropertyRelative("volume");
            float prevVolume = volumeProp.floatValue;
            EditorGUI.Slider(volumeRect, volumeProp, 0, 1, "└ Volume");
            if (Math.Abs(volumeProp.floatValue - prevVolume) > float.Epsilon) AdjustVolume(property, volumeProp.floatValue);
            // Pitch
            SerializedProperty pitchProp = property.FindPropertyRelative("pitch");
            float prevPitch = pitchProp.floatValue;
            EditorGUI.Slider(pitchRect, pitchProp, 0, 3, "└ Pitch");
            if (Math.Abs(pitchProp.floatValue - prevPitch) > float.Epsilon) AdjustPitch(property, pitchProp.floatValue);
            // Controls
            if (GUI.Button(btPlayRect, "►", DeGUI.styles.button.tool)) Play(property);
            if (GUI.Button(btStopRect, "■", DeGUI.styles.button.tool)) Stop(property);
            SerializedProperty loopProp = property.FindPropertyRelative("loop");
            loopProp.boolValue = DeGUI.ToggleButton(btLoopRect, loopProp.boolValue, "Loop");

            EditorGUI.indentLevel = orIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 4;
        }

        void Play(SerializedProperty property)
        {
            AudioClip clip = property.FindPropertyRelative("clip").objectReferenceValue as AudioClip;
            if (clip == null) return;
            MonoBehaviour m = GetValidMonoBehaviour(property);
            if (m == null) {
                // Can't use AudioSource: play clip regardless of volume
                DeEditorSoundUtils.StopAll();
                DeEditorSoundUtils.Play(clip);
                return;
            }
            AudioSource s = m.GetComponent<AudioSource>();
            if (s == null) {
                if (EditorUtility.DisplayDialog("Play DeAudioClipData", "Add AudioSource to preview the clip with the correct volume?", "Ok", "Cancel")) {
                    s = m.gameObject.AddComponent<AudioSource>();
                }
            }
            if (s == null) return;
            s.playOnAwake = false;
            s.volume = property.FindPropertyRelative("volume").floatValue;
            s.pitch = property.FindPropertyRelative("pitch").floatValue;
            s.loop = property.FindPropertyRelative("loop").boolValue;
            s.clip = clip;
            s.Play();
        }

        void Stop(SerializedProperty property)
        {
            MonoBehaviour m = GetValidMonoBehaviour(property);
            if (m == null) {
                DeEditorSoundUtils.StopAll();
                return;
            }
            AudioSource s = m.GetComponent<AudioSource>();
            if (s == null) return;

            s.Stop();
        }

        void AdjustVolume(SerializedProperty property, float volume)
        {
            MonoBehaviour m = GetValidMonoBehaviour(property);
            if (m == null) return;
            AudioSource s = m.GetComponent<AudioSource>();
            if (s != null && s.isPlaying) s.volume = volume;
        }

        void AdjustPitch(SerializedProperty property, float pitch)
        {
            MonoBehaviour m = GetValidMonoBehaviour(property);
            if (m == null) return;
            AudioSource s = m.GetComponent<AudioSource>();
            if (s != null && s.isPlaying) s.pitch = pitch;
        }

        MonoBehaviour GetValidMonoBehaviour(SerializedProperty property)
        {
            MonoBehaviour m = property.serializedObject.targetObject as MonoBehaviour;
            if (m == null) return null;
            PrefabType pType = PrefabUtility.GetPrefabType(m);
            if (pType == PrefabType.Prefab || pType == PrefabType.ModelPrefab) return null;
            return m;
        }
    }
}