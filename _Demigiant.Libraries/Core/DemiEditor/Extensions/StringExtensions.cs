// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/18 21:18
// License Copyright (c) Daniele Giardini

using System;
using System.Globalization;
using System.Reflection;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DemiEditor
{
    /// <summary>
    /// String extensions
    /// </summary>
    public static class StringExtensions
    {
        static readonly StringBuilder _Strb = new StringBuilder();

        #region Public Methods

        /// <summary>
        /// Returns TRUE if the string is null or empty
        /// </summary>
        /// <param name="trimSpaces">If TRUE (default) and the string contains only spaces, considers it empty</param>
        public static bool IsNullOrEmpty(this string s, bool trimSpaces = true)
        {
            if (s == null) return true;
            return trimSpaces ? s.Trim().Length == 0 : s.Length == 0;
        }

        /// <summary>
        /// Compares a version string (in format #.#.###) with another of the same format,
        /// and return TRUE if this one is minor. Boths trings must have the same number of dot separators.
        /// </summary>
        public static bool VersionIsMinorThan(this string s, string version)
        {
            string[] thisV = s.Split('.');
            string[] otherV = version.Split('.');
            if (thisV.Length != otherV.Length) throw new ArgumentException("Invalid");
            for (int i = 0; i < thisV.Length; ++i) {
                int thisInt = Convert.ToInt32(thisV[i]);
                int otherInt = Convert.ToInt32(otherV[i]);
                if (i == thisV.Length - 1) return thisInt < otherInt;
                else if (thisInt == otherInt) continue;
                else if (thisInt < otherInt) return true;
                else if (thisInt > otherInt) return false;
            }
            throw new ArgumentException("Invalid");
        }

        /// <summary>
        /// Converts a HEX color to a Unity Color and returns it
        /// </summary>
        /// <param name="hex">The HEX color, either with or without the initial # (accepts both regular and short format)</param>
        public static Color HexToColor(this string hex)
        {
            if (hex[0] == '#') hex = hex.Substring(1);
            int len = hex.Length;
            bool isShortFormat = len < 6;
            if (isShortFormat) {
                float r = (HexToInt(hex[0]) + HexToInt(hex[0]) * 16f) / 255f;
                float g = (HexToInt(hex[1]) + HexToInt(hex[1]) * 16f) / 255f;
                float b = (HexToInt(hex[2]) + HexToInt(hex[2]) * 16f) / 255f;
                float a = len == 4 ? (HexToInt(hex[3]) + HexToInt(hex[3]) * 16f) / 255f : 1;
                return new Color(r, g, b, a);
            } else {
                float r = (HexToInt(hex[1]) + HexToInt(hex[0]) * 16f) / 255f;
                float g = (HexToInt(hex[3]) + HexToInt(hex[2]) * 16f) / 255f;
                float b = (HexToInt(hex[5]) + HexToInt(hex[4]) * 16f) / 255f;
                float a = len == 8 ? (HexToInt(hex[7]) + HexToInt(hex[6]) * 16f) / 255f : 1;
                return new Color(r, g, b, a);
            }
        }

        /// <summary>
        /// Nicifies a string, replacing underscores with spaces, and adding a space before Uppercase letters (except the first character)
        /// </summary>
        public static string Nicify(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            _Strb.Length = 0;
            _Strb.Append(s[0].ToString().ToUpper());
            for (int i = 1; i < s.Length; i++) {
                char curr = s[i];
                char prev = s[i - 1];
                if (curr == '_') _Strb.Append(' '); // Replace underscores with spaces
                else {
                    // Add spaces before numbers and uppercase letters
                    if (curr != ' ' && (char.IsUpper(curr) && (prev != ' ' && prev != '_') || char.IsNumber(curr) && prev != ' ' && prev != '_' && !char.IsNumber(prev))) _Strb.Append(' ');
                    _Strb.Append(curr);
                }
            }
            return _Strb.ToString();
        }

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

        #endregion

        #region Helpers

        static int HexToInt(char hexVal)
        {
            return int.Parse(hexVal.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

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