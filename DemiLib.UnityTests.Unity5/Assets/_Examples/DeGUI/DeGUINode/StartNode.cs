// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:54
// License Copyright (c) Daniele Giardini

using System;
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
        public string id { get { return _id; } set { _id = value; } }
        public Vector2 guiPosition { get { return _guiPosition; } set { _guiPosition = value; } }
        
        #endregion
    }
}