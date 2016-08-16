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