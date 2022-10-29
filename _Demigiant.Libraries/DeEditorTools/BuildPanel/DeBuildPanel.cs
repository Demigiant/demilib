// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/12/07 13:24
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Format = DG.DemiEditor.Format;

namespace DG.DeEditorTools.BuildPanel
{
    public class DeBuildPanel : EditorWindow
    {
        public enum OnWillBuildResult
        {
            /// <summary>Continue</summary>
            Continue,
            /// <summary>Cancel this build</summary>
            Cancel,
            /// <summary>Cancel all builds in queue</summary>
            CancelAll
        }

        enum DeBuildResult
        {
            Success,
            Failed,
            Canceled,
            CancelAll
        }

        #region HOOKS

        /// <summary>
        /// Called when assigning the build's name. Hook to this to modify it and return the one you want.<para/>
        /// Must return a string with the name to use</summary>
        public static event OnBuildNameRequestHandler OnBuildNameRequest;
        public delegate string OnBuildNameRequestHandler(BuildTarget buildTarget, string buildName);
        static string Dispatch_OnBuildNameRequest(BuildTarget buildTarget, string buildName)
        {
            return OnBuildNameRequest == null ? buildName : OnBuildNameRequest(buildTarget, buildName);
        }

        /// <summary>
        /// Called before a build starts.<para/>
        /// Must return an <code>OnWillBuildResult</code> indicating if you wish to continue
        /// </summary>
        public static event OnWillBuildHandler OnWillBuild;
        public delegate OnWillBuildResult OnWillBuildHandler(BuildTarget buildTarget, bool isFirstBuildOfQueue);
        static OnWillBuildResult Dispatch_OnWillBuild(BuildTarget buildTarget, bool isFirstBuildOfQueue)
        {
            return OnWillBuild == null ? OnWillBuildResult.Continue : OnWillBuild(buildTarget, isFirstBuildOfQueue);
        }

        #endregion
        
        [MenuItem("Tools/Demigiant/" + _Title)]
        static void ShowWindow() { GetWindow(typeof(DeBuildPanel), false, _Title); }
        
        const string _Title = "Simple Build Panel";
        const string _SrcADBFilePath = "Assets/-DeBuildPanelData.asset";
        static readonly StringBuilder _Strb = new StringBuilder();
        static readonly StringBuilder _StrbAlt = new StringBuilder();
        DeBuildPanelData _src;
        Vector2 _scrollPos;
        const int _LabelWidth = 116;
        string _buildFolderComment;
        string[] _buildPathsLabels;
        // Includes only enabled builds, sorted so current platform is always first
        readonly List<DeBuildPanelData.Build> _sortedFilteredBuilds = new List<DeBuildPanelData.Build>();

        #region Unity and GUI Methods

        void OnEnable()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<DeBuildPanelData>(_SrcADBFilePath, true);
            RefreshBuildPathsLabels();
            _buildFolderComment = string.Format(
                "The build folder is relative to your Unity's project folder:\n\n\"{0}/\"\n\nYou can use \"../\" to navigate backwards",
                DeEditorFileUtils.projectPath.Replace('\\', '/')
            );

            Undo.undoRedoPerformed += Repaint;
        }

        void OnDisable()
        { Undo.undoRedoPerformed -= Repaint; }

