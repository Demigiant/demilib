// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/04/13 11:11
// License Copyright (c) Daniele Giardini

using System;
using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeAudioEditor
{
    public enum DeAudioGUIMode
    {
        /// <summary>1 row > AudioClip only</summary>
        ClipOnly, // Only clip, no groupId
        /// <summary>1 row > AudioClip and GroupId</summary>
        Compact,
        /// <summary>1 row > AudioClip, GroupId and play/stop buttons</summary>
        CompactWithPreview,
        /// <summary>2 rows > AudioClip, GroupId, Volume, play/stop buttons</summary>
        VolumeWithPreview,
        /// <summary>2 rows > AudioClip, GroupId, Volume, play/stop/loops buttons</summary>
        VolumeAndLoopsWithPreview,
        /// <summary>3 rows > All options</summary>
        Full
    }

    /// <summary>
    /// GUILayout methods for drawing DeAudio data objects
    /// </summary>
    public static class DeAudioGUILayout
    {
        public static DeAudioClipData DeAudioClip(string label, DeAudioClipData value, bool allowGroupChange = true, DeAudioGUIMode mode = DeAudioGUIMode.Full)
        { return DeAudioClip(new GUIContent(label, ""), value, allowGroupChange, mode); }
        public static DeAudioClipData DeAudioClip(GUIContent label, DeAudioClipData value, bool allowGroupChange = true, DeAudioGUIMode mode = DeAudioGUIMode.Full)
        {
            using (new GUILayout.HorizontalScope()) {
                value.clip = EditorGUILayout.ObjectField(label, value.clip, typeof(AudioClip), false) as AudioClip;
                if (mode != DeAudioGUIMode.ClipOnly) {
                    using (new EditorGUI.DisabledGroupScope(!allowGroupChange)) {
                        DeAudioGroupId newGroupId = (DeAudioGroupId)EditorGUILayout.EnumPopup(value.groupId, GUILayout.Width(78));
                        if (allowGroupChange) value.groupId = newGroupId;
                    }
                    if (mode == DeAudioGUIMode.CompactWithPreview) {
                        DeAudioGUI.PlayButton(value);
                        DeAudioGUI.StopButton();
                    }
                }
            }
            if (mode == DeAudioGUIMode.VolumeWithPreview || mode == DeAudioGUIMode.VolumeAndLoopsWithPreview || mode == DeAudioGUIMode.Full) {
                // Volume, pitch, play/pause/loop buttons
                GUILayout.Space(-1);
                using (new GUILayout.HorizontalScope()) {
                    value.volume = EditorGUILayout.Slider("└ Volume", value.volume, 0, 1);
                    if (mode == DeAudioGUIMode.VolumeAndLoopsWithPreview) DeAudioGUI.LoopToggle(value);
                    DeAudioGUI.PlayButton(value);
                    DeAudioGUI.StopButton();
                }
                if (mode == DeAudioGUIMode.Full) {
                    GUILayout.Space(-1);
                    using (new GUILayout.HorizontalScope()) {
                        value.pitch = EditorGUILayout.Slider("└ Pitch", value.pitch, 0, 3);
                        DeAudioGUI.LoopToggle(value);
                    }
                } else GUILayout.Space(4);
            } else if (mode == DeAudioGUIMode.CompactWithPreview) GUILayout.Space(4);
            return value;
        }

        #region GUI Helpers

        static class DeAudioGUI
        {
            public static void LoopToggle(DeAudioClipData value)
            {
                value.loop = DeGUILayout.ToggleButton(value.loop, "Loop", GUILayout.Width(44), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight - 1));
            }

            public static void PlayButton(DeAudioClipData value)
            {
                if (GUILayout.Button("►", DeGUI.styles.button.tool, GUILayout.Width(22), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight))) {
                    DeEditorSoundUtils.StopAll();
                    if (value.clip != null) DeEditorSoundUtils.Play(value.clip);
                }
            }

            public static void StopButton()
            {
                if (GUILayout.Button("■", DeGUI.styles.button.tool, GUILayout.Width(22), GUILayout.MaxHeight(EditorGUIUtility.singleLineHeight))) {
                    DeEditorSoundUtils.StopAll();
                }
            }
        }

        #endregion
    }
}