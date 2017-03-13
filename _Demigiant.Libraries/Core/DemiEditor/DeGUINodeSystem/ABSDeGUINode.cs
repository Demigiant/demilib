// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 12:10
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// Abstract dynamic class used for every node of the same type
    /// (meaning there is only a single recycled instance for all same-type nodes)
    /// </summary>
    public abstract class ABSDeGUINode
    {
        protected DeGUINodeProcess process;

        #region CONSTRUCTOR

        // Must be implemented but is never used
        public ABSDeGUINode()
        {
            Debug.LogError("ABSDeGUINode parameterless constructor should never be called");
        }

        protected internal ABSDeGUINode(DeGUINodeProcess process)
        {
            this.process = process;
        }

        #endregion
        
        #region Internal Methods

        /// <summary>Used to fill <see cref="DeGUINodeData"/></summary>
        protected internal abstract DeGUINodeData GetAreas(Vector2 position, IEditorGUINode iNode);

        /// <summary>Called only if the node is visible in the current area</summary>
        protected internal abstract void OnGUI(DeGUINodeData guiNodeData, IEditorGUINode iNode);

        #endregion
    }
}