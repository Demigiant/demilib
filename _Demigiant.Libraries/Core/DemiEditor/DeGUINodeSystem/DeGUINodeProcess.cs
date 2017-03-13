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
    /// Main class for DeGUI Node system. Call <see cref="Update"/> every GUI call when starting a nodes area.
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

        /// <summary>
        /// Updates the main node process and returns TRUE if the area shift or a node position has changed.
        /// Doesn't need to be concluded with an End.<para/>
        /// Automatically manages area drag operations with middle mouse.<para/>
        /// Sets <code>GUI.changed</code> to TRUE if the area is panned or a node is dragged.
        /// </summary>
        /// <param name="nodeArea">Area within which the nodes will be drawn</param>
        /// <param name="refAreaShift">Area shift (caused by dragging)</param>
        public bool Update(Rect nodeArea, ref Vector2 refAreaShift)
        {
            bool guiChanged = false;
            area = nodeArea;
            areaShift = refAreaShift;
            if (Event.current.type == EventType.Layout) _nodeToGUIData.Clear();
            interactionManager.Update();

            if (_drawBackgroundGrid) DeGUI.BackgroundGrid(area, areaShift, _forceDarkSkin);

            // MOUSE EVENTS
            switch (Event.current.type) {
            case EventType.MouseDown:
                switch (Event.current.button) {
                case 0:
                    IEditorGUINode targetNode = GetMouseTarget();
                    if (targetNode != null) {
                        // Mouse pressed on node
                        if (_nodeToGUIData[targetNode].dragArea.Contains(Event.current.mousePosition)) {
                            // Drag area pressed, start dragging
                            interactionManager.targetNode = targetNode;
                            interactionManager.SetState(DeGUINodeInteractionManager.State.DraggingNode);
                        }
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
                        guiChanged = GUI.changed = true;
                        editor.Repaint();
                    }
                    break;
                case 2:
                    // Panning
                    interactionManager.SetState(DeGUINodeInteractionManager.State.Panning);
                    refAreaShift = areaShift += Event.current.delta;
                    guiChanged = GUI.changed = true;
                    editor.Repaint();
                    break;
                }
                break;
            case EventType.MouseUp:
                interactionManager.SetState(DeGUINodeInteractionManager.State.Inactive);
                break;
            }

            return guiChanged;
        }

        public void Draw<T>(IEditorGUINode node) where T : ABSDeGUINode, new()
        {
            Type type = typeof(T);
            if (!_typeToGUINode.ContainsKey(type)) _typeToGUINode.Add(type, (T)Activator.CreateInstance(type, this));
            ABSDeGUINode guiNode = _typeToGUINode[type];
            Vector2 position = new Vector2((int)(node.guiPosition.x + areaShift.x), (int)(node.guiPosition.y + areaShift.y));
            DeGUINodeData guiNodeData = guiNode.GetAreas(position, node);
            if (NodeIsVisible(guiNodeData.fullArea)) guiNode.OnGUI(guiNodeData, node);

            if (Event.current.type == EventType.Layout) _nodeToGUIData.Add(node, guiNodeData);
        }

        #endregion

        #region Helpers

        IEditorGUINode GetMouseTarget()
        {
            foreach (KeyValuePair<IEditorGUINode, DeGUINodeData> kvp in _nodeToGUIData) {
                if (kvp.Value.fullArea.Contains(Event.current.mousePosition)) return kvp.Key;
            }
            return null;
        }

        bool NodeIsVisible(Rect nodeArea)
        {
            return nodeArea.xMax > area.xMin && nodeArea.xMin < area.xMax && nodeArea.yMax > area.yMin && nodeArea.yMin < area.yMax;
        }

        #endregion
    }
}