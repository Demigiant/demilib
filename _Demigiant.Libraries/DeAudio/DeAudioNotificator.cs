// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/22 13:20
// License Copyright (c) Daniele Giardini

using DG.DeAudio.Events;

namespace DG.DeAudio
{
    /// <summary>
    /// Static event notificator for all DeAudio events
    /// </summary>
    public static class DeAudioNotificator
    {
        public static event DeAudioEventDelegate DeAudioEvent;
        public static void DispatchDeAudioEvent(DeAudioEventType type, DeAudioGroup audioGroup = null, DeAudioSource source = null)
        { if (DeAudioEvent != null) DeAudioEvent(new DeAudioEventArgs(type, audioGroup, source)); }
    }
}