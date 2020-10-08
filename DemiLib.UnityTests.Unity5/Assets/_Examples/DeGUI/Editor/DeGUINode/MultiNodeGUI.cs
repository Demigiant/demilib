// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/20 14:19
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiEditor.DeGUINodeSystem;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using _Examples.DeGUI.DeGUINode;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class MultiNodeGUI : ABSDeGUINode
    {
        int _Width = 180;
        const int _LineH = 20;
        static readonly Styles _Styles = new Styles();

        protected override NodeGUIData GetAreas(Vector2 position, IEditorGUINode iNode)
        {
            Rect headerR = new Rect(position.x, position.y, _Width, 20);
            Rect fullR = new Rect(position.x, position.y, _Width, headerR.height + _LineH * 3);
            List<Rect> connectorRs = new List<Rect>(3);
            for (int i = 0; i < 3; ++i) connectorRs.Add(new Rect(position.x, headerR.yMax + _LineH * i, _Width, _LineH));
            return new NodeGUIData(fullR, headerR) { connectorAreas = connectorRs };
        }

        protected override void OnGUI(NodeGUIData nodeGuiData, IEditorGUINode iNode)
        {
            _Styles.Init();
            GenericNode node = (GenericNode)iNode;

            // Background
            DG.DemiEditor.DeGUI.DrawColoredSquare(nodeGuiData.fullArea, DG.DemiEditor.DeGUI.colors.global.yellow);
            // Header
            DG.DemiEditor.DeGUI.DrawColoredSquare(nodeGuiData.dragArea, new Color(0, 0, 0, 0.3f));
            GUI.Label(
                nodeGuiData.dragArea, node.normalPlusConnectionMode
                    ? node.id + " NormalPlus"
                    : node.id.ToString(),
                _Styles.headerLabelStyle
            );

            // Lines
            for (int i = 0; i < 3; ++i) {
                Rect r = nodeGuiData.connectorAreas[i];
                EditorGUI.DelayedTextField(r.Contract(2), "Text");
            }
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Styles
        {
            public GUIStyle headerLabelStyle;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                headerLabelStyle = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleCenter, Color.white, FontStyle.Bold).StretchHeight();
            }
        }
    }
}