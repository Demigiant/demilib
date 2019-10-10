// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 17:00
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorSoundUtils
    {
        static MethodInfo _miPlay, _miStop, _miStopAll;

        #region Public Methods

        /// <summary>
        /// Plays the given clip in the Editor
        /// </summary>
        public static void Play(AudioClip audioClip)
        {
            if (audioClip == null) return;

            if (_miPlay == null) {
                Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
                Type t = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType();
                if (DeUnityEditorVersion.MajorVersion < 2019) {
                    _miPlay = t.GetMethod(
                        "PlayClip",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder,
                        new[] {typeof(AudioClip)}, null
                    );
                } else {
                    _miPlay = t.GetMethod(
                        "PlayClip",
                        BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder,
                        new[] { typeof(AudioClip), typeof(int), typeof(bool) }, null
                    );
                }
            }
            if (DeUnityEditorVersion.MajorVersion < 2019) {
                _miPlay.Invoke(null, new object[] {audioClip});
            } else {
                _miPlay.Invoke(null, new object[] {audioClip, 0, false});
            }
        }

        /// <summary>
        /// Stops playing the given clip.
        /// </summary>
        public static void Stop(AudioClip audioClip)
        {
            if (audioClip == null) return;

            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            if (_miStop == null) {
                Type t = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType();
                _miStop = t.GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder, new[] {typeof(AudioClip)}, null);
            }
            _miStop.Invoke(null, new object[] { audioClip });
        }

        /// <summary>
        /// Stops all clips playing.
        /// </summary>
        public static void StopAll()
        {
            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            if (_miStopAll == null) {
                Type t = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType();
                _miStopAll = t.GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
            }
            _miStopAll.Invoke(null, null);
        }

        #endregion
    }
}