// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/18 21:18
// License Copyright (c) Daniele Giardini

using System;
using System.Globalization;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// If the given string is a directory path, returns its parent
        /// with or without final slash depending on the original directory format
        /// </summary>
        public static string Parent(this string dir)
        {
            if (dir.Length <= 1) return dir;
            string slashType = dir.IndexOf("/") == -1 ? "\\" : "/";
            int index = dir.LastIndexOf(slashType);
            if (index == -1) return dir; // Not a directory path
            if (index == dir.Length - 1) {
                // Had final slash
                index = dir.LastIndexOf(slashType, index - 1);
                if (index == -1) return dir;
                return dir.Substring(0, index + 1);
            }
            // No final slash
            return dir.Substring(0, index);
        }

        /// <summary>
        /// If the string is a directory, returns the directory name,
        /// if instead it's a file returns its name without extension.
        /// Works better than Path.GetDirectoryName, which kind of sucks imho
        /// </summary>
        public static string FileOrDirectoryName(this string path)
        {
            if (path.Length <= 1) return path;
            int slashIndex = path.LastIndexOfAnySlash();
            int dotIndex = path.LastIndexOf('.');
            if (dotIndex != -1 && dotIndex > slashIndex) path = path.Substring(0, dotIndex); // Remove extension if present
            if (slashIndex == -1) return path;
            if (slashIndex == path.Length - 1) {
                path = path.Substring(0, slashIndex); // Remove final slash
                slashIndex = path.LastIndexOfAnySlash();
                if (slashIndex == -1) return path;
            }
            return path.Substring(slashIndex + 1);
        }

        /// <summary>
        /// Evaluates the string as a property or field and returns its value.
        /// </summary>
        /// <param name="obj">If NULL considers the string as a static property, otherwise uses obj as the starting instance</param>
        public static T EvalAsProperty<T>(this string s, object obj = null, bool logErrors = false)
        {
            try {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                string[] split = s.Split('.');
                if (obj == null) {
                    // Static
                    string typeS = split[0];
                    for (int i = 1; i < split.Length - 1; ++i) typeS += '.' + split[i];
                    Type t = null;
                    for (int i = 0; i < assemblies.Length; ++i) {
                        t = assemblies[i].GetType(typeS);
                        if (t != null) break;
                    }
                    if (t == null) throw new NullReferenceException("Type " + typeS + " could not be found in any of the domain assemblies");
                    PropertyInfo pInfo = t.GetProperty(split[split.Length - 1]);
                    if (pInfo != null) {
                        return (T)pInfo.GetValue(
                            null, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                            null, null, CultureInfo.InvariantCulture
                        );
                    } else {
                        return (T)t.GetField(split[split.Length - 1]).GetValue(null);
                    }
                } else {
                    // TODO: needs testing for instance property/fields
                    foreach (string part in split) {
                        Type t = obj.GetType();
                        PropertyInfo pInfo = t.GetProperty(part);
                        if (pInfo != null) obj = pInfo.GetValue(obj, null);
                        else obj = t.GetField(part).GetValue(obj);
                    }
                    return (T)obj;
                }
            } catch (Exception e) {
                if (logErrors) Debug.LogError("EvalAsProperty error (" + e.Message + ")\n" + e.StackTrace);
                return default(T);
            }
        }

        #region Helpers

        // Returns the last index of any slash occurrence, either \ or /
        static int LastIndexOfAnySlash(this string str)
        {
            int index = str.LastIndexOf('/');
            if (index == -1) index = str.LastIndexOf('\\');
            return index;
        }

        #endregion
    }
}