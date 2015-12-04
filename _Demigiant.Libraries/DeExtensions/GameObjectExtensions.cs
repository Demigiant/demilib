// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/12/04 15:08
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Activates then immediately deactivates the target gameObject.
        /// Useful when wanting to call Awake before deactivating a gameObject.
        /// </summary>
        /// <param name="go"></param>
        public static void AwakeAndDeactivate(this GameObject go)
        {
            go.SetActive(true);
            go.SetActive(false);
        }
    }
}