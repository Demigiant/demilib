// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/18 21:18
// License Copyright (c) Daniele Giardini
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
    }
}