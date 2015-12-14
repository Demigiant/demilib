// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/15 00:33
// License Copyright (c) Daniele Giardini

using System;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorUtils
    {
        /// <summary>Calls the given action after the given delay</summary>
        public static void DelayedCall(float delay, Action callback)
        {
            new DelayedCall(delay, callback);
        }
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    public class DelayedCall
    {
        public float delay;
        public Action callback;
        readonly float _startupTime;

        public DelayedCall(float delay, Action callback)
        {
            this.delay = delay;
            this.callback = callback;
            _startupTime = Time.realtimeSinceStartup;
            EditorApplication.update += Update;
        }

        void Update()
        {
            if (Time.realtimeSinceStartup - _startupTime >= delay) {
                if (EditorApplication.update != null) EditorApplication.update -= Update;
                if (callback != null) callback();
            }
        }
    }
}