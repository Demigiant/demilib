// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeExtensions;
using DG.DemiEditor.Internal;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// Main class for DeGUI Node system.
    /// Create it, then enclose your GUI node calls inside a <see cref="NodeProcessScope"/>.<para/>
    /// CODING ORDER:<para/>
    /// - Create a <see cref="NodeProcess"/> to use for your node system (create it once, obviously)<para/>
    /// - Inside OnGUI, write all your nodes GUI code inside a <see cref="NodeProcessScope"/>
    /// </summary>
    public class NodeProcess
    {
        public enum GUIChangeType
        {
            None,
            Pan,
            DragNodes,
            SortedNodes
        }

        public EditorWindow editor { get; private set; }
        public InteractionManager interactionManager { get; private set; }
        public SelectionManager selection { get; private set; }
        public readonly ProcessOptions options = new ProcessOptions();
        public GUIChangeType guiChangeType { get; private set; } // Last GUI.changed reason if set by process (reset on process end)
        public Rect area { get; private set; }
        public Vector2 areaShift { get; private set; }

        readonly List<IEditorGUINode> _nodes = new List<IEditorGUINode>(); // Used in conjunction with dictionaries to loop them in desired order
        readonly Dictionary<IEditorGUINode,NodeGUIData> _nodeToGUIData = new Dictionary<IEditorGUINode,NodeGUIData>(); // Refilled on Layout event
        readonly Dictionary<Type,ABSDeGUINode> _typeToGUINode = new Dictionary<Type,ABSDeGUINode>();
        readonly Styles _styles = new Styles();
        bool _requiresRepaint; // Set to FALSE at each EndGUI

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new NodeProcess.
        /// </summary>
        /// <param name="editor">EditorWindow for this process</param>
        public NodeProcess(EditorWindow editor)
        {
            this.editor = editor;
            interactionManager = new InteractionManager(this);
            selection = new SelectionManager();
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
            NodeGUIData nodeGuiData = guiNode.GetAreas(position, node);

            // Draw node only if visible in area
            if (NodeIsVisible(nodeGuiData.fullArea)) guiNode.OnGUI(nodeGuiData, node);

            switch (Event.current.type) {
            case EventType.Layout:
                _nodes.Add(node);
                _nodeToGUIData.Add(node, nodeGuiData);
                break;
            case EventType.Repaint:
                // Draw evidence
                if (options.evidenceSelectedNodes && selection.IsSelected(node)) {
                    using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                        GUI.Box(nodeGuiData.fullArea.Expand(3), "", _styles.nodeOutlineThick);
                    }
                }
                break;
            }
        }

        #endregion

        #region Internal Methods

        // Updates the main node process.
        // Sets <code>GUI.changed</code> to TRUE if the area is panned, a node is dragged, or the eventual sortableNodes list is changed.
        internal void BeginGUI<T>(Rect nodeArea, ref Vector2 refAreaShift, IList<T> sortableNodes = null) where T : IEditorGUINode
        {
            _styles.Init();
            area = nodeArea;
            areaShift = refAreaShift;

            // Determine mouse target type before clearing nodeGUIData dictionary
            if (!interactionManager.mouseTargetIsLocked) StoreMouseTarget();
            if (Event.current.type == EventType.Layout) {
                _nodes.Clear();
                _nodeToGUIData.Clear();
            }

            // Update interactionManager
            if (interactionManager.Update()) _requiresRepaint = true;

            // Background grid
            if (options.drawBackgroundGrid) DeGUI.BackgroundGrid(area, areaShift, options.forceDarkSkin);

            // MOUSE EVENTS
            switch (Event.current.type) {
            case EventType.MouseDown:
                switch (Event.current.button) {
                case 0:
                    interactionManager.mousePositionOnLMBPress = Event.current.mousePosition;
                    switch (interactionManager.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        // LMB pressed on background
                        // Deselect all
                        if (!Event.current.shift && selection.DeselectAll()) _requiresRepaint = true;
                        // Start selection drawing
                        if (Event.current.shift) {
                            selection.selectionMode = SelectionManager.Mode.Add;
                            selection.StoreSnapshot();
                        }
                        interactionManager.SetReadyFor(InteractionManager.ReadyFor.DrawingSelection);
                        break;
                    case InteractionManager.TargetType.Node:
                        // LMB pressed on a node
                        // Select
                        bool isAlreadySelected = selection.IsSelected(interactionManager.targetNode);
                        if (Event.current.shift) {
                            if (isAlreadySelected) selection.Deselect(interactionManager.targetNode);
                            else selection.Select(interactionManager.targetNode, true);
                            _requiresRepaint = true;
                        } else if (!isAlreadySelected) {
                            selection.Select(interactionManager.targetNode, false);
                            _requiresRepaint = true;
                        }
                        //
                        if (interactionManager.nodeTargetType == InteractionManager.NodeTargetType.DraggableArea) {
                            // LMB pressed on a node's draggable area: set state to draggingNodes
                            interactionManager.SetReadyFor(InteractionManager.ReadyFor.DraggingNodes);
                        }
                        // Update eventual sorting
                        if (sortableNodes != null) UpdateSorting(sortableNodes);
                        break;
                    }
                    break;
                }
                break;
            case EventType.MouseDrag:
                switch (interactionManager.readyForState) {
                case InteractionManager.ReadyFor.DrawingSelection:
                    interactionManager.SetState(InteractionManager.State.DrawingSelection, true);
                    break;
                case InteractionManager.ReadyFor.DraggingNodes:
                    interactionManager.SetState(InteractionManager.State.DraggingNodes, true);
                    break;
                }
                switch (Event.current.button) {
                case 0:
                    switch (interactionManager.state) {
                    case InteractionManager.State.DrawingSelection:
                        selection.selectionRect = new Rect(
                            Mathf.Min(interactionManager.mousePositionOnLMBPress.x, Event.current.mousePosition.x),
                            Mathf.Min(interactionManager.mousePositionOnLMBPress.y, Event.current.mousePosition.y),
                            Mathf.Abs(Event.current.mousePosition.x - interactionManager.mousePositionOnLMBPress.x),
                            Mathf.Abs(Event.current.mousePosition.y - interactionManager.mousePositionOnLMBPress.y)
                        );
                        if (selection.selectionMode == SelectionManager.Mode.Add) {
                            // Add eventual nodes stored when starting to draw
                            selection.Select(selection.selectedNodesSnapshot, false);
                        } else selection.DeselectAll();
                        foreach (IEditorGUINode node in _nodes) {
                            if (selection.selectionRect.Includes(_nodeToGUIData[node].fullArea)) selection.Select(node, true);
                        }
                        _requiresRepaint = true;
                        break;
                    case InteractionManager.State.DraggingNodes:
                        // Drag node/s
                        foreach (IEditorGUINode node in selection.selectedNodes) node.guiPosition += Event.current.delta;
                        guiChangeType = GUIChangeType.DragNodes;
                        GUI.changed = _requiresRepaint = true;
                        break;
                    }
                    break;
                case 2:
                    // Panning
                    interactionManager.SetState(InteractionManager.State.Panning);
                    refAreaShift = areaShift += Event.current.delta;
                    guiChangeType = GUIChangeType.Pan;
                    GUI.changed = _requiresRepaint = true;
                    break;
                }
                break;
            case EventType.MouseUp:
                switch (interactionManager.state) {
                case InteractionManager.State.DrawingSelection:
                    selection.selectionMode = SelectionManager.Mode.Default;
                    selection.ClearSnapshot();
                    selection.selectionRect = new Rect();
                    _requiresRepaint = true;
                    break;
                }
                interactionManager.SetState(InteractionManager.State.Inactive, true);
                break;
            case EventType.ContextClick:
                break;
            }
        }

        internal void EndGUI()
        {
            // EVIDENCE SELECTED NODES + DRAW RECTANGULAR SELECTION
            if (Event.current.type == EventType.Repaint) {
                // Draw selection
                if (interactionManager.state == InteractionManager.State.DrawingSelection) {
                    using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                        GUI.Box(selection.selectionRect, "", _styles.selectionRect);
                    }
                }
            }

            // Clean
            guiChangeType = GUIChangeType.None;

            // Repaint if necessary
            if (_requiresRepaint) {
                editor.Repaint();
                _requiresRepaint = false;
            }
        }

        #endregion

        #region Methods

        // Store mouse target (even in case of rollovers)
        void StoreMouseTarget()
        {
            if (!area.Contains(Event.current.mousePosition)) {
                // Mouse out of editor
                interactionManager.SetMouseTargetType(InteractionManager.TargetType.None);
                interactionManager.targetNode = null;
                return;
            }
            for (int i = _nodes.Count - 1; i > -1; --i) {
                IEditorGUINode node = _nodes[i];
                NodeGUIData data = _nodeToGUIData[node];
                if (!data.fullArea.Contains(Event.current.mousePosition)) continue;
                // Mouse on node
                interactionManager.targetNode = node;
                interactionManager.SetMouseTargetType(
                    InteractionManager.TargetType.Node,
                    data.dragArea.Contains(Event.current.mousePosition)
                        ? InteractionManager.NodeTargetType.DraggableArea
                        : InteractionManager.NodeTargetType.NonDraggableArea
                );
                return;
            }
            interactionManager.SetMouseTargetType(InteractionManager.TargetType.Background);
            interactionManager.targetNode = null;
        }

        // Called only if sortableNodes is not NULL and the event is MouseDown
        void UpdateSorting<T>(IList<T> sortableNodes) where T : IEditorGUINode
        {
            int totSelected = selection.selectedNodes.Count;
            int totSortables = sortableNodes.Count;
            if (totSelected == 0 || totSortables == 0) return;
            bool sortingRequired = false;
            if (totSelected == 1) {
                // Single selection
                IEditorGUINode selectedNode = selection.selectedNodes[0];
                sortingRequired = selectedNode.id != sortableNodes[totSortables - 1].id;
                if (!sortingRequired) return;
                for (int i = 0; i < totSortables; ++i) {
                    if (sortableNodes[i].id != interactionManager.targetNode.id) continue;
                    sortableNodes.Shift(i, totSortables - 1);
                    break;
                }
            } else {
                // Multiple selections
                for (int i = totSortables - 1; i > totSortables - totSelected - 1; --i) {
                    if (selection.selectedNodes.Contains(sortableNodes[i])) continue;
                    sortingRequired = true;
                    break;
                }
                if (!sortingRequired) return;
                int shiftOffset = 0;
                for (int i = totSortables - 1; i > -1; --i) {
                    if (!selection.selectedNodes.Contains(sortableNodes[i])) continue;
                    sortableNodes.Shift(i, totSortables - 1 - shiftOffset);
                    shiftOffset++;
                }
            }
            guiChangeType = GUIChangeType.SortedNodes;
            GUI.changed = _requiresRepaint = true;
        }

        #endregion

        #region Helpers

        bool NodeIsVisible(Rect nodeArea)
        {
            return nodeArea.xMax > area.xMin && nodeArea.xMin < area.xMax && nodeArea.yMax > area.yMin && nodeArea.yMin < area.yMax;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle selectionRect, nodeOutline, nodeOutlineThick;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                selectionRect = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.squareBorderAlpha15);
                nodeOutline = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.squareBorderEmpty);
                nodeOutlineThick = nodeOutline.Clone().Border(new RectOffset(5, 5, 5, 5)).Background(DeStylePalette.squareBorderThickEmpty);
            }
        }
    }
}