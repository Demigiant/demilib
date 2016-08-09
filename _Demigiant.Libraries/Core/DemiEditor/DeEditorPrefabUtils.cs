// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/02/16 18:36
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
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

        /// <summary>
        /// Completely removes any prefab connection from the given prefab instances.
        /// <para>
        /// Based on RodGreen's method (http://forum.unity3d.com/threads/82883-Breaking-connection-from-gameObject-to-prefab-for-good.?p=726602&amp;viewfull=1#post726602)
        /// </para>
        /// </summary>
        public static void BreakPrefabInstances(List<GameObject> prefabInstances)
        { foreach (GameObject instance in prefabInstances) BreakPrefabInstance(instance); }
        /// <summary>
        /// Completely removes any prefab connection from the given prefab instance.
        /// <para>
        /// Based on RodGreen's method (http://forum.unity3d.com/threads/82883-Breaking-connection-from-gameObject-to-prefab-for-good.?p=726602&amp;viewfull=1#post726602)
        /// </para>
        /// </summary>
        public static void BreakPrefabInstance(GameObject prefabInstance)
        {
            string name = prefabInstance.name;
            Transform transform = prefabInstance.transform;
            Transform parent = transform.parent;
            // Unparent the GO so that world transforms are preserved.
            transform.parent = null;
            // Clone and re-assign.
            GameObject newInstance = (GameObject)Object.Instantiate(prefabInstance);
            newInstance.name = name;
            newInstance.SetActive(prefabInstance.activeSelf);
            newInstance.transform.parent = parent;
            // Remove old.
            Object.DestroyImmediate(prefabInstance, false);
        }
    }
}