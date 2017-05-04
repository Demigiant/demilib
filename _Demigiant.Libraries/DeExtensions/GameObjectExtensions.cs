// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/04 15:08
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeExtensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Activates then immediately deactivates the target gameObject.
        /// Useful when wanting to call Awake before deactivating a gameObject.
        /// </summary>
        /// <param name="go"></param>
        public static void AwakeAndDeactivate(this GameObject go)
        {
            go.SetActive(true);
            go.SetActive(false);
        }

        /// <summary>Returns TRUE if the gameObject is a child of an object with the given Component</summary>
        public static bool IsChildOfComponent<T>(this GameObject go) where T : Component
        { return DoHasOrIsChildOfComponent<T>(go, false); }
        /// <summary>Returns TRUE if the gameObject has or is a child of an object with the given Component</summary>
        public static bool HasOrIsChildOfComponent<T>(this GameObject go) where T : Component
        { return DoHasOrIsChildOfComponent<T>(go, true); }
        static bool DoHasOrIsChildOfComponent<T>(GameObject go, bool includeSelf = false) where T : Component
        {
            if (includeSelf && go.GetComponent<T>()) return true;
            Transform t = go.transform;
            while (t.parent != null) {
                t = t.parent;
                if (t.GetComponent<T>()) return true;
            }
            return false;
        }

        /// <summary>Returns TRUE if the gameObject is a child of the given tag</summary>
        public static bool IsChildOfTag(this GameObject go, string tag)
        { return DoIsChildOfTag(go, tag, false); }
        /// <summary>Returns TRUE if the gameObject has or is a child of the given tag</summary>
        public static bool HasOrIsChildOfTag(this GameObject go, string tag)
        { return DoIsChildOfTag(go, tag, true); }
        static bool DoIsChildOfTag(GameObject go, string tag, bool includeSelf = false)
        {
            if (includeSelf && go.tag == tag) return true;
            Transform t = go.transform;
            while (t.parent != null) {
                t = t.parent;
                if (t.tag == tag) return true;
            }
            return false;
        }

        /// <summary>
        /// Returns ONLY the Components in the children, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also includes inactive children</param>
        public static T[] GetOnlyComponentsInChildren<T>(this GameObject go, bool includeInactive = false) where T : Component
        {
            T[] components = go.GetComponentsInChildren<T>(includeInactive);
            int len = components.Length;
            if (len == 0) return components;

            T thisT = go.GetComponent<T>();
            if (thisT == null) return components;

            T lastT = components[components.Length - 1];
            len--;
            Array.Resize(ref components, len);
            bool requiresShifting = false;
            for (int i = 0; i < len; ++i) {
                T f = components[i];
                if (f == thisT) requiresShifting = true;
                if (requiresShifting) {
                    if (i < len - 1) components[i] = components[i + 1];
                    else components[i] = lastT;
                }
            }
            return components;
        }

        /// <summary>
        /// Returns the Component only if it's in a child, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also searches inactive children</param>
        public static T GetOnlyComponentInChildren<T>(this GameObject go, bool includeInactive = false) where T : Component
        {
            T component = go.GetComponentInChildren<T>(includeInactive);
            if (component.transform == go.transform) return null;
            return component;
        }

        /// <summary>
        /// Finds the component in the given MonoBehaviour or its parents, with options to choose if ignoring inactive objects or not
        /// </summary>
        /// <param name="includeInactive">If TRUE also searches inactive parents</param>
        public static T GetComponentInParentExtended<T>(this GameObject go, bool includeInactive = false) where T : Component
        {
            T result = null;
            Transform target = go.transform;
            while (target.parent != null && result == null) {
                if (includeInactive || target.gameObject.activeInHierarchy) result = target.gameObject.GetComponent<T>();
                target = target.parent;
            }
            return result;
        }
    }
}