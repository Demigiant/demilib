// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/12/07 13:24
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.BuildPanel
{
    [Serializable]
    public class DeBuildPanelData : ScriptableObject
    {
        #region Serialized

        public string suffix = "";
        public List<Build> builds = new List<Build>();

        #endregion

        public const string Version = "0.9.000";
        internal static readonly BuildTarget[] AllowedBuildTargets = new [] {
            BuildTarget.StandaloneWindows64,
            BuildTarget.StandaloneOSX,
            BuildTarget.Android,
            BuildTarget.iOS,
        };

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class Build
        {
            public BuildTarget buildTarget;
            public bool foldout = true;
            public bool enabled = true;
            public string buildFolder; // Relative to project directory
            public bool clearBuildFolder = true;
            public string bundleIdentifier;
            // Android/iOS only
            public string key;
            public bool increaseInternalBuildNumber = false;

            public Build(BuildTarget buildTarget)
            {
                this.buildTarget = buildTarget;
            }
        }
    }
}