// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/02/08

using UnityEngine;
using UnityEngine.Events;

namespace DG.DemiEditor
{
    public static class UnityEventExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns a clone of the event
        /// </summary>
        public static UnityEvent Clone(this UnityEvent unityEvent)
        {
            return DeEditorReflectionUtils.DeepCopy(unityEvent);
        }

        #endregion
    }
}