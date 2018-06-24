// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/01/03 12:22
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utils to use he correct method based on Unity's version
    /// </summary>
    public static class DeEditorCompatibilityUtils
    {
        static MethodInfo _miEncodeToPng;
        static MethodInfo _miGetPrefabParent;

        #region Public Methods

        /// <summary>
        /// Encodes to PNG using reflection to use correct method depending if editor is version 2017 or earlier
        /// </summary>
        public static byte[] EncodeToPNG(Texture2D texture)
        {
            if (_miEncodeToPng == null) {
                _miEncodeToPng = typeof(Texture2D).GetMethod("EncodeToPNG", BindingFlags.Instance | BindingFlags.Public);
                if (_miEncodeToPng == null) {
                    _miEncodeToPng = Type.GetType("UnityEngine.ImageConversion, UnityEngine")
                        .GetMethod("EncodeToPNG", BindingFlags.Static | BindingFlags.Public);
                }
            }
            if (DeUnityEditorVersion.MajorVersion >= 2017) {
                object[] parms = new[] { texture };
                return _miEncodeToPng.Invoke(null, parms) as byte[];
            } else {
                return _miEncodeToPng.Invoke(texture, null) as byte[];
            }
        }

        /// <summary>
        /// Returns the prefab parent by using different code on Unity 2018 or later
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static Object GetPrefabParent(GameObject instance)
        {
            if (_miGetPrefabParent == null) {
                if (DeUnityEditorVersion.Version < 2017.2f) {
                    _miGetPrefabParent = typeof(PrefabUtility).GetMethod("GetPrefabParent", BindingFlags.Public | BindingFlags.Static);
                } else {
                    _miGetPrefabParent = typeof(PrefabUtility).GetMethod("GetCorrespondingObjectFromSource", BindingFlags.Public | BindingFlags.Static);
                }
            }
            return (Object)_miGetPrefabParent.Invoke(null, new[] {instance});
        }

        #endregion
    }
}