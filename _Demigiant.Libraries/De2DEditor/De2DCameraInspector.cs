// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/13 12:18
// License Copyright (c) Daniele Giardini

using DG.De2D;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    [RequireComponent(typeof(Camera))]
    [CustomEditor(typeof(De2DCamera))]
    public class De2DCameraInspector : Editor
    {
        De2DCamera _src;
        Vector2 _lastGameViewSize;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as De2DCamera;
            EditorApplication.update -= Refresh;
            EditorApplication.update += Refresh;
        }

        public override void OnInspectorGUI()
        {
            DeGUI.BeginGUI();
            Undo.RecordObject(_src, "De2DCamera");

            _src.orthoMode = (De2DCamera.OrthoMode)EditorGUILayout.EnumPopup("Orthographic Mode", _src.orthoMode);
            _src.targetSize = EditorGUILayout.IntField(_src.orthoMode == De2DCamera.OrthoMode.FixedHeight ? "Target Height" : "Target Width", _src.targetSize);
            _src.ppu = EditorGUILayout.IntField("Pixels Per Unit", _src.ppu);

            GUILayout.BeginHorizontal();
            _src.editorAutoRefresh = DeGUILayout.ToggleButton(_src.editorAutoRefresh, new GUIContent("Editor Auto Refresh", "If toggled, automatically refreshes the ortho size while in the editor"));
            if (!_src.editorAutoRefresh && GUILayout.Button("Apply")) {
                Camera cam = _src.GetComponent<Camera>();
                SetOrthoSize(cam);
            }
            GUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("Correct orthographic size will be automatically applied at runtime", MessageType.Info);
        }

        void Refresh()
        {
            if (Application.isPlaying || _src == null || !_src.editorAutoRefresh) return;

            Vector2 gameViewSize = DeEditorUtils.GetGameViewSize();
            if (gameViewSize != _lastGameViewSize) {
                _lastGameViewSize = gameViewSize;
                SetOrthoSize(_src.GetComponent<Camera>());
            }
        }

        #endregion

        #region Methods

        // Can't use _src.Adapt, because this method requires editor methods in order to get the screen size (Screen.width only works at runtime)
        void SetOrthoSize(Camera cam)
        {
            Undo.RecordObject(cam, "Camera");
            if (cam.orthographic) cam.orthographic = true;
            cam.orthographicSize = (_src.targetSize / (float)_src.ppu) * 0.5f;
            if (_src.orthoMode == De2DCamera.OrthoMode.FixedWidth) {
                Vector2 gameViewSize = DeEditorUtils.GetGameViewSize();
                float ratio = gameViewSize.y / gameViewSize.x;
                cam.orthographicSize *= ratio;
            }
            EditorUtility.SetDirty(cam);
        }

        #endregion
    }
}