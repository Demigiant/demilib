// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/01/27

using UnityEngine;

namespace DG.DemiEditor.Internal
{
    internal static class GUIContentExtensions
    {
        public static bool HasText(this GUIContent content)
        {
            return content != null && !string.IsNullOrEmpty(content.text);
        }
    }
}