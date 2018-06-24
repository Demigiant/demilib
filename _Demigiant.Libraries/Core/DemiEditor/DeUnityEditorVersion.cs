// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/23 21:05
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Util to determine Unity editor version and store them as comparable numbers
    /// </summary>
    public static class DeUnityEditorVersion
    {
        /// <summary>Full major version + first minor version (ex: 2018.1f)</summary>
        public static readonly float Version;
        /// <summary>Major version</summary>
        public static readonly int MajorVersion;
        /// <summary>First minor version (ex: in 2018.1 it would be 1)</summary>
        public static readonly int MinorVersion;

        static DeUnityEditorVersion()
        {
            string sVersion = Application.unityVersion;
            string sMajor, sMinor;
            int dotIndex = sVersion.IndexOf('.');
            if (dotIndex == -1) {
                MajorVersion = int.Parse(sVersion);
                Version = MajorVersion;
            } else {
                sMajor = sVersion.Substring(0, dotIndex);
                MajorVersion = int.Parse(sMajor);
                // Remove and ignore extra minor versions dots
                sMinor = sVersion.Substring(dotIndex + 1);
                dotIndex = sMinor.IndexOf('.');
                if (dotIndex != -1) sMinor = sMinor.Substring(0, dotIndex);
                MinorVersion = int.Parse(sMinor);
                Version = float.Parse(sMajor + '.' + sMinor);
            }
        }
    }
}