// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/06 10:55
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.De2D
{
    /// <summary>
    /// Allows to set the sortingOrder of Renderers, not just SpriteRenderers
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class DeRendererSortOrder : MonoBehaviour
    {
        public int fooSortingOrder = 0;

        public int sortingOrder {
            get { return fooSortingOrder; }
            set {
                fooSortingOrder = value;
                this.GetComponent<Renderer>().sortingOrder = value;
            }
        }
    }
}