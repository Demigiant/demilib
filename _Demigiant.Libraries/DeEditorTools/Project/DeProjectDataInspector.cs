// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/14 13:08
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    [CustomEditor(typeof(DeProjectData))]
    public class DeProjectDataInspector : Editor
    {
        DeProjectData _src;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as DeProjectData;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeProjectData");

            if (GUILayout.Button("Clean Leftovers")) _src.Clean();
            EditorGUILayout.HelpBox("This will analyze the Project and remove any leftovers that don't exist in the Project anymore", MessageType.Info);
            GUILayout.Space(8);

            using (new EditorGUI.DisabledScope(true)) {
                base.OnInspectorGUI();
            }
        }

        #endregion
    }
}