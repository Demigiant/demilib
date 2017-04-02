// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:39
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib
{
    public interface IEditorGUINode
    {
        /// <summary>Must be univocal</summary>
        string id { get; set; }
        /// <summary>Node position in editor GUI</summary>
        Vector2 guiPosition { get; set; }
        /// <summary>Ids of all forward connected nodes. Length indicates how many forward connections are allowed.</summary>
        List<string> connectedNodesIds { get; set; }
    }
}