// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/13 20:31
// License Copyright (c) Daniele Giardini

using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    public class DeGUINodeProcessScope : DeScope
    {
        DeGUINodeProcess _process;

        /// <summary>
        /// Use this to encapsulate node GUI operations.<para/>
        /// Automatically manages area drag operations with middle mouse and node drag operations with left mouse.<para/>
        /// Sets <code>GUI.changed</code> to TRUE if the area is panned or a node is dragged.
        /// </summary>
        /// <param name="process">The <see cref="DeGUINodeProcess"/> to use</param>
        /// <param name="nodeArea">Area within which the nodes will be drawn</param>
        /// <param name="refAreaShift">Area shift (caused by dragging)</param>
        public DeGUINodeProcessScope(DeGUINodeProcess process, Rect nodeArea, ref Vector2 refAreaShift)
        {
            _process = process;
            _process.BeginGUI(nodeArea, ref refAreaShift);
        }

        // Used to call final operations when closing scope
        protected override void CloseScope()
        {
            _process.EndGUI();
            _process = null;
        }
    }
}