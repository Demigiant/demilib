// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/02/26 18:23
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code><para/>
    /// Shows a popup with available layers in the Inspector. Requires a valye of type <see cref="int"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeLayerAttribute : PropertyAttribute {}
}