// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/01 13:59
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    /// <summary>
    /// Always connects a node from BottomOrRight side to TopOrLeft side
    /// </summary>
    internal static class Connector
    {
        public static readonly DragData dragData = new DragData();
        const int _LineSize = 2;
        const int _MaxDistanceForSmartStraight = 120;
        const int _TangentDistance = 50;
        const int _TangentDistanceIfInverse = 120; // Tangent distance if TO is behind FROM

        #region Public Methods

        /// <summary>
        /// Always connects from BottomOrRight side to TopOrLeft side
        /// </summary>
        public static void Connect(
            int connectionIndex, int fromTotConnections,
            IEditorGUINode fromNode, NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, 
            IEditorGUINode toNode, NodeGUIData toGUIData, NodeConnectionOptions toOptions
        ) {
            AnchorsData anchorsData = GetAnchors(fromGUIData.fullArea, toGUIData.fullArea, fromOptions.connectorMode);
            // Line
            Handles.DrawBezier(anchorsData.fromP, anchorsData.toP, anchorsData.fromTangent, anchorsData.toTangent, Color.cyan, null, _LineSize);
            Rect arrowR = new Rect(
                anchorsData.toP.x - DeStylePalette.ico_nodeArrow.width,
                anchorsData.toP.y - DeStylePalette.ico_nodeArrow.height * 0.5f,
                DeStylePalette.ico_nodeArrow.width,
                DeStylePalette.ico_nodeArrow.height
            );
            // Arrow
            Matrix4x4 currGUIMatrix = GUI.matrix;
            if (anchorsData.arrowRequiresRotation) GUIUtility.RotateAroundPivot(anchorsData.arrowRotationAngle, anchorsData.toP);
            using (new DeGUI.ColorScope(null, null, Color.cyan)) GUI.DrawTexture(arrowR, DeStylePalette.ico_nodeArrow);
            GUI.matrix = currGUIMatrix;
        }

        public static void Drag(IEditorGUINode fromNode, NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, Vector2 mousePosition)
        {
            dragData.Set(fromNode);
            Vector2 attachP = GetDragAttachPoint(fromGUIData, fromOptions, mousePosition);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.white, null, _LineSize + 2);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.black, null, _LineSize);
        }

        #endregion

        #region Helpers

        static AnchorsData GetAnchors(Rect fromArea, Rect toArea, ConnectorMode connectorMode)
        {
            AnchorsData a = new AnchorsData();
            float distX = toArea.xMin - fromArea.xMax;
            float distY = toArea.yMin - fromArea.yMax;
            bool fromIsBottom = fromArea.yMax < toArea.yMin && distY >= distX;
            a.fromP = fromIsBottom
                ? new Vector2(fromArea.center.x, fromArea.yMax)
                : new Vector2(fromArea.xMax, fromArea.center.y);
            bool toIsTop = toArea.y > a.fromP.y && (fromArea.xMax > toArea.xMin || toArea.yMin - a.fromP.y > toArea.center.x - a.fromP.x);
            a.toP = toIsTop
                ? new Vector2(toArea.center.x, toArea.yMin)
                : new Vector2(toArea.xMin, toArea.center.y);
            // Set tangents
            float dist = Vector2.Distance(a.toP, a.fromP);
            float tangentDistance = a.toP.x < a.fromP.x && a.toP.y < a.fromP.y
                ? _TangentDistanceIfInverse
                : Mathf.Min(_TangentDistance, dist * 0.5f);
            a.isStraight = connectorMode == ConnectorMode.Straight
                              || connectorMode == ConnectorMode.Smart && dist <= _MaxDistanceForSmartStraight;
            if (a.isStraight) {
                a.fromTangent = a.fromP;
                a.toTangent = a.toP;
            } else {
                a.fromTangent = a.fromP + (fromIsBottom ? Vector2.up * tangentDistance : Vector2.right * tangentDistance);
                a.toTangent = a.toP + (toIsTop? Vector2.up * -tangentDistance : Vector2.right * -tangentDistance);
            }
            // Set arrow
            if (a.isStraight) {
                a.arrowRequiresRotation = true;
                a.arrowRotationAngle = -AngleBetween(Vector2.right, a.toP - a.fromP);
            } else if (toIsTop) {
                a.arrowRequiresRotation = true;
                a.arrowRotationAngle = 90;
            }
            return a;
        }

        static Vector2 GetDragAttachPoint(NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, Vector2 mousePosition)
        {
            // Bottom or right center
            bool isRight = mousePosition.y < fromGUIData.fullArea.y
                           || mousePosition.x - fromGUIData.fullArea.xMax >= mousePosition.y - fromGUIData.fullArea.yMax;
            return isRight
                ? new Vector2(fromGUIData.fullArea.xMax, fromGUIData.fullArea.center.y)
                : new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.yMax);
        }

        // Returns the angle in degrees between from and to (0 to 180, negative if to is on left, positive if to is on right).
        static float AngleBetween(Vector2 from, Vector2 to)
        {
            from.Normalize();
            to.Normalize();
            float angle = Mathf.Acos(Mathf.Clamp(Vector2.Dot(from, to), -1f, 1f)) * 57.29578f;
            float cross = to.x * from.y - to.y * from.x;
            if (cross < 0) angle = -angle;
            return angle;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        internal class DragData
        {
            public IEditorGUINode node;
            public void Reset()
            {
                node = null;
            }
            public void Set(IEditorGUINode node)
            {
                this.node = node;
            }
        }

        struct AnchorsData
        {
            public Vector2 fromP, toP;
            public Vector2 fromTangent, toTangent;
            public bool isStraight;
            public bool arrowRequiresRotation;
            public float arrowRotationAngle;
        }
    }
}