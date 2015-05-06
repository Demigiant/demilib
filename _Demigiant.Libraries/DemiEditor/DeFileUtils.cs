// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/05/01 01:50

using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// File utils
    /// </summary>
    public static class DeFileUtils
    {
        /// <summary>Path slash for AssetDatabase format</summary>
        public static readonly string ADBPathSlash = "/";
        /// <summary>Path slash to replace for AssetDatabase format</summary>
        public static readonly string ADBPathSlashToReplace = "\\";
        /// <summary>Current OS path slash</summary>
        public static readonly string PathSlash;
        /// <summary>Path slash to replace on current OS</summary>
        public static readonly string PathSlashToReplace;

        static DeFileUtils()
        {
            bool useWindowsSlashes = Application.platform == RuntimePlatform.WindowsEditor;
            PathSlash = useWindowsSlashes ? "\\" : "/";
            PathSlashToReplace = useWindowsSlashes ? "/" : "\\";
        }
    }
}