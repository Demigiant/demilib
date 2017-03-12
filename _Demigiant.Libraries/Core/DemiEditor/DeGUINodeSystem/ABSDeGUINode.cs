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
        DeGUINodeProcess _process;

        #region CONSTRUCTOR

        // Must be implemented but is never used
        public ABSDeGUINode()
        {
            Debug.LogError("ABSDeGUINode parameterless constructor should never be called");
        }

        protected internal ABSDeGUINode(DeGUINodeProcess process)
        {
            _process = process;
        }

        #endregion
        
        #region Internal Methods

        protected internal abstract void Draw(Vector2 position, IEditorGUINode iNode, bool isDraggable);

        #endregion
    }
}