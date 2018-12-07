// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/12/07 13:24
// License Copyright (c) Daniele Giardini

using System.IO;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.BuildPanel
{
    class DeBuildPanel : EditorWindow
    {
        [MenuItem("Tools/Demigiant/" + _Title)]
        static void ShowWindow() { GetWindow(typeof(DeBuildPanel), true, _Title); }
		
        const string _Title = "Simple Build Panel";
        const string _SrcADBFilePath = "Assets/-DeBuildPanelData.asset";
        DeBuildPanelData _src;
        Vector2 _scrollPos;
        const int _DragId = 235;
        const int _LabelWidth = 116;
        string _buildFolderComment;

        #region Unity and GUI Methods

        void OnEnable()
        {
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
                    if (GUILayout.Button("BUILD ALL ENABLED", DeGUI.styles.button.toolL)) BuildAllEnabled();
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

            // Global options
            GUILayout.Space(3);
            _src.suffix = EditorGUILayout.TextField(
                new GUIContent("Build Folder Suffix", "Suffix (if any) is applied to all Build Folder paths"),
                _src.suffix
            );
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
                if (DeGUIDrag.Drag(_DragId, _src.builds, i).outcome == DeDragResultType.Accepted) {
                    EditorUtility.SetDirty(_src);
                }
            }

            GUILayout.EndScrollView();
            if (GUI.changed) EditorUtility.SetDirty(_src);
        }

        // Returns FALSE if the given build was deleted
        bool DrawBuild(int index)
        {
            DeBuildPanelData.Build build = _src.builds[index];

            // Toolbar
            using (new DeGUILayout.ToolbarScope()) {
                if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                    DeGUIDrag.StartDrag(_DragId, this, _src.builds, index);
                }
                build.foldout = DeGUILayout.ToolbarFoldoutButton(build.foldout, build.buildTarget.ToString(), false, true);
                build.enabled = DeGUILayout.ToggleButton(build.enabled, "Enabled", Styles.btToolbarToggle, GUILayout.Width(60));
                if (GUILayout.Button("BUILD NOW", DeGUI.styles.button.tool, GUILayout.Width(80))) {
                    Build(build);
                }
                if (GUILayout.Button("×", Styles.btDeleteBuild)) {
                    _src.builds.RemoveAt(index);
                    return false;
                }
            }
            // Data
            if (build.foldout) {
                using (new GUILayout.VerticalScope(DeGUI.styles.box.stickyTop)) {
                    using (new GUILayout.HorizontalScope()) {
                        build.clearBuildFolder = DeGUILayout.ToggleButton(
                            build.clearBuildFolder,
                            new GUIContent("Clear At Build", "If selected, deletes the build folder contents before creating the new build"),
                            GUILayout.Width(_LabelWidth)
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
                        //
                    }
                    using (new DeGUI.ColorScope(string.IsNullOrEmpty(build.buildFolder) ? Color.red : Color.white)) {
                        build.buildFolder = EditorGUILayout.TextField(new GUIContent("Build Folder", _buildFolderComment), build.buildFolder);
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
        }

        #endregion

        #region Methods

        void BuildAllEnabled()
        {
            int totEnabled = 0;
            foreach (DeBuildPanelData.Build build in _src.builds) {
                if (build.enabled) totEnabled++;
            }

            if (totEnabled == 0) {
                EditorUtility.DisplayDialog("Build All", "0 platforms enabled, nothing to build", "Ok");
                return;
            }

            bool proceed = EditorUtility.DisplayDialog(
                "Build All",
                string.Format("Build for {0} platform{1}?", totEnabled, totEnabled > 1 ? "s" : ""),
                "Yes", "Cancel"
            );
            if (!proceed) return;

            foreach (DeBuildPanelData.Build build in _src.builds) Build(build);
        }

        void Build(DeBuildPanelData.Build build)
        {
            string dialogTitle = string.Format("Build ({0})", build.buildTarget);

            if (string.IsNullOrEmpty(build.buildFolder)) {
                EditorUtility.DisplayDialog(dialogTitle, "Build folder can't be empty!", "Ok");
                return;
            }

            string buildFolder = Path.GetFullPath(DeEditorFileUtils.projectPath + "/" + build.buildFolder + _src.suffix);
            string buildPath = buildFolder;

            if (!Directory.Exists(buildFolder)) {
                EditorUtility.DisplayDialog(dialogTitle, string.Format("Build folder doesn't exist!\n\n\"{0}\"", buildFolder), "Ok");
                return;
            }

            if (string.IsNullOrEmpty(build.bundleIdentifier)) {
                EditorUtility.DisplayDialog(dialogTitle, "Bundle Identifier can't be empty!", "Ok");
                return;
            }

            switch (build.buildTarget) {
            case BuildTarget.StandaloneWindows64:
                buildPath += ".exe";
                break;
            case BuildTarget.StandaloneOSX:
                buildPath += ".app";
                break;
            case BuildTarget.Android:
                buildPath += ".apk";
                break;
            }

            // Clear build folder
            if (build.clearBuildFolder && Directory.Exists(buildFolder)) {
                string[] files = Directory.GetFiles(buildFolder);
                for (int i = 0; i < files.Length; ++i) File.Delete(files[i]);
                string[] subdirs = Directory.GetDirectories(buildFolder);
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
            buildOptions.locationPathName = buildPath;
            buildOptions.target = build.buildTarget;
            buildOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildOptions);
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;

            public static GUIStyle buildContainer,
                                   btToolbarToggle, btDeleteBuild;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                buildContainer = new GUIStyle().Margin(0).Padding(0).ContentOffset(0, 0);
                btToolbarToggle = DeGUI.styles.button.bBlankBorderCompact.Margin(2, 2, 2, 0);
                btDeleteBuild = DeGUI.styles.button.tool.Clone(Color.white, FontStyle.Bold).Background(DeStylePalette.redSquare)
                    .Width(16).Height(14).Margin(0, 0, 2, 0);
            }
        }
    }
}