// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2022/03/15

using System;
using UnityEngine;

namespace DG.DemiLib.Utils
{
    public static class DeUtils
    {
        #region Public Methods

        /// <summary>
        /// Returns ONLY the Components in the children, and ignores the parent.
        /// </summary>
        /// <param name="includeInactive">If TRUE also includes inactive children</param>
        public static T[] GetOnlyComponentsInChildren<T>(GameObject go, bool includeInactive = false) where T : Component
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
        public static T GetOnlyComponentInChildren<T>(GameObject go, bool includeInactive = false) where T : Component
        {
            T component = go.GetComponentInChildren<T>(includeInactive);
            if (component.transform == go.transform) {
                T[] components = go.GetComponentsInChildren<T>(includeInactive);
                return components.Length <= 1 ? null : components[1];
            }
            return component;
        }

        /// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #
        /// </summary>
        /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(Color32 color, bool includeAlpha = false)
        {
            string result = color.r.ToString("x2") + color.g.ToString("x2") + color.b.ToString("x2");
            if (includeAlpha) result += color.a.ToString("x2");
            return result;
        }

        /// <summary>
        /// Returns a HEX version of the given Unity Color, without the initial #
        /// </summary>
        /// <param name="includeAlpha">If TRUE, also converts the alpha value and returns a hex of 8 characters,
        /// otherwise doesn't and returns a hex of 6 characters</param>
        public static string ToHex(Color color, bool includeAlpha = false)
        {
            return ToHex((Color32)color, includeAlpha);
        }

        #endregion
    }
}