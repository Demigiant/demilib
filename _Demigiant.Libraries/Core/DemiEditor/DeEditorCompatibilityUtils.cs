// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/01/03 12:22
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Utils to use he correct method based on Unity's version
    /// </summary>
    public static class DeEditorCompatibilityUtils
    {
        static bool _is2017_orLater;
        static MethodInfo _miEncodeToPng;

        #region Public Methods

        /// <summary>
        /// Encodes to PNG using reflection to use correct method depending if editor is version 2017 or earlier
        /// </summary>
        public static byte[] EncodeToPNG(Texture2D texture)
        {
            if (_miEncodeToPng == null) {
                _miEncodeToPng = typeof(Texture2D).GetMethod("EncodeToPNG", BindingFlags.Instance | BindingFlags.Public);
                if (_miEncodeToPng == null) {
                    _is2017_orLater = true;
                    _miEncodeToPng = Type.GetType("UnityEngine.ImageConversion, UnityEngine")
                        .GetMethod("EncodeToPNG", BindingFlags.Static | BindingFlags.Public);
                }
            }
            if (_is2017_orLater) {
                object[] parms = new[] {texture};
                return _miEncodeToPng.Invoke(null, parms) as byte[];
            } else return _miEncodeToPng.Invoke(texture, null) as byte[];
        }

        #endregion
    }
}