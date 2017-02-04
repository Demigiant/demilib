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
        static readonly List<GameObject> _rootGOs = new List<GameObject>(500);

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

        /// <summary>
        /// Returns all components of type T in the currently open scene, or NULL if none could be found.<para/>
        /// If you're on Unity 5 or later, and have <code>DeEditorTools</code>, use <code>DeEditorToolsUtils.FindAllComponentsOfType</code>
        /// instead, which is more efficient.
        /// </summary>
        public static List<T> FindAllComponentsOfType<T>() where T : Component
        {
            GameObject[] allGOs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            if (allGOs == null) return null;
            List<T> result = null;
            foreach (GameObject go in allGOs) {
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) continue;
                T[] components = go.GetComponentsInChildren<T>();
                if (components.Length == 0) continue;
                if (result == null) result = new List<T>();
                foreach (T component in components) {
                    result.Add(component);
                }
            }
            return result;
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