        void OnFocus()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<DeBuildPanelData>(_SrcADBFilePath, true);
            RefreshBuildPathsLabels();
        }

        void OnGUI()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<DeBuildPanelData>(_SrcADBFilePath, true);
            Undo.RecordObject(_src, _Title);
            DeGUI.BeginGUI();
            Styles.Init();
            
            // Main toolbar
            using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.large)) {
                GUILayout.Label("v" + DeBuildPanelData.Version, DeGUI.styles.label.toolbarL);
                GUILayout.FlexibleSpace();
                using (new DeGUI.ColorScope(new DeSkinColor(0.9f, 0.65f))) {
                    if (GUILayout.Button("BUILD ALL ENABLED", DeGUI.styles.button.toolL)) {
                        BuildAllEnabled();
                    }
                    GUILayout.Space(2);
                    if (GUILayout.Button("+ Add platform", DeGUI.styles.button.toolL)) {
                        CM_AddPlatform();
                    }
                    GUILayout.Space(2);
                    if (GUILayout.Button("▲", DeGUI.styles.button.toolL)) {
                        foreach (DeBuildPanelData.Build build in _src.builds) build.foldout = false;
                        GUI.changed = true;
                    }
                    if (GUILayout.Button("▼", DeGUI.styles.button.toolL)) {
                        foreach (DeBuildPanelData.Build build in _src.builds) build.foldout = true;
                        GUI.changed = true;
                    }
                }
            }

            _scrollPos = GUILayout.BeginScrollView(_scrollPos);

            // Affixes
            GUILayout.Space(3);
            using (new DeGUI.LabelFieldWidthScope(_LabelWidth + 4))
            using (new GUILayout.VerticalScope(DeGUI.styles.box.def))
            using (var check = new EditorGUI.ChangeCheckScope()) {
                // Prefixes
                using (new DeGUILayout.ToolbarScope()) {
                    GUILayout.Label("Prefixes", DeGUI.styles.label.toolbar);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+ Add Prefix", DeGUI.styles.button.tool)) {
                        _src.prefixes.Add(new DeBuildPanelData.Affix());
                        GUI.changed = true;
                    }
                }
                if (_src.prefixes.Count > 0) {
                    using (new GUILayout.VerticalScope(DeGUI.styles.box.sticky)) {
                        DrawAffixes(_src.prefixes);
                    }
                }
                // Suffixes
                using (new DeGUILayout.ToolbarScope()) {
                    GUILayout.Label("Suffixes", DeGUI.styles.label.toolbar);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("+ Add Suffix", DeGUI.styles.button.tool)) {
                        _src.suffixes.Add(new DeBuildPanelData.Affix());
                        GUI.changed = true;
                    }
                }
                if (_src.suffixes.Count > 0) {
                    using (new GUILayout.VerticalScope(DeGUI.styles.box.sticky)) {
                        DrawAffixes(_src.suffixes);
                    }
                }
                if (check.changed) RefreshBuildPathsLabels();
            }
            GUILayout.Space(4);

            // Builds
            for (int i = 0; i < _src.builds.Count; ++i) {
                using (new DeGUI.LabelFieldWidthScope(_LabelWidth))
                using (new GUILayout.VerticalScope(Styles.buildContainer)) {
                    if (!DrawBuild(i)) {
                        i--;
                        GUI.changed = true;
                    }
                }
                if (DeGUIDrag.Drag(_src.builds, i).outcome == DeDragResultType.Accepted) {
                    EditorUtility.SetDirty(_src);
                }
            }

            GUILayout.EndScrollView();
            if (GUI.changed) {
                RefreshBuildPathsLabels();
                EditorUtility.SetDirty(_src);
            }
        }

        void DrawAffixes(List<DeBuildPanelData.Affix> affixes)
        {
            GUILayout.Label(
                "If you start an affix with \"@\" it will be evaluated as a static string property/field via Reflection",
                Styles.labelAffixComment
            );
            for (int i = 0; i < affixes.Count; ++i) {
                DeBuildPanelData.Affix affix = affixes[i];
                using (new GUILayout.HorizontalScope()) {
                    if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                        DeGUIDrag.StartDrag(this, affixes, i);
                    }
                    using (new DeGUI.ColorScope(affix.enabled ? Color.white : (Color)new DeSkinColor(0.7f, 0.7f), affix.enabled ? Color.white : (Color)new DeSkinColor(0.7f, 0.5f))) {
                        affix.target = (DeBuildPanelData.AffixTarget)EditorGUILayout.EnumPopup(affix.target, GUILayout.Width(74));
                        affix.text = EditorGUILayout.TextField(affix.text);
                    }
                    if (!string.IsNullOrEmpty(affix.text) && affix.text.StartsWith("@")) {
                        using (new DeGUI.ColorScope(new DeSkinColor(0.9f, 0.2f), new DeSkinColor(0.3f, 0.8f))) {
                            if (GUILayout.Button("Test", Styles.btInline, GUILayout.ExpandWidth(false))) {
                                Debug.Log(affix.GetText(true));
                            }
                        }
                    }
                    switch (affix.target) {
                    case DeBuildPanelData.AffixTarget.All:
                    case DeBuildPanelData.AffixTarget.Windows:
                    case DeBuildPanelData.AffixTarget.Linux:
                        using (new EditorGUI.DisabledScope(!affix.enabled)) {
                            affix.enabledForInnerExecutable = DeGUILayout.ToggleButton(
                                affix.enabledForInnerExecutable,
                                new GUIContent("Win/Linux Filename", "If toggled applies this also to the filename on WIN/Linux builds only"),
                                Styles.btInlineToggle, GUILayout.Width(121)
                            );
                        }
                        break;
                    }
                    affix.enabled = DeGUILayout.ToggleButton(affix.enabled, "Enabled", Styles.btInlineToggle, GUILayout.Width(60));
                    if (GUILayout.Button("×", Styles.btDeleteBuild)) {
                        affixes.RemoveAt(i);
                        --i;
                        GUI.changed = true;
                    }
                }
                if (DeGUIDrag.Drag(affixes, i).outcome == DeDragResultType.Accepted) {
                    GUI.changed = true;
                }
            }
        }

        // Returns FALSE if the given build was deleted
        bool DrawBuild(int index)
        {
            DeBuildPanelData.Build build = _src.builds[index];

            // Toolbar
            using (new DeGUILayout.ToolbarScope()) {
                if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                    DeGUIDrag.StartDrag(this, _src.builds, index);
                }
                build.foldout = DeGUILayout.ToolbarFoldoutButton(build.foldout, build.buildTarget.ToString(), false, true);
                build.enabled = DeGUILayout.ToggleButton(build.enabled, "Enabled", Styles.btToolbarToggle, GUILayout.Width(60));
                using (new EditorGUI.DisabledScope(!ValidateBuildData(build))) {
                    if (GUILayout.Button("BUILD NOW", DeGUI.styles.button.tool, GUILayout.Width(80))) {
                        Build(build);
                    }
                }
                if (GUILayout.Button("×", Styles.btDeleteBuildToolbar)) {
                    _src.builds.RemoveAt(index);
                    return false;
                }
            }
            // Data
            if (build.foldout) {
                using (new EditorGUI.DisabledScope(!build.enabled))
                using (new GUILayout.VerticalScope(DeGUI.styles.box.stickyTop)) {
                    using (new GUILayout.VerticalScope(Styles.buildPathContainer)) {
                        GUILayout.Label(_buildPathsLabels[index], Styles.labelBuildPath);
                    }
                    using (new GUILayout.HorizontalScope()) {
                        if (!build.BuildsDirectlyToFile() || build.buildTarget == BuildTarget.StandaloneOSX) {
                            build.clearBuildFolder = DeGUILayout.ToggleButton(
                                build.clearBuildFolder,
                                new GUIContent(
                                    "Clear At Build",
                                    "If selected and a build with the same name already exists, deletes the build contents before creating the new build"
                                ),
                                GUILayout.Width(_LabelWidth)
                            );
                        }
                        build.deleteBackupThisFolder = DeGUILayout.ToggleButton(
                            build.deleteBackupThisFolder,
                            new GUIContent(
                                "Delete Debug Folder",
                                "If selected, at the end of the build deletes the eventual \"[game]_BackUpThisFolder_ButDontShipItWithYourGame\" folder generated by IL2CPP for debug purposes"
                            ),
                            GUILayout.ExpandWidth(false)
                        );
                        // Special Android/iOS behaviour
                        bool hasIncreaseInternalBuildNumberOption = false;
                        string increaseInternalBuildNumberOptionButton = "";
                        string increaseInternalBuildNumberOptionDescr = "";
                        switch (build.buildTarget) {
                        case BuildTarget.Android:
                            hasIncreaseInternalBuildNumberOption = true;
                            increaseInternalBuildNumberOptionButton = "Auto-increase bundleVersionCode";
                            increaseInternalBuildNumberOptionDescr = "If selected auto-increases the Android \"bundleVersionCode\" at each build";
                            break;
                        case BuildTarget.iOS:
                            hasIncreaseInternalBuildNumberOption = true;
                            increaseInternalBuildNumberOptionButton = "Auto-increase buildNumber";
                            increaseInternalBuildNumberOptionDescr = "If selected auto-increases the iOS \"buildNumber\" at each build";
                            break;
                        }
                        if (hasIncreaseInternalBuildNumberOption) {
                            build.increaseInternalBuildNumber = DeGUILayout.ToggleButton(
                                build.increaseInternalBuildNumber,
                                new GUIContent(increaseInternalBuildNumberOptionButton, increaseInternalBuildNumberOptionDescr),
                                GUILayout.ExpandWidth(false)
                            );
                        }
                    }
                    using (new DeGUI.ColorScope(string.IsNullOrEmpty(build.buildFolder) ? Color.red : Color.white))
                    using (var check = new EditorGUI.ChangeCheckScope()) {
                        build.buildFolder = EditorGUILayout.TextField(new GUIContent("Build Folder", _buildFolderComment), build.buildFolder);
                        if (check.changed) RefreshBuildPathsLabels();
                    }
                    using (new DeGUI.ColorScope(string.IsNullOrEmpty(build.buildName) ? Color.red : Color.white))
                    using (var check = new EditorGUI.ChangeCheckScope()) {
                        build.buildName = EditorGUILayout.TextField("Build Name", build.buildName);
                        if (check.changed) RefreshBuildPathsLabels();
                    }
                    using (new DeGUI.ColorScope(string.IsNullOrEmpty(build.bundleIdentifier) ? Color.red : Color.white)) {
                        build.bundleIdentifier = EditorGUILayout.TextField("Bundle Identifier", build.bundleIdentifier);
                    }
                    // Special Android/iOS behaviour
                    switch (build.buildTarget) {
                    case BuildTarget.Android:
                        build.key = EditorGUILayout.TextField("Key Alias Password", build.key);
                        break;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Context Menus

        void CM_AddPlatform()
        {
            GenericMenu menu = new GenericMenu();
            foreach (BuildTarget buildTarget in DeBuildPanelData.AllowedBuildTargets) {
                if (buildTarget == BuildTarget.NoTarget) continue;
                menu.AddItem(new GUIContent(buildTarget.ToString()), false, CM_AddPlatform_Apply, buildTarget);
            }
            menu.ShowAsContext();
        }

        void CM_AddPlatform_Apply(object buildTargetObj)
        {
            BuildTarget buildTarget = (BuildTarget)buildTargetObj;
            _src.builds.Add(new DeBuildPanelData.Build(buildTarget));
            EditorUtility.SetDirty(_src);
            RefreshBuildPathsLabels();
        }

        #endregion

        #region Methods

        void BuildAllEnabled()
        {
            EditorUtility.DisplayProgressBar("Build All", "Preparing...", 0.2f);
            // Use delayed call to prevent Unity GUILayout bug
            DeEditorUtils.ClearAllDelayedCalls();
            DeEditorUtils.DelayedCall(0.1f, ()=> DoBuildAllEnabled());
        }

        void DoBuildAllEnabled()
        {
            RefreshSortedFilteredBuilds();
            int totEnabled = _sortedFilteredBuilds.Count;

            if (totEnabled == 0) {
                EditorUtility.DisplayDialog("Build All", "0 platforms enabled, nothing to build", "Ok");
                EditorUtility.ClearProgressBar();
                return;
            }

            bool proceed = EditorUtility.DisplayDialog(
                "Build All",
                string.Format("Build for {0} platform{1}?", totEnabled, totEnabled > 1 ? "s" : ""),
                "Yes", "Cancel"
            );
            if (!proceed) {
                EditorUtility.ClearProgressBar();
                return;
            }

            for (int i = 0; i < _sortedFilteredBuilds.Count; i++) {
                DeBuildPanelData.Build build = _sortedFilteredBuilds[i];
                // Hook ► Verify if build should continue
                OnWillBuildResult onWillBuildResult = Dispatch_OnWillBuild(build.buildTarget, i == 0);
                switch (onWillBuildResult) {
                case OnWillBuildResult.Cancel:
                    EditorUtility.ClearProgressBar();
                    continue;
                case OnWillBuildResult.CancelAll:
                    EditorUtility.ClearProgressBar();
                    return;
                }
                //
                DeBuildResult result = DoBuild(build);
                if (result == DeBuildResult.CancelAll) return;
            }
        }

        void Build(DeBuildPanelData.Build build)
        {
            // Hook ► Verify if build should continue
            OnWillBuildResult onWillBuildResult = Dispatch_OnWillBuild(build.buildTarget, true);
            if (onWillBuildResult != OnWillBuildResult.Continue) return;

            EditorUtility.DisplayProgressBar(string.Format("Build ({0})", build.buildTarget), "Preparing...", 0.2f);
            // Use delayed call to prevent Unity GUILayout bug
            DeEditorUtils.ClearAllDelayedCalls();
            DeEditorUtils.DelayedCall(0.1f, ()=> DoBuild(build));
        }

        // Returns TRUE if all builds in queue should be canceled
        DeBuildResult DoBuild(DeBuildPanelData.Build build)
        {
            string dialogTitle = string.Format("Build ({0})", build.buildTarget);

            if (string.IsNullOrEmpty(build.buildFolder)) {
                bool cancelAll = !EditorUtility.DisplayDialog(dialogTitle, "Build folder can't be empty!", "Ok", "Cancel All");
                EditorUtility.ClearProgressBar();
                return cancelAll ? DeBuildResult.CancelAll : DeBuildResult.Canceled;
            }
            if (string.IsNullOrEmpty(build.bundleIdentifier)) {
                bool cancelAll = !EditorUtility.DisplayDialog(dialogTitle, "Bundle Identifier can't be empty!", "Ok", "Cancel All");
                EditorUtility.ClearProgressBar();
                return cancelAll ? DeBuildResult.CancelAll : DeBuildResult.Canceled;
            }

            string buildFolder = Path.GetFullPath(DeEditorFileUtils.projectPath + "/" + build.buildFolder);

            if (!Directory.Exists(buildFolder)) {
                bool cancelAll = !EditorUtility.DisplayDialog(dialogTitle, string.Format("Build folder doesn't exist!\n\n\"{0}\"", buildFolder), "Ok", "Cancel All");
                EditorUtility.ClearProgressBar();
                return cancelAll ? DeBuildResult.CancelAll : DeBuildResult.Canceled;
            }

            bool buildIsSingleFile = build.BuildsDirectlyToFile();
            string completeBuildFolder = buildIsSingleFile
                ? buildFolder
                : Path.GetFullPath(buildFolder + DeEditorFileUtils.PathSlash + GetFullBuildName(build, false));
            string buildFilePath = build.buildTarget == BuildTarget.iOS || build.buildTarget == BuildTarget.WebGL
                ? completeBuildFolder
                : completeBuildFolder + DeEditorFileUtils.PathSlash + GetFullBuildName(build, true);

            if (!buildIsSingleFile) {
                if (build.clearBuildFolder && Directory.Exists(completeBuildFolder)) {
                    // Clear build folder
                    string[] files = Directory.GetFiles(completeBuildFolder);
                    for (int i = 0; i < files.Length; ++i) File.Delete(files[i]);
                    string[] subdirs = Directory.GetDirectories(completeBuildFolder);
                    for (int i = 0; i < subdirs.Length; ++i) Directory.Delete(subdirs[i], true);
                }
            } else if (build.clearBuildFolder) {
                // Clear build file if it's a directory (OSX)
                if (build.buildTarget == BuildTarget.StandaloneOSX && Directory.Exists(buildFilePath)) Directory.Delete(buildFilePath, true);
            }

            // Build
            switch (build.buildTarget) {
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSX:
            case BuildTarget.StandaloneLinuxUniversal:
            case BuildTarget.StandaloneLinux:
            case BuildTarget.StandaloneLinux64:
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, build.bundleIdentifier);
                // if (PlayerSettings.GetScriptingBackend(BuildTargetGroup.Standalone) == ScriptingImplementation.IL2CPP && !EditorUserBuildSettings.allowDebugging) {
                //     PlayerSettings.SetIl2CppCompilerConfiguration(BuildTargetGroup.Standalone, Il2CppCompilerConfiguration.Release);
                // }
                break;
            case BuildTarget.Android:
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, build.bundleIdentifier);
                PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + (build.increaseInternalBuildNumber ? 1 : 0);
                PlayerSettings.Android.keystorePass = PlayerSettings.Android.keyaliasPass = build.key;
                break;
            case BuildTarget.iOS:
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, build.bundleIdentifier);
                PlayerSettings.iOS.buildNumber = PlayerSettings.iOS.buildNumber + (build.increaseInternalBuildNumber ? 1 : 0);
                break;
            case BuildTarget.WebGL:
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.WebGL, build.bundleIdentifier);
                break;
            }
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            buildOptions.options = BuildOptions.None;
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; ++i) scenes[i] = EditorBuildSettings.scenes[i].path;
            buildOptions.scenes = scenes;
            buildOptions.locationPathName = buildFilePath;
            buildOptions.target = build.buildTarget;
            if (EditorUserBuildSettings.development) buildOptions.options |= BuildOptions.Development;
            if (EditorUserBuildSettings.allowDebugging) buildOptions.options |= BuildOptions.AllowDebugging;
            EditorUtility.ClearProgressBar();
            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);

            if (build.deleteBackupThisFolder) {
                // Can't find a way to prevent Unity from generating this, so I'll delete it afterwards
                string debugDataFolder = Path.GetDirectoryName(buildFilePath)
                                         + DeEditorFileUtils.PathSlash
                                         + build.buildName + "_BackUpThisFolder_ButDontShipItWithYourGame";
                if (Directory.Exists(debugDataFolder)) Directory.Delete(debugDataFolder, true);
            }

            return report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded ? DeBuildResult.Success : DeBuildResult.Failed;
        }

        #endregion

        #region Helpers

        void RefreshSortedFilteredBuilds()
        {
            _sortedFilteredBuilds.Clear();
            BuildTarget currBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            foreach (DeBuildPanelData.Build build in _src.builds) {
                if (!build.enabled) continue;
                if (build.buildTarget == currBuildTarget) _sortedFilteredBuilds.Insert(0, build);
                else _sortedFilteredBuilds.Add(build);
            }
        }

        void RefreshBuildPathsLabels()
        {
            _buildPathsLabels = new string[_src.builds.Count];
            for (int i = 0; i < _src.builds.Count; ++i) {
                _buildPathsLabels[i] = GetFullBuildPathLabel(_src.builds[i]);
            }
        }

        bool ValidateBuildData(DeBuildPanelData.Build build)
        {
            return !string.IsNullOrEmpty(build.buildFolder)
                   && !string.IsNullOrEmpty(build.buildName)
                   && !string.IsNullOrEmpty(build.bundleIdentifier);
        }

        string GetFullBuildPathLabel(DeBuildPanelData.Build build)
        {
            _Strb.Length = 0;
            _Strb.Append(DeEditorFileUtils.projectPath);
            if (!ValidateBuildData(build)) {
                _Strb.Replace('\\', '/');
                _Strb.Insert(0, "<b>");
                _Strb.Append("</b>");
                return _Strb.ToString();
            }

            _Strb.Append(DeEditorFileUtils.PathSlash).Append(build.buildFolder);
            string fullPath = Path.GetFullPath(_Strb.ToString());
            _Strb.Length = 0;
            _Strb.Append(fullPath).Append(DeEditorFileUtils.PathSlash);
            _Strb.Append("<b><color=#00ff00>").Append(GetFullBuildName(build, build.BuildsDirectlyToFile())).Append("</color></b>");
            if (build.HasInnerExecutable()) {
                _Strb.Append('/').Append(GetFullBuildName(build, true));
            }
            _Strb.Replace('\\', '/');
            return _Strb.ToString();
        }

        string GetFullBuildName(DeBuildPanelData.Build build, bool withExtension)
        {
            bool isInnerExecutable = withExtension && build.HasInnerExecutable();
            _StrbAlt.Length = 0;
            foreach (DeBuildPanelData.Affix affix in _src.prefixes) {
                // if (affix.enabled && (!isInnerExecutable || affix.enabledForInnerExecutable)) _StrbAlt.Append(affix.GetText());
                if (affix.IsEnabledFor(build.buildTarget, isInnerExecutable, _src)) _StrbAlt.Append(affix.GetText());
            }
            _StrbAlt.Append(build.buildName);
            foreach (DeBuildPanelData.Affix affix in _src.suffixes) {
                // if (affix.enabled && (!isInnerExecutable || affix.enabledForInnerExecutable)) _StrbAlt.Append(affix.GetText());
                if (affix.IsEnabledFor(build.buildTarget, isInnerExecutable, _src)) _StrbAlt.Append(affix.GetText());
            }
            string result = _StrbAlt.ToString();
            if (!isInnerExecutable) {
                string prevResult = result;
                result = Dispatch_OnBuildNameRequest(build.buildTarget, result);
                if (string.IsNullOrEmpty(result)) result = prevResult;
            }
            if (withExtension) {
                switch (build.buildTarget) {
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneWindows:
                    result += ".exe";
                    break;
                case BuildTarget.StandaloneOSX:
                    result += ".app";
                    break;
                case BuildTarget.Android:
                    result += ".apk";
                    break;
                }
            }
            return result;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;

            public static GUIStyle buildContainer, buildPathContainer,
                                   btToolbarToggle, btInline, btInlineToggle, btDeleteBuildToolbar, btDeleteBuild,
                                   labelBuildPath, labelAffixComment;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                buildContainer = new GUIStyle().Margin(0).Padding(0).ContentOffset(0, 0);
                buildPathContainer = DeGUI.styles.box.stickyTop.Clone().Padding(1);

                btToolbarToggle = DeGUI.styles.button.bBlankBorderCompact.Margin(2, 2, 2, 0).Padding(0, 0, 1, 0).ContentOffsetX(1);
                btInline = DeGUI.styles.button.bBlankBorder.Clone().Margin(1, 1, 2, 0).PaddingBottom(2);
                btInlineToggle = btInline.Clone();
                btDeleteBuildToolbar = DeGUI.styles.button.tool.Clone(Color.white, FontStyle.Bold).Background(DeStylePalette.redSquare)
                    .Width(16).Height(14).Margin(0, 0, 2, 0);
                btDeleteBuild = btDeleteBuildToolbar.Clone().Height(16);

                labelBuildPath = new GUIStyle(GUI.skin.label).Add(9, Format.WordWrap, Format.RichText);
                labelAffixComment = DeGUI.styles.label.wordwrapRichtText.Clone(new DeSkinColor(0.4f, 0.6f), 10)
                    .Margin(0, 0, 0, 2).Padding(0);
            }
        }
    }
}