// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/04 15:08
// License Copyright (c) Daniele Giardini

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
    }
}