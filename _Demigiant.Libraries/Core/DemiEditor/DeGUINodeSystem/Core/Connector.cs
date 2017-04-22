// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/01 13:59
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor.Internal;
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

        const int _LineSize = 3;
        const int _MaxDistanceForSmartStraight = 40; // was 120
        const int _TangentDistance = 50;
        const int _TangentDistanceIfInverse = 120; // Tangent distance if TO is behind FROM
        static readonly Styles _Styles = new Styles();

        #region Public Methods

        /// <summary>
        /// Always connects from BottomOrRight side to TopOrLeft side.
        /// If ALT is pressed shows the delete connection button.
        /// Called during Repaint or MouseDown/Up.
        /// Returns TRUE if the connection was deleted using the delete connection button.
        /// </summary>
        public static bool Connect(
            NodeProcess process, int connectionIndex,
            int fromTotConnections, NodeGUIData fromGUIData, NodeConnectionOptions fromOptions,
            NodeGUIData toGUIData
        )
        {
            _Styles.Init();
            bool altMode = Event.current.alt;

            bool useSubFromAreas = fromGUIData.connectorAreas != null;
            Rect fromArea = useSubFromAreas ? fromGUIData.connectorAreas[connectionIndex] : fromGUIData.fullArea;
            AnchorsData anchorsData = GetAnchors(fromArea, toGUIData.fullArea, fromOptions.connectorMode, useSubFromAreas);
            Color color = fromTotConnections < 2 || fromOptions.gradientColor == null
                ? fromOptions.startColor == Color.clear ? fromGUIData.mainColor : fromOptions.startColor
                : fromOptions.gradientColor.Evaluate(connectionIndex / (float)(fromTotConnections - 1));
            // Line start point
            const int fromRWidth = 6; // Height and width are switched if start point is bottom
            const int fromRHeight = 8;
            Rect fromR;
            if (anchorsData.fromIsSide) {
                fromR = new Rect(anchorsData.fromP.x, anchorsData.fromP.y - fromRHeight * 0.5f, fromRWidth, fromRHeight);
                anchorsData.fromP.x += fromRWidth;
                if (anchorsData.isStraight) anchorsData.fromTangent.x += fromRWidth;
            } else {
                fromR = new Rect(anchorsData.fromP.x - fromRHeight * 0.5f, anchorsData.fromP.y, fromRHeight, fromRWidth);
                anchorsData.fromP.y += fromRWidth;
                if (anchorsData.isStraight) anchorsData.fromTangent.y += fromRWidth;
            }
            using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(fromR, DeStylePalette.whiteSquare);
            // Line
            Handles.DrawBezier(anchorsData.fromP, anchorsData.toNicerP, anchorsData.fromTangent, anchorsData.toTangent, color, null, _LineSize);
            // Arrow
            Rect arrowR = new Rect(
                anchorsData.arrowP.x - DeStylePalette.ico_nodeArrow.width,
                anchorsData.arrowP.y - DeStylePalette.ico_nodeArrow.height * 0.5f,
                DeStylePalette.ico_nodeArrow.width,
                DeStylePalette.ico_nodeArrow.height
            );
            Matrix4x4 currGUIMatrix = GUI.matrix;
            if (anchorsData.arrowRequiresRotation) {
                GUIUtility.RotateAroundPivot(anchorsData.arrowRotationAngle, anchorsData.arrowP * process.guiScale + process.guiScalePositionDiff);
            }
            using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(arrowR, DeStylePalette.ico_nodeArrow);
            GUI.matrix = currGUIMatrix;
            // Delete connection button (placed at center of line)
            if (altMode) {
                Vector2 midP = anchorsData.fromTangent + (anchorsData.toTangent - anchorsData.fromTangent) * 0.5f;
                Vector2 midPAlt = anchorsData.fromP + (anchorsData.toNicerP - anchorsData.fromP) * 0.5f;
                midP += (midPAlt - midP) * 0.25f;
                Rect btR = new Rect(midP.x - 5, midP.y - 5, 10, 10);
                using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(btR.Expand(2), DeStylePalette.circle);
                using (new DeGUI.ColorScope(null, null, DeGUI.colors.global.red)) {
                    if (GUI.Button(btR, "", _Styles.btDelete)) return true;
                }
                GUI.DrawTexture(btR.Contract(2), DeStylePalette.ico_delete);
            }
            return false;
        }

        public static void Drag(InteractionManager interaction, Vector2 mousePosition)
        {
            dragData.Set(interaction.targetNode);
            Vector2 attachP = GetDragAttachPoint(interaction.targetNodeConnectorArea, mousePosition);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.black, null, _LineSize + 2);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, DeGUI.colors.global.orange, null, _LineSize);
        }

        #endregion

        #region Helpers

        static AnchorsData GetAnchors(Rect fromArea, Rect toArea, ConnectorMode connectorMode, bool sideOnly){
            AnchorsData a = new AnchorsData();
            float distX = toArea.x - fromArea.xMax;
            float distY = toArea.y - fromArea.yMax;
            bool fromIsBottom = !sideOnly && fromArea.yMax < toArea.y && distY >= distX;
            a.fromP = fromIsBottom
                ? new Vector2(fromArea.center.x, fromArea.yMax)
                : new Vector2(fromArea.xMax, fromArea.center.y);
            bool toIsTop = toArea.y > a.fromP.y && (fromArea.xMax > toArea.x || toArea.y - a.fromP.y > toArea.center.x - a.fromP.x);
            a.toP = a.toNicerP = toIsTop
                ? new Vector2(toArea.center.x, toArea.y)
                : new Vector2(toArea.x, toArea.center.y);
            a.arrowP = a.toP;
            a.fromIsSide = !fromIsBottom;
            a.toIsSide = !toIsTop;
            // Set tangents
            bool isToBehindFrom = a.toP.x < a.fromP.x && a.toP.y < fromArea.yMax;
            float dist = Vector2.Distance(a.toP, a.fromP);
            a.isStraight = connectorMode == ConnectorMode.Straight
                              || !isToBehindFrom && connectorMode == ConnectorMode.Smart && dist <= _MaxDistanceForSmartStraight;
            if (a.isStraight) {
                a.fromTangent = a.fromP;
                a.toTangent = a.toP;
            } else {
                if (toIsTop) a.toNicerP.y -= DeStylePalette.ico_nodeArrow.width;
                else a.toNicerP.x -= DeStylePalette.ico_nodeArrow.width;
                float tangentDistance = isToBehindFrom ? _TangentDistanceIfInverse : Mathf.Min(_TangentDistance, dist * 0.4f);
                a.fromTangent = a.fromP + (fromIsBottom ? Vector2.up * tangentDistance : Vector2.right * tangentDistance);
                a.toTangent = a.toNicerP + (toIsTop? Vector2.up * -tangentDistance : Vector2.right * -tangentDistance);
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

        static Vector2 GetDragAttachPoint(Rect nodeArea, Vector2 mousePosition)
        {
            // Bottom or right center
            bool isRight = mousePosition.y < nodeArea.y
                           || mousePosition.x - nodeArea.xMax >= mousePosition.y - nodeArea.yMax;
            return isRight
                ? new Vector2(nodeArea.xMax, nodeArea.center.y)
                : new Vector2(nodeArea.center.x, nodeArea.yMax);
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
        // ███ CLASSES █████████████████████████████████████████████████████████████████████████████████████████████████████████
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
            public Vector2 toNicerP; // P adapted in case of curved arrow to make line look nicer
            public Vector2 fromTangent, toTangent;
            public Vector2 arrowP;
            public bool fromIsSide, toIsSide; // If FALSE it means it's either top (toNode) or bottom (fromNode)
            public bool isStraight;
            public bool arrowRequiresRotation;
            public float arrowRotationAngle;
        }

        struct ConnectionData
        {
            public int index, totConnections;
            public ConnectionData(int index, int totConnections)
            {
                this.index = index;
                this.totConnections = totConnections;
            }
        }

        class Styles
        {
            public GUIStyle btDelete;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                btDelete = new GUIStyle().StretchWidth().StretchHeight().Background(DeStylePalette.circle);
            }
        }
    }
}