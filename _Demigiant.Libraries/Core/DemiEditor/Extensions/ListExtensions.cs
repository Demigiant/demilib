// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/16 20:26
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;

namespace DG.DemiEditor
{
    /// <summary>
    /// Replicates parts of DeExtensions.ListExtensions for internal usage
    /// </summary>
    public static class ListExtensions
    {
        static Random _rng;

        #region Public Methods

        /// <summary>
        /// Shifts an item from an index to another, without modifying the list except than by moving elements around
        /// </summary>
        public static void Shift<T>(this IList<T> list, int fromIndex, int toIndex)
        {
            if (toIndex == fromIndex) return;
            int index = fromIndex;
            T shifted = list[fromIndex];
            while (index > toIndex) {
                index--;
                list[index + 1] = list[index];
                list[index] = shifted;
            }
            while (index < toIndex) {
                index++;
                list[index - 1] = list[index];
                list[index] = shifted;
            }
        }

        /// <summary>
        /// Shuffles the list
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            if (_rng == null) _rng = new Random();
            int len = list.Count;
            while (len > 1) {
                len--;
                int k = _rng.Next(len + 1);
                T value = list[k];
                list[k] = list[len];
                list[len] = value;
            }
        }

        #endregion
    }
}