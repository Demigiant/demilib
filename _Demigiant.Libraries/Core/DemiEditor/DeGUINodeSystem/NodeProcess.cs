// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeExtensions;
using DG.DemiEditor.DeGUINodeSystem.Core;
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
        public InteractionManager interaction { get; private set; }
        public SelectionManager selection { get; private set; }
        public readonly ProcessOptions options = new ProcessOptions();
        public GUIChangeType guiChangeType { get; private set; } // Last GUI.changed reason if set by process (reset on process end)
        /// <summary>Full area without zeroed coordinates</summary>
        public Rect position { get; private set; }
        /// <summary>Position with zeroed coordinates (used by all node GUI since it's inside a GUILayout(area))</summary>
        public Rect relativeArea { get; private set; }
        public Vector2 areaShift { get; private set; }
        public float guiScale { get; private set; }
        internal Vector2 guiScalePositionDiff { get; private set; } // Used by GUI calls that need to rotate the matrix

        internal readonly List<IEditorGUINode> nodes = new List<IEditorGUINode>(); // Used in conjunction with dictionaries to loop them in desired order
        internal readonly Dictionary<IEditorGUINode,NodeGUIData> nodeToGUIData = new Dictionary<IEditorGUINode,NodeGUIData>(); // Refilled on Layout event
        readonly Dictionary<string,IEditorGUINode> _idToNode = new Dictionary<string,IEditorGUINode>();
        readonly Dictionary<Type,ABSDeGUINode> _typeToGUINode = new Dictionary<Type,ABSDeGUINode>();
        readonly Dictionary<IEditorGUINode,NodeConnectionOptions> _nodeToConnectionOptions = new Dictionary<IEditorGUINode,NodeConnectionOptions>();
        readonly NodeDragManager _nodeDragManager;
        readonly List<IEditorGUINode> _tmp_nodes = new List<IEditorGUINode>(); // Used for temporary operations
        readonly Styles _styles = new Styles();
        Minimap _minimap;
        bool _helpPanelActive;
        bool _repaintOnEnd; // If TRUE, repaints the editor during EndGUI. Set to FALSE at each EndGUI
        bool _resetInteractionOnEnd;

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new NodeProcess.
        /// </summary>
        /// <param name="editor">EditorWindow for this process</param>
        public NodeProcess(EditorWindow editor)
        {
            this.editor = editor;
            interaction = new InteractionManager(this);
            selection = new SelectionManager();
            guiScale = 1f;
            _nodeDragManager = new NodeDragManager(this);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Draws the given node using the given T editor GUINode type.
        /// Retuns the full area of the node
        /// </summary>
        public Rect Draw<T>(IEditorGUINode node, NodeConnectionOptions? connectionOptions = null) where T : ABSDeGUINode, new()
        {
            ABSDeGUINode guiNode;
            Type type = typeof(T);
            if (!_typeToGUINode.ContainsKey(type)) {
                guiNode = new T { process = this };
                _typeToGUINode.Add(type, guiNode);
            } else guiNode = _typeToGUINode[type];
            Vector2 nodePosition = new Vector2((int)(node.guiPosition.x + relativeArea.x + areaShift.x), (int)(node.guiPosition.y + relativeArea.y + areaShift.y));
            NodeGUIData nodeGuiData = guiNode.GetAreas(nodePosition, node);

            // Draw node only if visible in area
            if (NodeIsVisible(nodeGuiData.fullArea)) guiNode.OnGUI(nodeGuiData, node);

            switch (Event.current.type) {
            case EventType.Layout:
                nodes.Add(node);
                _idToNode.Add(node.id, node);
                nodeToGUIData.Add(node, nodeGuiData);
                _nodeToConnectionOptions.Add(node, connectionOptions == null ? new NodeConnectionOptions(true) : (NodeConnectionOptions)connectionOptions);
                break;
            case EventType.Repaint:
                // Draw evidence
                if (options.evidenceSelectedNodes && selection.IsSelected(node)) {
                    using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                        GUI.Box(nodeGuiData.fullArea.Expand(6), "", _styles.nodeSelectionOutline);
                    }
                }
                // Draw end node icon
                if (options.evidenceEndNodes && node.connectedNodesIds.Count > 0 && string.IsNullOrEmpty(node.connectedNodesIds[0])) {
                    float icoSize = Mathf.Min(nodeGuiData.fullArea.height, 20);
                    Rect r = new Rect(nodeGuiData.fullArea.xMax - icoSize * 0.5f, nodeGuiData.fullArea.yMax - icoSize * 0.5f, icoSize, icoSize);
                    GUI.DrawTexture(r, DeStylePalette.ico_end);
                }
                break;
            }

            return nodeGuiData.fullArea;
        }

        #endregion

        #region Internal Methods

        // Updates the main node process.
        // Sets <code>GUI.changed</code> to TRUE if the area is panned, a node is dragged, or the eventual sortableNodes list is changed.
        internal void BeginGUI<T>(Rect nodeArea, ref Vector2 refAreaShift, IList<T> sortableNodes = null) where T : IEditorGUINode
        {
            _styles.Init();
            position = nodeArea;
            areaShift = refAreaShift;
            if (options.showMinimap) {
                if (_minimap == null) _minimap = new Minimap(this);
            } else _minimap = null;

            // Set scale
            if (!Mathf.Approximately(guiScale, 1)) {
                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * guiScale);
            }

            // Scale area size to guiScale and begin GUILayout area (so base node area coordinates are 0,0 and everything is easier and faster)
            relativeArea = new Rect(0, 0, nodeArea.width / guiScale, nodeArea.height / guiScale);
            guiScalePositionDiff = new Vector2(nodeArea.x - nodeArea.x / guiScale, nodeArea.y - nodeArea.y / guiScale);
            GUILayout.BeginArea(new Rect(nodeArea.x / guiScale, nodeArea.y / guiScale, relativeArea.width, relativeArea.height));

            // Determine mouse target type before clearing nodeGUIData dictionary
            if (!interaction.mouseTargetIsLocked) EvaluateAndStoreMouseTarget();
            if (Event.current.type == EventType.Layout) {
                nodes.Clear();
                _idToNode.Clear();
                nodeToGUIData.Clear();
                _nodeToConnectionOptions.Clear();
            }

            // Update interaction
            if (interaction.Update()) _repaintOnEnd = true;

            // Background grid
            if (options.drawBackgroundGrid) {
                if (options.gridTextureOverride == null) DeGUI.BackgroundGrid(relativeArea, areaShift, options.forceDarkSkin, 1 / guiScale);
                else DeGUI.BackgroundGrid(relativeArea, areaShift, options.gridTextureOverride, 1 / guiScale);
            }

            switch (Event.current.type) {
            // MOUSE EVENTS ///////////////////////////////////////////////////////////////////////////////////////////////////////
            case EventType.MouseDown:
                switch (Event.current.button) {
                case 0:
                    interaction.mousePositionOnLMBPress = Event.current.mousePosition;
                    switch (interaction.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        // LMB pressed on background
                        // Deselect all
                        if (!Event.current.shift && selection.DeselectAll()) _repaintOnEnd = true;
                        // Start selection drawing
                        if (Event.current.shift) {
                            selection.selectionMode = SelectionManager.Mode.Add;
                            selection.StoreSnapshot();
                        }
                        interaction.SetReadyFor(InteractionManager.ReadyFor.DrawingSelection);
                        break;
                    case InteractionManager.TargetType.Node:
                        // LMB pressed on a node
                        if (Event.current.control || Event.current.command) {
                            // CTRL+Drag on node > drag connection (eventually)
                            bool canDragConnector = interaction.targetNode.connectedNodesIds.Count >= 1
                                                    && _nodeToConnectionOptions[interaction.targetNode].allowManualConnections;
                            if (canDragConnector) interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingConnector);
                        } else if (interaction.nodeTargetType == InteractionManager.NodeTargetType.DraggableArea) {
                            // LMB pressed on a node's draggable area > set readyFor state to DraggingNodes and select node
                            interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingNodes);
                            // Select
                            bool isAlreadySelected = selection.IsSelected(interaction.targetNode);
                            if (Event.current.shift) {
                                if (Event.current.alt) {
                                    // Select this plus all forward connected nodes
                                    if (!isAlreadySelected) selection.Select(interaction.targetNode, true);
                                    SelectAllForwardConnectedNodes(interaction.targetNode);
                                } else {
                                    // Add or remove from selection
                                    if (isAlreadySelected) selection.Deselect(interaction.targetNode);
                                    else selection.Select(interaction.targetNode, true);
                                }
                                _repaintOnEnd = true;
                            } else if (!isAlreadySelected) {
                                // If unselected, select and deselect all others
                                selection.Select(interaction.targetNode, false);
                                _repaintOnEnd = true;
                            }
                        }
                        // Update eventual sorting
                        if (sortableNodes != null) UpdateSorting(sortableNodes);
                        break;
                    }
                    break;
                case 1:
                    switch (interaction.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        // Deselect all nodes
                        _repaintOnEnd = selection.selectedNodes.Count > 0;
                        selection.DeselectAll();
                        break;
                    case InteractionManager.TargetType.Node:
                        // If nodes is not selected, select it and deselect others
                        if (!selection.IsSelected(interaction.targetNode)) {
                            _repaintOnEnd = true;
                            selection.Select(interaction.targetNode, false);
                        }
                        break;
                    }
                    break;
                }
                break;
            case EventType.MouseDrag:
                switch (interaction.readyForState) {
                case InteractionManager.ReadyFor.DrawingSelection:
                    interaction.SetState(InteractionManager.State.DrawingSelection);
                    break;
                case InteractionManager.ReadyFor.DraggingNodes:
                    if ((Event.current.mousePosition - interaction.mousePositionOnLMBPress).magnitude >= InteractionManager.MinDragStartupDistance) {
                        _nodeDragManager.BeginDrag(interaction.targetNode, selection.selectedNodes, nodes, nodeToGUIData);
                        _nodeDragManager.ApplyDrag(Event.current.mousePosition - interaction.mousePositionOnLMBPress - Event.current.delta);
                        interaction.SetState(InteractionManager.State.DraggingNodes);
                    }
                    break;
                case InteractionManager.ReadyFor.DraggingConnector:
                    if ((Event.current.mousePosition - interaction.mousePositionOnLMBPress).magnitude >= InteractionManager.MinDragStartupDistance) {
                        interaction.SetState(InteractionManager.State.DraggingConnector);
                    }
                    break;
                }
                switch (Event.current.button) {
                case 0:
                    switch (interaction.state) {
                    case InteractionManager.State.DrawingSelection:
                        selection.selectionRect = new Rect(
                            Mathf.Min(interaction.mousePositionOnLMBPress.x, Event.current.mousePosition.x),
                            Mathf.Min(interaction.mousePositionOnLMBPress.y, Event.current.mousePosition.y),
                            Mathf.Abs(Event.current.mousePosition.x - interaction.mousePositionOnLMBPress.x),
                            Mathf.Abs(Event.current.mousePosition.y - interaction.mousePositionOnLMBPress.y)
                        );
                        if (selection.selectionMode == SelectionManager.Mode.Add) {
                            // Add eventual nodes stored when starting to draw
                            selection.Select(selection.selectedNodesSnapshot, false);
                        } else selection.DeselectAll();
                        foreach (IEditorGUINode node in nodes) {
                            if (selection.selectionRect.Includes(nodeToGUIData[node].fullArea)) selection.Select(node, true);
                        }
                        _repaintOnEnd = true;
                        break;
                    case InteractionManager.State.DraggingNodes:
                        // Drag selected nodes
                        _nodeDragManager.ApplyDrag(Event.current.delta);
                        guiChangeType = GUIChangeType.DragNodes;
                        GUI.changed = _repaintOnEnd = true;
                        break;
                    case InteractionManager.State.DraggingConnector:
                        _repaintOnEnd = true;
                        break;
                    }
                    break;
                case 2:
                    // Panning
                    interaction.SetState(InteractionManager.State.Panning);
                    refAreaShift = areaShift += Event.current.delta;
                    guiChangeType = GUIChangeType.Pan;
                    GUI.changed = _repaintOnEnd = true;
                    break;
                }
                break;
            case EventType.ContextClick:
                interaction.SetState(InteractionManager.State.ContextClick);
                _resetInteractionOnEnd = true;
                break;
            case EventType.ScrollWheel:
                if (options.mouseWheelScalesGUI) {
                    // Zoom
                    bool isScaleUp = Event.current.delta.y < 0;
                    if (isScaleUp && Mathf.Approximately(options.guiScaleValues[0], guiScale)) break;
                    if (!isScaleUp && Mathf.Approximately(options.guiScaleValues[options.guiScaleValues.Length - 1], guiScale)) break;
                    for (int i = 0; i < options.guiScaleValues.Length; ++i) {
                        if (!Mathf.Approximately(options.guiScaleValues[i], guiScale)) continue;
                        guiScale = isScaleUp ? options.guiScaleValues[i - 1] : options.guiScaleValues[i + 1];
                        _repaintOnEnd = true;
                        break;
                    }
                }
                break;
            // KEYBOARD EVENTS //////////////////////////////////////////////////////////////////////////////////////////////////////
            case EventType.KeyUp:
                switch (Event.current.keyCode) {
                case KeyCode.F1:
                    // Help Panel
                    _helpPanelActive = !_helpPanelActive;
                    _repaintOnEnd = true;
                    break;
                case KeyCode.A:
                    if (interaction.HasControlKeyModifier()) {
                        // CTRL+A > Select all nodes
                        selection.Select(nodes, false);
                        _repaintOnEnd = true;
                    }
                    break;
                }
                break;
            }
            // RAW MOUSE EVENTS (used to capture mouseUp outside editorWindow) //////////////////////////////////////////////////////
            switch (Event.current.rawType) {
            case EventType.MouseUp:
                switch (interaction.state) {
                case InteractionManager.State.DrawingSelection:
                    selection.selectionMode = SelectionManager.Mode.Default;
                    selection.ClearSnapshot();
                    selection.selectionRect = new Rect();
                    _repaintOnEnd = true;
                    break;
                case InteractionManager.State.DraggingConnector:
                    // TODO MouseUp on DraggingConnector > Implement connection indexes higher than 0
                    IEditorGUINode overNode = GetMouseOverNode();
                    if (overNode != null && overNode != interaction.targetNode) {
                        // Create new connection
                        Connector.dragData.node.connectedNodesIds[0] = overNode.id;
                        GUI.changed = _repaintOnEnd = true;
                    } else {
                        // Disconnect
                        bool changed = !string.IsNullOrEmpty(Connector.dragData.node.connectedNodesIds[0]);
                        Connector.dragData.node.connectedNodesIds[0] = null;
                        if (changed) GUI.changed = true;
                        _repaintOnEnd = true;
                    }
                    break;
                }
                bool isLMBDoubleClick = interaction.EvaluateMouseUp();
                if (isLMBDoubleClick) {
                    interaction.SetState(InteractionManager.State.DoubleClick);
                    _resetInteractionOnEnd = true;
                } else interaction.SetState(InteractionManager.State.Inactive);
                break;
            }
        }

        internal void EndGUI()
        {
            // Draw elements if a repaint is not going to be called in the end
            if (!_repaintOnEnd) {
                if (Event.current.type == EventType.Repaint) {
                    // DRAW CONNECTIONS BETWEEN NODES
                    for (int i = 0; i < nodes.Count; ++i) {
                        IEditorGUINode fromNode = nodes[i];
                        List<string> connections = fromNode.connectedNodesIds;
                        int totConnections = fromNode.connectedNodesIds.Count;
                        for (int c = 0; c < totConnections; ++c) {
                            string connId = connections[c];
                            if (string.IsNullOrEmpty(connId)) continue;
                            if (!_idToNode.ContainsKey(connId)) {
                                // Node eliminated externally, remove from dictionary
                                _idToNode.Remove(connId);
                            } else {
                                IEditorGUINode toNode = _idToNode[connId];
                                Connector.Connect(
                                    this, c, totConnections, nodeToGUIData[fromNode], _nodeToConnectionOptions[fromNode], nodeToGUIData[toNode]
                                );
                            }
                        }
                    }
                    switch (interaction.state) {
                    // DRAW RECTANGULAR SELECTION
                    case InteractionManager.State.DrawingSelection:
                        // Draw selection
                        using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                            GUI.Box(selection.selectionRect, "", _styles.selectionRect);
                        }
                        break;
                    // DRAW CONNECTOR DRAGGING
                    case InteractionManager.State.DraggingConnector:
                        Connector.Drag(
                            interaction.targetNode, nodeToGUIData[interaction.targetNode], _nodeToConnectionOptions[interaction.targetNode],
                            Event.current.mousePosition
                        );
                        // Evidence possible connection
                        IEditorGUINode overNode = GetMouseOverNode();
                        if (overNode != null && overNode != interaction.targetNode) {
                            using (new DeGUI.ColorScope(DeGUI.colors.global.orange)) {
                                GUI.Box(nodeToGUIData[overNode].fullArea.Expand(4), "", _styles.nodeSelectionOutline);
                            }
                        }
                        break;
                    }
                    // NODE DRAGGING GUIDES
                    if (interaction.state == InteractionManager.State.DraggingNodes) _nodeDragManager.EndGUI();
                    // MINIMAP
                    if (_minimap != null) _minimap.Draw();
                    // HELP PANEL
                    if (_helpPanelActive) HelpPanel.Draw(this);
                }
            }

            // Clean
            guiChangeType = GUIChangeType.None;

            // EXTRA END ACTIONS
            // Reset interaction
            if (_resetInteractionOnEnd) {
                _resetInteractionOnEnd = false;
                interaction.SetState(InteractionManager.State.Inactive);
            }
            // Repaint
            if (_repaintOnEnd) {
                _repaintOnEnd = false;
                if (_minimap != null) _minimap.RefreshMapTextureOnNextPass();
                editor.Repaint();
            }

            // Close area and reset GUI matrix
            GUILayout.EndArea();
            GUI.matrix = Matrix4x4.identity;
        }

        #endregion

        #region Methods

        // Store mouse target (even in case of rollovers) and set related data
        void EvaluateAndStoreMouseTarget()
        {
            if (!relativeArea.Contains(Event.current.mousePosition)) {
                // Mouse out of editor
                interaction.SetMouseTargetType(InteractionManager.TargetType.None);
                interaction.targetNode = null;
                return;
            }
            IEditorGUINode targetNode = GetMouseOverNode();
            if (targetNode != null) {
                interaction.targetNode = targetNode;
                interaction.SetMouseTargetType(
                    InteractionManager.TargetType.Node,
                    nodeToGUIData[targetNode].dragArea.Contains(Event.current.mousePosition)
                        ? InteractionManager.NodeTargetType.DraggableArea
                        : InteractionManager.NodeTargetType.NonDraggableArea
                );
                return;
            }
            interaction.SetMouseTargetType(InteractionManager.TargetType.Background);
            interaction.targetNode = null;
        }

        IEditorGUINode GetMouseOverNode()
        {
            for (int i = nodes.Count - 1; i > -1; --i) {
                IEditorGUINode node = nodes[i];
                if (nodeToGUIData[node].fullArea.Contains(Event.current.mousePosition)) return node;
            }
            return null;
        }

        // Called only if sortableNodes is not NULL and the event is MouseDown
        void UpdateSorting<T>(IList<T> sortableNodes) where T : IEditorGUINode
        {
            int totSelected = selection.selectedNodes.Count;
            int totSortables = sortableNodes.Count;
            if (totSelected == 0 || totSortables == 0 || totSelected >= totSortables) return;
            bool sortingRequired = false;
            if (totSelected == 1) {
                // Single selection
                IEditorGUINode selectedNode = selection.selectedNodes[0];
                sortingRequired = selectedNode.id != sortableNodes[totSortables - 1].id;
                if (!sortingRequired) return;
                for (int i = 0; i < totSortables; ++i) {
                    if (sortableNodes[i].id != interaction.targetNode.id) continue;
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
            GUI.changed = _repaintOnEnd = true;
        }

        // Adds all forward connected nodes to selection, paying attention not to select a node twice
        void SelectAllForwardConnectedNodes(IEditorGUINode fromNode)
        {
            _tmp_nodes.Clear(); // used to store nodes for which we already parsed forward connections
            SelectAllForwardConnectedNodes_Select(fromNode);
        }
        void SelectAllForwardConnectedNodes_Select(IEditorGUINode fromNode)
        {
            foreach (string id in fromNode.connectedNodesIds) {
                if (string.IsNullOrEmpty(id)) continue;
                IEditorGUINode forwardConnectedNode = _idToNode[id];
                if (_tmp_nodes.Contains(forwardConnectedNode)) continue;
                _tmp_nodes.Add(forwardConnectedNode);
                selection.Select(forwardConnectedNode, true);
                SelectAllForwardConnectedNodes_Select(forwardConnectedNode);
            }
        }

        #endregion

        #region Helpers

        bool NodeIsVisible(Rect nodeArea)
        {
            return nodeArea.xMax > relativeArea.xMin && nodeArea.xMin < relativeArea.xMax && nodeArea.yMax > relativeArea.yMin && nodeArea.yMin < relativeArea.yMax;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle selectionRect, nodeSelectionOutline, endNodeOutline;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                selectionRect = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.squareBorderAlpha15);
                nodeSelectionOutline = DeGUI.styles.box.outline01.Clone()
                    .Border(new RectOffset(5, 5, 5, 5)).Background(DeStylePalette.squareBorderCurvedEmpty);
                endNodeOutline = nodeSelectionOutline.Clone().Background(DeStylePalette.squareCornersEmpty02)
                    .Border(new RectOffset(7, 7, 7, 7));
            }
        }
    }
}