// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/22 12:56
// License Copyright (c) Daniele Giardini

using DG.DeExtensions;
using DG.DemiEditor;
using DG.DemiEditor.DeGUINodeSystem;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using _Examples.DeGUI.DeGUINode;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class GenericNodeGUI : ABSDeGUINode
    {
        int _Width = 180;
        int _LineH = 16;
        int _LineGap = 1;
        static readonly Styles _Styles = new Styles();

        protected override NodeGUIData GetAreas(Vector2 position, IEditorGUINode iNode)
        {
            GenericNode node = (GenericNode)iNode;
            Rect headerR = new Rect(position.x, position.y, _Width, 20);
            Rect fullR = new Rect(position.x, position.y, _Width, headerR.height + _LineGap + (_LineH + _LineGap) * 5);
            return new NodeGUIData(fullR, headerR, node.colorValue);
        }

        protected override void OnGUI(NodeGUIData nodeGuiData, IEditorGUINode iNode)
        {
            _Styles.Init();
            GenericNode node = (GenericNode)iNode;

            // Background
            DG.DemiEditor.DeGUI.DrawColoredSquare(nodeGuiData.fullArea, node.colorValue);
            // Header
            DG.DemiEditor.DeGUI.DrawColoredSquare(nodeGuiData.dragArea, new Color(0, 0, 0, 0.3f));
            GUI.Label(
                nodeGuiData.dragArea, node.dualConnectionMode
                    ? node.id + " Dual"
                    : node.flexibleConnectionMode ? string.Format("{0} Flexible ({1})", node.id, node.connectedNodesIds.Count)
                    : node.id.ToString(),
                _Styles.headerLabelStyle
            );

            // Content
            Rect r = nodeGuiData.fullArea.Shift(2, nodeGuiData.dragArea.height + _LineGap, -4, -nodeGuiData.dragArea.height - _LineGap).SetHeight(_LineH);
            node.colorValue = EditorGUI.ColorField(r, node.colorValue);
            r = r.Shift(0, _LineH + _LineGap, 0, 0);
            node.stringValue = EditorGUI.TextField(r, node.stringValue);
            r = r.Shift(0, _LineH + _LineGap, 0, 0);
            node.boolValue = DG.DemiEditor.DeGUI.ToggleButton(r, node.boolValue, "Toggle");
            r = r.Shift(0, _LineH + _LineGap, 0, 0);
            node.floatValue = EditorGUI.Slider(r, node.floatValue, 0, 10);
            r = r.Shift(0, _LineH + _LineGap, 0, 0);
            node.enumValue = (GenericNode.SampleEnum)EditorGUI.EnumPopup(r, node.enumValue);

            // Outline
//            GUI.Box(nodeGuiData.fullArea.Expand(1), "", DG.DemiEditor.DeGUI.styles.box.outline01);
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