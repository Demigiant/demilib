// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2022/01/26
// License Copyright (c) Daniele Giardini

using System;
using UnityEditor;

namespace DG.DemiEditor
{
    [InitializeOnLoad]
    public static class DeEditorNotification
    {
        #region EVENTS

        /// <summary>
        /// Dispatched when Unity has finished compiling code and updating the AssetDatabase
        /// </summary>
        public static event Action OnUnityReady;

        static void Dispatch_OnUnityReady() { if (OnUnityReady != null) OnUnityReady(); }

        #endregion

        static DeEditorNotification()
        {
            if (DeEditorUtils.isUnityReady) Dispatch_OnUnityReady();
            else EditorApplication.update += OnUnityReadyChecker;
        }

        #region Methods

        static void OnUnityReadyChecker()
        {
            if (!DeEditorUtils.isUnityReady) return;
            EditorApplication.update -= OnUnityReadyChecker;
            Dispatch_OnUnityReady();
        }

        #endregion
    }
}