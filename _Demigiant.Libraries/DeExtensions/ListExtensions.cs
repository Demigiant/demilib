// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/16 20:26
// License Copyright (c) Daniele Giardini

using System.Collections;
using System.Collections.Generic;

namespace DG.DeExtensions
{
    /// <summary>
    /// List, IList, Array and Dictionary utils
    /// </summary>
    public static class ListExtensions
    {
        #region IList

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

        #endregion

        #region Dictionary

        /// <summary>
        /// Adds a key/value pair to the dictionary and returns its value
        /// </summary>
        public static T2 AddAndReturnValue<T1, T2>(this Dictionary<T1, T2> dictionary, T1 key, T2 value)
        {
            dictionary.Add(key, value);
            return value;
        }

        #endregion
    }
}