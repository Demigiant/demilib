// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 12:09
// License Copyright (c) Daniele Giardini

using UnityEditor.Callbacks;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    public class DeHierarchyPostProcessScene
    {
        // Removes any DeHierarchyComponent in the scene before building
        [PostProcessScene]
        public static void OnPostProcessScene()
        {
            Debug.Log("OnPostProcessScene");
        }
    }
}