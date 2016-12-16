// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/12/11 20:41
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeUtils
{
    public static class DeLayerUtils
    {
        /// <summary>
        /// Returns the int value of the given LayerMask
        /// </summary>
        public static int LayerMaskToIndex(LayerMask mask)
        {
            int layerIndex = 0;
            int layer = mask.value;
            while(layer > 1) {
                layer = layer >> 1;
                layerIndex++;
            }
            return layerIndex;
        }
    }
}