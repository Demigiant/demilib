// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/16 17:52
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DemiLib.Attributes
{
    /// <summary>
    /// Begins a group that will be drawn inside a box GUIStyle.
    /// Must always be closed by a <see cref="DeEndGroupAttribute"/>.
    /// <para>NOTE: doesn't respect order as usual decorators do: the group will contain any other decorators added to the same field</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeBeginGroupAttribute : PropertyAttribute {}

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    /// <summary>
    /// Closes a box group
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeEndGroupAttribute : PropertyAttribute {}
}