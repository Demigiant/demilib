// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/05/01 01:52

using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Assembly extensions
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>AssetDatabase path to the assembly directory, without final slash</summary>
        public static string ADBDir(this Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string fullPath = Uri.UnescapeDataString(uri.Path);
            if (fullPath.Substring(fullPath.Length - 3) == "dll") {
                fullPath = Path.GetDirectoryName(fullPath);
            } else {
                // Regular method failed (happens in case of folders that start with # and are inside Documents)
                fullPath = Path.GetDirectoryName(assembly.Location);
            }
            string adbPath = fullPath.Substring(Application.dataPath.Length - 6); // Was +1, but excluded Assets folder (which is instead part of ADB path)
            return adbPath.Replace(DeEditorFileUtils.ADBPathSlashToReplace, DeEditorFileUtils.ADBPathSlash);
        }
    }
}