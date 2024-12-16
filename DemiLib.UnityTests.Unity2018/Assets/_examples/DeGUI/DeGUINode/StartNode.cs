// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:54
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace _Examples.DeGUI.DeGUINode
{
    [Serializable]
    public class StartNode : IEditorGUINode
    {
        #region IEditorGUINode

        [SerializeField] string _id;
        [SerializeField] Vector2 _guiPosition;
        [SerializeField] List<string> _connectedNodesIds = new List<string>() { null };
        public string id { get { return _id; } set { _id = value; } }
        public Vector2 guiPosition { get { return _guiPosition; } set { _guiPosition = value; } }
        public List<string> connectedNodesIds { get { return _connectedNodesIds; } set { _connectedNodesIds = value; } }
        
        #endregion
    }
}