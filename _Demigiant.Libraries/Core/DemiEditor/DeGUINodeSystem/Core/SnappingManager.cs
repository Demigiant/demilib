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
        public bool hasSnapX { get; private set; }
        public bool hasSnapY { get; private set; }
        public int snapXPosition { get; private set; } // 0 is left, 1 is right
        public int snapYPosition { get; private set; } // 0 is top, 1 is bottom
        public bool showVerticalGuide { get; private set; }
        public bool showHorizontalGuide { get; private set; }
        public float snapX { get; private set; }
        public float snapY { get; private set; }

        const float _MaxSnappingDistance = 7;
        readonly List<float> _topSnappingPs = new List<float>();
        readonly List<float> _bottomSnappingPs = new List<float>();
        readonly List<float> _leftSnappingPs = new List<float>();
        readonly List<float> _rightSnappingPs = new List<float>();

        #region Public Methods

        public void EvaluateSnapping(
            IEditorGUINode forNode, Rect forArea, List<IEditorGUINode> allNodes, List<IEditorGUINode> excludedNodes,
            Dictionary<IEditorGUINode, NodeGUIData> nodeToGuiData, Rect processRelativeArea
        ){
            hasSnapX = hasSnapY = showHorizontalGuide = showVerticalGuide = false;
            snapXPosition = snapYPosition = 0;
            _topSnappingPs.Clear();
            _bottomSnappingPs.Clear();
            _leftSnappingPs.Clear();
            _rightSnappingPs.Clear();
            bool hasNearSnappingX = false;
            bool hasNearSnappingY = false;
            bool hasBorderSnapping = false;
            bool hasTopSnappingPs = false;
            bool hasBottomSnappingPs = false;
            bool hasLeftSnappingPs = false;
            bool hasRightSnappingPs = false;

            if (Event.current.alt || nodeToGuiData[forNode].disableSnapping) return; // ALT pressed or snapping disabled - no snapping

            // Find snapping points and store them
            int len = allNodes.Count;
            // Near snapping
            for (int i = 0; i < len; ++i) {
                IEditorGUINode node = allNodes[i];
                if (node == forNode || excludedNodes.Contains(node)) continue;
                NodeGUIData nodeGUIData = nodeToGuiData[node];
                if (nodeGUIData.disableSnapping) continue;
                Rect toArea = nodeGUIData.fullArea;
                if (!hasNearSnappingX && forArea.yMax > toArea.y && forArea.y < toArea.yMax) {
                    // Within nearSnappingX range
                    // Check rightToLeft then leftToRight
                    if (forArea.xMax < toArea.x && toArea.x - forArea.xMax <= NodeProcess.SnapOffset) {
                        hasSnapX = hasNearSnappingX = true;
                        snapX = toArea.x - forArea.width - NodeProcess.SnapOffset;
                        if (hasNearSnappingY) break;
                    } else if (forArea.x > toArea.xMax && forArea.x - toArea.xMax <= NodeProcess.SnapOffset) {
                        hasSnapX = hasNearSnappingX = true;
                        snapX = toArea.xMax + NodeProcess.SnapOffset;
                        if (hasNearSnappingY) break;
                    }
                }
                if (!hasNearSnappingY && forArea.xMax >= toArea.x && forArea.x < toArea.xMax) {
                    // Within nearSnappingY range
                    // Check bottomToTop then topToBottom
                    if (forArea.yMax < toArea.y && toArea.y - forArea.yMax <= NodeProcess.SnapOffset) {
                        hasSnapY = hasNearSnappingY = true;
                        snapY = toArea.y - forArea.height - NodeProcess.SnapOffset;
                        if (hasNearSnappingX) break;
                    } else if (forArea.y > toArea.yMax && forArea.y - toArea.yMax <= NodeProcess.SnapOffset) {
                        hasSnapY = hasNearSnappingY = true;
                        snapY = toArea.yMax + NodeProcess.SnapOffset;
                        if (hasNearSnappingX) break;
                    }
                }
            }
            if (!hasNearSnappingX || !hasNearSnappingY) {
                // Border snapping
                for (int i = 0; i < len; ++i) {
                    IEditorGUINode node = allNodes[i];
                    if (node == forNode || excludedNodes.Contains(node)) continue;
                    NodeGUIData nodeGUIData = nodeToGuiData[node];
                    if (nodeGUIData.disableSnapping) continue;
                    Rect toArea = nodeGUIData.fullArea;
                    if (!processRelativeArea.Overlaps(toArea)) continue;
                    if (!hasNearSnappingX) {
                        if (ValuesAreWithinBorderSnappingRange(forArea.x, toArea.x)) {
                            _leftSnappingPs.Add(toArea.x);
                            hasSnapX = showVerticalGuide = hasBorderSnapping = hasLeftSnappingPs = true;
                        } else if (ValuesAreWithinBorderSnappingRange(forArea.x, toArea.xMax)) {
                            _rightSnappingPs.Add(toArea.xMax);
                            hasSnapX = showVerticalGuide = hasBorderSnapping = hasRightSnappingPs = true;
                        } else if (ValuesAreWithinBorderSnappingRange(forArea.xMax, toArea.xMax)) {
                            _rightSnappingPs.Add(toArea.xMax);
                            hasSnapX = showVerticalGuide = hasBorderSnapping = hasRightSnappingPs = true;
                            snapXPosition = 1;
                        }
                    }
                    if (!hasNearSnappingY) {
                        if (ValuesAreWithinBorderSnappingRange(forArea.y, toArea.y)) {
                            _topSnappingPs.Add(toArea.y);
                            hasSnapY = showHorizontalGuide = hasBorderSnapping = hasTopSnappingPs = true;
                        } else if (ValuesAreWithinBorderSnappingRange(forArea.y, toArea.yMax)) {
                            _bottomSnappingPs.Add(toArea.yMax);
                            hasSnapY = showHorizontalGuide = hasBorderSnapping = hasBottomSnappingPs = true;
                        } else if (ValuesAreWithinBorderSnappingRange(forArea.yMax, toArea.yMax)) {
                            _bottomSnappingPs.Add(toArea.yMax);
                            hasSnapY = showHorizontalGuide = hasBorderSnapping = hasBottomSnappingPs = true;
                            snapYPosition = 1;
                        }
                    }
                }
                // Find closest snapping point
                if (hasBorderSnapping) {
                    if (hasLeftSnappingPs) snapX = FindNearestValueTo(forArea.x, _leftSnappingPs);
                    else if (hasRightSnappingPs) {
                        snapX = snapXPosition == 0
                            ? FindNearestValueTo(forArea.x, _rightSnappingPs)
                            : FindNearestValueTo(forArea.xMax, _rightSnappingPs) - forArea.width;
                    }
                    if (hasTopSnappingPs) snapY = FindNearestValueTo(forArea.y, _topSnappingPs);
                    else if (hasBottomSnappingPs) {
                        snapY = snapYPosition == 0
                            ? FindNearestValueTo(forArea.y, _bottomSnappingPs)
                            : FindNearestValueTo(forArea.yMax, _bottomSnappingPs) - forArea.height;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        bool ValuesAreWithinBorderSnappingRange(float a, float b)
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