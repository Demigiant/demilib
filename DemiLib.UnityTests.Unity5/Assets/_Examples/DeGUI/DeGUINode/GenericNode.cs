// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/22 12:53
// License Copyright (c) Daniele Giardini

using System;
using DG.DemiLib;
using UnityEngine;

namespace _Examples.DeGUI.DeGUINode
{
    [Serializable]
    public class GenericNode : IEditorGUINode
    {
        public int id;

        #region Editor-Only

        [SerializeField] Vector2 _guiPosition;
        public Vector2 guiPosition { get { return _guiPosition; } set { _guiPosition = value; } }
        
        #endregion
    }
}