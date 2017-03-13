// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/13 14:33
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// One per <see cref="DeGUINodeProcess"/>
    /// </summary>
    public class DeGUINodeInteractionManager
    {
        public enum State
        {
            Inactive,
            Panning, // Panning the whole area
            DrawingSelection, // Drawing a rectangular selection
            DraggingNode // Dragging a node
        }

        public State state { get; private set; }
        public IEditorGUINode targetNode { get; internal set; }

        DeGUINodeProcess _process;

        #region CONSTRUCTOR

        public DeGUINodeInteractionManager(DeGUINodeProcess process)
        {
            _process = process;
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
            case State.DraggingNode:
                _process.editor.Repaint();
                break;
            }
        }

        internal void Update()
        {
            switch (state) {
            case State.Panning:
                EditorGUIUtility.AddCursorRect(_process.area, MouseCursor.Pan);
                break;
            case State.DraggingNode:
                EditorGUIUtility.AddCursorRect(_process.area, MouseCursor.MoveArrow);
                break;
            }
        }

        #endregion
    }
}