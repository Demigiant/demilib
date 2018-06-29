// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/06/29 10:17
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code><para/>
    /// Shows the GUID with as a clickable button that pings the target object (if it exist), only works with string fields
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeObjectGuidAttribute : PropertyAttribute
    {
        internal bool showObjectName;

        public DeObjectGuidAttribute(bool showObjectName = false)
        {
            this.showObjectName = showObjectName;
        }
    }
}