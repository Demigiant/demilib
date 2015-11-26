// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 16:26
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeAudio
{
    [System.Serializable]
    public class DeAudioClipData
    {
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 1;
        public bool loop = false;
    }
}