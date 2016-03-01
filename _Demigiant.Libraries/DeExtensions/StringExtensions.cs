// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/03/01 12:29
// License Copyright (c) Daniele Giardini
namespace DG.DeExtensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Returns TRUE if the string is null or empty
        /// </summary>
        /// <param name="trimSpaces">If TRUE (default) and the string contains only spaces, considers it empty</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s, bool trimSpaces = true)
        {
            if (s == null) return true;
            return trimSpaces ? s.Trim().Length == 0 : s.Length == 0;
        }
    }
}