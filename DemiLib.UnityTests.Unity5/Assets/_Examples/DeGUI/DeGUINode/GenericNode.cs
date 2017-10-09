// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/22 12:53
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace _Examples.DeGUI.DeGUINode
{
    public enum NodeType
    {
        Generic,
        Multi,
        AltConnection // Dual connection
    }

    [Serializable]
    public class GenericNode : IEditorGUINode
    {
        public enum SampleEnum {
            A, B, C, D
        }

        #region Serialized

        public NodeType type;
        public bool flexibleConnectionMode = false;
        public bool normalPlusConnectionMode = false;
        public bool dualConnectionMode = false;
        public bool boolValue;
        public string stringValue = "Sample Text";
        public SampleEnum enumValue = SampleEnum.B;
        public float floatValue = 3.5f;
        public Color colorValue = new Color(0.92f, 0.5f, 0.07f);
        public float[] sampleArray = new[] {1f, 2f, 3f};
        
        [SerializeField] string _id;
        [SerializeField] Vector2 _guiPosition;
        [SerializeField] List<string> _connectedNodesIds = new List<string>() { null };

        #endregion
        
        #region IEditorGUINode

        public string id { get { return _id; } set { _id = value; } }
        public Vector2 guiPosition { get { return _guiPosition; } set { _guiPosition = value; } }
        public List<string> connectedNodesIds { get { return _connectedNodesIds; } set { _connectedNodesIds = value; } }
        
        #endregion
    }
}