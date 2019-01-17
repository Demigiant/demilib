// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/12/07 13:24
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using Format = DG.DemiEditor.Format;

namespace DG.DeEditorTools.BuildPanel
{
    class DeBuildPanel : EditorWindow
    {
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

        void OnGUI()
        {
            if (_src == null) _src = DeEditorPanelUtils.ConnectToSourceAsset<DeBuildPanelData>(_SrcADBFilePath, true);
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            Undo.RecordObject(_src, _Title);
            DeGUI.BeginGUI();
            Styles.Init();
			
            // Main toolbar
            using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.large)) {
                GUILayout.Label("v" + DeBuildPanelData.Version, DeGUI.styles.label.toolbar);
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
                        DrawAffix(_src.prefixes);
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
                        DrawAffix(_src.suffixes);
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
            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        void DrawAffix(List<DeBuildPanelData.Affix> affixes)
        {
            for (int i = 0; i < affixes.Count; ++i) {
                DeBuildPanelData.Affix affix = affixes[i];
                using (new GUILayout.HorizontalScope()) {
                    if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                        DeGUIDrag.StartDrag(this, affixes, i);
                    }
                    using (new DeGUI.ColorScope(affix.enabled ? Color.white : (Color)new DeSkinColor(0.7f, 0.7f), affix.enabled ? Color.white : (Color)new DeSkinColor(0.7f, 0.5f))) {
                        affix.text = EditorGUILayout.TextField(affix.text);
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
                        if (!BuildsAsSingleFile(build.buildTarget)) {
                            build.clearBuildFolder = DeGUILayout.ToggleButton(
                                build.clearBuildFolder,
                                new GUIContent(
                                    "Clear At Build",
                                    "If selected and a build with the same name already exists, deletes the build contents before creating the new build"
                                ),
                                GUILayout.Width(_LabelWidth)
                            );
                        }
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
                        //
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
                    //
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
            int totEnabled = 0;
            foreach (DeBuildPanelData.Build build in _src.builds) {
                if (build.enabled) totEnabled++;
            }

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

            foreach (DeBuildPanelData.Build build in _src.builds) DoBuild(build);
        }

        void Build(DeBuildPanelData.Build build)
        {
            EditorUtility.DisplayProgressBar(string.Format("Build ({0})", build.buildTarget), "Preparing...", 0.2f);
            // Use delayed call to prevent Unity GUILayout bug
            DeEditorUtils.ClearAllDelayedCalls();
            DeEditorUtils.DelayedCall(0.1f, ()=> DoBuild(build));
        }

        void DoBuild(DeBuildPanelData.Build build)
        {
            string dialogTitle = string.Format("Build ({0})", build.buildTarget);

            if (string.IsNullOrEmpty(build.buildFolder)) {
                EditorUtility.DisplayDialog(dialogTitle, "Build folder can't be empty!", "Ok");
                EditorUtility.ClearProgressBar();
                return;
            }
            if (string.IsNullOrEmpty(build.bundleIdentifier)) {
                EditorUtility.DisplayDialog(dialogTitle, "Bundle Identifier can't be empty!", "Ok");
                EditorUtility.ClearProgressBar();
                return;
            }

            string buildFolder = Path.GetFullPath(DeEditorFileUtils.projectPath + "/" + build.buildFolder);

            if (!Directory.Exists(buildFolder)) {
                EditorUtility.DisplayDialog(dialogTitle, string.Format("Build folder doesn't exist!\n\n\"{0}\"", buildFolder), "Ok");
                EditorUtility.ClearProgressBar();
                return;
            }

            bool buildIsSingleFile = BuildsAsSingleFile(build.buildTarget);
            string completeBuildFolder = buildIsSingleFile || build.buildTarget == BuildTarget.StandaloneOSX || build.buildTarget == BuildTarget.iOS
                ? buildFolder
                : Path.GetFullPath(buildFolder + DeEditorFileUtils.PathSlash + GetFullBuildName(build, false));
            string buildFilePath = completeBuildFolder + DeEditorFileUtils.PathSlash + GetFullBuildName(build, true);

            // Clear build folder
            if (!buildIsSingleFile && build.clearBuildFolder && Directory.Exists(completeBuildFolder)) {
                string[] files = Directory.GetFiles(completeBuildFolder);
                for (int i = 0; i < files.Length; ++i) File.Delete(files[i]);
                string[] subdirs = Directory.GetDirectories(completeBuildFolder);
                for (int i = 0; i < subdirs.Length; ++i) Directory.Delete(subdirs[i], true);
            }

            // Build
            switch (build.buildTarget) {
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneOSX:
                PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Standalone, build.bundleIdentifier);
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
            }
            BuildPlayerOptions buildOptions = new BuildPlayerOptions();
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < scenes.Length; ++i) scenes[i] = EditorBuildSettings.scenes[i].path;
            buildOptions.scenes = scenes;
            buildOptions.locationPathName = buildFilePath;
            buildOptions.target = build.buildTarget;
            buildOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildOptions);
        }

        #endregion

        #region Helpers

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

        bool BuildsAsSingleFile(BuildTarget buildTarget)
        {
            switch (buildTarget) {
            case BuildTarget.Android: return true;
            default: return false;
            }
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
            _Strb.Append("<b>").Append(fullPath).Append("</b>").Append(DeEditorFileUtils.PathSlash);
            _Strb.Append(GetFullBuildName(build, build.buildTarget == BuildTarget.StandaloneOSX || build.buildTarget == BuildTarget.Android));
            _Strb.Replace('\\', '/');
            return _Strb.ToString();
        }

        string GetFullBuildName(DeBuildPanelData.Build build, bool withExtension)
        {
            _StrbAlt.Length = 0;
            foreach (DeBuildPanelData.Affix affix in _src.prefixes) {
                if (affix.enabled) _StrbAlt.Append(affix.text);
            }
            _StrbAlt.Append(build.buildName);
            foreach (DeBuildPanelData.Affix affix in _src.suffixes) {
                if (affix.enabled) _StrbAlt.Append(affix.text);
            }
            if (withExtension) {
                switch (build.buildTarget) {
                case BuildTarget.StandaloneWindows64:
                    _StrbAlt.Append(".exe");
                    break;
                case BuildTarget.StandaloneOSX:
                    _StrbAlt.Append(".app");
                    break;
                case BuildTarget.Android:
                    _StrbAlt.Append(".apk");
                    break;
                }
            }
            return _StrbAlt.ToString();
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;

            public static GUIStyle buildContainer, buildPathContainer,
                                   btToolbarToggle, btInlineToggle, btDeleteBuildToolbar, btDeleteBuild,
                                   labelBuildPath;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                buildContainer = new GUIStyle().Margin(0).Padding(0).ContentOffset(0, 0);
                buildPathContainer = DeGUI.styles.box.stickyTop.Clone().Padding(1);

                btToolbarToggle = DeGUI.styles.button.bBlankBorderCompact.Margin(2, 2, 2, 0);
                btInlineToggle = DeGUI.styles.button.bBlankBorder.Clone().MarginTop(2);
                btDeleteBuildToolbar = DeGUI.styles.button.tool.Clone(Color.white, FontStyle.Bold).Background(DeStylePalette.redSquare)
                    .Width(16).Height(14).Margin(0, 0, 2, 0);
                btDeleteBuild = btDeleteBuildToolbar.Clone().Height(16);

                labelBuildPath = new GUIStyle(GUI.skin.label).Add(9, Format.WordWrap, Format.RichText);
            }
        }
    }
}