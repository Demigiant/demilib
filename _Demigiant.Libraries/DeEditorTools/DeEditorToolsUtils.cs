// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 12:36
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG.DeEditorTools
{
    public static class DeEditorToolsUtils
    {
        static readonly List<GameObject> _RootGOs = new List<GameObject>(500);

        /// <summary>
        /// Returns all components of type T in the currently open scene/s, or NULL if none could be found.<para/>
        /// This is a more efficient version of <code>DeEditorUtils.FindAllComponentsOfType</code>.
        /// </summary>
        /// <param name="activeSceneOnly">If TRUE only finds them in the active scene, otherwise in all loaded scenes</param>
        public static List<T> FindAllComponentsOfType<T>(bool activeSceneOnly = true) where T : Component
        {
            List<T> result = new List<T>();
            if (activeSceneOnly) FindAllComponentsOfTypeInScene<T>(SceneManager.GetActiveScene(), result);
            else {
                for (int i = 0; i < SceneManager.sceneCount; ++i) FindAllComponentsOfTypeInScene<T>(SceneManager.GetSceneAt(i), result);
            }
            return result.Count > 0 ? result : null;
        }
        static void FindAllComponentsOfTypeInScene<T>(UnityEngine.SceneManagement.Scene scene, List<T> addToList) where T : Component
        {
            scene.GetRootGameObjects(_RootGOs);
            foreach (GameObject go in _RootGOs) {
                if (go.hideFlags == HideFlags.HideInHierarchy || go.hideFlags == HideFlags.HideAndDontSave) continue;
                T[] components = go.GetComponentsInChildren<T>();
                if (components.Length == 0) continue;
                foreach (T component in components) addToList.Add(component);
            }
        }

        /// <summary>
        /// Returns the first of type T in the currently open scene/s, or NULL if none could be found.<para/>
        /// </summary>
        /// <param name="activeSceneOnly">If TRUE only finds it in the active scene, otherwise in all loaded scenes</param>
        public static T FindFirstComponentOfType<T>(bool activeSceneOnly = true) where T : Component
        {
            if (activeSceneOnly) return FindFirstComponentOfTypeInScene<T>(SceneManager.GetActiveScene());
            else {
                for (int i = 0; i < SceneManager.sceneCount; ++i) {
                    T result = FindFirstComponentOfTypeInScene<T>(SceneManager.GetSceneAt(i));
                    if (result != null) return result;
                }
            }
            return null;
        }
        static T FindFirstComponentOfTypeInScene<T>(UnityEngine.SceneManagement.Scene scene) where T : Component
        {
            scene.GetRootGameObjects(_RootGOs);
            foreach (GameObject go in _RootGOs) {
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) continue;
                T component = go.GetComponentInChildren<T>();
                if (component != null) return component;
            }
            return null;
        }

        /// <summary>
        /// Captures a screenshot of the gameView from the given camera, and returns it as a Texture2D object
        /// </summary>
        /// <param name="width">Width</param>
        /// <param name="height">Height</param>
        /// <param name="cam">If NULL uses the main camera</param>
        /// <param name="alpha">If TRUE uses transparency</param>
        public static Texture2D CaptureGameViewScreenshot(int width, int height, Camera cam = null, bool alpha = false)
        {
            Vector2 gameSize = DeEditorUtils.GetGameViewSize();
            int screenshotW = width;
            int screenshotH = height;

            Camera screenshotCam = cam == null ? Camera.main : cam;
            TextureFormat screenshotFormat = alpha ? TextureFormat.ARGB32 : TextureFormat.RGB24;
            RenderTexture renderT = new RenderTexture(screenshotW, screenshotH, 24);
            screenshotCam.targetTexture = renderT;
            Texture2D screenshot = new Texture2D(screenshotW, screenshotH, screenshotFormat, false);

            RenderTexture.active = renderT;
            screenshotCam.Render();
            screenshot.ReadPixels(new Rect(0, 0, screenshotW, screenshotH), 0, 0);
            screenshotCam.targetTexture = null;
            RenderTexture.active = null;

            return screenshot;
        }
    }
}