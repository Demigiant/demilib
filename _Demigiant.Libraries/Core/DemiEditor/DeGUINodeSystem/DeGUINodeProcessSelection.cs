// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/23 11:20
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public class DeGUINodeProcessSelection
    {
        public readonly List<IEditorGUINode> selectedNodes = new List<IEditorGUINode>();

        #region Public Methods

        public bool IsSelected(IEditorGUINode node)
        {
            return selectedNodes.Contains(node);
        }

        public void Deselect(IEditorGUINode node)
        {
            selectedNodes.Remove(node);
        }

        public void DeselectAll()
        {
            selectedNodes.Clear();
        }

        public void Select(IEditorGUINode node, bool keepExistingSelections)
        {
            if (!keepExistingSelections) DeselectAll();
            selectedNodes.Add(node);
        }

        #endregion
    }
}