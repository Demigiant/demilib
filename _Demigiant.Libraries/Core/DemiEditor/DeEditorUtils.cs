// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/15 00:33
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorUtils
    {
        static readonly List<DelayedCall> _DelayedCalls = new List<DelayedCall>();
        static MethodInfo _clearConsoleMI;

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

        /// <summary>
        /// Return the size of the editor game view, eventual extra bars excluded (meaning the true size of the game area)
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetGameViewSize()
        {
            return Handles.GetMainGameViewSize();
        }

        /// <summary>
        /// Clears all logs from Unity's console
        /// </summary>
        public static void ClearConsole()
        {
            if (_clearConsoleMI == null) {
                Type logEntries = Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
                if (logEntries != null) _clearConsoleMI = logEntries.GetMethod("Clear", BindingFlags.Static | BindingFlags.Public);
                if (_clearConsoleMI == null) return;
            }
            _clearConsoleMI.Invoke(null,null);
        }

        public static Color HexToColor(string hex)
        {
            if (hex[0] == '#') hex = hex.Substring(1);
            float r = (HexToInt(hex[1]) + HexToInt(hex[0]) * 16f) / 255f;
            float g = (HexToInt(hex[3]) + HexToInt(hex[2]) * 16f) / 255f;
            float b = (HexToInt(hex[5]) + HexToInt(hex[4]) * 16f) / 255f;
            return new Color(r, g, b, 1);
        }
        static int HexToInt(char hexVal)
        {
            return int.Parse(hexVal.ToString(), System.Globalization.NumberStyles.HexNumber);
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