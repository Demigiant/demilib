// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/19 12:27
// License Copyright (c) Daniele Giardini

using DG.De2D;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    [CustomEditor(typeof(De2DLayout))]
    public class De2DLayoutInspector : Editor
    {
        De2DLayout _src;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as De2DLayout;
        }

        public override void OnInspectorGUI()
        {
//            Undo.RecordObject(_src, "De2DLayout");

            SerializedProperty p_camera = serializedObject.FindProperty("cam");
            SerializedProperty p_alignment = serializedObject.FindProperty("_alignment");
            SerializedProperty p_offset = serializedObject.FindProperty("_offset");

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope()) {
                using (new DeGUI.ColorScope(null, null, p_camera.objectReferenceValue == null ? Color.red : Color.green)) {
                    EditorGUILayout.PropertyField(p_camera, new GUIContent("Camera"));
                }
                EditorGUILayout.PropertyField(p_alignment, new GUIContent("Alignment"));
                EditorGUILayout.PropertyField(p_offset, new GUIContent("Offset"));

                if (check.changed) _src.Refresh();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}