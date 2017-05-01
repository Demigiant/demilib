// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/03/01 12:29
// License Copyright (c) Daniele Giardini

using System;
using System.Text;
using UnityEngine;

namespace DG.DeExtensions
{
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
            _Strb.Append(s[0]);
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
        /// Converts the string to the given enum value.
        /// Throws an exception if the string can't be converted.
        /// If the enum value can't be found, returns the 0 indexed value.<para/>
        /// NOTE: doesn't use try/catch (TryParse) since on some platforms that won't work.
        /// </summary>
        public static T ToEnum<T>(this string s, T? defaultValue = null) where T : struct, IConvertible
        {
            Type tType = typeof(T);
            if (!tType.IsEnum) throw new ArgumentException("T must be of type Enum");

            if (Enum.IsDefined(tType, s)) return (T)Enum.Parse(tType, s);
            if (defaultValue == null) throw new ArgumentException("Value not found and defaultValue not set");
            return (T)defaultValue;
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

        #endregion
        
        #region Methods

        static int HexToInt(char hexVal)
        {
            return int.Parse(hexVal.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        #endregion
    }
}