// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/16 20:26
// License Copyright (c) Daniele Giardini

using System.Collections;
using System.Collections.Generic;

namespace DG.DeExtensions
{
    /// <summary>
    /// List, IList and Array utils
    /// </summary>
    public static class ListExtensions
    {
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
    }
}