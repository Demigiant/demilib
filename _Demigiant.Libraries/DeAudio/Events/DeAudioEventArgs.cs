// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/22 13:18
// License Copyright (c) Daniele Giardini

using System;

namespace DG.DeAudio.Events
{
    public enum DeAudioEventType
    {
        GlobalVolumeChange,
        GroupVolumeChange,
        GroupPitchChange,
        TimeScaleChange,
        GroupTimeScaleChange,
    }

    public delegate void DeAudioEventDelegate(DeAudioEventArgs e);

    public class DeAudioEventArgs : EventArgs
    {
        /// <summary>Type of event</summary>
        public DeAudioEventType type { get; private set; }
        /// <summary>Eventual <see cref="DeAudioGroup"/> involved in the event (can be NULL)</summary>
        public DeAudioGroup audioGroup { get; private set; }
        /// <summary>Eventual <see cref="DeAudioSource"/> involved in the event (can be NULL)</summary>
        public DeAudioSource source { get; private set; }

        public DeAudioEventArgs(DeAudioEventType type, DeAudioGroup audioGroup, DeAudioSource source)
        {
            this.type = type;
            this.audioGroup = audioGroup;
            this.source = source;
        }
    }
}