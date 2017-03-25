// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/22 12:56
// License Copyright (c) Daniele Giardini

using DG.DemiEditor;
using DG.DemiEditor.DeGUINodeSystem;
using DG.DemiLib;
using UnityEngine;
using _Examples.DeGUI.DeGUINode;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class GenericNodeGUI : ABSDeGUINode
    {
        GUIStyle _labelStyle;

        protected override NodeGUIData GetAreas(Vector2 position, IEditorGUINode iNode)
        {
            Rect fullR = new Rect(position.x, position.y, 100, 70);
            return new NodeGUIData(fullR, fullR);
        }

        protected override void OnGUI(NodeGUIData nodeGuiData, IEditorGUINode iNode)
        {
            SetStyles();
            GenericNode node = (GenericNode)iNode;
            GUI.DrawTexture(nodeGuiData.fullArea, DeStylePalette.orangeSquare);
            GUI.Box(nodeGuiData.fullArea, "", DG.DemiEditor.DeGUI.styles.box.outline);
            GUI.Label(nodeGuiData.fullArea, node.id.ToString(), _labelStyle);
        }

        void SetStyles()
        {
            if (_labelStyle != null) return;
            _labelStyle = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleCenter, Color.white, 30);
        }
    }
}