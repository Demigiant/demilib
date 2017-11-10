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
        internal NodeProcess process; // Set by NodeProcess when instantiating this
        
        #region Internal Methods

        /// <summary>Used to fill <see cref="NodeGUIData"/></summary>
        protected internal abstract NodeGUIData GetAreas(Vector2 position, IEditorGUINode iNode);

        /// <summary>Called when the node needs to be drawn</summary>
        protected internal abstract void OnGUI(NodeGUIData nodeGuiData, IEditorGUINode iNode);

        #endregion
    }
}