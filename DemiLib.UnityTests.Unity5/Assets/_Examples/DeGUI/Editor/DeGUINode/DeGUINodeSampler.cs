// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/09 11:43
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DeEditorTools;
using DG.DeExtensions;
using DG.DemiEditor.DeGUINodeSystem;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using _Examples.DeGUI.DeGUINode;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class DeGUINodeSampler : EditorWindow
    {
        const string _Title = "DeGUINodeSampler";
        static DeSampler src;
        NodeProcess _nodeProcess;

        #region Open

        public static void ShowWindow(DeSampler src)
        {
            DeGUINodeSampler.src = src;
            GetWindow(typeof(DeGUINodeSampler), false, _Title);
        }

        #endregion

        #region Unity and GUI Methods

        void OnEnable()
        {
            if (src == null) src = DeEditorToolsUtils.FindFirstComponentOfType<DeSampler>();
            _nodeProcess = new NodeProcess(this, OnDeleteNodes);
            _nodeProcess.options.evidenceEndNodes = ProcessOptions.EvidenceEndNodesMode.Invasive;
            Undo.undoRedoPerformed -= this.Repaint;
            Undo.undoRedoPerformed += this.Repaint;
        }

        void OnHierarchyChange()
        { Repaint(); }

        void OnDestroy() { Undo.undoRedoPerformed -= Repaint; }

        void OnGUI()
        {
            if (src == null) {
                Close();
                return;
            }

            Undo.RecordObject(src, "DeSampler");
            DG.DemiEditor.DeGUI.BeginGUI();

            // Node GUI Process
            Rect nodeArea = this.position.ResetXY().SetY(100).Shift(0, 0, 0, -100);
            using (new NodeProcessScope<GenericNode>(_nodeProcess, nodeArea, ref src.nodeSystem.areaShift, src.nodeSystem.genericNodes)) {
                // Draw nodes
                // Generic and multi nodes
                Gradient multiGradient = new Gradient() {
                    colorKeys = new GradientColorKey[] {new GradientColorKey(Color.yellow, 0), new GradientColorKey(new Color(1f, 0.46f, 0f), 1)}
                };
                Gradient normalPlusGradient = new Gradient() {
                    colorKeys = new GradientColorKey[] {new GradientColorKey(new Color(0.02f, 1f, 0.66f), 0), new GradientColorKey(new Color(0.73f, 0.98f, 0.04f), 1)}
                };
                Gradient dualGradient = new Gradient() {
                    colorKeys = new GradientColorKey[] {new GradientColorKey(Color.green, 0), new GradientColorKey(Color.red, 1)}
                };
                NodeConnectionOptions normalNodeConnOptions = new NodeConnectionOptions(true, ConnectorMode.Smart);
                NodeConnectionOptions normalPlusNodeConnOptions = new NodeConnectionOptions(true, ConnectionMode.NormalPlus, ConnectorMode.Smart, Color.red, normalPlusGradient);
                NodeConnectionOptions multiNodeConnOptions = new NodeConnectionOptions(true, ConnectorMode.Smart, multiGradient);
                NodeConnectionOptions flexibleNodeConnOptions = new NodeConnectionOptions(true, ConnectionMode.Flexible, ConnectorMode.Smart);
                NodeConnectionOptions dualNodeConnOptions = new NodeConnectionOptions(true, ConnectionMode.Dual, ConnectorMode.Smart, dualGradient);
//                foreach (GenericNode node in src.nodeSystem.genericNodes) {
                foreach (IEditorGUINode editorGUINode in _nodeProcess.orderedNodes) {
                    GenericNode node = (GenericNode)editorGUINode;
                    switch (node.type) {
                    case NodeType.Multi:
                        _nodeProcess.Draw<MultiNodeGUI>(node, node.normalPlusConnectionMode ? normalPlusNodeConnOptions : multiNodeConnOptions);
                        break;
                    default:
                        _nodeProcess.Draw<GenericNodeGUI>(
                            node,
                            node.dualConnectionMode ? dualNodeConnOptions : node.flexibleConnectionMode ? flexibleNodeConnOptions : normalNodeConnOptions
                        );
                        break;
                    }
                }
                // Start node (last so it's always over other nodes)
                _nodeProcess.Draw<StartNodeGUI>(src.nodeSystem.startNode);
                
                // EVENTS
                switch (_nodeProcess.interaction.state) {
                case InteractionManager.State.ContextClick:
                    switch (_nodeProcess.interaction.mouseTargetType) {
                    case InteractionManager.TargetType.Background:
                        Debug.Log("ContextClick > Background");
                        ContextMenu_Background();
                        break;
                    case InteractionManager.TargetType.Node:
                        if (_nodeProcess.selection.selectedNodes.Count == 0) Debug.Log("ContextClick > Unselected node");
                        else if (_nodeProcess.selection.selectedNodes.Count == 1) Debug.Log("ContextClick > Single selected node");
                        else Debug.Log("ContextClick > Multiple selected nodes");
                        break;
                    }
                    break;
                case InteractionManager.State.DoubleClick:
                    Debug.Log("<color=#00ff00>DoubleClick > " + _nodeProcess.interaction.mouseTargetType + (_nodeProcess.interaction.targetNode == null ? "" : " > " + _nodeProcess.interaction.targetNode.id) + "</color>");
                    break;
                }
            }
            // Header test
            DG.DemiEditor.DeGUI.DrawColoredSquare(this.position.ResetXY().SetHeight(100), new Color(0f, 0.84f, 1f, 0.5f));

            if (GUI.changed) EditorUtility.SetDirty(src);
        }

        #endregion

        #region ContextMenus

        void ContextMenu_Background()
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent(string.Format("Reset Panning (current: {0},{1})", src.nodeSystem.areaShift.x, src.nodeSystem.areaShift.y)), false, () => {
                src.nodeSystem.areaShift = Vector2.zero;
                EditorUtility.SetDirty(src);
                this.Repaint();
            });
            menu.AddItem(new GUIContent("Capture screenshot"), false, () => {
                _nodeProcess.CaptureScreenshot(NodeProcess.ScreenshotMode.VisibleArea, DG.DemiEditor.DeGUI.ShowTexturePreview);
            });
            menu.DropDown(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0));
        }

        #endregion

        #region Callbacks

        bool OnDeleteNodes(List<IEditorGUINode> nodes)
        {
            Debug.Log(string.Format("OnDeleteNodes callback > Deleting {0} nodes", nodes.Count));
            return true;
        }

        #endregion
    }
}