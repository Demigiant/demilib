// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/11 20:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiEditor.DeGUINodeSystem.Core;
using DG.DemiEditor.DeGUINodeSystem.Core.DebugSystem;
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
    /// - Inside OnGUI, write all your nodes GUI code inside a <see cref="NodeProcessScope"/><para/>
    /// - To draw the nodes, loop through the <see cref="orderedNodes"/> list and call <see cref="Draw{T}"/> for each node
    /// </summary>
    public class NodeProcess
    {
        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
        // ■■■ EVENTS

        public event Action<GUIChangeType> OnGUIChange;
        internal void DispatchOnGUIChange(GUIChangeType type) { if (OnGUIChange != null) OnGUIChange(type); }

        // ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

        public enum GUIChangeType
        {
            None,
            Pan,
            DragNodes,
            SortedNodes,
            DeletedNodes,
            AddedNodes,
            NodeConnection, // Node connection changed, added or removed
            GUIScale
        }

        public enum ScreenshotMode
        {
            VisibleArea,
            AllNodes
        }

        public const string Version = "1.0.060";
        /// <summary>Distance at which nodes will be placed when snapping next to each other</summary>
        public const int SnapOffset = 12;
        public EditorWindow editor; // Get/set so it can be refreshed if necessary
        public readonly ProcessOptions options = new ProcessOptions();
        public readonly InteractionManager interaction;
        public readonly SelectionManager selection;
        public readonly HelpPanel helpPanel;
        public GUIChangeType guiChangeType { get; private set; } // Last GUI.changed reason if set by process (reset on process end)
        /// <summary>Full area without zeroed coordinates</summary>
        public Rect position { get; private set; }
        /// <summary>Position with zeroed coordinates (used by all node GUI since it's inside a GUILayout(area))</summary>
        public Rect relativeArea { get; private set; }
        public Vector2 areaShift { get; private set; }
        public float guiScale { get; internal set; }
        public readonly Dictionary<IEditorGUINode,NodeGUIData> nodeToGUIData = new Dictionary<IEditorGUINode,NodeGUIData>(); // Refilled on Layout event
        /// <summary>Contains the nodes passed to NodeProcessScope ordered by depth.
        /// You should loop through this list when drawing nodes</summary>
        public readonly List<IEditorGUINode> orderedNodes = new List<IEditorGUINode>();
        internal Vector2 guiScalePositionDiff { get; private set; } // Used by GUI calls that need to rotate the matrix

        readonly NodeProcessDebug _debug = new NodeProcessDebug();
        internal readonly List<IEditorGUINode> nodes = new List<IEditorGUINode>(); // Used in conjunction with dictionaries to loop them in desired order
        internal readonly Dictionary<string,IEditorGUINode> idToNode = new Dictionary<string,IEditorGUINode>();
        internal readonly Dictionary<IEditorGUINode,NodeConnectionOptions> nodeToConnectionOptions = new Dictionary<IEditorGUINode,NodeConnectionOptions>();
        readonly Dictionary<Type,ABSDeGUINode> _typeToGUINode = new Dictionary<Type,ABSDeGUINode>();
        readonly List<IEditorGUINode> _endGUINodes = new List<IEditorGUINode>(); // Used to draw invasive end node markers before drawing the nodes
        readonly Connector _connector;
        readonly NodeDragManager _nodeDragManager;
        readonly NodesClipboard _clipboard;
        readonly ContextPanel _contextPanel;
        Minimap _minimap;
        readonly List<IEditorGUINode> _tmp_nodes = new List<IEditorGUINode>(); // Used for temporary operations
        readonly List<string> _tmp_string = new List<string>(); // Used for temporary operations
        static readonly Styles _Styles = new Styles();
        readonly Func<List<IEditorGUINode>,bool> _onDeleteNodesCallback; // Returns FALSE if deletion shouldn't happen
        readonly Func<IEditorGUINode,IEditorGUINode,bool> _onCloneNodeCallback; // Returns FALSE if cloning shouldn't happen
        bool _guiInitialized; // Indicates that eventual calls that require a first GUI call to happen were done
        bool _isDockableEditor; // Used in conjunction with hack to allow correct GUI scaling on dockable windows
        bool _repaintOnEnd; // If TRUE, repaints the editor during EndGUI. Set to FALSE at each EndGUI
        bool _resetInteractionOnEnd;
        Vector2 _forceApplyAreaShiftBy; // Used by ShiftAreaBy to apply a shift the next time the process begins
        Vector2 _forceApplyAreaShift; // Used by SetShiftArea to apply a shift the next time the process begins
        bool _doForceApplyAreaShift; // Used by SetShiftArea to apply a shift the next time the process begins

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
            _connector = new Connector(this);
            _nodeDragManager = new NodeDragManager(this);
            _clipboard = new NodesClipboard(this);
            _contextPanel = new ContextPanel(this);
            helpPanel = new HelpPanel(this);
            Undo.undoRedoPerformed -= this.OnUndoRedoCallback;
            Undo.undoRedoPerformed += this.OnUndoRedoCallback;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Needs to be called when loading a complete new series of nodes
        /// </summary>
        public void Reset()
        {
            interaction.Reset();
            selection.Reset();
            _connector.Reset();
            orderedNodes.Clear();
            if (_minimap != null) _minimap.RefreshMapTextureOnNextPass();
        }

        /// <summary>
        /// Call this when the layout/size of one or more nodes changed because of external intervention
        /// (if a whole new range of nodes has been loaded, just call <see cref="Reset"/> instead)
        /// </summary>
        public void MarkLayoutAsDirty()
        {
            _connector.Reset();
            if (_minimap != null) _minimap.RefreshMapTextureOnNextPass();
        }

        /// <summary>
        /// Forces the refresh of the area calculations. Useful if you need them before the first GUI call has run
        /// </summary>
        public void ForceRefreshAreas(Rect nodeArea)
        {
            position = nodeArea;
            relativeArea = new Rect(0, 0, nodeArea.width / guiScale, nodeArea.height / guiScale);
        }

        /// <summary>
        /// Shifts the visible are to the given coordinates and repaints on end
        /// </summary>
        public void ShiftAreaBy(Vector2 shift)
        {
            _forceApplyAreaShiftBy = shift;
            DispatchOnGUIChange(GUIChangeType.Pan);
            RepaintOnEnd();
        }

        /// <summary>
        /// Shifts the visible are to the given coordinates and repaints on end
        /// </summary>
        public void SetAreaShift(Vector2 shift)
        {
            _doForceApplyAreaShift = true;
            _forceApplyAreaShift = shift;
            DispatchOnGUIChange(GUIChangeType.Pan);
            RepaintOnEnd();
        }

        /// <summary>
        /// Tells the process to repaint once the process has ended.
        /// Calling this
        /// </summary>
        public void RepaintOnEnd()
        {
            _repaintOnEnd = true;
        }

        /// <summary>
        /// Draws the given node using the given T editor GUINode type.
        /// Returns the full area of the node
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
            nodeGuiData.isVisible = AreaIsVisible(nodeGuiData.fullArea);

            // Draw node (always, not only when visible, otherwise Unity messes up selections)
            // OnGUI
            guiNode.OnGUI(nodeGuiData, node);

            // Fade out unselected nodes if node is visible and there's others that are selected (and more than one)
//            if (AreaIsVisible(nodeGuiData.fullArea)) {
            if (nodeGuiData.isVisible) {
                bool faded = selection.selectedNodes.Count > 1 && !selection.IsSelected(node);
                if (faded) {
                    using (new DeGUI.ColorScope(null, null, new Color(1, 1, 1, 0.4f))) GUI.DrawTexture(nodeGuiData.fullArea, DeStylePalette.blackSquare);
                }
            }

            bool evidenceEndNode = options.evidenceEndNodes != ProcessOptions.EvidenceEndNodesMode.None;
            switch (Event.current.type) {
            case EventType.Layout:
                nodes.Add(node);
                idToNode.Add(node.id, node);
                nodeToGUIData.Add(node, nodeGuiData);
                nodeToConnectionOptions.Add(node, connectionOptions == null ? new NodeConnectionOptions(true) : (NodeConnectionOptions)connectionOptions);
                if (evidenceEndNode) {
                    // Determine if mark as end node
                    NodeConnectionOptions connOptions = nodeToConnectionOptions[node];
                    if (!connOptions.neverMarkAsEndNode) {
                        bool markAsEndNode = false;
                        switch (connOptions.connectionMode) {
                        case ConnectionMode.Flexible:
                            markAsEndNode = node.connectedNodesIds.Count == 0;
                            break;
                        case ConnectionMode.Dual:
                            markAsEndNode = string.IsNullOrEmpty(node.connectedNodesIds[0]) || string.IsNullOrEmpty(node.connectedNodesIds[1]);
                            break;
                        default:
                            for (int i = 0; i < node.connectedNodesIds.Count; ++i) {
                                if (!string.IsNullOrEmpty(node.connectedNodesIds[i])) continue;
                                markAsEndNode = true;
                                break;
                            }
                            break;
                        }
                        if (markAsEndNode) _endGUINodes.Add(node);
                    }
                }
                break;
            case EventType.Repaint:
//                if (nodes.Count > 0 && node == nodes[0]) Debug.Log("Updated node layout (FIRST)" + Event.current.type);
//                if (nodes.Count > 0 && node == nodes[nodes.Count - 1]) Debug.Log("Updated node layout (LAST)" + Event.current.type);
                nodeToGUIData[node] = nodeGuiData;
                if (nodeGuiData.isVisible) {
                    // Draw evidence
                    if (options.evidenceSelectedNodes && selection.IsSelected(node)) {
                        using (new DeGUI.ColorScope(options.evidenceSelectedNodesColor)) {
                            GUI.Box(nodeGuiData.fullArea.Expand(5), "", _Styles.nodeSelectionOutlineThick);
                        }
                    }
                    // Draw end node icon
                    if (evidenceEndNode && _endGUINodes.Contains(node)) {
                        float icoSize = Mathf.Min(nodeGuiData.fullArea.height, 20);
                        Rect r = new Rect(nodeGuiData.fullArea.xMax - icoSize * 0.5f, nodeGuiData.fullArea.yMax - icoSize * 0.5f, icoSize, icoSize);
                        GUI.DrawTexture(r, DeStylePalette.ico_end);
                    }
                }
                break;
            }

            return nodeGuiData.fullArea;
        }

        /// <summary>
        /// Opens the Help Panel
        /// </summary>
        public void OpenHelpPanel()
        {
            helpPanel.Open(true);
            _repaintOnEnd = true;
        }

        /// <summary>
        /// Closes the Help Panel
        /// </summary>
        public void CloseHelpPanel()
        {
            helpPanel.Open(false);
            _repaintOnEnd = true;
        }

        /// <summary>
        /// Opens or closes the Help panel based on its current state
        /// </summary>
        public void ToggleHelpPanel()
        {
            if (helpPanel.isOpen) CloseHelpPanel();
            else OpenHelpPanel();
        }

        /// <summary>
        /// Returns TRUE if the given area is visible (even if partially) inside the current nodeProcess area
        /// </summary>
        public bool AreaIsVisible(Rect area)
        {
            return area.xMax > relativeArea.xMin && area.xMin < relativeArea.xMax && area.yMax > relativeArea.yMin && area.yMin < relativeArea.yMax;
        }

        /// <summary>
        /// Captures a screenshot of the node editor area and returns it when calling the onComplete method.<para/>
        /// Sadly this requires a callback because if called immediately the capture will fail
        /// with a "[d3d11] attempting to ReadPixels outside of RenderTexture bounds!" error in most cases
        /// </summary>
        /// <param name="screenshotMode">Screenshot mode</param>
        /// <param name="onComplete">A callback that accepts the generated Texture2D object</param>
        /// <param name="allNodesScaleFactor">Screenshot scale factor (only used if screenshotMode is set to <see cref="ScreenshotMode.AllNodes"/>)</param>
        /// <param name="useProgressBar">If TRUE (default) displays a progress bar during the operation.
        /// You'll want to set this to FALSE when you're already using a custom progressBar
        /// and the screenshot is only part of a larger queue of operations</param>
        public void CaptureScreenshot(ScreenshotMode screenshotMode, Action<Texture2D> onComplete, float allNodesScaleFactor = 1, bool useProgressBar = true)
        {
            ScreenshotManager.CaptureScreenshot(this, screenshotMode, onComplete, allNodesScaleFactor, useProgressBar);
        }

        #endregion

        #region Internal Methods

        internal Rect EvaluateFullNodesArea()
        {
            Rect area = nodeToGUIData[nodes[0]].fullArea;
            foreach (IEditorGUINode node in nodes) area = area.Add(nodeToGUIData[node].fullArea);
            return area;
        }

        // Updates the main node process.
        // Sets <code>GUI.changed</code> to TRUE if the area is panned, a node is dragged, or controlNodes are reordered or deleted.
        internal void BeginGUI<T>(Rect nodeArea, ref Vector2 refAreaShift, IList<T> controlNodes) where T : class, IEditorGUINode, new()
        {
            if (!_guiInitialized) {
                _guiInitialized = true;
                _isDockableEditor = DeEditorPanelUtils.IsDockableWindow(editor);
            }
            if (options.debug_showFps) _debug.OnNodeProcessStart(interaction.state);

            if (_isDockableEditor) {
                // Hack to avoid clipping when zooming on dockable window
                GUI.EndGroup();
                nodeArea.y += 22;
            }

            // Validate nodes order
            if (controlNodes == null) {
                if (orderedNodes.Count > 0) orderedNodes.Clear();
            } else {
                if (orderedNodes.Count != controlNodes.Count) orderedNodes.Clear();
                if (orderedNodes.Count == 0) {
                    for (int i = 0; i < controlNodes.Count; ++i) orderedNodes.Add(controlNodes[i]);
                }
            }

            _Styles.Init();
            position = nodeArea;
            if (_doForceApplyAreaShift) {
                refAreaShift = _forceApplyAreaShift;
                _doForceApplyAreaShift = false;
            } else if (_forceApplyAreaShiftBy != Vector2.zero) {
                refAreaShift += _forceApplyAreaShiftBy;
                _forceApplyAreaShiftBy = Vector2.zero;
            }
            areaShift = new Vector2((int)refAreaShift.x, (int)refAreaShift.y);
            if (options.showMinimap) {
                if (_minimap == null) _minimap = new Minimap(this);
            } else _minimap = null;

            // Disable GUI if Help Panel is active
            EditorGUI.BeginDisabledGroup(helpPanel.isOpen);

            // Set scale
            bool isScaled = !Mathf.Approximately(guiScale, 1);
            if (isScaled) {
                GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * guiScale);
            }

            // Scale area size to guiScale and begin GUILayout area (so base node area coordinates are 0,0 and everything is easier and faster)
            relativeArea = new Rect(0, 0, nodeArea.width / guiScale, nodeArea.height / guiScale);
            guiScalePositionDiff = new Vector2(nodeArea.x - nodeArea.x / guiScale, nodeArea.y - nodeArea.y / guiScale);
            GUILayout.BeginArea(new Rect(nodeArea.x / guiScale, nodeArea.y / guiScale, relativeArea.width, relativeArea.height));

            // Reset readyForState before checking if interaction is locked
            // (otherwise mouse target isn't evaluated because readyForState locks it to previous one)
            if (Event.current.type == EventType.MouseDown) {
                switch (interaction.readyForState) {
                case InteractionManager.ReadyFor.Unset: break;
                default:
                    interaction.SetReadyFor(InteractionManager.ReadyFor.Unset);
                    break;
                }
            }

            // Determine mouse target type before clearing nodeGUIData dictionary
            if (!interaction.mouseTargetIsLocked) EvaluateAndStoreMouseTarget();
            if (Event.current.type == EventType.Layout) {
                nodes.Clear();
                idToNode.Clear();
                nodeToGUIData.Clear();
                nodeToConnectionOptions.Clear();
                _endGUINodes.Clear();
            }

            // Update interaction and DeGUIKey
            DeGUIKey.KeysRefreshResult refreshResult = DeGUIKey.Refresh("INTERNAL_DeGUINodeProcess");
            if (refreshResult.pressed.alt || refreshResult.released.alt) _repaintOnEnd = true;
            if (interaction.Update()) _repaintOnEnd = true;

            // Background grid
            if (options.drawBackgroundGrid) {
                if (options.gridTextureOverride == null) DeGUI.BackgroundGrid(relativeArea, areaShift, options.forceDarkSkin, 1 / guiScale);
                else DeGUI.BackgroundGrid(relativeArea, areaShift, options.gridTextureOverride, 1 / guiScale);
            }

            // Nodes heavy evidence
            if (options.evidenceEndNodes == ProcessOptions.EvidenceEndNodesMode.Invasive && Event.current.type == EventType.Repaint) {
                foreach (IEditorGUINode guiNode in _endGUINodes) {
                    NodeGUIData nodeGuiData = nodeToGUIData[guiNode];
                    Rect evArea = nodeGuiData.fullArea.Expand(options.evidenceEndNodesBackgroundBorder);
                    Texture2D tex = DeStylePalette.tileBars_slanted;
                    using (new DeGUI.ColorScope(null, null, options.evidenceEndNodesBackgroundColor)) {
                        GUI.DrawTextureWithTexCoords(evArea, tex, new Rect(0, 0, evArea.width / tex.width, evArea.height / tex.height));
                    }
                }
            }

            // MINIMAP : BUTTON (view drawn after nodes, at end of EndGUI)
            if (_minimap != null) _minimap.DrawButton();

            switch (Event.current.type) {
            // MOUSE EVENTS ///////////////////////////////////////////////////////////////////////////////////////////////////////
            case EventType.MouseDown:
                if (_contextPanel.HasMouseOver()) break; // Ignore in case mouse is over ContextPanel
                switch (Event.current.button) {
                case 0:
                    interaction.mousePositionOnLMBPress = Event.current.mousePosition;
                    switch (interaction.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        // LMB pressed on background
                        if (DeGUIKey.Exclusive.shift) {
                            // Prepare for selection drawing in add mode
                            selection.selectionMode = SelectionManager.Mode.Add;
                            selection.StoreSnapshot();
                        } else {
                            // Deselect all
                            if (selection.DeselectAll()) _repaintOnEnd = true;
                        }
                        UnfocusAll();
                        interaction.SetReadyFor(InteractionManager.ReadyFor.DrawingSelection);
                        break;
                    case InteractionManager.TargetType.Node:
                        // LMB pressed on a node
                        if (DeGUIKey.Exclusive.ctrl) {
                            // CTRL+Drag on node > drag connection (eventually)
                            NodeConnectionOptions connectionOptions = nodeToConnectionOptions[interaction.targetNode];
                            bool canDragConnector = connectionOptions.allowManualConnections
                                                    && (
                                                        connectionOptions.connectionMode == ConnectionMode.Flexible
                                                        || interaction.targetNode.connectedNodesIds.Count >= 1
                                                    );
                            if (canDragConnector) {
                                interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingConnector);
                                UnfocusAll();
                                Event.current.Use();
                            }
                        } else {
                            bool isAlreadySelected = selection.IsSelected(interaction.targetNode);
                            if (interaction.nodeTargetType == InteractionManager.NodeTargetType.DraggableArea) {
                                // LMB pressed on a node's draggable area > set readyFor state to DraggingNodes and select/deselect node
                                interaction.SetReadyFor(InteractionManager.ReadyFor.DraggingNodes);
                                UnfocusAll();
                                // Select/deselect
                                if (DeGUIKey.Exclusive.shiftAlt) {
                                    // Select this plus all forward connected nodes
                                    if (!isAlreadySelected) selection.Select(interaction.targetNode, true);
                                    SelectAllForwardConnectedNodes(interaction.targetNode);
                                    _repaintOnEnd = true;
                                } else if (DeGUIKey.Exclusive.shift) {
                                    // Add to selection if not already selected
                                    // (deselection happens on mouseUp instead, for various reasons)
                                    selection.StoreSnapshot();
                                    if (!isAlreadySelected) selection.Select(interaction.targetNode, true);
                                    _repaintOnEnd = true;
                                } else if (!isAlreadySelected) {
                                    // If unselected, select and deselect all others
                                    selection.Select(interaction.targetNode, false);
                                    _repaintOnEnd = true;
                                }
                            } else {
                                // LMB on non-draggable area. Just select the node if not already selected
                                if (!isAlreadySelected) {
                                    UnfocusAll();
                                    selection.Select(interaction.targetNode, false);
                                    _repaintOnEnd = true;
                                }
                            }
                        }
                        // Update eventual sorting
                        if (controlNodes != null) UpdateOrderedNodesSorting();
                        break;
                    }
                    break;
                case 1:
                    switch (interaction.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        // Deselect all nodes
                        if (selection.selectedNodes.Count > 0) _repaintOnEnd = true;
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
                        if (DeGUIKey.Exclusive.ctrlShift && options.allowCopyPaste) {
                            // Clone nodes before starting to drag
                            CloneAndCopySelectedNodes(controlNodes);
                            if (PasteNodesFromClipboard(controlNodes, false)) {
                                // Select copied nodes plus original ones that weren't copied (meaning they were not part of controlNodes)
                                selection.DeselectAll();
                                foreach (IEditorGUINode uncopiedNode in _tmp_nodes) selection.Select(uncopiedNode, true);
                                foreach (IEditorGUINode node in _clipboard.currClones) selection.Select(node, true);
                                // Set interaction to mirror the new nodes setup
                                IEditorGUINode clonedTargetNode = _clipboard.GetCloneByOriginalId(interaction.targetNode.id);
                                if (clonedTargetNode != null) interaction.targetNode = clonedTargetNode;
                            }
                        }
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
                        DispatchOnGUIChange(GUIChangeType.DragNodes);
                        break;
                    case InteractionManager.State.DraggingConnector:
                        _repaintOnEnd = true;
                        break;
                    }
                    break;
                case 2:
                    // Panning
                    interaction.SetState(InteractionManager.State.Panning);
                    refAreaShift += Event.current.delta;
                    guiChangeType = GUIChangeType.Pan;
                    GUI.changed = _repaintOnEnd = true;
                    DispatchOnGUIChange(GUIChangeType.Pan);
                    break;
                }
                break;
            case EventType.ContextClick:
                if (_contextPanel.HasMouseOver()) break; // Ignore in case mouse is over ContextPanel
                interaction.SetState(InteractionManager.State.ContextClick);
                _resetInteractionOnEnd = true;
                break;
            case EventType.ScrollWheel:
                if (options.mouseWheelScalesGUI && DeGUIKey.Exclusive.ctrl) {
                    // Zoom
                    bool isScaleUp = Event.current.delta.y < 0;
                    if (isScaleUp && Mathf.Approximately(options.guiScaleValues[0], guiScale)) break;
                    if (!isScaleUp && Mathf.Approximately(options.guiScaleValues[options.guiScaleValues.Length - 1], guiScale)) break;
                    for (int i = 0; i < options.guiScaleValues.Length; ++i) {
                        if (!Mathf.Approximately(options.guiScaleValues[i], guiScale)) continue;
                        float prevGuiScale = guiScale;
                        guiScale = isScaleUp ? options.guiScaleValues[i - 1] : options.guiScaleValues[i + 1];
                        // Center on mouse (only works if new guiScale is exactly half or double the previous one)
                        Vector2 mouseP = Event.current.mousePosition;
                        Vector2 offset = isScaleUp ? -mouseP : mouseP;
                        float scaleFactor = guiScale / prevGuiScale;
                        scaleFactor = guiScale < prevGuiScale
                            ? (1 - scaleFactor) / scaleFactor
                            : (guiScale - prevGuiScale) / guiScale;
                        offset *= scaleFactor;
                        refAreaShift += offset;
                        _repaintOnEnd = true;
                        DispatchOnGUIChange(GUIChangeType.GUIScale);
                        break;
                    }
                }
                break;
            // KEYBOARD EVENTS //////////////////////////////////////////////////////////////////////////////////////////////////////
            case EventType.KeyDown:
                if (Event.current.keyCode == KeyCode.Escape) UnfocusAll();
                if (GUIUtility.keyboardControl > 0) break; // Ignore keys if textField is focused
                switch (Event.current.keyCode) {
                case KeyCode.LeftAlt:
                case KeyCode.RightAlt:
                case KeyCode.AltGr:
//                    if (_isAltPressed) break;
//                    _isAltPressed = _repaintOnEnd = true;
                    break;
                case KeyCode.UpArrow:
                case KeyCode.DownArrow:
                case KeyCode.LeftArrow:
                case KeyCode.RightArrow:
                    if (DeGUIKey.Exclusive.softCtrlShift) {
                        // Align and arrange nodes vertically or horizontally
                        switch (Event.current.keyCode) {
                        case KeyCode.LeftArrow:
                            _contextPanel.AlignAndArrangeNodes(false, selection.selectedNodes);
                            break;
                        case KeyCode.UpArrow:
                            _contextPanel.AlignAndArrangeNodes(true, selection.selectedNodes);
                            break;
                        }
                    } else {
                        // Move selected nodes (on key down so it can repeat when keeping they key pressed)
                        if (selection.selectedNodes.Count == 0 || !DeGUIKey.none && !DeGUIKey.Exclusive.shift) break;
                        Vector2 shift = Vector2.zero;
                        switch (Event.current.keyCode) {
                        case KeyCode.UpArrow:
                            shift.y = -1;
                            break;
                        case KeyCode.DownArrow:
                            shift.y = 1;
                            break;
                        case KeyCode.LeftArrow:
                            shift.x = -1;
                            break;
                        case KeyCode.RightArrow:
                            shift.x = 1;
                            break;
                        }
                        if (DeGUIKey.Exclusive.shift) shift *= 10;
                        foreach (IEditorGUINode node in selection.selectedNodes) node.guiPosition += shift;
                    }
                    guiChangeType = GUIChangeType.DragNodes;
                    GUI.changed = _repaintOnEnd = true;
                    DispatchOnGUIChange(GUIChangeType.DragNodes);
                    break;
                }
                break;
            case EventType.KeyUp:
