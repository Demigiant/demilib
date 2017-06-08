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
        enum ConnectionSide
        {
            Top, Bottom, Left, Right
        }

        public static readonly DragData dragData = new DragData();

        const int _LineSize = 3;
        const int _MaxDistanceForSmartStraight = 40; // was 120
        const int _TangentDistance = 50;
        const int _TangentDistanceIfInverse = 90; // Tangent distance if TO is behind FROM
        const int FromSquareWidth = 2; // Height and width are switched if start point is top/bottom
        const int FromSquareHeight = 8;
        static readonly Styles _Styles = new Styles();
        static Color _lineShadowColor = new Color(0, 0, 0, 0.4f);

        #region Public Methods

        /// <summary>
        /// Always connects from BottomOrRight side to TopOrLeft side.
        /// If ALT is pressed shows the delete connection button.
        /// Called during Repaint or MouseDown/Up.
        /// Returns TRUE if the connection was deleted using the delete connection button.
        /// </summary>
        public static bool Connect(
            NodeProcess process, int connectionIndex, int fromTotConnections, NodeConnectionOptions fromOptions,
            IEditorGUINode fromNode, IEditorGUINode toNode
        )
        {
            _Styles.Init();
            NodeGUIData fromGUIData = process.nodeToGUIData[fromNode];
            NodeGUIData toGUIData = process.nodeToGUIData[toNode];

            bool useSubFromAreas = fromOptions.connectionMode != ConnectionMode.Dual && fromGUIData.connectorAreas != null;
            Rect fromArea = useSubFromAreas ? fromGUIData.connectorAreas[connectionIndex] : fromGUIData.fullArea;
            AnchorsData anchorsData = GetAnchorsAllSides(process, connectionIndex, fromNode, fromArea, toNode, toGUIData.fullArea, fromOptions, useSubFromAreas);
            Color color = GetConnectionColor(connectionIndex, fromTotConnections, fromGUIData, fromOptions);
            // Line (shadow + line)
            Handles.DrawBezier(
                anchorsData.fromLineP, anchorsData.toLineP, anchorsData.fromTangent, anchorsData.toTangent, _lineShadowColor, null, _LineSize + 2
            );
            Handles.DrawBezier(anchorsData.fromLineP, anchorsData.toLineP, anchorsData.fromTangent, anchorsData.toTangent, color, null, _LineSize);
            // Line start square
            Rect fromSquareR;
            switch (anchorsData.fromSide) {
            case ConnectionSide.Top:
                fromSquareR = new Rect(anchorsData.fromMarkP.x - FromSquareHeight * 0.5f, anchorsData.fromMarkP.y - FromSquareWidth, FromSquareHeight, FromSquareWidth);
                break;
            case ConnectionSide.Bottom:
                fromSquareR = new Rect(anchorsData.fromMarkP.x - FromSquareHeight * 0.5f, anchorsData.fromMarkP.y, FromSquareHeight, FromSquareWidth);
                break;
            case ConnectionSide.Left:
                fromSquareR = new Rect(anchorsData.fromMarkP.x - FromSquareWidth, anchorsData.fromMarkP.y - FromSquareHeight * 0.5f, FromSquareWidth, FromSquareHeight);
                break;
            default: // Right
                fromSquareR = new Rect(anchorsData.fromMarkP.x, anchorsData.fromMarkP.y - FromSquareHeight * 0.5f, FromSquareWidth, FromSquareHeight);
                break;
            }
            using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(fromSquareR, DeStylePalette.whiteSquare);
            // Arrow
            Rect arrowR = new Rect(
                anchorsData.toArrowP.x - DeStylePalette.ico_nodeArrow.width, anchorsData.toArrowP.y - DeStylePalette.ico_nodeArrow.height * 0.5f,
                DeStylePalette.ico_nodeArrow.width, DeStylePalette.ico_nodeArrow.height
            );
            Matrix4x4 currGUIMatrix = GUI.matrix;
            if (anchorsData.arrowRequiresRotation) {
                GUIUtility.RotateAroundPivot(anchorsData.arrowRotationAngle, anchorsData.toArrowP * process.guiScale + process.guiScalePositionDiff);
            }
            using (new DeGUI.ColorScope(null, null, color)) GUI.DrawTexture(arrowR, DeStylePalette.ico_nodeArrow);
            GUI.matrix = currGUIMatrix;
            // Delete connection button (placed at center of line)
            if (DeGUIKey.Exclusive.alt) {
                Vector2 midP = anchorsData.fromTangent + (anchorsData.toTangent - anchorsData.fromTangent) * 0.5f;
                Vector2 midPAlt = anchorsData.fromLineP + (anchorsData.toLineP - anchorsData.fromLineP) * 0.5f;
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

        // Returns the connection color
        public static Color Drag(
            InteractionManager interaction, Vector2 mousePosition,
            NodeGUIData nodeGuiData, NodeConnectionOptions connectionOptions
        ){
            dragData.Set(interaction.targetNode);
            int connectionIndex = connectionOptions.connectionMode == ConnectionMode.Dual
                ? DeGUIKey.Exclusive.ctrl && DeGUIKey.Extra.space ? 1 : 0
                : interaction.targetNodeConnectorAreaIndex;
            Color color = GetConnectionColor(
                connectionIndex, interaction.targetNode.connectedNodesIds.Count, nodeGuiData, connectionOptions
            );
            Vector2 attachP = interaction.targetNodeConnectorArea.center;
            // Pointers
            Rect pointerFrom = new Rect(attachP.x - 4, attachP.y - 4, 8, 8);
            Rect pointerTo = new Rect(mousePosition.x - 4, mousePosition.y - 4, 8, 8);
            using (new DeGUI.ColorScope(null, null, Color.black)) {
                GUI.DrawTexture(pointerFrom.Expand(4), DeStylePalette.circle);
                GUI.DrawTexture(pointerTo.Expand(4), DeStylePalette.circle);
            }
            using (new DeGUI.ColorScope(null, null, color)) {
                GUI.DrawTexture(pointerFrom, DeStylePalette.circle);
                GUI.DrawTexture(pointerTo, DeStylePalette.circle);
            }
            // Line
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.black, null, _LineSize + 2);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, color, null, _LineSize + 2);

            return color;
        }

        #endregion

        #region Helpers

        static AnchorsData GetAnchorsAllSides(
            NodeProcess process, int connectionIndex, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea,
            NodeConnectionOptions connectionOptions, bool sideOnly
        ){
            AnchorsData a = new AnchorsData();

            // From/to side
            if (sideOnly) a.fromSide = ConnectionSide.Right;
            else {
                if (toArea.x >= fromArea.center.x) {
                    // R/T/B
                    if (toArea.x < fromArea.xMax) {
                        a.fromSide = toArea.center.y < fromArea.center.y ? ConnectionSide.Top : ConnectionSide.Bottom;
                    } else {
                        if (toArea.yMax > fromArea.y && toArea.y < fromArea.yMax) a.fromSide = ConnectionSide.Right;
                        else {
                            float dX = toArea.x - fromArea.xMax;
                            float dY = toArea.center.y < fromArea.y ? toArea.center.y - fromArea.y : toArea.center.y - fromArea.yMax;
//                            if (dX > Mathf.Abs(dY)) a.fromSide = ConnectionSide.Right;
                            if (dX > Mathf.Abs(dY) || !FromAnchorCanBeVertical(process, fromNode, fromArea, toNode, toArea)) a.fromSide = ConnectionSide.Right;
                            else a.fromSide = dY < 0 ? ConnectionSide.Top : ConnectionSide.Bottom;
                        }
                    }
                } else {
                    // L/T/B
                    if (toArea.xMax > fromArea.x) {
                        a.fromSide = toArea.center.y < fromArea.center.y ? ConnectionSide.Top : ConnectionSide.Bottom;
                    } else {
                        if (toArea.yMax > fromArea.y && toArea.y < fromArea.yMax) a.fromSide = ConnectionSide.Left;
                        else {
                            float dX = fromArea.x - toArea.xMax;
                            float dY = toArea.yMax < fromArea.y ? toArea.yMax - fromArea.y : toArea.y - fromArea.yMax;
                            if (dX > Mathf.Abs(dY) || !FromAnchorCanBeVertical(process, fromNode, fromArea, toNode, toArea)) a.fromSide = ConnectionSide.Left;
                            else a.fromSide = dY < 0 ? ConnectionSide.Top : ConnectionSide.Bottom;
                        }
                    }
                }
            }
            // To side
            a.toSide = ConnectionSide.Left;
            switch (a.fromSide) {
            case ConnectionSide.Top:
                a.toSide = ToAnchorCanBeVertical(process, fromNode, fromArea, toNode, toArea)
                    ? ConnectionSide.Bottom
                    : toArea.center.x < fromArea.center.x ? ConnectionSide.Right : ConnectionSide.Left;
                break;
            case ConnectionSide.Bottom:
                a.toSide = ToAnchorCanBeVertical(process, fromNode, fromArea, toNode, toArea)
                    ? ConnectionSide.Top
                    : toArea.center.x < fromArea.center.x ? ConnectionSide.Right : ConnectionSide.Left;
                break;
            case ConnectionSide.Left:
                a.toSide = ConnectionSide.Right;
                break;
            default: // Right
                if (sideOnly && toArea.x < fromArea.xMax) {
                    // Right side was forced, see if we can connect to sweeter sides
                    a.toSide = toArea.center.x >= fromArea.xMax
                        ? toArea.yMax < fromArea.y ? ConnectionSide.Bottom : ConnectionSide.Top
                        : ConnectionSide.Right;
                } else a.toSide = ConnectionSide.Left;
                break;
            }
            //
            a.fromIsSide = a.fromSide == ConnectionSide.Left || a.fromSide == ConnectionSide.Right;
            a.toIsSide = a.toSide == ConnectionSide.Left || a.toSide == ConnectionSide.Right;
            //
            int fromDisplacement = sideOnly ? 0 : 0;
            const int toDisplacement = -0;
            switch (a.fromSide) {
            case ConnectionSide.Top:
                a.fromLineP = a.fromMarkP = new Vector2(fromArea.center.x + fromDisplacement, fromArea.y);
                a.fromLineP.y -= FromSquareWidth;
                if (connectionOptions.connectionMode == ConnectionMode.Dual) a.fromLineP.x = a.fromMarkP.x += connectionIndex == 1 ? 4 : -4;
                break;
            case ConnectionSide.Bottom:
                a.fromLineP = a.fromMarkP = new Vector2(fromArea.center.x + fromDisplacement, fromArea.yMax);
                a.fromLineP.y += FromSquareWidth;
                if (connectionOptions.connectionMode == ConnectionMode.Dual) a.fromLineP.x = a.fromMarkP.x += connectionIndex == 1 ? 4 : -4;
                break;
            case ConnectionSide.Left:
                a.fromLineP = a.fromMarkP = new Vector2(fromArea.x, fromArea.center.y + fromDisplacement);
                a.fromLineP.x -= FromSquareWidth;
                if (connectionOptions.connectionMode == ConnectionMode.Dual) a.fromLineP.y = a.fromMarkP.y += connectionIndex == 1 ? 4 : -4;
                break;
            case ConnectionSide.Right:
                a.fromLineP = a.fromMarkP = new Vector2(fromArea.xMax, fromArea.center.y + fromDisplacement);
                a.fromLineP.x += FromSquareWidth;
                if (connectionOptions.connectionMode == ConnectionMode.Dual) a.fromLineP.y = a.fromMarkP.y += connectionIndex == 1 ? 4 : -4;
                break;
            }
            switch (a.toSide) {
            case ConnectionSide.Top:
                a.toArrowP = a.toLineP = new Vector2(toArea.center.x + toDisplacement, toArea.y);
                break;
            case ConnectionSide.Bottom:
                a.toArrowP = a.toLineP = new Vector2(toArea.center.x + toDisplacement, toArea.yMax);
                break;
            case ConnectionSide.Left:
                a.toArrowP = a.toLineP = new Vector2(toArea.x, toArea.center.y + toDisplacement);
                break;
            case ConnectionSide.Right:
                a.toArrowP = a.toLineP = new Vector2(toArea.xMax, toArea.center.y + toDisplacement);
                break;
            }
            // Set tangents + arrows
//            bool toIsBehindFrom = a.fromSide == ConnectionSide.Right && a.toArrowP.x < a.fromMarkP.x && a.toArrowP.y < fromArea.yMax;
            bool toIsBehindFrom = a.fromSide == ConnectionSide.Right && a.toArrowP.x < fromArea.center.x;
            float d = Vector2.Distance(a.toArrowP, a.fromLineP);
            a.isStraight = connectionOptions.connectorMode == ConnectorMode.Straight
                              || !toIsBehindFrom && connectionOptions.connectorMode == ConnectorMode.Smart && d <= _MaxDistanceForSmartStraight;
            if (a.isStraight) {
                a.fromTangent = a.fromLineP;
                a.toTangent = a.toArrowP;
                a.arrowRequiresRotation = true;
                a.arrowRotationAngle = -AngleBetween(Vector2.right, a.toArrowP - a.fromLineP);
            } else {
                float axisDistance = a.fromIsSide ? Mathf.Abs(a.toArrowP.x - a.fromLineP.x) : Mathf.Abs(a.toArrowP.y - a.fromLineP.y);
                float tangentDistance = toIsBehindFrom ? _TangentDistanceIfInverse : Mathf.Min(_TangentDistance, axisDistance * 0.2f + d * 0.2f);
                Vector2 fromTangentOffset, toTangentOffset;
                switch (a.fromSide) {
                case ConnectionSide.Top:
                    fromTangentOffset = Vector2.up * -tangentDistance;
                    break;
                case ConnectionSide.Bottom:
                    fromTangentOffset = Vector2.up * tangentDistance;
                    break;
                case ConnectionSide.Left:
                    fromTangentOffset = Vector2.right * -tangentDistance;
                    break;
                default: // Right
                    fromTangentOffset = Vector2.right * tangentDistance;
                    break;
                }
                switch (a.toSide) {
                case ConnectionSide.Top:
                    a.toLineP.y -= DeStylePalette.ico_nodeArrow.width;
                    toTangentOffset = Vector2.up * -tangentDistance;
                    a.arrowRequiresRotation = true;
                    a.arrowRotationAngle = 90;
                    break;
                case ConnectionSide.Bottom:
                    a.toLineP.y += DeStylePalette.ico_nodeArrow.width;
                    toTangentOffset = Vector2.up * tangentDistance;
                    a.arrowRequiresRotation = true;
                    a.arrowRotationAngle = -90;
                    break;
                case ConnectionSide.Left:
                    a.toLineP.x -= DeStylePalette.ico_nodeArrow.width;
                    toTangentOffset = Vector2.right * -tangentDistance;
                    break;
                default: // Right
                    a.toLineP.x += DeStylePalette.ico_nodeArrow.width;
                    toTangentOffset = Vector2.right * tangentDistance;
                    a.arrowRequiresRotation = true;
                    a.arrowRotationAngle = 180;
                    break;
                }
                a.fromTangent = a.fromLineP + fromTangentOffset;
                a.toTangent = a.toLineP + toTangentOffset;
            }
            return a;
        }

        static AnchorsData GetAnchors_2Sides(
            NodeProcess process, int connectionIndex, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea,
            NodeConnectionOptions connectionOptions, bool sideOnly
        ){
            AnchorsData a = new AnchorsData();
            float distX = toArea.x - fromArea.xMax;
            float distY = toArea.y - fromArea.yMax;
            bool fromIsBottom = !sideOnly && fromArea.yMax < toArea.y && distY >= distX
                && FromAnchorCanBeBottom(process, fromNode, fromArea, toNode, toArea);
            a.fromMarkP = fromIsBottom
                ? new Vector2(fromArea.center.x, fromArea.yMax)
                : new Vector2(fromArea.xMax, fromArea.center.y);
            if (connectionOptions.connectionMode == ConnectionMode.Dual) {
                if (fromIsBottom) a.fromMarkP.x += connectionIndex == 1 ? 4 : -4;
                else a.fromMarkP.y += connectionIndex == 1 ? 4 : -4;
            }
            // Find correct fromLineP
            a.fromLineP = a.fromMarkP;
            if (fromIsBottom) a.fromLineP.y += FromSquareWidth;
            else a.fromLineP.x += FromSquareWidth;
            //
            bool toIsTop = toArea.y > a.fromMarkP.y
                && (fromArea.xMax > toArea.x || toArea.y - a.fromMarkP.y > toArea.center.x - a.fromMarkP.x)
                && ToAnchorCanBeTop(process, fromNode, fromArea, toNode, toArea);
            a.toArrowP = a.toLineP = toIsTop
                ? new Vector2(toArea.center.x, toArea.y)
                : new Vector2(toArea.x, toArea.center.y);
            a.fromIsSide = !fromIsBottom;
            a.toIsSide = !toIsTop;
            // Set tangents
            bool isToBehindFrom = a.toArrowP.x < a.fromMarkP.x && a.toArrowP.y < fromArea.yMax;
            float dist = Vector2.Distance(a.toArrowP, a.fromLineP);
            a.isStraight = connectionOptions.connectorMode == ConnectorMode.Straight
                              || !isToBehindFrom && connectionOptions.connectorMode == ConnectorMode.Smart && dist <= _MaxDistanceForSmartStraight;
            if (a.isStraight) {
                a.fromTangent = a.fromLineP;
                a.toTangent = a.toArrowP;
            } else {
                if (toIsTop) a.toLineP.y -= DeStylePalette.ico_nodeArrow.width;
                else a.toLineP.x -= DeStylePalette.ico_nodeArrow.width;
                float axisDistance = a.fromIsSide ? Mathf.Abs(a.toArrowP.x - a.fromLineP.x) : Mathf.Abs(a.toArrowP.y - a.fromLineP.y);
                float tangentDistance = isToBehindFrom ? _TangentDistanceIfInverse : Mathf.Min(_TangentDistance, axisDistance * 0.2f + dist * 0.2f);
                a.fromTangent = a.fromLineP + (fromIsBottom ? Vector2.up * tangentDistance : Vector2.right * tangentDistance);
                a.toTangent = a.toLineP + (toIsTop? Vector2.up * -tangentDistance : Vector2.right * -tangentDistance);
            }
            // Set arrow
            if (a.isStraight) {
                a.arrowRequiresRotation = true;
                a.arrowRotationAngle = -AngleBetween(Vector2.right, a.toArrowP - a.fromLineP);
            } else if (toIsTop) {
                a.arrowRequiresRotation = true;
                a.arrowRotationAngle = 90;
            }
            return a;
        }

        static bool FromAnchorCanBeVertical(NodeProcess process, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea)
        {
            bool isBottom = fromArea.center.y <= toArea.y;
            foreach (IEditorGUINode node in process.nodes) {
                if (node == fromNode || node == toNode) continue;
                Rect r = process.nodeToGUIData[node].fullArea;
                if (isBottom) {
                    if (r.y > toArea.center.y || r.yMax < fromArea.yMax || r.x > fromArea.center.x || r.xMax < fromArea.center.x) continue;
                } else {
                    if (r.yMax < toArea.center.y || r.y > fromArea.y || r.xMax < fromArea.center.x || r.x > fromArea.center.x) continue;
                }
                return false;
            }
            return true;
        }
        static bool ToAnchorCanBeVertical(NodeProcess process, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea)
        {
            bool isTop = fromArea.center.y <= toArea.y;
            foreach (IEditorGUINode node in process.nodes) {
                if (node == fromNode || node == toNode) continue;
                Rect r = process.nodeToGUIData[node].fullArea;
                if (isTop) {
                    if (r.y > toArea.y || r.yMax < fromArea.center.y || r.x > toArea.center.x || r.xMax < toArea.x) continue;
                } else {
                    if (r.yMax < toArea.y || r.y > fromArea.center.y || r.xMax < toArea.center.x || r.x > toArea.x) continue;
                }
                return false;
            }
            return true;
        }

        static bool FromAnchorCanBeBottom(NodeProcess process, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea)
        {
            if (toArea.xMax <= fromArea.center.x) return true;
            foreach (IEditorGUINode node in process.nodes) {
                if (node == fromNode || node == toNode) continue;
                Rect r = process.nodeToGUIData[node].fullArea;
                if (r.y > toArea.center.y || r.yMax < fromArea.yMax || r.x > fromArea.center.x || r.xMax < fromArea.center.x) continue;
                return false;
            }
            return true;
        }
        static bool ToAnchorCanBeTop(NodeProcess process, IEditorGUINode fromNode, Rect fromArea, IEditorGUINode toNode, Rect toArea)
        {
            if (toArea.x < fromArea.x) return true;
            foreach (IEditorGUINode node in process.nodes) {
                if (node == fromNode || node == toNode) continue;
                Rect r = process.nodeToGUIData[node].fullArea;
                if (r.y > toArea.y || r.yMax < fromArea.center.y || r.x > toArea.center.x || r.xMax < toArea.x) continue;
                return false;
            }
            return true;
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

        static Color GetConnectionColor(int connectionIndex, int totConnections, NodeGUIData nodeGuiData, NodeConnectionOptions connectionOptions)
        {
            return totConnections < 2 || connectionOptions.gradientColor == null
                ? connectionOptions.startColor == Color.clear ? nodeGuiData.mainColor : connectionOptions.startColor
                : connectionOptions.gradientColor.Evaluate(connectionIndex / (float)(totConnections - 1));
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
            public Vector2 fromMarkP, fromLineP, toArrowP, toLineP;
            public Vector2 fromTangent, toTangent;
            public ConnectionSide fromSide, toSide;
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