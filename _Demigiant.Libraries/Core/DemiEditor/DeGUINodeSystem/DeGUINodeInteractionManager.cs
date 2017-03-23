// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/13 14:33
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// One per <see cref="DeGUINodeProcess"/>.
    /// Partially independent, mainly controlled by process.
    /// </summary>
    public class DeGUINodeInteractionManager
    {
        public enum State
        {
            Inactive,
            Panning,
            DrawingSelection,
            DraggingNodes
        }

        public enum TargetType
        {
            None,
            Background,
            Node
        }

        public enum NodeTargetType
        {
            None,
            DraggableArea,
            NonDraggableArea
        }

        public State state { get; private set; }
        public TargetType mouseTargetType { get; private set; } // Always updated, even no rollover
        public NodeTargetType nodeTargetType { get; private set; }
        public IEditorGUINode targetNode { get; internal set; }
        public bool mouseTargetIsLocked { get { return state == State.DraggingNodes || state == State.Panning; } }

        DeGUINodeProcess _process;

        #region CONSTRUCTOR

        public DeGUINodeInteractionManager(DeGUINodeProcess process)
        {
            _process = process;
        }

        #endregion

        #region Public Methods

        /// <summary>Returns TRUE if the given node is currently being dragged</summary>
        public bool IsDragging(IEditorGUINode node)
        {
            return state == State.DraggingNodes && targetNode == node;
        }

        #endregion

        #region Internal Methods

        internal void SetState(State toState)
        {
            State prevState = state;
            state = toState;

            // Repaint editor if necessary
            switch (prevState) {
            case State.Panning:
            case State.DraggingNodes:
                _process.editor.Repaint();
                break;
            }
        }

        internal void SetMouseTargetType(TargetType targetType, NodeTargetType nodeTargetType = NodeTargetType.None)
        {
            this.mouseTargetType = targetType;
            this.nodeTargetType = nodeTargetType;
        }

        internal void Update()
        {
            switch (state) {
            case State.Panning:
                EditorGUIUtility.AddCursorRect(_process.area, MouseCursor.Pan);
                break;
            case State.DraggingNodes:
                EditorGUIUtility.AddCursorRect(_process.area, MouseCursor.MoveArrow);
                break;
            }
        }

        #endregion
    }
}