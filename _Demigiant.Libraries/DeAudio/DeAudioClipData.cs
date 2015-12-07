// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 16:26
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeAudio
{
    [System.Serializable]
    public class DeAudioClipData
    {
        public DeAudioGroupId groupId;
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 1;
        public bool loop = false;

        public DeAudioClipData(DeAudioGroupId groupId, float volume = 1, float pitch = 1, bool loop = false)
        {
            this.groupId = groupId;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
        }

        public DeAudioClipData(DeAudioGroupId groupId, bool loop)
        {
            this.groupId = groupId;
            this.loop = loop;
        }

        public DeAudioClipData()
        {
            volume = 1;
            pitch = 1;
            loop = false;
        }
    }
}