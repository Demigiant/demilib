// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/13 13:39
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DeExtensions
{
    public static class ComponentExtensions
    {
        public enum CopyMode
        {
            /// <summary>Adds the component even if another component of the same type already exists on the target</summary>
            Add,
            /// <summary>Adds the component only if the same component doesn't already exist on the target</summary>
            AddIfMissing,
        }

        /// <summary>
        /// Copies a component with all its public and private dynamic fields, and adds it to the given target
        /// </summary>
        /// <param name="original">Component to copy</param>
        /// <param name="to">GameObject to add the component to</param>
        /// <param name="copyMode">Copy mode</param>
        /// <param name="removeOriginalComponent">If TRUE, removes the original components after copying it</param>
        public static T CopyTo<T>(this T original, GameObject to, CopyMode copyMode = CopyMode.AddIfMissing, bool removeOriginalComponent = false) where T : Component
        {
            Type type = original.GetType();
            Component copy = copyMode == CopyMode.AddIfMissing ? to.GetComponent<T>() : null;
            if (copy == null) copy = to.AddComponent(type);
            FieldInfo[] fInfos = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo fi in fInfos) fi.SetValue(copy, fi.GetValue(original));
            if (removeOriginalComponent) Object.Destroy(original);
            return copy as T;
        }

        /// <summary>
        /// Returns ONLY the Components in the children, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also includes inactive children</param>
        public static T[] GetOnlyComponentsInChildren<T>(this MonoBehaviour m, bool includeInactive = false) where T : Component
        {
            return m.gameObject.GetOnlyComponentsInChildren<T>(includeInactive);
        }

        /// <summary>
        /// Returns the Component only if it's in a child, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also searches inactive children</param>
        public static T GetOnlyComponentInChildren<T>(this MonoBehaviour m, bool includeInactive = false) where T : Component
        {
            return m.gameObject.GetOnlyComponentInChildren<T>(includeInactive);
        }

        /// <summary>
        /// Finds the component in the given MonoBehaviour or its parents, with options to choose if ignoring inactive objects or not
        /// </summary>
        /// <param name="includeInactive">If TRUE also searches inactive parents</param>
        public static T GetComponentInParentExtended<T>(this MonoBehaviour m, bool includeInactive = false) where T : Component
        {
            return m.gameObject.GetComponentInParentExtended<T>(includeInactive);
        }
    }
}