// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/19 18:27
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal class SnappingManager
    {
        public bool hasSnap { get; private set; }
        public bool hasSnapX { get; private set; }
        public bool hasSnapY { get; private set; }
        public bool showVerticalGuide { get; private set; }
        public bool showHorizontalGuide { get; private set; }
        public float snapX { get; private set; }
        public float snapY { get; private set; }

        const float _MaxSnappingDistance = 7;
        const float _BorderSnapping = 20; // Distance at which nodes will snap to each other when on different lines
        readonly List<float> _topSnappingPs = new List<float>();
        readonly List<float> _bottomSnappingPs = new List<float>();
        readonly List<float> _leftSnappingPs = new List<float>();
        readonly List<float> _rightSnappingPs = new List<float>();

        #region Public Methods

        public void EvaluateSnapping(
            IEditorGUINode forNode, Rect forArea, List<IEditorGUINode> allNodes, List<IEditorGUINode> excludedNodes,
            Dictionary<IEditorGUINode, NodeGUIData> nodeToGuiData
        ){
            hasSnap = hasSnapX = hasSnapY = showHorizontalGuide = showVerticalGuide = false;
            _topSnappingPs.Clear();
            _bottomSnappingPs.Clear();
            _leftSnappingPs.Clear();
            _rightSnappingPs.Clear();
            bool hasTopSnappingPs = false;
            bool hasBottomSnappingPs = false;
            bool hasLeftSnappingPs = false;
            bool hasRightSnappingPs = false;

            // Find snapping points and store them
            int len = allNodes.Count;
            for (int i = 0; i < len; ++i) {
                IEditorGUINode node = allNodes[i];
                if (node == forNode || excludedNodes.Contains(node)) continue;
                Rect area = nodeToGuiData[node].fullArea;
                if (ValuesAreWithinRange(forArea.x, area.x)) {
                    _leftSnappingPs.Add(area.x);
                    hasSnap = hasSnapX = showVerticalGuide = hasLeftSnappingPs = true;
                } else if (ValuesAreWithinRange(forArea.x, area.xMax)) {
                    _rightSnappingPs.Add(area.xMax);
                    hasSnap = hasSnapX = showVerticalGuide = hasRightSnappingPs = true;
                }
                if (ValuesAreWithinRange(forArea.y, area.y)) {
                    _topSnappingPs.Add(area.y);
                    hasSnap = hasSnapY = showHorizontalGuide = hasTopSnappingPs = true;
                } else if (ValuesAreWithinRange(forArea.y, area.yMax)) {
                    _bottomSnappingPs.Add(area.yMax);
                    hasSnap = hasSnapY = showHorizontalGuide = hasBottomSnappingPs = true;
                }
            }

            // Find closes snapping point
            if (hasLeftSnappingPs) snapX = FindNearestValueTo(forArea.x, _leftSnappingPs);
            else if (hasRightSnappingPs) snapX = FindNearestValueTo(forArea.x, _rightSnappingPs);
            if (hasTopSnappingPs) snapY = FindNearestValueTo(forArea.y, _topSnappingPs);
            else if (hasBottomSnappingPs) snapY = FindNearestValueTo(forArea.y, _bottomSnappingPs);
        }

        #endregion

        #region Helpers

        bool ValuesAreWithinRange(float a, float b)
        {
            return Mathf.Abs(a - b) <= _MaxSnappingDistance;
        }

        float FindNearestValueTo(float a, List<float> values)
        {
            float closest = a;
            float distance = 10000;
            for (int i = 0; i < values.Count; ++i) {
                float v = values[i];
                float d = Mathf.Abs(a - v);
                if (d < distance) {
                    distance = d;
                    closest = v;
                }
            }
            return closest;
        }

        #endregion
    }
}