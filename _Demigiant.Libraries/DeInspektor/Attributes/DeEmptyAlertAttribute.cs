// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/23 13:06
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// <code>Property attribute</code>
    /// <para>Only for object reference fields: shows them red if empty</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeEmptyAlertAttribute : PropertyAttribute
    {
        
    }
}