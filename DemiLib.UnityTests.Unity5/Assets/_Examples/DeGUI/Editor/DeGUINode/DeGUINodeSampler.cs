// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/09 11:43
// License Copyright (c) Daniele Giardini

using DG.DeExtensions;
using DG.DemiEditor.DeGUINodeSystem;
using UnityEditor;
using UnityEngine;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class DeGUINodeSampler : EditorWindow
    {
        const string _Title = "DeGUINodeSampler";
        static DeSampler src;
        DeGUINodeProcess _nodeProcess;

        #region Open

        public static void ShowWindow(DeSampler src)
        {
            DeGUINodeSampler.src = src;
            EditorWindow editor = GetWindow(typeof(DeGUINodeSampler), true, _Title);
            Undo.undoRedoPerformed -= editor.Repaint;
            Undo.undoRedoPerformed += editor.Repaint;
        }

        #endregion

        #region Unity and GUI Methods

        void OnEnable()
        {
            if (src == null) {
                Close();
                return;
            }
            _nodeProcess = new DeGUINodeProcess(this, true);
        }

        void OnHierarchyChange()
        { Repaint(); }

        void OnDestroy() { Undo.undoRedoPerformed -= Repaint; }

        void OnGUI()
        {
            if (src == null) {
                Close();
                return;
            }

            Undo.RecordObject(src, "DeSampler");
            DG.DemiEditor.DeGUI.BeginGUI();

            // Node GUI Process
            using (new DeGUINodeProcessScope(_nodeProcess, this.position.ResetXY(), ref src.nodeSystem.areaShift)) {
                // Draw nodes
                _nodeProcess.Draw<StartNodeGUI>(src.nodeSystem.startNode);
            }

            if (GUI.changed) EditorUtility.SetDirty(src);
        }

        #endregion
    }
}