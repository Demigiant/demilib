// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/29 13:40
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.ScenesPanel
{
    class ScenesPanel : EditorWindow
    {
        [MenuItem("Tools/Demigiant Tools/" + _Title)]
        static void ShowWindow() { GetWindow(typeof(ScenesPanel), false, _Title); }
		
        const string _Title = "Scenes Panel";
        Vector2 _scrollPos;
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

            int len = EditorBuildSettings.scenes.Length;

            // Get and show total enabled + disabled scenes
            int totEnabled = 0;
            int totDisabled = 0;
            for (var i = 0; i < len; i++) {
                if (EditorBuildSettings.scenes[i].enabled) totEnabled++;
                else totDisabled++;
            }
            _strb.Length = 0;
            _strb.Append("Build Scenes: ").Append(totEnabled).Append("/").Append(totEnabled + totDisabled);
            GUILayout.Label(_strb.ToString());

            // Draw scenes
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            for (int i = 0; i < len; i++) {
                EditorBuildSettingsScene scene = scenes[i];
                if (Application.isPlaying && !scene.enabled) continue;

                string sceneName = Path.GetFileNameWithoutExtension(scene.path);
                bool isCurrent = Application.isPlaying ? Application.loadedLevelName == sceneName : scene.path == EditorApplication.currentScene;
                Color bgShade = isCurrent ? DeGUI.colors.bg.toggleOn : DeGUI.colors.bg.def;
                Color labelColor = isCurrent ? DeGUI.colors.content.toggleOn : DeGUI.colors.content.def;
                if (Application.isPlaying) {
                    DeGUILayout.Toolbar(sceneName, bgShade, DeGUI.styles.toolbar.def, DeGUI.styles.label.toolbar.Add(labelColor));
                } else {
                    DeGUILayout.BeginToolbar();
                    scene.enabled = EditorGUILayout.Toggle(scene.enabled, GUILayout.Width(16));
                    if (DeGUILayout.ColoredButton(bgShade, labelColor, sceneName, DeGUI.styles.button.tool.Add(TextAnchor.MiddleLeft))) {
                        if (Event.current.button == 1) {
                            // Right-click: ping scene in Project panel
                            Object sceneObj = AssetDatabase.LoadAssetAtPath<Object>(scene.path);
                            EditorGUIUtility.PingObject(sceneObj);
                        } else if (EditorApplication.SaveCurrentSceneIfUserWantsTo()) {
                            // Left-click: open scene
                            EditorApplication.OpenScene(scene.path);
                        }
                    }
                    if (DeGUILayout.PressButton("≡", DeGUI.styles.button.tool, GUILayout.Width(16))) DeGUIDrag.StartDrag(0, this, scenes, scene, i);
                    DeGUILayout.EndToolbar();
                }

                if (DeGUIDrag.Drag(0, scenes, i)) GUI.changed = true;
            }
            if (GUI.changed) EditorBuildSettings.scenes = scenes;

            // Drag drop area
            if (!Application.isPlaying) DrawDragDropSceneArea();

            GUILayout.EndScrollView();
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
    }
}