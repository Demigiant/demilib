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
        #region Editor-Only

        [SerializeField] Vector2 _guiPosition;
        public Vector2 guiPosition { get { return _guiPosition; } set { _guiPosition = value; } }
        
        #endregion
    }
}