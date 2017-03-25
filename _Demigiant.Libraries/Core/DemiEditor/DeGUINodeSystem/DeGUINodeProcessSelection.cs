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
        public DeGUISelectionMode selectionMode { get; internal set; }
        public readonly List<IEditorGUINode> selectedNodes = new List<IEditorGUINode>();
        internal readonly List<IEditorGUINode> selectedNodesSnapshot = new List<IEditorGUINode>(); // Used by process to store snapshot before starting a new selection
        internal Rect selectionRect; // Updated by process when drawing selection via mouse

        #region Public Methods

        public bool IsSelected(IEditorGUINode node)
        {
            return selectedNodes.Contains(node);
        }

        public void Deselect(IEditorGUINode node)
        {
            selectedNodes.Remove(node);
        }

        /// <summary>
        /// Returns TRUE if something was actually deselected, FALSE if there were no selected nodes
        /// </summary>
        /// <returns></returns>
        public bool DeselectAll()
        {
            if (selectedNodes.Count == 0) return false;
            selectedNodes.Clear();
            return true;
        }

        public void Select(IEditorGUINode node, bool keepExistingSelections)
        {
            if (!keepExistingSelections) selectedNodes.Clear();
            if (!selectedNodes.Contains(node)) selectedNodes.Add(node);
        }

        public void Select(List<IEditorGUINode> nodes, bool keepExistingSelections)
        {
            if (!keepExistingSelections) selectedNodes.Clear();
            foreach (IEditorGUINode node in nodes) {
                if (!selectedNodes.Contains(node)) selectedNodes.Add(node);
            }
        }

        #endregion

        #region Internal Methods

        internal void ClearSnapshot()
        {
            selectedNodesSnapshot.Clear();
        }

        internal void StoreSnapshot()
        {
            selectedNodesSnapshot.Clear();
            foreach (IEditorGUINode node in selectedNodes) selectedNodesSnapshot.Add(node);
        }

        #endregion
    }
}