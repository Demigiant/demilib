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

        public static void Subscribe(De2DCamera src)
        {
            
        }

        void OnEnable()
        {
            _src = target as De2DCamera;
        }

        public override void OnInspectorGUI()
        {
            DeGUI.BeginGUI();
            Undo.RecordObject(_src, "De2DCamera");

            _src.orthoMode = (De2DCamera.OrthoMode)EditorGUILayout.EnumPopup("Orthographic Mode", _src.orthoMode);
            using (new GUILayout.HorizontalScope()) {
                EditorGUILayout.PrefixLabel("Target Width/Height");
                _src.targetWidth = EditorGUILayout.IntField(_src.targetWidth);
                _src.targetHeight = EditorGUILayout.IntField(_src.targetHeight);
            }
            _src.ppu = EditorGUILayout.IntField("Pixels Per Unit", _src.ppu);

            if (GUI.changed) _src.Adapt();
        }

        #endregion
    }
}