//                if (_isAltPressed && !Event.current.alt) {
//                    _isAltPressed = false;
//                    _repaintOnEnd = true;
//                }
                if (GUIUtility.keyboardControl > 0) break; // Ignore keys if textField is focused
                switch (Event.current.keyCode) {
                case KeyCode.F1:
                    // Help Panel
                    OpenHelpPanel();
                    break;
                case KeyCode.Delete:
                case KeyCode.Backspace:
                    // Delete selected nodes
                    if (
                        !options.allowDeletion
                        || selection.selectedNodes.Count == 0
                        || _onDeleteNodesCallback != null && !_onDeleteNodesCallback(selection.selectedNodes)
                    ) break;
                    DeleteSelectedNodesInList(controlNodes);
                    selection.DeselectAll();
                    guiChangeType = GUIChangeType.DeletedNodes;
                    GUI.changed = _repaintOnEnd = true;
                    DispatchOnGUIChange(GUIChangeType.DeletedNodes);
                    break;
                case KeyCode.A:
                    if (DeGUIKey.Exclusive.softCtrl) {
                        // CTRL+A > Select all nodes
                        selection.Select(nodes, false);
                        _repaintOnEnd = true;
                    }
                    break;
                case KeyCode.C:
                    if (DeGUIKey.Exclusive.softCtrl) {
                        // CTRL+C > Clone selected nodes (only if they're part of controlNodes) and place them in the clipboard
                        if (options.allowCopyPaste) CloneAndCopySelectedNodes(controlNodes);
                    }
                    break;
                case KeyCode.X:
                    if (DeGUIKey.Exclusive.softCtrl) {
                        // CTRL+X > Clone selected nodes (only if they're part of controlNodes), place them in the clipboard and delete them
                        if (options.allowCopyPaste) {
                            if (CloneAndCopySelectedNodes(controlNodes, true)) {
                                DeleteSelectedNodesInList(controlNodes);
                                selection.DeselectAll();
                                guiChangeType = GUIChangeType.DeletedNodes;
                                GUI.changed = _repaintOnEnd = true;
                                DispatchOnGUIChange(GUIChangeType.DeletedNodes);
                            }
                        }
                    }
                    break;
                case KeyCode.V:
                    if (DeGUIKey.Exclusive.softCtrl) {
                        // CTRL+V > Paste copied nodes (which were cloned and in clipboard) and select them
                        if (options.allowCopyPaste && PasteNodesFromClipboard(controlNodes, true)) {
                            selection.DeselectAll();
                            foreach (IEditorGUINode node in _clipboard.currClones) selection.Select(node, true);
                            guiChangeType = GUIChangeType.AddedNodes;
                            GUI.changed = _repaintOnEnd = true;
                            DispatchOnGUIChange(GUIChangeType.AddedNodes);
                        }
                    }
                    break;
                }
                break;
            }

            switch (Event.current.rawType) {
//            // RAW KEY EVENTS ////////////////////////////////////////////////////////////////////////////////////////////////////////////
//            case EventType.KeyDown:
//                if (_isAltPressed) break;
//                _isAltPressed = _repaintOnEnd = true;
//                break;
//            case EventType.KeyUp:
//                if (_isAltPressed && !Event.current.alt) {
//                    _isAltPressed = false;
//                    _repaintOnEnd = true;
//                }
//                break;
            // RAW MOUSE EVENTS (used to capture mouseUp also outside editorWindow) //////////////////////////////////////////////////////
            case EventType.MouseUp:
                switch (interaction.state) {
                case InteractionManager.State.Inactive:
                    if (_contextPanel.HasMouseOver()) break; // Ignore in case mouse is over ContextPanel
                    if (DeGUIKey.Exclusive.shift && interaction.nodeTargetType == InteractionManager.NodeTargetType.DraggableArea) {
                        // SHIFT + LMB UP on selected node's draggable area: deselect
                        if (selection.IsSelected(interaction.targetNode) && selection.selectedNodesSnapshot.Contains(interaction.targetNode)) {
                            selection.Deselect(interaction.targetNode);
                            _repaintOnEnd = true;
                        }
                    }
                    break;
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
                        NodeConnectionOptions connectionOptions = nodeToConnectionOptions[Connector.dragData.node];
                        switch (connectionOptions.connectionMode) {
                        case ConnectionMode.Dual:
                            // Alt connection mode > add element to connectedNodeId 0 or 1 depending if SPACE is pressed or not
                            int connectionIndex = DeGUIKey.Exclusive.ctrl && DeGUIKey.Extra.space ? 1 : 0;
                            if (Connector.dragData.node.connectedNodesIds[connectionIndex] != overNode.id) {
                                Connector.dragData.node.connectedNodesIds[connectionIndex] = overNode.id;
                                guiChangeType = GUIChangeType.NodeConnection;
                                GUI.changed = true;
                                DispatchOnGUIChange(GUIChangeType.NodeConnection);
                            }
                            break;
                        case ConnectionMode.Flexible:
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
                                guiChangeType = GUIChangeType.NodeConnection;
                                GUI.changed = true;
                                DispatchOnGUIChange(GUIChangeType.NodeConnection);
                            }
                            break;
                        case ConnectionMode.NormalPlus:
                            // Normal connection > use existing connection index or PLUS one if SPACE is pressed
                            if (DeGUIKey.Exclusive.ctrl && DeGUIKey.Extra.space) {
                                // Extra connection
                                int plusConnectionIndex = Connector.dragData.node.connectedNodesIds.Count - 1;
                                if (Connector.dragData.node.connectedNodesIds[plusConnectionIndex] != overNode.id) {
                                    Connector.dragData.node.connectedNodesIds[plusConnectionIndex] = overNode.id;
                                    guiChangeType = GUIChangeType.NodeConnection;
                                    GUI.changed = true;
                                    DispatchOnGUIChange(GUIChangeType.NodeConnection);
                                }
                            } else if (Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] != overNode.id) {
                                // Normal
                                Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] = overNode.id;
                                guiChangeType = GUIChangeType.NodeConnection;
                                GUI.changed = true;
                                DispatchOnGUIChange(GUIChangeType.NodeConnection);
                            }
                            break;
                        default:
                            // Normal connection, use existing connection index
                            if (Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] != overNode.id) {
                                Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] = overNode.id;
                                guiChangeType = GUIChangeType.NodeConnection;
                                GUI.changed = true;
                                DispatchOnGUIChange(GUIChangeType.NodeConnection);
                            }
                            break;
                        }
                        _repaintOnEnd = true;
                    } else {
                        // Disconnect (unless connectionOptions are set to flexibleConnections)
                        if (nodeToConnectionOptions[Connector.dragData.node].connectionMode != ConnectionMode.Flexible) {
                            bool changed = !string.IsNullOrEmpty(Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex]);
                            Connector.dragData.node.connectedNodesIds[interaction.targetNodeConnectorAreaIndex] = null;
                            if (changed) {
                                guiChangeType = GUIChangeType.NodeConnection;
                                GUI.changed = true;
                                DispatchOnGUIChange(GUIChangeType.NodeConnection);
                            }
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
            // Only in case of Repaint or MouseButtonDown/Up, and if both nodes are visible
            switch (Event.current.type) {
            case EventType.Repaint:
            case EventType.MouseDown:
            case EventType.MouseUp:
                bool aConnectionWasAddedOrDeleted = false;
                for (int i = 0; i < nodes.Count; ++i) {
                    IEditorGUINode fromNode = nodes[i];
                    List<string> connections = fromNode.connectedNodesIds;
                    int totConnections = fromNode.connectedNodesIds.Count;
                    for (int c = totConnections - 1; c > -1; --c) {
                        string connId = connections[c];
                        if (string.IsNullOrEmpty(connId)) continue;
                        if (!idToNode.ContainsKey(connId)) {
                            // Node eliminated externally, remove from dictionary
                            idToNode.Remove(connId);
                        } else {
                            Connector.ConnectResult connectResult = _connector.Connect(c, totConnections, nodeToConnectionOptions[fromNode], fromNode);
                            if (connectResult.changed) {
                                // Connection added or deleted, deal with flexibleConnections and non-flexibleConnections
                                aConnectionWasAddedOrDeleted = true;
                                ConnectionMode connectionMode = nodeToConnectionOptions[fromNode].connectionMode;
                                switch (connectionMode) {
                                case ConnectionMode.Flexible:
                                    if (connectResult.aConnectionWasAdded) totConnections++;
                                    else {
                                        totConnections--;
                                        fromNode.connectedNodesIds.RemoveAt(c);
                                    }
                                    break;
                                default:
                                    if (connectResult.aConnectionWasDeleted) fromNode.connectedNodesIds[c] = null;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (aConnectionWasAddedOrDeleted) {
                    guiChangeType = GUIChangeType.NodeConnection;
                    GUI.changed = _repaintOnEnd = true;
                    DispatchOnGUIChange(GUIChangeType.NodeConnection);
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
                        // Draw drag connector
                        Color connectionColor = _connector.Drag(
                            interaction, Event.current.mousePosition,
                            nodeToGUIData[interaction.targetNode], nodeToConnectionOptions[interaction.targetNode],
                            options.connectorsThickness + 2
                        );
                        // Evidence origin
                        DeGUI.DrawColoredSquare(interaction.targetNodeConnectorArea.Expand(1), connectionColor.SetAlpha(0.32f));
                        using (new DeGUI.ColorScope(connectionColor)) {
                            GUI.Box(interaction.targetNodeConnectorArea.Expand(4), "", _Styles.nodeSelectionOutlineThick);
                        }
                        // Evidence possible connection
                        IEditorGUINode overNode = GetMouseOverNode();
                        if (overNode != null && overNode != interaction.targetNode) {
                            using (new DeGUI.ColorScope(connectionColor)) {
                                GUI.Box(nodeToGUIData[overNode].fullArea.Expand(4), "", _Styles.nodeSelectionOutlineThick);
                            }
                        }
                        // If node has alternative CTRL+SPACE connection show a tooltip
                        if (!DeGUIKey.Extra.space) {
                            NodeConnectionOptions connectionOptions = nodeToConnectionOptions[interaction.targetNode];
                            switch (connectionOptions.connectionMode) {
                            case ConnectionMode.Dual:
                            case ConnectionMode.NormalPlus:
                                const float tooltipW = 176;
                                const float tooltipH = 32;
                                Vector2 mouseP = Event.current.mousePosition;
                                Rect tooltipR = new Rect(mouseP.x, mouseP.y - tooltipH - 6, tooltipW, tooltipH);
                                using (new DeGUI.ColorScope(new Color(0f, 0f, 0f, 0.8f))) {
                                    GUI.Label(tooltipR, "<b><color=#ffffcd>CTRL+SPACE</color></b>\nto drag alternate connection", _Styles.draggingTooltip);
                                }
                                break;
                            }
                        }
                        break;
                    }
                    // NODE DRAGGING GUIDES
                    if (interaction.state == InteractionManager.State.DraggingNodes) _nodeDragManager.EndGUI();
                }
                // MINIMAP : VIEW (button drawn before nodes, at end of BeginGUI)
                if (_minimap != null && Event.current.type != EventType.Layout) _minimap.Draw();
                // CONTEXT PANEL
                _contextPanel.Draw();
                // FPSDEBUG PANEL
                if (options.debug_showFps) _debug.Draw(relativeArea);
                // HELP PANEL
                // Re-enables GUI so it can capture F1 to close it
                if (helpPanel.isOpen) {
                    bool wasGUIEnabled = GUI.enabled;
                    GUI.enabled = true;
                    bool closeHelpPanel = !helpPanel.Draw();
                    GUI.enabled = wasGUIEnabled;
                    if (closeHelpPanel) CloseHelpPanel();
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

            // Close areas and reset GUI matrix
            GUILayout.EndArea();
            EditorGUI.EndDisabledGroup();
            GUI.matrix = Matrix4x4.identity;

            if (options.debug_showFps) _debug.OnNodeProcessEnd();
            if (_isDockableEditor) GUI.BeginGroup(editor.position.ResetXY().SetY(22)); // Hack to avoid clipping when zooming on dockable window
        }

        #endregion

        #region Methods

        void OnUndoRedoCallback()
        {
            // Clear selection so it doesn't creates error when trying to draw the selection box after an undo
            selection.DeselectAll();
        }

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
                    interaction.SetMouseTargetType(InteractionManager.TargetType.Node, InteractionManager.NodeTargetType.NonDraggableArea);
                    int totConns = targetNode.connectedNodesIds.Count;
//                    // (ignore in case tot areas are different from tot connections (happens in case of flexibleConnections option)
//                    if (totConns > 1 && nodeGuiData.connectorAreas != null && totConns == nodeGuiData.connectorAreas.Count) {
//                        for (int i = 0; i < totConns; ++i) {
                    if (totConns > 1 && nodeGuiData.connectorAreas != null) {
                        for (int i = 0; i < nodeGuiData.connectorAreas.Count; ++i) {
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

        // Also fill _tmp_nodes with a list of nodes that were not copied
        // (meaning they were not part of controlNodes or the clone operation callback returned FALSE).
        // If cut is TRUE deletes the nodes after copying them.
        // Returns TRUE if something was actually copied/cut
        bool CloneAndCopySelectedNodes<T>(IList<T> controlNodes, bool cut = false) where T : IEditorGUINode, new()
        {
            if (selection.selectedNodes.Count == 0) return false;

            _tmp_nodes.Clear();
            _clipboard.Clear();
            // Store clones
            foreach (IEditorGUINode node in selection.selectedNodes) {
                string id = node.id;
                if (IndexOfNodeById(id, controlNodes) == -1) {
                    _tmp_nodes.Add(node);
                    continue;
                }
                IEditorGUINode clone = _clipboard.CloneNode<T>(node); // First batch of clones to see if it's allowed
                if (_onCloneNodeCallback != null && !_onCloneNodeCallback(node, clone)) {
                    _tmp_nodes.Add(node);
                    continue;
                }
                _clipboard.Add(node, clone, nodeToConnectionOptions[node], _onCloneNodeCallback);
            }
            if (_clipboard.currClones.Count == 0) return false;
            
            if (cut) DeleteSelectedNodesInList(controlNodes);
            return true;
        }

        // Returns TRUE if something was actually pasted
        bool PasteNodesFromClipboard<T>(IList<T> controlNodes, bool adaptGuiPositionToMouse) where T : class, IEditorGUINode, new()
        {
            if (!_clipboard.hasContent) return false;

            List<T> clones = _clipboard.GetNodesToPaste<T>();
            Vector2 offset = Vector2.zero;
            if (adaptGuiPositionToMouse) {
                // Find top left node to determine offset for paste
                Vector2 topLeftP = clones[0].guiPosition;
                for (int i = 1; i < clones.Count; ++i) {
                    IEditorGUINode node = clones[i];
                    if (node.guiPosition.x < topLeftP.x) topLeftP.x = node.guiPosition.x;
                    if (node.guiPosition.y < topLeftP.y) topLeftP.y = node.guiPosition.y;
                }
                offset = Event.current.mousePosition - (topLeftP + areaShift);
            }
            // Add nodes and also nodeGuiData etc based on original nodes
            // Also add them to ordered nodes
            foreach (T node in clones) {
                node.guiPosition += offset;
                controlNodes.Add(node);
                nodeToGUIData.Add(node, _clipboard.GetGuiDataByCloneId(node.id));
                nodeToConnectionOptions.Add(node, _clipboard.GetConnectionOptionsByCloneId(node.id));
                idToNode.Add(node.id, node);
                orderedNodes.Add(node);
            }
            return true;
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
                // Remove from ordered nodes
                index = orderedNodes.IndexOf(selectedNode);
                if (index != -1) orderedNodes.RemoveAt(index);
            }
            // Remove references to deleted nodes from connections
            foreach (IEditorGUINode node in nodes) {
                for (int i = 0; i < node.connectedNodesIds.Count; ++i) {
                    if (_tmp_string.Contains(node.connectedNodesIds[i])) node.connectedNodesIds[i] = null;
                }
            }
        }

        // Called only if controlNodes is not NULL and the event is MouseDown
        void UpdateOrderedNodesSorting()
        {
            int totSelected = selection.selectedNodes.Count;
            int totSortables = orderedNodes.Count;
            if (totSelected == 0 || totSortables == 0 || totSelected >= totSortables) return;
            bool sortingRequired = false;
            if (totSelected == 1) {
                // Single selection
                IEditorGUINode selectedNode = selection.selectedNodes[0];
                sortingRequired = selectedNode.id != orderedNodes[totSortables - 1].id;
                if (!sortingRequired) return;
                for (int i = 0; i < totSortables; ++i) {
                    if (orderedNodes[i].id != interaction.targetNode.id) continue;
                    orderedNodes.Shift(i, totSortables - 1);
                    break;
                }
            } else {
                // Multiple selections
                for (int i = totSortables - 1; i > totSortables - totSelected - 1; --i) {
                    if (selection.selectedNodes.Contains(orderedNodes[i])) continue;
                    sortingRequired = true;
                    break;
                }
                if (!sortingRequired) return;
                int shiftOffset = 0;
                for (int i = totSortables - 1; i > -1; --i) {
                    if (!selection.selectedNodes.Contains(orderedNodes[i])) continue;
                    orderedNodes.Shift(i, totSortables - 1 - shiftOffset);
                    shiftOffset++;
                }
            }
            guiChangeType = GUIChangeType.SortedNodes;
            GUI.changed = _repaintOnEnd = true;
            DispatchOnGUIChange(GUIChangeType.SortedNodes);
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
                IEditorGUINode forwardConnectedNode = idToNode[id];
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
            public GUIStyle selectionRect, nodeSelectionOutline, nodeSelectionOutlineThick, endNodeOutline, draggingTooltip;
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
                draggingTooltip = new GUIStyle(GUI.skin.label).Add(new DeSkinColor(0.85f), TextAnchor.MiddleLeft, Format.RichText)
                    .Padding(5, 0, 0, 0).Background(DeStylePalette.blackSquare);
            }
        }
    }
}