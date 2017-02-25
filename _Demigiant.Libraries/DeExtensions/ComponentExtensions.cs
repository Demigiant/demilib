// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/13 13:39
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeExtensions
{
    public static class ComponentExtensions
    {
        /// <summary>
        /// Returns ONLY the Components in the children, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also includes inactive children</param>
        public static T[] GetOnlyComponentsInChildren<T>(this MonoBehaviour m, bool includeInactive = false) where T : Component
        {
            T[] components = m.GetComponentsInChildren<T>(includeInactive);
            int len = components.Length;
            if (len == 0) return components;

            T thisT = m.GetComponent<T>();
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
        public static T GetOnlyComponentInChildren<T>(this MonoBehaviour m, bool includeInactive = false) where T : Component
        {
            T component = m.GetComponentInChildren<T>(includeInactive);
            if (component.transform == m.transform) return null;
            return component;
        }
    }
}