// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/12/07 13:24
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.BuildPanel
{
    [Serializable]
    public class DeBuildPanelData : ScriptableObject
    {
        public enum AffixTarget // Corresponds to BuildTarget[] indexes
        {
            All,
            Windows,
            OSX,
            Linux,
            Android,
            iOS,
            WebGL,
            PS4,
            XboxOne,
            Switch
        }

        #region Serialized

        public List<Affix> prefixes = new List<Affix>();
        public List<Affix> suffixes = new List<Affix>();
        public List<Build> builds = new List<Build>();

        #endregion

        public const string Version = "1.0.080";
        internal static readonly BuildTarget[] AllowedBuildTargets = new [] {
            BuildTarget.NoTarget, // Here so indexes correspond to AffixTarget enum
            BuildTarget.StandaloneWindows64,
            BuildTarget.StandaloneOSX,
            BuildTarget.StandaloneLinuxUniversal,
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.WebGL,
            BuildTarget.PS4,
            BuildTarget.XboxOne,
            BuildTarget.Switch
        };

        internal BuildTarget AffixTargetToBuildTarget(AffixTarget affixTarget)
        {
            return AllowedBuildTargets[(int)affixTarget];
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class Affix
        {
            public bool enabled = true;
            public bool enabledForInnerExecutable = false; // If TRUE also uses this affix when determining the fileName on standalone platforms
            public AffixTarget target = AffixTarget.All;
            public string text;

            // Returns the right text value, evaluating it as a static string property in case it starts with a @
            public string GetText(bool logErrors = false)
            {
                if (string.IsNullOrEmpty(text) || !text.StartsWith("@")) return text;
                return text.Substring(1).EvalAsProperty<string>(null, logErrors);
            }

            public bool IsEnabledFor(BuildTarget buildTarget, bool isInnerExecutable, DeBuildPanelData src)
            {
                return enabled
                       && (target == AffixTarget.All || src.AffixTargetToBuildTarget(target) == buildTarget)
                       && (!isInnerExecutable || enabledForInnerExecutable);
            }
        }
        
        [Serializable]
        public class Build
        {
            public BuildTarget buildTarget;
            public bool foldout = true;
            public bool enabled = true;
            public string buildFolder; // Relative to project directory
            public string buildName; // Folder within buildFolder where the build will be created (this one is created if it doesn't exist)
            public bool clearBuildFolder = true;
            public string bundleIdentifier;
            public bool deleteBackupThisFolder = true;
            // Android/iOS only
            public string key;
            public bool increaseInternalBuildNumber = false;

            public Build(BuildTarget buildTarget)
            {
                this.buildTarget = buildTarget;
            }

            public bool HasInnerExecutable()
            {
                switch (buildTarget) {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return true;
                default:
                    return false;
                }
            }

            public bool BuildsDirectlyToFile()
            {
                switch (buildTarget) {
                case BuildTarget.StandaloneOSX:
                case BuildTarget.Android:
                    return true;
                default:
                    return false;
                }
            }
        }
    }
}