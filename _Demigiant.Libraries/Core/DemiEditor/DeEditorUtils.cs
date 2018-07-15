// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/15 00:33
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class DeEditorUtils
    {
        static readonly List<DelayedCall> _DelayedCalls = new List<DelayedCall>();
        static MethodInfo _clearConsoleMI;
        static readonly List<GameObject> _RootGOs = new List<GameObject>(500);
        static readonly StringBuilder _Strb = new StringBuilder();
        static MethodInfo _miGetTargetStringFromBuildTargetGroup;
        static MethodInfo _miGetPlatformNameFromBuildTargetGroup;

        #region Public Methods

        #region DelayedCall

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

        #endregion

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
        /// Adds the given global define (if it's not already present) to all the <see cref="BuildTargetGroup"/>
        /// or only to the given <see cref="BuildTargetGroup"/>, depending on passed parameters,
        /// and returns TRUE if it was added, FALSE otherwise.<para/>
        /// NOTE: when adding to all of them some legacy warnings might appear, which you can ignore.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to add to all of them.</param>
        public static bool AddGlobalDefine(string id, BuildTargetGroup? buildTargetGroup = null)
        {
            bool added = false;
            BuildTargetGroup[] targetGroups = buildTargetGroup == null
                ? (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))
                : new[] {(BuildTargetGroup)buildTargetGroup};
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) != -1) continue; // Already present
                added = true;
                defs += defs.Length > 0 ? ";" + id : id;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, defs);
            }
            return added;
        }

        /// <summary>
        /// Removes the given global define (if present) from all the <see cref="BuildTargetGroup"/>
        /// or only from the given <see cref="BuildTargetGroup"/>, depending on passed parameters,
        /// and returns TRUE if it was removed, FALSE otherwise.<para/>
        /// NOTE: when removing from all of them some legacy warnings might appear, which you can ignore.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to remove from all of them.</param>
        public static bool RemoveGlobalDefine(string id, BuildTargetGroup? buildTargetGroup = null)
        {
            bool removed = false;
            BuildTargetGroup[] targetGroups = buildTargetGroup == null
                ? (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))
                : new[] {(BuildTargetGroup)buildTargetGroup};
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) == -1) continue; // Not present
                removed = true;
                _Strb.Length = 0;
                for (int i = 0; i < singleDefs.Length; ++i) {
                    if (singleDefs[i] == id) continue;
                    if (_Strb.Length > 0) _Strb.Append(';');
                    _Strb.Append(singleDefs[i]);
                }
                PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, _Strb.ToString());
            }
            _Strb.Length = 0;
            return removed;
        }

        /// <summary>
        /// Returns TRUE if the given global define is present in all the <see cref="BuildTargetGroup"/>
        /// or only in the given <see cref="BuildTargetGroup"/>, depending on passed parameters.<para/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="buildTargetGroup"><see cref="BuildTargetGroup"/>to use. Leave NULL to check in all of them.</param>
        public static bool HasGlobalDefine(string id, BuildTargetGroup? buildTargetGroup = null)
        {
            BuildTargetGroup[] targetGroups = buildTargetGroup == null
                ? (BuildTargetGroup[])Enum.GetValues(typeof(BuildTargetGroup))
                : new[] {(BuildTargetGroup)buildTargetGroup};
            foreach(BuildTargetGroup btg in targetGroups) {
                if (!IsValidBuildTargetGroup(btg)) continue;
                string defs = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
                string[] singleDefs = defs.Split(';');
                if (Array.IndexOf(singleDefs, id) != -1) return true;
            }
            return false;
        }

        #region Legacy

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

        #endregion

        #endregion

        #region Methods

        static bool IsValidBuildTargetGroup(BuildTargetGroup group)
        {
            if (group == BuildTargetGroup.Unknown) return false;

            if (_miGetTargetStringFromBuildTargetGroup == null) {
                Type moduleManager = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
//            MethodInfo miIsPlatformSupportLoaded = moduleManager.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
                _miGetTargetStringFromBuildTargetGroup = moduleManager.GetMethod(
                    "GetTargetStringFromBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic
                );
                _miGetPlatformNameFromBuildTargetGroup = typeof(PlayerSettings).GetMethod(
                    "GetPlatformName", BindingFlags.Static | BindingFlags.NonPublic
                );
            }

            string targetString = (string)_miGetTargetStringFromBuildTargetGroup.Invoke(null, new object[] {group});
            string platformName = (string)_miGetPlatformNameFromBuildTargetGroup.Invoke(null, new object[] {group});

            // Group is valid if at least one betweeen targetString and platformName is not empty.
            // This seems to me the safest and more reliant way to check,
            // since ModuleManager.IsPlatformSupportLoaded dosn't work well with BuildTargetGroup (only BuildTarget)
            return !string.IsNullOrEmpty(targetString) || !string.IsNullOrEmpty(platformName);
        }

        #endregion
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