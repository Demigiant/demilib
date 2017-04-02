// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/01 13:59
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    internal static class Connector
    {
        public static readonly DragData dragData = new DragData();

        #region Public Methods

        public static void Connect(
            int connectionIndex, int fromTotConnections,
            IEditorGUINode fromNode, NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, 
            IEditorGUINode toNode, NodeGUIData toGUIData, NodeConnectionOptions toOptions
        ) {
            // TODO Draw connector between two nodes
            Vector2 from = EvaluateFromAttachment(fromGUIData, fromOptions, toGUIData, toOptions);
            Vector2 to = EvaluateToAttachment(from, toGUIData, toOptions);
        }

        public static void Drag(IEditorGUINode fromNode, NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, Vector2 mousePosition)
        {
            dragData.Set(fromNode);
            Vector2 attachP = GetDragAttachPoint(fromGUIData, fromOptions, mousePosition);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.white, null, 5);
            Handles.DrawBezier(attachP, mousePosition, attachP, mousePosition, Color.black, null, 3);
        }

        #endregion

        #region Helpers

        static Vector2 EvaluateFromAttachment(NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, NodeGUIData toGUIData, NodeConnectionOptions toOptions)
        {
            switch (fromOptions.attachFromMode) {
            case ConnectorAttachMode.AnySideCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.BottomOrRightCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.TopOrLeftCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.AnyCorner:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.TopCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.LeftCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.RightCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.BottomCorners:
                // TODO GetAttachments
                break;
            //
//            case ConnectorAttachMode.TopLeft:
//                return fromGUIData.fullArea.min;
//            case ConnectorAttachMode.TopCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.min.y);
//            case ConnectorAttachMode.TopRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.min.y);
//            case ConnectorAttachMode.MiddleLeft:
//                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.MiddleCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.MiddleRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.BottomLeft:
//                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.max.y);
//            case ConnectorAttachMode.BottomCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.max.y);
//            case ConnectorAttachMode.BottomRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.max.y);
            }
            throw new System.NotImplementedException();
        }

        static Vector2 EvaluateToAttachment(Vector2 fromPosition, NodeGUIData toGUIData, NodeConnectionOptions toOptions)
        {
            switch (toOptions.attachToMode) {
            case ConnectorAttachMode.AnySideCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.BottomOrRightCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.TopOrLeftCenter:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.AnyCorner:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.TopCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.LeftCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.RightCorners:
                // TODO GetAttachments
                break;
            case ConnectorAttachMode.BottomCorners:
                // TODO GetAttachments
                break;
            //
//            case ConnectorAttachMode.TopLeft:
//                return fromGUIData.fullArea.min;
//            case ConnectorAttachMode.TopCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.min.y);
//            case ConnectorAttachMode.TopRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.min.y);
//            case ConnectorAttachMode.MiddleLeft:
//                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.MiddleCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.MiddleRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.center.y);
//            case ConnectorAttachMode.BottomLeft:
//                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.max.y);
//            case ConnectorAttachMode.BottomCenter:
//                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.max.y);
//            case ConnectorAttachMode.BottomRight:
//                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.max.y);
            }
            throw new System.NotImplementedException();
        }

        static Vector2 GetDragAttachPoint(NodeGUIData fromGUIData, NodeConnectionOptions fromOptions, Vector2 mousePosition)
        {
            switch (fromOptions.attachFromMode) {
            case ConnectorAttachMode.AnySideCenter:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.BottomOrRightCenter:
                bool isRight = mousePosition.y < fromGUIData.fullArea.y
                               || mousePosition.x - fromGUIData.fullArea.xMax >= mousePosition.y - fromGUIData.fullArea.yMax;
                return isRight
                    ? new Vector2(fromGUIData.fullArea.xMax, fromGUIData.fullArea.center.y)
                    : new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.yMax);
            case ConnectorAttachMode.TopOrLeftCenter:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.AnyCorner:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.TopCorners:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.LeftCorners:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.RightCorners:
                // TODO GetDragAttachPoint
                break;
            case ConnectorAttachMode.BottomCorners:
                // TODO GetDragAttachPoint
                break;
            //
            case ConnectorAttachMode.TopLeft:
                return fromGUIData.fullArea.min;
            case ConnectorAttachMode.TopCenter:
                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.min.y);
            case ConnectorAttachMode.TopRight:
                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.min.y);
            case ConnectorAttachMode.MiddleLeft:
                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.center.y);
            case ConnectorAttachMode.MiddleCenter:
                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.center.y);
            case ConnectorAttachMode.MiddleRight:
                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.center.y);
            case ConnectorAttachMode.BottomLeft:
                return new Vector2(fromGUIData.fullArea.min.x, fromGUIData.fullArea.max.y);
            case ConnectorAttachMode.BottomCenter:
                return new Vector2(fromGUIData.fullArea.center.x, fromGUIData.fullArea.max.y);
            case ConnectorAttachMode.BottomRight:
                return new Vector2(fromGUIData.fullArea.max.x, fromGUIData.fullArea.max.y);
            }
            throw new System.NotImplementedException();
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
    }
}