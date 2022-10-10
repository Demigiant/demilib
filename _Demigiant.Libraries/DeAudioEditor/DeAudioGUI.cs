// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/07/24 10:43
// License Copyright (c) Daniele Giardini

using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeAudioEditor
{
    /// <summary>
    /// GUI methods for drawing DeAudio data objects
    /// </summary>
    public static class DeAudioGUI
    {
        internal const int VSpace = 2;
        const int _GroupW = 78;
        const int _ControlButtonW = 19;
        const int _LoopToggleW = 38;

        #region Public Methods

        public static int GetRequiredHeight(DeAudioClipGUIMode mode = DeAudioClipGUIMode.Full)
        {
            switch (mode) {
            case DeAudioClipGUIMode.ClipOnly:
            case DeAudioClipGUIMode.Compact:
            case DeAudioClipGUIMode.CompactPreviewOnly:
            case DeAudioClipGUIMode.CompactWithGroupAndPreview:
                return (int)EditorGUIUtility.singleLineHeight + VSpace;
            case DeAudioClipGUIMode.VolumeWithPreview:
            case DeAudioClipGUIMode.VolumeAndLoopsWithPreview:
                return (int)EditorGUIUtility.singleLineHeight * 2 + VSpace;
            default:
                return (int)EditorGUIUtility.singleLineHeight * 3 + VSpace * 2;
            }
        }

        public static DeAudioClipData DeAudioClip(Rect rect, string label, DeAudioClipData value, bool allowGroupChange = true, DeAudioClipGUIMode mode = DeAudioClipGUIMode.Full)
        { return DeAudioClip(rect, new GUIContent(label, ""), value, allowGroupChange, mode); }
        public static DeAudioClipData DeAudioClip(Rect rect, GUIContent label, DeAudioClipData value, bool allowGroupChange = true, DeAudioClipGUIMode mode = DeAudioClipGUIMode.Full)
        {
            Styles.Init();

            bool hasGroup = false;
            bool hasPlayStopButtonsInFirstRow = false;
            bool hasPlayStopButtonsInSecondRow = false;
            bool hasVolume = false;
            bool hasLoopInSecondRow = false;
            bool hasLoopInThirdRow = false;
            bool hasPitch = false;
            if (mode != DeAudioClipGUIMode.ClipOnly) {
                if (mode != DeAudioClipGUIMode.CompactPreviewOnly && mode != DeAudioClipGUIMode.FullNoGroup) hasGroup = true;
                switch (mode) {
                case DeAudioClipGUIMode.CompactPreviewOnly:
                case DeAudioClipGUIMode.CompactWithGroupAndPreview:
                    hasPlayStopButtonsInFirstRow = true;
                    break;
                case DeAudioClipGUIMode.VolumeWithPreview:
                case DeAudioClipGUIMode.VolumeAndLoopsWithPreview:
                case DeAudioClipGUIMode.FullNoGroup:
                case DeAudioClipGUIMode.Full:
                    hasVolume = true;
                    hasPlayStopButtonsInSecondRow = true;
                    if (mode == DeAudioClipGUIMode.VolumeAndLoopsWithPreview) hasLoopInSecondRow = true;
                    if (mode == DeAudioClipGUIMode.FullNoGroup || mode == DeAudioClipGUIMode.Full) {
                        hasPitch = true;
                        hasLoopInThirdRow = true;
                    }
                    break;
                }
            }

            Rect lineR = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
            // First line
            if (hasPlayStopButtonsInFirstRow) {
                Rect btStopR = new Rect(lineR.xMax - _ControlButtonW, lineR.y, _ControlButtonW, lineR.height);
                Rect btPlayR = new Rect(btStopR.x - _ControlButtonW, btStopR.y, btStopR.width, btStopR.height);
                lineR.width -= _ControlButtonW * 2;
                PlayButton(btPlayR, value);
                StopButton(btStopR);
            }
            if (hasGroup) {
                Rect groupR = new Rect(lineR.xMax - _GroupW, lineR.y, _GroupW, lineR.height);
                lineR.width -= groupR.width;
                using (new EditorGUI.DisabledGroupScope(!allowGroupChange)) {
                    DeAudioGroupId newGroupId = (DeAudioGroupId)EditorGUI.EnumPopup(groupR, value.groupId);
                    if (allowGroupChange) value.groupId = newGroupId;
                }
            }
            Rect clipR = lineR;
            value.clip = label.text == ""
                ? EditorGUI.ObjectField(clipR, value.clip, typeof(AudioClip), false) as AudioClip
                : EditorGUI.ObjectField(clipR, label, value.clip, typeof(AudioClip), false) as AudioClip;
            // Second line
            lineR.y = lineR.yMax + VSpace;
            lineR.width = rect.width;
            if (hasPlayStopButtonsInSecondRow) {
                Rect btStopR = new Rect(lineR.xMax - _ControlButtonW, lineR.y, _ControlButtonW, lineR.height);
                Rect btPlayR = new Rect(btStopR.x - _ControlButtonW, btStopR.y, btStopR.width, btStopR.height);
                lineR.width -= _ControlButtonW * 2;
                PlayButton(btPlayR, value);
                StopButton(btStopR);
            }
            if (hasLoopInSecondRow) {
                Rect loopR = new Rect(lineR.xMax - _LoopToggleW, lineR.y, _LoopToggleW, lineR.height);
                lineR.width -= loopR.width;
                LoopToggle(loopR, value);
            }
            if (hasVolume) {
                Rect volumeR = lineR;
                using (new DeGUI.LabelFieldWidthScope(60)) {
                    value.volume = EditorGUI.Slider(volumeR, "└ Volume", value.volume, 0, 1);
                }
            }
            // Third line
            lineR.y = lineR.yMax + VSpace;
            lineR.width = rect.width;
            if (hasLoopInThirdRow) {
                Rect loopR = new Rect(lineR.xMax - _LoopToggleW, lineR.y, _LoopToggleW, lineR.height);
                lineR.width -= loopR.width;
                LoopToggle(loopR, value);
            }
            if (hasPitch) {
                Rect pitchR = lineR;
                using (new DeGUI.LabelFieldWidthScope(60)) {
                    value.pitch = EditorGUI.Slider(pitchR, "└ Pitch", value.pitch, 0, 3);
                }
            }

            return value;
        }

        #endregion

        #region GUI Helpers

        static void LoopToggle(Rect r, DeAudioClipData value)
        {
            value.loop = DeGUI.ToggleButton(r, value.loop, "Loop");
        }

        static void PlayButton(Rect r, DeAudioClipData value)
        {
            if (GUI.Button(r, "►", Styles.btPlay)) {
                DeEditorSoundUtils.StopAll();
                if (value.clip != null) DeEditorSoundUtils.Play(value.clip);
            }
        }

        static void StopButton(Rect r)
        {
            if (GUI.Button(r, "■", Styles.btStop)) {
                DeEditorSoundUtils.StopAll();
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;
            public static GUIStyle btPlay, btStop;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                btPlay = DeGUI.styles.button.toolNoFixedH.Clone().ContentOffset(1, 0);
                btStop = DeGUI.styles.button.toolNoFixedH.Clone().ContentOffsetY(-1);
            }
        }
    }
}