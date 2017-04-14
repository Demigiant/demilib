// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/13 20:31
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public class NodeProcessScope<T> : DeScope where T : IEditorGUINode
    {
        NodeProcess _process;

        /// <summary>
        /// Use this to encapsulate node GUI operations.<para/>
        /// Automatically manages area drag operations with middle mouse and node drag operations with left mouse.<para/>
        /// Sets <code>GUI.changed</code> to TRUE if the area is panned, a node is dragged, or sortableNodes changes sorting.<para/>
        /// Wraps all content inside a GUILayout.BeginArea(nodeArea).
        /// </summary>
        /// <param name="process">The <see cref="NodeProcess"/> to use</param>
        /// <param name="nodeArea">Area within which the nodes will be drawn</param>
        /// <param name="refAreaShift">Area shift (caused by dragging)</param>
        /// <param name="sortableNodes">This list will be sorted based on current node draw order</param>
        public NodeProcessScope(NodeProcess process, Rect nodeArea, ref Vector2 refAreaShift, IList<T> sortableNodes = null)
        {
            _process = process;
            _process.BeginGUI(nodeArea, ref refAreaShift, sortableNodes);
        }

        // Used to call final operations when closing scope
        protected override void CloseScope()
        {
            _process.EndGUI();
            _process = null;
        }
    }

    public class NodeProcessScope : DeScope
    {
        NodeProcess _process;

        /// <summary>
        /// Use this to encapsulate node GUI operations.<para/>
        /// Automatically manages area drag operations with middle mouse and node drag operations with left mouse.<para/>
        /// Sets <code>GUI.changed</code> to TRUE if the area is panned or a node is dragged.
        /// </summary>
        /// <param name="process">The <see cref="NodeProcess"/> to use</param>
        /// <param name="nodeArea">Area within which the nodes will be drawn</param>
        /// <param name="refAreaShift">Area shift (caused by dragging)</param>
        public NodeProcessScope(NodeProcess process, Rect nodeArea, ref Vector2 refAreaShift)
        {
            _process = process;
            _process.BeginGUI<IEditorGUINode>(nodeArea, ref refAreaShift);
        }

        // Used to call final operations when closing scope
        protected override void CloseScope()
        {
            _process.EndGUI();
            _process = null;
        }
    }
}