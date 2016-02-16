// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/16 18:36
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Prefab utilities
    /// </summary>
    public static class DeEditorPrefabUtils
    {
        /// <summary>
        /// Behaves as the Inspector's Apply button, applying any modification of this instance to the prefab parent
        /// </summary>
        /// <param name="instance"></param>
        public static void ApplyPrefabInstanceModifications(GameObject instance)
        {
            PrefabUtility.ReplacePrefab(instance, PrefabUtility.GetPrefabParent(instance), ReplacePrefabOptions.ConnectToPrefab);
        }
    }
}