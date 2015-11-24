// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 17:00
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeSoundUtils
    {
        #region Public Methods

        /// <summary>
        /// Plays the given clip in the Editor
        /// </summary>
        public static void Play(AudioClip audioClip)
        {
            if (audioClip == null) return;

            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType()
                .GetMethod("PlayClip", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder, new[] { typeof(AudioClip) }, null);
            mInfo.Invoke(null, new object[] { audioClip });
        }

        /// <summary>
        /// Stops playing the given clip.
        /// </summary>
        public static void Stop(AudioClip audioClip)
        {
            if (audioClip == null) return;

            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType()
                .GetMethod("StopClip", BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod, Type.DefaultBinder, new[] { typeof(AudioClip) }, null);
            mInfo.Invoke(null, new object[] { audioClip });
        }

        /// <summary>
        /// Stops all clips playing.
        /// </summary>
        public static void StopAll()
        {
            Assembly editorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
            MethodInfo mInfo = editorAssembly.CreateInstance("UnityEditor.AudioUtil").GetType().GetMethod("StopAllClips", BindingFlags.Static | BindingFlags.Public);
            mInfo.Invoke(null, null);
        }

        #endregion
    }
}