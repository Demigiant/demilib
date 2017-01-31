// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/31 13:33
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DG.De2DEditor
{
    /// <summary>
    /// Adds extra features to the Scene panel (like right-click).
    /// </summary>
    [InitializeOnLoad]
    public class De2DScene
    {
        static Rect _sceneArea;

        static De2DScene()
        {
            SceneView.onSceneGUIDelegate -= OnSceneGUI;
            SceneView.onSceneGUIDelegate += OnSceneGUI;
            SceneView.RepaintAll();
        }

        #region GUI

        static void OnSceneGUI(SceneView sceneView)
        {
            InitGUI(sceneView);
            Handles.BeginGUI();
            GUILayout.BeginArea(_sceneArea);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 1 && De2DPrefs.enableSceneContextMenu) CreateSelectionCM();

            GUILayout.EndArea();
            Handles.EndGUI();
        }

        static void InitGUI(SceneView sceneView)
        {
            DeGUI.BeginGUI();

            _sceneArea = sceneView.position;
            _sceneArea.x = _sceneArea.y = 0;
            _sceneArea.height -= 17; // Somehow scene area is wrong otherwise
        }

        #region ContextMenus

        static void CreateSelectionCM()
        {
            List<SpriteRenderer> selectedSpriteRenderers = GetSelectedSpriteRenderers();
            if (selectedSpriteRenderers == null) return;

            GenericMenu menu = null;

            // Align sprite/s to camera actions
            List<Camera> orthoCams = FindAllOrthoCams();
            if (orthoCams != null) {
                if (menu == null) menu = new GenericMenu();
                foreach (Camera cam in orthoCams) {
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Top Left", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.UpperLeft));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Top Center", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.UpperCenter));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Top Right", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.UpperRight));
                    menu.AddSeparator(string.Format("Align To → {0}/", cam.name));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Middle Left", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.MiddleLeft));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Middle Center", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.MiddleCenter));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Middle Right", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.MiddleRight));
                    menu.AddSeparator(string.Format("Align To → {0}/", cam.name));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Bottom Left", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.LowerLeft));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Bottom Center", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.LowerCenter));
                    menu.AddItem(new GUIContent(string.Format("Align To → {0}/Bottom Right", cam.name)), false, CM_AlignToCam, new AlignToCamData(cam, selectedSpriteRenderers, TextAnchor.LowerRight));
                }
            }

            if (menu != null) menu.ShowAsContext();
        }

        static void CM_AlignToCam(object alignToCamData)
        {
            AlignToCamData data = (AlignToCamData)alignToCamData;
            Vector2 gameviewSize = DeEditorUtils.GetGameViewSize();
            float camVerticalSpanHalf = data.cam.orthographicSize;
            float camHorizontalSpanHalf = camVerticalSpanHalf * (gameviewSize.x / gameviewSize.y);
            Rect camRect = new Rect(
                data.cam.transform.position.x - camHorizontalSpanHalf, data.cam.transform.position.y - camVerticalSpanHalf,
                camHorizontalSpanHalf * 2, camVerticalSpanHalf * 2
            );
            foreach (SpriteRenderer sr in data.spriteRenderers) {
                Vector3 toPos = new Vector3(0, 0, sr.transform.position.z);
                Bounds bounds = sr.bounds;
                Vector2 pivotRatio = new Vector2(sr.sprite.pivot.x / sr.sprite.rect.width, sr.sprite.pivot.y / sr.sprite.rect.height);
                AlignToCamData.Horizontal horAlignment = data.GetHorizontalAlignment();
                AlignToCamData.Vertical vertAlignment = data.GetVerticalAlignment();
                switch (horAlignment) {
                case AlignToCamData.Horizontal.Left:
                    toPos.x = camRect.xMin + bounds.size.x * pivotRatio.x;
                    break;
                case AlignToCamData.Horizontal.Middle:
                    toPos.x = camRect.center.x - (bounds.extents.x - bounds.size.x * pivotRatio.x);
                    break;
                case AlignToCamData.Horizontal.Right:
                    toPos.x = camRect.xMax - (bounds.size.x - bounds.size.x * pivotRatio.x);
                    break;
                }
                switch (vertAlignment) {
                case AlignToCamData.Vertical.Top:
                    toPos.y = camRect.yMax - (bounds.size.y - bounds.size.y * pivotRatio.y);
                    break;
                case AlignToCamData.Vertical.Middle:
                    toPos.y = camRect.center.y - (bounds.extents.y - bounds.size.y * pivotRatio.y);
                    break;
                case AlignToCamData.Vertical.Bottom:
                    toPos.y = camRect.yMin + bounds.size.y * pivotRatio.y;
                    break;
                }
                Undo.RecordObject(sr.transform, "Align to Camera");
                sr.transform.position = toPos;
            }
            EditorSceneManager.MarkAllScenesDirty();
        }

        #endregion

        #endregion

        #region Helpers

        // Returns NULL if no SpriteRenderer is selected
        static List<SpriteRenderer> GetSelectedSpriteRenderers()
        {
            List<SpriteRenderer> result = null;
            foreach (GameObject go in Selection.gameObjects) {
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr == null) continue;
                if (result == null) result = new List<SpriteRenderer>();
                result.Add(sr);
            }
            return result;
        }

        // Returns NULL if no component of type was found
        static List<T> FindAllComponentsOfType<T>() where T : Component
        {
            GameObject[] allGOs = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
            if (allGOs == null) return null;
            List<T> result = null;
            foreach (GameObject go in allGOs) {
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave) continue;
                T[] components = go.GetComponentsInChildren<T>();
                if (components.Length == 0) continue;
                if (result == null) result = new List<T>();
                foreach (T component in components) {
                    result.Add(component);
                }
            }
            return result;
        }

        // Returns NULL if no ortographic camera was found
        static List<Camera> FindAllOrthoCams()
        {
            List <Camera> allCams = FindAllComponentsOfType<Camera>();
            if (allCams == null) return null;
            List<Camera> result = null;
            foreach (Camera cam in allCams) {
                if (!cam.orthographic) continue;
                if (result == null) result = new List<Camera>();
                result.Add(cam);
            }
            return result;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public class AlignToCamData
        {
            public enum Horizontal
            {
                Left, Middle, Right
            }
            public enum Vertical
            {
                Top, Middle, Bottom
            }

            public Camera cam;
            public List<SpriteRenderer> spriteRenderers;
            public TextAnchor alignment;

            public AlignToCamData(Camera cam, List<SpriteRenderer> spriteRenderers, TextAnchor alignment)
            {
                this.cam = cam;
                this.spriteRenderers = spriteRenderers;
                this.alignment = alignment;
            }

            public Horizontal GetHorizontalAlignment()
            {
                switch (alignment) {
                case TextAnchor.LowerLeft:
                case TextAnchor.MiddleLeft:
                case TextAnchor.UpperLeft:
                    return Horizontal.Left;
                case TextAnchor.LowerRight:
                case TextAnchor.MiddleRight:
                case TextAnchor.UpperRight:
                    return Horizontal.Right;
                default:
                    return Horizontal.Middle;
                }
            }

            public Vertical GetVerticalAlignment()
            {
                switch (alignment) {
                case TextAnchor.UpperLeft:
                case TextAnchor.UpperRight:
                case TextAnchor.UpperCenter:
                    return Vertical.Top;
                case TextAnchor.LowerLeft:
                case TextAnchor.LowerRight:
                case TextAnchor.LowerCenter:
                    return Vertical.Bottom;
                default:
                    return Vertical.Middle;
                }
            }
        }
    }
}