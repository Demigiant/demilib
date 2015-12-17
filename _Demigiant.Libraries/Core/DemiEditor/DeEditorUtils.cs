// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/15 00:33
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorUtils
    {
        static readonly List<DelayedCall> _DelayedCalls = new List<DelayedCall>();

        /// <summary>Calls the given action after the given delay</summary>
        public static DelayedCall DelayedCall(float delay, Action callback)
        {
            DelayedCall res = new DelayedCall(delay, callback);
            _DelayedCalls.Add(res);
            return res;
        }

        public static void ClearAllDelayedCalls()
        {
            foreach (DelayedCall dc in _DelayedCalls) dc.Clear();
            _DelayedCalls.Clear();
        }

        public static void ClearDelayedCall(DelayedCall call)
        {
            call.Clear();
            int index = _DelayedCalls.IndexOf(call);
            if (index == -1) return;
            _DelayedCalls.Remove(call);
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

        public void Clear()
        {
            if (EditorApplication.update != null) EditorApplication.update -= Update;
            callback = null;
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