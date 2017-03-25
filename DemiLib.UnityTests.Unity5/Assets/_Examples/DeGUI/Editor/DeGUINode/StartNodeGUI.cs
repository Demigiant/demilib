// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 12:41
// License Copyright (c) Daniele Giardini

using DG.DeExtensions;
using DG.DemiEditor;
using DG.DemiEditor.DeGUINodeSystem;
using DG.DemiLib;
using UnityEngine;

namespace _Examples.DeGUI.Editor.DeGUINode
{
    public class StartNodeGUI : ABSDeGUINode
    {
        protected override NodeGUIData GetAreas(Vector2 position, IEditorGUINode iNode)
        {
            Rect fullR = new Rect(position.x, position.y, DeStylePalette.ico_play.width, DeStylePalette.ico_play.height);
            return new NodeGUIData(fullR, fullR);
        }

        protected override void OnGUI(NodeGUIData nodeGuiData, IEditorGUINode iNode)
        {
            GUI.DrawTexture(nodeGuiData.fullArea, DeStylePalette.ico_play);
        }
    }
}