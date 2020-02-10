// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/01/27

using UnityEngine;

namespace DG.DemiEditor
{
    public static class GUIContentExtensions
    {
        #region Public Methods

        public static bool HasText(this GUIContent content)
        {
            return content != null && !string.IsNullOrEmpty(content.text);
        }

        #endregion
    }
}