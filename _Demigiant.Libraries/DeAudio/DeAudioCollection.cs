// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 16:26
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

namespace DG.DeAudio
{
    /// <summary>
    /// A collection of audio files. Still needs to be completed.
    /// </summary>
    [Obsolete("This was a test, but using DeAudioClipData inside custom classes is much better in my opinion, so I abandoned it")]
    public class DeAudioCollection : MonoBehaviour
    {
        public DeAudioClipData[] data;
        public AudioSource previewSource; // used only by the Inspector to preview sounds at given volume

        void Awake()
        {
            if (previewSource != null) {
                Destroy(previewSource);
                previewSource = null;
            }
        }
    }
}