// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/19 19:09
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal class NodeDragManager
    {
        public readonly Dictionary<IEditorGUINode,Vector2> nodeToFullDragPosition = new Dictionary<IEditorGUINode, Vector2>();

        readonly NodeProcess _process;
        readonly SnappingManager _snapper = new SnappingManager();
        IEditorGUINode _mainNode;
        Rect _nodeArea;
        // References to existing lists/dictionaries
        List<IEditorGUINode> _allNodesRef;
        Dictionary<IEditorGUINode, NodeGUIData> _nodeToGuiDataRef;
        List<IEditorGUINode> _draggedNodesRef;
        bool _lastDragWasSnappedOnX, _lastDragWasSnappedOnY;

        #region CONSTRUCTOR

        public NodeDragManager(NodeProcess process)
        {
            _process = process;
        }

        #endregion

        #region Public Methods

        public void BeginDrag(
            IEditorGUINode mainNode, List<IEditorGUINode> draggedNodes,
            List<IEditorGUINode> allNodes, Dictionary<IEditorGUINode, NodeGUIData> nodeToGuiData
        ){
            _mainNode = mainNode;
            _allNodesRef = allNodes;
            _nodeToGuiDataRef = nodeToGuiData;
            _draggedNodesRef = draggedNodes;
            _lastDragWasSnappedOnX = false;
            _lastDragWasSnappedOnY = false;
            nodeToFullDragPosition.Clear();
            foreach (IEditorGUINode node in _draggedNodesRef) nodeToFullDragPosition.Add(node, node.guiPosition);
        }

        public void ApplyDrag(Vector2 delta)
        {
            Vector2 mainFullP = nodeToFullDragPosition[_mainNode];
            _nodeArea = _nodeToGuiDataRef[_mainNode].fullArea;
            _nodeArea.x = mainFullP.x + _process.areaShift.x;
            _nodeArea.y = mainFullP.y + _process.areaShift.y;
            Vector2 snappedDelta = delta;
            _snapper.EvaluateSnapping(_mainNode, _nodeArea,  _allNodesRef, _draggedNodesRef, _nodeToGuiDataRef, _process.relativeArea);
            if (_snapper.hasSnapX) {
                _lastDragWasSnappedOnX = true;
                snappedDelta.x = _snapper.snapX - (_mainNode.guiPosition.x + _process.areaShift.x);
            } else if (_lastDragWasSnappedOnX) {
                // Readapt from snapped to unsnapped position
                Vector2 diff = new Vector2(mainFullP.x - _mainNode.guiPosition.x, 0);
                foreach (IEditorGUINode node in _draggedNodesRef) node.guiPosition += diff;
                _lastDragWasSnappedOnX = false;
            }
            if (_snapper.hasSnapY) {
                _lastDragWasSnappedOnY = true;
                snappedDelta.y = _snapper.snapY - (_mainNode.guiPosition.y + _process.areaShift.y);
            } else if (_lastDragWasSnappedOnY) {
                // Readapt from snapped to unsnapped position
                Vector2 diff = new Vector2(0, mainFullP.y - _mainNode.guiPosition.y);
                foreach (IEditorGUINode node in _draggedNodesRef) node.guiPosition += diff;
                _lastDragWasSnappedOnY = false;
            }
            foreach (IEditorGUINode node in _draggedNodesRef) {
                nodeToFullDragPosition[node] += delta;
                node.guiPosition += snappedDelta;
            }
        }

        // Called on NodeProcess.EndGUI, repaint only - draws eventual horizontal/vertical guides
        public void EndGUI()
        {
            float guideSize = 2 / _process.guiScale;
            if (_snapper.showHorizontalGuide) {
                float y = _snapper.snapYPosition == 0 ? _snapper.snapY : _snapper.snapY + _nodeArea.height;
                Vector2 fromP = new Vector2(0, y);
                Vector2 toP = new Vector2(_process.relativeArea.width, y);
                Handles.DrawBezier(fromP, toP, fromP, toP, Color.cyan, null, guideSize);
            }
            if (_snapper.showVerticalGuide) {
                float x = _snapper.snapXPosition == 0 ? _snapper.snapX : _snapper.snapX + _nodeArea.width;
                Vector2 fromP = new Vector2(x, 0);
                Vector2 toP = new Vector2(x, _process.relativeArea.height);
                Handles.DrawBezier(fromP, toP, fromP, toP, Color.cyan, null, guideSize);
            }
        }

        #endregion
    }
}