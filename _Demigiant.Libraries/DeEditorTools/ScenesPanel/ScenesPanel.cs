// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/29 13:40
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG.DeEditorTools.ScenesPanel
{
    enum ScenesPanelState
    {
        Default,
        Info
    }

    class ScenesPanel : EditorWindow
    {
        [MenuItem("Tools/Demigiant/" + _Title)]
        static void ShowWindow() { GetWindow(typeof(ScenesPanel), false, _Title); }

        public const string Version = "1.1.000";
        const string _Title = "Scenes Panel";
        Vector2 _scrollPos;
        ScenesPanelState _state = ScenesPanelState.Default;
        bool _enableDelete;
        readonly StringBuilder _strb = new StringBuilder();

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ UNITY METHODS

        void OnHierarchyChange()
        { Repaint(); }

        void OnGUI()
        {
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed") Repaint();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            DeGUI.BeginGUI();

            if (!Application.isPlaying && _state == ScenesPanelState.Info) DrawHelpState();
            else DrawDefaultState();

            GUILayout.EndScrollView();
        }

        void DrawDefaultState()
        {
            // ? and Enable Remove button
            if (!Application.isPlaying) {
                DeGUILayout.BeginToolbar(DeGUI.styles.toolbar.large);
                _enableDelete = DeGUILayout.ToggleButton(_enableDelete, "Enable Remove", DeGUI.styles.button.toolL);
                if (GUILayout.Button("Info", DeGUI.styles.button.toolL, GUILayout.Width(46))) _state = ScenesPanelState.Info;
                DeGUILayout.EndToolbar();
            }

            int len = EditorBuildSettings.scenes.Length;

            // Get and show total enabled + disabled scenes
            int totEnabled = 0;
            int totDisabled = 0;
            for (var i = 0; i < len; i++) {
                if (EditorBuildSettings.scenes[i].enabled) totEnabled++;
                else totDisabled++;
            }
            _strb.Length = 0;
            _strb.Append("Scenes In Build: ").Append(totEnabled).Append("/").Append(totEnabled + totDisabled);
            DeGUILayout.Toolbar(_strb.ToString(), DeGUI.styles.toolbar.def, DeGUI.styles.label.toolbar.Clone(10, FontStyle.Bold));

            // Draw scenes
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            EditorBuildSettingsScene[] scenesMod = null;
            int sceneIndex = -1;
            for (int i = 0; i < len; i++) {
                EditorBuildSettingsScene scene = scenes[i];
                if (Application.isPlaying && !scene.enabled) continue;

                if (scene.enabled) sceneIndex++;
                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                bool isCurrent = SceneManager.sceneCount > 1 ? IsAdditiveSceneLoaded(scene.path) : SceneManager.GetActiveScene().path == scene.path;
                Color bgShade = isCurrent ? DeGUI.colors.bg.toggleOn : DeGUI.colors.bg.def;
                Color labelColor = isCurrent ? DeGUI.colors.content.toggleOn : DeGUI.colors.content.def;
                DeGUILayout.BeginToolbar(DeGUI.styles.toolbar.def.Clone().PaddingLeft(0).PaddingRight(0));
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                if (_enableDelete) {
                    if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "x", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                        // Remove scene from build
                        scenesMod = CloneAndRemove(scenes, i);
                        GUI.changed = true;
                    }
                    GUILayout.Space(4);
                }
                if (Application.isPlaying)
                    GUILayout.Button(sceneIndex.ToString(), DeGUI.styles.button.tool.Clone(TextAnchor.MiddleCenter).PaddingLeft(0).PaddingRight(0), GUILayout.Width(20));
                else
                    scene.enabled = DeGUILayout.ToggleButton(scene.enabled, scene.enabled ? sceneIndex.ToString() : "-", DeGUI.styles.button.tool.Clone(TextAnchor.MiddleCenter).PaddingLeft(0).PaddingRight(0), GUILayout.Width(20));
                EditorGUI.EndDisabledGroup();
                if (DeGUILayout.ColoredButton(bgShade, labelColor, sceneName, DeGUI.styles.button.tool.Clone(TextAnchor.MiddleLeft))) {
                    if (Application.isPlaying) {
                        SceneManager.LoadScene(sceneIndex);
                    } else {
                        if (Event.current.button == 1) {
                            // Right-click: ping scene in Project panel and store its name in the clipboard
                            Object sceneObj = AssetDatabase.LoadAssetAtPath<Object>(scene.path);
                            EditorGUIUtility.PingObject(sceneObj);
                            EditorGUIUtility.systemCopyBuffer = sceneName;
                        } else {
                            if (Event.current.shift) {
                                // Shift click: open scene additive
                                EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                            } else {
                                // Left-click: open scene
                                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) EditorSceneManager.OpenScene(scene.path);
                            }
                        }
                    }
                }
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) DeGUIDrag.StartDrag(this, scenes, i);
                EditorGUI.EndDisabledGroup();
                DeGUILayout.EndToolbar();

                if (DeGUIDrag.Drag(scenes, i).outcome == DeDragResultType.Accepted) GUI.changed = true;
            }
            if (GUI.changed) EditorBuildSettings.scenes = scenesMod == null ? scenes : scenesMod;

            // Drag drop area
            if (!Application.isPlaying) DrawDragDropSceneArea();
        }

        void DrawHelpState()
        {
            DeGUILayout.BeginToolbar(DeGUI.styles.toolbar.large);
            if (GUILayout.Button("Main", DeGUI.styles.button.toolL)) _state = ScenesPanelState.Default;
            DeGUILayout.EndToolbar();

            _strb.Length = 0;
            _strb.Append("<b>Scenes Panel v").Append(Version).Append("</b>\n")
                .Append("<i>Part of DemiLib</i>\n")
                .Append("Developed by Daniele Giardini (Demigiant)");
            GUILayout.Label(_strb.ToString(), DeGUI.styles.label.wordwrapRichtText);

            GUILayout.Space(4);
            DeGUILayout.Toolbar("Tips", DeGUI.styles.toolbar.stickyTop);
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            _strb.Length = 0;
            _strb.Append("<b>SHIFT + click on a scene</b>\nOpen scene additive")
                .Append("\n<b>Right-click on a scene</b>\nPing it and copy its name in the clipboard");
            GUILayout.Label(_strb.ToString(), DeGUI.styles.label.wordwrapRichtText);
            DeGUILayout.EndVBox();

            DeGUILayout.Toolbar("Links", DeGUI.styles.toolbar.stickyTop);
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            if (GUILayout.Button("DemiLib on GitHub")) Application.OpenURL("https://github.com/Demigiant/demilib/wiki");
            if (GUILayout.Button("Demigiant")) Application.OpenURL("http://www.demigiant.com");
            DeGUILayout.EndVBox();
        }

        void DrawDragDropSceneArea()
        {
            Event e = Event.current;
            Rect dropRect = GUILayoutUtility.GetRect(0, 44, GUILayout.ExpandWidth(true));
            dropRect.x += 3;
            dropRect.y += 3;
            dropRect.width -= 6;
            EditorGUI.HelpBox(dropRect, "Drop Scenes here to add them to the build list", MessageType.Info);

            switch (e.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropRect.Contains(e.mousePosition)) return;
                bool isValid = true;
                // Verify if drop is valid (contains only scenes)
                foreach (Object dragged in DragAndDrop.objectReferences) {
                    if (!dragged.ToString().EndsWith(".SceneAsset)")) {
                        // Invalid
                        DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                        isValid = false;
                        break;
                    }
                }
                if (!isValid) return;
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                if (e.type == EventType.DragPerform) {
                    // Add scenes
                    DragAndDrop.AcceptDrag();
                    EditorBuildSettingsScene[] currScenes = EditorBuildSettings.scenes;
                    List<EditorBuildSettingsScene> newScenes = new List<EditorBuildSettingsScene>(currScenes.Length + DragAndDrop.objectReferences.Length);
                    foreach (EditorBuildSettingsScene s in currScenes) newScenes.Add(s);
                    foreach (Object dragged in DragAndDrop.objectReferences) {
                        EditorBuildSettingsScene scene = new EditorBuildSettingsScene(AssetDatabase.GetAssetPath(dragged), true);
                        newScenes.Add(scene);
                    }
                    EditorBuildSettings.scenes = newScenes.ToArray();
                }
                break;
            }
        }

        static EditorBuildSettingsScene[] CloneAndRemove(EditorBuildSettingsScene[] scenes, int index)
        {
            EditorBuildSettingsScene[] res = new EditorBuildSettingsScene[scenes.Length - 1];
            int diff = 0;
            for (int i = 0; i < res.Length; ++i) {
                if (i == index) diff++;
                res[i] = scenes[i + diff];
            }
            return res;
        }

        static bool IsAdditiveSceneLoaded(string scenePath)
        {
            for (int i = 0; i < SceneManager.sceneCount; ++i) {
                if (SceneManager.GetSceneAt(i).path == scenePath) return true;
            }
            return false;
        }
    }
}