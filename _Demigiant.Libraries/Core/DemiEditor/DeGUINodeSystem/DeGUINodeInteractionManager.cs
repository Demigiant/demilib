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

        public enum SelectionMode
        {
            Default,
            Add,
            Subtract
        }

        public State state { get; private set; }
        public TargetType mouseTargetType { get; private set; } // Always updated, even no rollover
        public NodeTargetType nodeTargetType { get; private set; }
        public IEditorGUINode targetNode { get; internal set; }
        public SelectionMode selectionMode { get; internal set; }
        public bool mouseTargetIsLocked { get { return state == State.DraggingNodes || state == State.Panning; } }
        public Vector2 mousePositionOnLMBPress { get; internal set; } // Stored mouse position last time LMB was pressed

        DeGUINodeProcess _process;
        MouseCursor _currMouseCursor;

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

        /// <summary>
        /// Returns TRUE if a repaint is required
        /// </summary>
        /// <returns></returns>
        internal bool Update()
        {
            MouseCursor prevMouseCursor = _currMouseCursor;

            switch (state) {
            case State.Panning:
                _currMouseCursor = MouseCursor.Pan;
                break;
            case State.DrawingSelection:
                switch (selectionMode) {
                case SelectionMode.Add:
                    _currMouseCursor = MouseCursor.ArrowPlus;
                    break;
                case SelectionMode.Subtract:
                    _currMouseCursor = MouseCursor.ArrowMinus;
                    break;
                }
                break;
            case State.DraggingNodes:
                _currMouseCursor = MouseCursor.MoveArrow;
                break;
            default:
                if (Event.current.shift) _currMouseCursor = MouseCursor.ArrowPlus;
                else _currMouseCursor = MouseCursor.Arrow;
                break;
            }

            if (_currMouseCursor != MouseCursor.Arrow) {
                EditorGUIUtility.AddCursorRect(_process.area, _currMouseCursor);
            }
            return _currMouseCursor == prevMouseCursor;
        }

        #endregion
    }
}