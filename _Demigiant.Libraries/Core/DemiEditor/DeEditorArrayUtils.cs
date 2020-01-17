// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/01/17

using System;

namespace DG.DemiEditor
{
    public static class DeEditorArrayUtils
    {
        /// <summary>
        /// Expands the given array and adds the given element as the last one
        /// </summary>
        public static void ExpandAndAdd<T>(ref T[] array, T element)
        {
            int len = array.Length;
            Array.Resize(ref array, len + 1);
            array[len] = element;
        }

        /// <summary>
        /// Removes the element at index from the given array, shifts everything after by -1 position and resizes the array
        /// </summary>
        public static void RemoveAtIndexAndContract<T>(ref T[] array, int index)
        {
            int len = array.Length;
            for (int i = index + 1; i < len; ++i) array[i - 1] = array[i];
            Array.Resize(ref array, len - 1);
        }
    }
}