// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/23 19:32
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;

namespace DG.DemiEditor.DeGUINodeSystem.Core
{
    /// <summary>
    /// Stores cloned nodes for pasting
    /// </summary>
    internal class NodesClipboard
    {
        public readonly List<IEditorGUINode> nodes = new List<IEditorGUINode>();
        public readonly Dictionary<IEditorGUINode,string> nodeToOriginalId = new Dictionary<IEditorGUINode, string>(); // Node to id before cloning

        #region Public Methods

        public void Clear()
        {
            nodes.Clear();
            nodeToOriginalId.Clear();
        }

        public void Add(IEditorGUINode node, string originalId)
        {
            nodes.Add(node);
            nodeToOriginalId.Add(node, originalId);
        }

        public IEditorGUINode GetClipboardCloneByOriginalId(string id)
        {
            foreach (IEditorGUINode node in nodes) {
                if (nodeToOriginalId[node] == id) return node;
            }
            return null;
        }

        #endregion
    }
}