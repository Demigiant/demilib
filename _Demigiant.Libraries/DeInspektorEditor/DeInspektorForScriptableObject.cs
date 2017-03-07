// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/03/07 19:13
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor
{
    /// <summary>
    /// Inherits from DeInspektor to allow everything to work on ScriptableObjects too
    /// </summary>
    [CustomEditor(typeof(ScriptableObject), true, isFallback = true)] [CanEditMultipleObjects]
    public class DeInspektorForScriptableObject : DeInspektor {}
}