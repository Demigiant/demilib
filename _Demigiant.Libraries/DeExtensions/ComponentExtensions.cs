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