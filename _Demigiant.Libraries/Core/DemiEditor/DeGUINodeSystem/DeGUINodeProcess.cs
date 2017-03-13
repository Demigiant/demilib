// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// Main class for DeGUI Node system.
    /// Create it, then enclose your GUI node calls inside a <see cref="DeGUINodeProcessScope"/>.<para/>
    /// CODING ORDER:<para/>
    /// - Create a <see cref="DeGUINodeProcess"/> to use for your node system (create it once, obviously)<para/>
    /// - Inside OnGUI, write all your nodes GUI code inside a <see cref="DeGUINodeProcessScope"/>
    /// </summary>
    public class DeGUINodeProcess
    {
        public EditorWindow editor { get; private set; }
        public DeGUINodeInteractionManager interactionManager { get; private set; }
        public Rect area { get; private set; }
        public Vector2 areaShift { get; private set; }

        readonly bool _drawBackgroundGrid;
        readonly bool _forceDarkSkin;
        readonly Dictionary<Type,ABSDeGUINode> _typeToGUINode = new Dictionary<Type,ABSDeGUINode>();
        readonly Dictionary<IEditorGUINode,DeGUINodeData> _nodeToGUIData = new Dictionary<IEditorGUINode,DeGUINodeData>(); // Refilled on Layout event

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new DeGUINodeProcess.
        /// </summary>
        /// <param name="editor">EditorWindow for this process</param>
        /// <param name="drawBackgroundGrid">If TRUE draws a background grid</param>
        /// <param name="forceDarkSkin">If TRUE forces the grid skin to black, otherwise varies based on current Unity free/pro skin</param>
        public DeGUINodeProcess(EditorWindow editor, bool drawBackgroundGrid = false, bool forceDarkSkin = false)
        {
            this.editor = editor;
            interactionManager = new DeGUINodeInteractionManager(this);
            _drawBackgroundGrid = drawBackgroundGrid;
            _forceDarkSkin = forceDarkSkin;
        }

        #endregion

        #region Public Methods

        /// <summary>Draws the given node using the given T editor GUINode type</summary>
        public void Draw<T>(IEditorGUINode node) where T : ABSDeGUINode, new()
        {
            ABSDeGUINode guiNode;
            Type type = typeof(T);
            if (!_typeToGUINode.ContainsKey(type)) {
                guiNode = new T { process = this };
                _typeToGUINode.Add(type, guiNode);
            } else guiNode = _typeToGUINode[type];
            Vector2 position = new Vector2((int)(node.guiPosition.x + areaShift.x), (int)(node.guiPosition.y + areaShift.y));
            DeGUINodeData guiNodeData = guiNode.GetAreas(position, node);
            if (NodeIsVisible(guiNodeData.fullArea)) guiNode.OnGUI(guiNodeData, node);

            if (Event.current.type == EventType.Layout) _nodeToGUIData.Add(node, guiNodeData);
        }

        #endregion

        #region Internal Methods

        // Updates the main node process.
        // Sets <code>GUI.changed</code> to TRUE if the area is panned or a node is dragged.
        internal void BeginGUI(Rect nodeArea, ref Vector2 refAreaShift)
        {
            area = nodeArea;
            areaShift = refAreaShift;

            // Determine mouse target type before clearing nodeGUIData dictionary
            if (!interactionManager.mouseTargetIsLocked) StoreMouseTarget();
            if (Event.current.type == EventType.Layout) _nodeToGUIData.Clear();

            // Update interactionManager
            interactionManager.Update();

            // Background grid
            if (_drawBackgroundGrid) DeGUI.BackgroundGrid(area, areaShift, _forceDarkSkin);

            // MOUSE EVENTS
            switch (Event.current.type) {
            case EventType.MouseDown:
                switch (Event.current.button) {
                case 0:
                    if (interactionManager.nodeTargetType == DeGUINodeInteractionManager.NodeTargetType.DraggableArea) {
                        // Mouse pressed on a node's draggable area
                        interactionManager.SetState(DeGUINodeInteractionManager.State.DraggingNode);
                    }
                    break;
                }
                break;
            case EventType.MouseDrag:
                switch (Event.current.button) {
                case 0:
                    if (interactionManager.state == DeGUINodeInteractionManager.State.DraggingNode) {
                        // Drag node
                        interactionManager.targetNode.guiPosition += Event.current.delta;
                        GUI.changed = true;
                        editor.Repaint();
                    }
                    break;
                case 2:
                    // Panning
                    interactionManager.SetState(DeGUINodeInteractionManager.State.Panning);
                    refAreaShift = areaShift += Event.current.delta;
                    GUI.changed = true;
                    editor.Repaint();
                    break;
                }
                break;
            case EventType.MouseUp:
                interactionManager.SetState(DeGUINodeInteractionManager.State.Inactive);
                break;
            case EventType.ContextClick:
                break;
            }
        }

        internal void EndGUI()
        {
            // Unused for now
        }

        #endregion

        #region Methods

        void StoreMouseTarget()
        {
            if (!area.Contains(Event.current.mousePosition)) {
                // Mouse out of editor
                interactionManager.SetMouseTargetType(DeGUINodeInteractionManager.TargetType.None);
                interactionManager.targetNode = null;
                return;
            }
            foreach (KeyValuePair<IEditorGUINode, DeGUINodeData> kvp in _nodeToGUIData) {
                if (kvp.Value.fullArea.Contains(Event.current.mousePosition)) {
                    // Mouse on node
                    interactionManager.targetNode = kvp.Key;
                    if (_nodeToGUIData[kvp.Key].dragArea.Contains(Event.current.mousePosition)) {
                        // Mouse on node's drag area
                        interactionManager.SetMouseTargetType(DeGUINodeInteractionManager.TargetType.Node, DeGUINodeInteractionManager.NodeTargetType.DraggableArea);
                    } else {
                        // Mouse on node but outside drag area
                        interactionManager.SetMouseTargetType(DeGUINodeInteractionManager.TargetType.Node, DeGUINodeInteractionManager.NodeTargetType.NonDraggableArea);
                    }
                    return;
                }
            }
            interactionManager.SetMouseTargetType(DeGUINodeInteractionManager.TargetType.Background);
            interactionManager.targetNode = null;
        }

        #endregion

        #region Helpers

        bool NodeIsVisible(Rect nodeArea)
        {
            return nodeArea.xMax > area.xMin && nodeArea.xMin < area.xMax && nodeArea.yMax > area.yMin && nodeArea.yMin < area.yMax;
        }

        #endregion
    }
}