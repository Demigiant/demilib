// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            SortedNodes,
            DeletedNodes,
            AddedNodes
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
        static readonly List<IEditorGUINode> _NodesClipboard = new List<IEditorGUINode>(); // Nodes copied and currently stored in clipboard
        readonly List<IEditorGUINode> _tmp_nodes = new List<IEditorGUINode>(); // Used for temporary operations
        readonly List<string> _tmp_string = new List<string>(); // Used for temporary operations
        static readonly Styles _Styles = new Styles();
        readonly Func<List<IEditorGUINode>,bool> _onDeleteNodesCallback; // Returns FALSE if deletion shouldn't happen
        readonly Func<IEditorGUINode,IEditorGUINode,bool> _onCloneNodeCallback; // Returns FALSE if cloning shouldn't happen
        Minimap _minimap;
        bool _helpPanelActive;
        bool _repaintOnEnd; // If TRUE, repaints the editor during EndGUI. Set to FALSE at each EndGUI
        bool _resetInteractionOnEnd;
        bool _isAltPressed; // Used to prevent a repaint when ALT is being kept pressed

        #region CONSTRUCTOR

        /// <summary>
        /// Creates a new NodeProcess.
        /// </summary>
        /// <param name="editor">EditorWindow for this process</param>
        /// <param name="onDeleteNodesCallback">Callback called when one or more nodes are going to be deleted.
        /// Return FALSE if you want the deletion to be canceled.
        /// Can be NULL, in which case it will be ignored</param>
        /// <param name="onCloneNodeCallback">Callback called when a node is cloned.
        /// Return FALSE if you want the cloning to be canceled.
        /// Can be NULL, in which case it will be ignored</param>
        public NodeProcess(
            EditorWindow editor,
            Func<List<IEditorGUINode>,bool> onDeleteNodesCallback = null,
            Func<IEditorGUINode,IEditorGUINode,bool> onCloneNodeCallback = null
        ){
            this.editor = editor;
            this._onDeleteNodesCallback = onDeleteNodesCallback;
            this._onCloneNodeCallback = onCloneNodeCallback;
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
            if (NodeIsVisible(nodeGuiData.fullArea)) {
                guiNode.OnGUI(nodeGuiData, node);
                // Fade out unselected nodes if there's others that are selected
                bool faded = selection.selectedNodes.Count > 0 && !selection.IsSelected(node);
                if (faded) GUI.DrawTexture(nodeGuiData.fullArea, DeStylePalette.blackSquareAlpha50);
            }

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
                        GUI.Box(nodeGuiData.fullArea.Expand(5), "", _Styles.nodeSelectionOutlineThick);
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

        /// <summary>
        /// Returns a clone of the given node (clones also lists, but leaves other references as references).
        /// A new ID will be automatically generated.
        /// </summary>
        public T CloneNode<T>(IEditorGUINode node) where T : IEditorGUINode, new()
        {
            T clone = new T();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            FieldInfo[] srcFields = node.GetType().GetFields(flags);
            FieldInfo[] destFields = clone.GetType().GetFields(flags);
            foreach (FieldInfo srcField in srcFields) {
                FieldInfo destField = destFields.FirstOrDefault(field => field.Name == srcField.Name);
                if (destField == null || destField.IsLiteral || srcField.FieldType != destField.FieldType) continue;
                object srcValue = srcField.GetValue(node);
                Type valueType = srcValue.GetType();
                if (valueType.IsArray) {
                    // Clone Array
                    Type arrayType = Type.GetType(valueType.FullName.Replace("[]", string.Empty)); 
                    Array srcArray = srcValue as Array;
                    Array clonedArray = Array.CreateInstance(arrayType, srcArray.Length);
                    for (int i = 0; i < srcArray.Length; ++i) clonedArray.SetValue(srcArray.GetValue(i), i);
                    destField.SetValue(clone, clonedArray);
                } else if (valueType.IsGenericType) {
                    // Clone List
                    Type listType = Type.GetType(valueType.FullName.Replace("[]", string.Empty));
                    if (listType == null) {
                        Debug.LogWarning(string.Format("Couldn't clone correctly the {0} field, a shallow copy will be used", srcField.Name));
                        destField.SetValue(clone, srcField.GetValue(node));
                    } else {
                        IList srcList = srcValue as IList;
                        IList clonedList = Activator.CreateInstance(listType) as IList;
                        for (int i = 0; i < srcList.Count; ++i) clonedList.Add(srcList[i]);
                        destField.SetValue(clone, clonedList);
                    }
                } else destField.SetValue(clone, srcField.GetValue(node));
            }
            clone.id = Guid.NewGuid().ToString();
            return clone;
        }

        #endregion

        #region Internal Methods

        // Updates the main node process.
        // Sets <code>GUI.changed</code> to TRUE if the area is panned, a node is dragged, or controlNodes are reordered or deleted.
        internal void BeginGUI<T>(Rect nodeArea, ref Vector2 refAreaShift, IList<T> controlNodes) where T : IEditorGUINode, new()
        {
            _Styles.Init();
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
                        UnfocusAll();
                        interaction.SetReadyFor(InteractionManager.ReadyFor.DrawingSelection);
                        break;
                    case InteractionManager.TargetType.Node:
                        // LMB pressed on a node
                        if (Event.current.control || Event.current.command) {
                            // CTRL+Drag on node > drag connection (eventually)
                            NodeConnectionOptions connectionOptions = _nodeToConnectionOptions[interaction.targetNode];
                            bool canDragConnector = connectionOptions.allowManualConnections
                                                    && (connectionOptions.flexibleConnections || interaction.targetNode.connectedNodesIds.Count >= 1);
                            if (canDragConnector) {
                                interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingConnector);
                                UnfocusAll();
                                Event.current.Use();
                            }
                        } else if (interaction.nodeTargetType == InteractionManager.NodeTargetType.DraggableArea) {
                            // LMB pressed on a node's draggable area > set readyFor state to DraggingNodes and select node
                            interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingNodes);
                            UnfocusAll();
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
                        if (controlNodes != null) UpdateSorting(controlNodes);
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
                if (options.mouseWheelScalesGUI && (Event.current.control || Event.current.command)) {
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
            case EventType.KeyDown:
                switch (Event.current.keyCode) {
                case KeyCode.LeftAlt:
                case KeyCode.RightAlt:
                case KeyCode.AltGr:
                    if (_isAltPressed) break;
                    _isAltPressed = _repaintOnEnd = true;
                    break;
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.LeftArrow:
                case KeyCode.RightArrow:
                    // Move selected nodes (on key down so it can repeat when keeping they key pressed)
                    if (selection.selectedNodes.Count == 0) break;
                    Vector2 shift = Vector2.zero;
                    switch (Event.current.keyCode) {
                    case KeyCode.UpArrow: shift.y = -1;
                        break;
                    case KeyCode.DownArrow: shift.y = 1;
                        break;
                    case KeyCode.LeftArrow: shift.x = -1;
                        break;
                    case KeyCode.RightArrow: shift.x = 1;
                        break;
                    }
                    if (Event.current.shift) shift *= 10;
                    foreach (IEditorGUINode node in selection.selectedNodes) node.guiPosition += shift;
                    guiChangeType = GUIChangeType.DragNodes;
                    GUI.changed = _repaintOnEnd = true;
                    break;
                }
                break;
            case EventType.KeyUp:
                if (_isAltPressed && !Event.current.alt) {
                    _isAltPressed = false;
                    _repaintOnEnd = true;
                }
                switch (Event.current.keyCode) {
                case KeyCode.F1:
                    // Help Panel
                    _helpPanelActive = !_helpPanelActive;
                    _repaintOnEnd = true;
                    break;
                case KeyCode.Delete:
                case KeyCode.Backspace:
                    // Delete selected nodes
                    if (selection.selectedNodes.Count == 0 || _onDeleteNodesCallback != null && !_onDeleteNodesCallback(selection.selectedNodes)) break;
                    DeleteSelectedNodesInList(controlNodes);
                    selection.DeselectAll();
                    guiChangeType = GUIChangeType.DeletedNodes;
                    GUI.changed = _repaintOnEnd = true;
                    break;
                case KeyCode.A:
                    if (interaction.HasControlKeyModifier()) {
                        // CTRL+A > Select all nodes
                        selection.Select(nodes, false);
                        _repaintOnEnd = true;
                    }
                    break;
                case KeyCode.C:
                    if (interaction.HasControlKeyModifier()) {
                        // CTRL+C > Clone selected nodes (only if they're part of controlNodes) and place them in the clipboard
                        CloneAndCopySelectedNodes(controlNodes);
                    }
                    break;
                case KeyCode.V:
                    if (interaction.HasControlKeyModifier()) {
                        // CTRL+V > Paste copied nodes (which were cloned and in clipboard) and select them
                        if (_NodesClipboard.Count == 0) break;
                        selection.DeselectAll();
                        IEditorGUINode topLeftNode = _NodesClipboard[0];
                        for (int i = 1; i < _NodesClipboard.Count; ++i) {
                            IEditorGUINode node = _NodesClipboard[i];
                            if (node.guiPosition.y <= topLeftNode.guiPosition.x && node.guiPosition.x < topLeftNode.guiPosition.x) {
                                topLeftNode = node;
                            }
                        }
                        Vector2 offset = Event.current.mousePosition - (topLeftNode.guiPosition + areaShift);
                        foreach (IEditorGUINode node in _NodesClipboard) {
                            selection.Select(node, true);
                            node.guiPosition += offset;
                            controlNodes.Add((T)node);
                        }
                        guiChangeType = GUIChangeType.AddedNodes;
                        GUI.changed = _repaintOnEnd = true;
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
                    IEditorGUINode overNode = GetMouseOverNode();
                    if (overNode != null && overNode != interaction.targetNode) {
                        // Create new connection
                        if (_nodeToConnectionOptions[Connector.dragData.node].flexibleConnections) {
                            // Flexible connection > add new element to connectedNodesIds
                            if (!NodeIsForwardConnectedTo(Connector.dragData.node, overNode.id)) {
                                bool assignedToExistingConnection = false;
                                // First check if there's some leftover empty connection to fill
                                for (int i = 0; i < Connector.dragData.node.connectedNodesIds.Count; ++i) {
                                    if (!string.IsNullOrEmpty(Connector.dragData.node.connectedNodesIds[i])) continue;
                                    Connector.dragData.node.connectedNodesIds[i] = overNode.id;
                                    assignedToExistingConnection = true;
                                    break;
                                }
                                // Otherwise add new one
                                if (!assignedToExistingConnection) Connector.dragData.node.connectedNodesIds.Add(overNode.id);
                                GUI.changed = true;
                            }
                        } else {
                            // Normal connection, use existing connection index
                            Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] = overNode.id;
                            GUI.changed = true;
                        }
                        _repaintOnEnd = true;
                    } else {
                        // Disconnect (unless connectionOptions are set to flexibleConnections)
                        if (!_nodeToConnectionOptions[Connector.dragData.node].flexibleConnections) {
                            bool changed = !string.IsNullOrEmpty(Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex]);
                            Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] = null;
                            if (changed) GUI.changed = true;
                        }
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
            // DRAW CONNECTIONS BETWEEN NODES
            // Only in case of Repaint or MouseButtonDown/Up
            switch (Event.current.type) {
            case EventType.Repaint:
            case EventType.MouseDown:
            case EventType.MouseUp:
                bool aConnectionWasDeleted = false;
                for (int i = 0; i < nodes.Count; ++i) {
                    IEditorGUINode fromNode = nodes[i];
                    List<string> connections = fromNode.connectedNodesIds;
                    int totConnections = fromNode.connectedNodesIds.Count;
                    for (int c = totConnections - 1; c > -1; --c) {
                        string connId = connections[c];
                        if (string.IsNullOrEmpty(connId)) continue;
                        if (!_idToNode.ContainsKey(connId)) {
                            // Node eliminated externally, remove from dictionary
                            _idToNode.Remove(connId);
                        } else {
                            IEditorGUINode toNode = _idToNode[connId];
                            bool deletedConnection = Connector.Connect(
                                this, c, totConnections, nodeToGUIData[fromNode], _nodeToConnectionOptions[fromNode], nodeToGUIData[toNode]
                            );
                            if (deletedConnection) {
                                // Connection deleted, deal with flexibleConnections and non-flexibleConnections
                                aConnectionWasDeleted = true;
                                if (_nodeToConnectionOptions[fromNode].flexibleConnections) {
                                    totConnections--;
                                    fromNode.connectedNodesIds.RemoveAt(c);
                                } else {
                                    fromNode.connectedNodesIds[c] = null;
                                }
                            }
                        }
                    }
                }
                if (aConnectionWasDeleted) {
                    GUI.changed = _repaintOnEnd = true;
                }
                break;
            }

            // Draw elements if a repaint is not going to be called in the end
            if (!_repaintOnEnd) {
                if (Event.current.type == EventType.Repaint) {
                    // EVIDENCE FULL SELECTION AREA
                    if (options.evidenceSelectedNodesArea && selection.selectedNodes.Count > 1) {
                        Rect fullEvidenceR = nodeToGUIData[selection.selectedNodes[0]].fullArea;
                        for (int i = 1; i < selection.selectedNodes.Count; ++i) {
                            fullEvidenceR = fullEvidenceR.Add(nodeToGUIData[selection.selectedNodes[i]].fullArea);
                        }
                        using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor.SetAlpha(0.4f))) {
                            GUI.Box(fullEvidenceR.Expand(5), "", _Styles.nodeSelectionOutline);
                        }
                    }
                    switch (interaction.state) {
                    // DRAW RECTANGULAR SELECTION
                    case InteractionManager.State.DrawingSelection:
                        // Draw selection
                        using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                            GUI.Box(selection.selectionRect, "", _Styles.selectionRect);
                        }
                        break;
                    // DRAW CONNECTOR DRAGGING
                    case InteractionManager.State.DraggingConnector:
                        Connector.Drag(interaction, Event.current.mousePosition);
                        // Evidence origin
                        DeGUI.DrawColoredSquare(interaction.targetNodeConnectorArea.Expand(3), DeGUI.colors.global.orange.SetAlpha(0.32f));
                        using (new DeGUI.ColorScope(DeGUI.colors.global.black)) {
                            GUI.Box(interaction.targetNodeConnectorArea.Expand(2), "", _Styles.nodeSelectionOutlineThick);
                            GUI.Box(interaction.targetNodeConnectorArea.Expand(4), "", _Styles.nodeSelectionOutlineThick);
                        }
                        using (new DeGUI.ColorScope(DeGUI.colors.global.orange)) {
                            GUI.Box(interaction.targetNodeConnectorArea.Expand(3), "", _Styles.nodeSelectionOutlineThick);
                        }
                        // Evidence possible connection
                        IEditorGUINode overNode = GetMouseOverNode();
                        if (overNode != null && overNode != interaction.targetNode) {
                            using (new DeGUI.ColorScope(DeGUI.colors.global.orange)) {
                                GUI.Box(nodeToGUIData[overNode].fullArea.Expand(4), "", _Styles.nodeSelectionOutlineThick);
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
                NodeGUIData nodeGuiData = nodeToGUIData[targetNode];
                interaction.targetNodeConnectorAreaIndex = 0;
                interaction.targetNodeConnectorArea = nodeGuiData.fullArea;
                if (nodeGuiData.dragArea.Contains(Event.current.mousePosition)) {
                    // Mouse on draggable area
                    interaction.SetMouseTargetType(InteractionManager.TargetType.Node, InteractionManager.NodeTargetType.DraggableArea);
                } else {
                    // Mouse on non-draggable area: check if a sub connectorArea should be stored
                    // (ignore in case tot areas are different from tot connections (happens in case of flexibleConnections option)
                    interaction.SetMouseTargetType(InteractionManager.TargetType.Node, InteractionManager.NodeTargetType.NonDraggableArea);
                    if (targetNode.connectedNodesIds.Count > 1 && nodeGuiData.connectorAreas != null) {
                        for (int i = 0; i < targetNode.connectedNodesIds.Count; ++i) {
                            Rect connectorArea = nodeGuiData.connectorAreas[i];
                            if (!connectorArea.Contains(Event.current.mousePosition)) continue;
                            interaction.targetNodeConnectorAreaIndex = i;
                            interaction.targetNodeConnectorArea = connectorArea;
                            break;
                        }
                    }
                }
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

        void CloneAndCopySelectedNodes<T>(IList<T> controlNodes) where T : IEditorGUINode, new()
        {
            if (selection.selectedNodes.Count == 0) return;

            _NodesClipboard.Clear();
            List<string> originalIds = new List<string>();
            List<string> cloneIds = new List<string>();
            // Store clones
            foreach (IEditorGUINode node in selection.selectedNodes) {
                string id = node.id;
                if (IndexOfNodeById(id, controlNodes) == -1) continue;
                IEditorGUINode clone = CloneNode<T>(node);
                if (_onCloneNodeCallback != null && !_onCloneNodeCallback(node, clone)) continue;
                _NodesClipboard.Add(clone);
                originalIds.Add(id);
                cloneIds.Add(clone.id);
            }
            if (_NodesClipboard.Count == 0) return;

            // Replace all interconnected ids with new ones, and eliminate others
            foreach (IEditorGUINode node in _NodesClipboard) {
                IEditorGUINode originalNode = GetNodeById(originalIds[cloneIds.IndexOf(node.id)], controlNodes);
                NodeConnectionOptions connectionOptions = _nodeToConnectionOptions[originalNode];
                for (int c = node.connectedNodesIds.Count - 1; c > -1; --c) {
                    int replaceIndex = originalIds.IndexOf(node.connectedNodesIds[c]);
                    if (replaceIndex == -1) {
                        // Clear or delete connection
                        if (connectionOptions.flexibleConnections) node.connectedNodesIds.RemoveAt(c);
                        else node.connectedNodesIds[c] = null;
                    } else {
                        // Replace connection ID
                        node.connectedNodesIds[c] = cloneIds[replaceIndex];
                    }
                }
            }
        }

        // Deletes all selected nodes if they're part of the given list,
        // and removes their ID from any other node that referenced them as a connection
        void DeleteSelectedNodesInList<T>(IList<T> removeFrom) where T : IEditorGUINode
        {
            _tmp_string.Clear(); // Used to store ids to remove
            // Delete nodes
            foreach (IEditorGUINode selectedNode in selection.selectedNodes) {
                int index = IndexOfNodeById(selectedNode.id, removeFrom);
                if (index == -1) continue;
                _tmp_string.Add(selectedNode.id);
                removeFrom.RemoveAt(index);
            }
            // Remove references to deleted nodes from connections
            foreach (T node in removeFrom) {
                for (int i = 0; i < node.connectedNodesIds.Count; ++i) {
                    if (_tmp_string.Contains(node.connectedNodesIds[i])) node.connectedNodesIds[i] = null;
                }
            }
        }

        // Called only if controlNodes is not NULL and the event is MouseDown
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

        void UnfocusAll()
        {
            if (GUIUtility.keyboardControl > 0) _repaintOnEnd = true;
            GUI.FocusControl(null);
        }

        bool NodeIsVisible(Rect nodeArea)
        {
            return nodeArea.xMax > relativeArea.xMin && nodeArea.xMin < relativeArea.xMax && nodeArea.yMax > relativeArea.yMin && nodeArea.yMin < relativeArea.yMax;
        }

        bool NodeIsForwardConnectedTo(IEditorGUINode node, string id)
        {
            for (int i = 0; i < node.connectedNodesIds.Count; ++i) {
                if (node.connectedNodesIds[i] == id) return true;
            }
            return false;
        }

        IEditorGUINode GetNodeById<T>(string id, IList<T> lookInto) where T : IEditorGUINode
        {
            for (int i = 0; i < lookInto.Count; ++i) {
                if (lookInto[i].id == id) return lookInto[i];
            }
            return null;
        }

        int IndexOfNodeById<T>(string id, IList<T> lookInto) where T : IEditorGUINode
        {
            for (int i = 0; i < lookInto.Count; ++i) {
                if (lookInto[i].id == id) return i;
            }
            return -1;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle selectionRect, nodeSelectionOutline, nodeSelectionOutlineThick, endNodeOutline;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                selectionRect = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.squareBorderAlpha15);
                nodeSelectionOutline = DeGUI.styles.box.outline01.Clone()
                    .Border(new RectOffset(5, 5, 5, 5)).Background(DeStylePalette.squareBorderCurvedEmpty);
                nodeSelectionOutlineThick = nodeSelectionOutline.Clone().Background(DeStylePalette.squareBorderCurvedEmptyThick);
                endNodeOutline = nodeSelectionOutlineThick.Clone().Background(DeStylePalette.squareCornersEmpty02)
                    .Border(new RectOffset(7, 7, 7, 7));
            }
        }
    }
}