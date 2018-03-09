// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/09 15:34
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeAudio.Attributes
{
    /// <summary>
    /// <code>Property attribute</code> for <see cref="DeAudioClipData"/> elements<para/>
    /// Sets how it should be drawn in the editor
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class DeAudioClipDataModeAttribute : PropertyAttribute
    {
        public DeAudioClipGUIMode mode;
        public bool allowGroupChange;

        public DeAudioClipDataModeAttribute(DeAudioClipGUIMode mode, bool allowGroupChange)
        {
            this.mode = mode;
            this.allowGroupChange = allowGroupChange;
        }
    }
}