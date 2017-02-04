// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/01 10:38
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using System.Reflection;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Scene
{
    /// <summary>
    /// Adds extra features to the Scene panel (like right-click).
    /// </summary>
    [InitializeOnLoad]
    public class DeScene
    {
        public static readonly List<SpriteRenderer> SelectedSpriteRenderers = new List<SpriteRenderer>();
        public static readonly List<Camera> OrthoCams = new List<Camera>();
        public static Rect sceneArea { get; private set; }
        static MethodInfo _mi_resetMouseDown; // Used to reset mouseCursor to arrow instead of pan, in case of right-click
        static bool _popupIsOpen;

        static DeScene()
        {
            _mi_resetMouseDown = typeof(EditorUtility).GetMethod("ResetMouseDown", BindingFlags.NonPublic | BindingFlags.Static);
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            SceneView.RepaintAll();
        }

        #region GUI

        static void OnSceneGUI(SceneView sceneView)
        {
            if (!DeEditorToolsPrefs.deScene_enableContextMenu) return;

            sceneArea = new Rect(
                0, 0,
                sceneView.position.width,
                sceneView.position.height - 17 // Somehow scene area is wrong without subtracting 17
            );

            switch (Event.current.type) {
            case EventType.MouseDown:
                if (Event.current.button == 1) {
                    FillSelectedSpriteRenderers();
                    if (SelectedSpriteRenderers.Count > 0) {
                        FillAllOrthoCams();
                        Rect popupArea = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
                        if (OrthoCams.Count > 0) {
                            _mi_resetMouseDown.Invoke(null, null); // Reset mouse to prevent panning tool
                            Event.current.Use();
                            PopupWindow.Show(popupArea, new DeSceneMainPopup());
                        }
                    }
                }
                break;
            }
        }

        #endregion

        #region Helpers

        // Returns NULL if no SpriteRenderer is selected
        static void FillSelectedSpriteRenderers()
        {
            SelectedSpriteRenderers.Clear();
            foreach (GameObject go in Selection.gameObjects) {
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr == null) continue;
                SelectedSpriteRenderers.Add(sr);
            }
        }

        // Returns NULL if no ortographic camera was found
        static void FillAllOrthoCams()
        {
            OrthoCams.Clear();
            List <Camera> allCams = DeEditorUtils.FindAllComponentsOfType<Camera>();
            if (allCams == null) return;
            foreach (Camera cam in allCams) {
                if (!cam.orthographic) continue;
                OrthoCams.Add(cam);
            }
        }

        #endregion
    }
}