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
            GetWindow(typeof(DeGUINodeSampler), true, _Title);
        }

        #endregion

        #region Unity and GUI Methods

        void OnEnable()
        {
            _nodeProcess = new DeGUINodeProcess(this, true);
        }

        void OnHierarchyChange()
        { Repaint(); }

        void OnGUI()
        {
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed") Repaint();
            Undo.RecordObject(src, "DeSampler");
            DG.DemiEditor.DeGUI.BeginGUI();

            // Begin Node GUI Process
            if (_nodeProcess.BeginGUI(this.position.ResetXY(), ref src.nodeSystem.areaShift)) EditorUtility.SetDirty(src);

            // Draw nodes
            _nodeProcess.Draw<StartNodeGUI>(src.nodeSystem.startNode);
        }

        #endregion
    }
}