// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/12 11:39
// License Copyright (c) Daniele Giardini

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib
{
    public interface IEditorGUINode
    {
        /// <summary>Must be univocal</summary>
        string id { get; set; }
        Vector2 guiPosition { get; set; }
    }
}