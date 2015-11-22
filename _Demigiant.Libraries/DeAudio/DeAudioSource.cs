// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:41
// License Copyright (c) Daniele Giardini

using System;
using DG.DeAudio.Events;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DeAudio
{
    /// <summary>
    /// Data connected to every AudioSource added by DeAudioManager
    /// </summary>
    public class DeAudioSource : IDisposable
    {
        /// <summary>If TRUE, the AudioSource won't be recycled until it's unlocked</summary>
        public bool locked;
        public DeAudioGroup audioGroup { get; private set; }
        public AudioSource audioSource { get; private set; }
        /// <summary>TRUE if the audioSource is not playing and is not locked</summary>
        public bool isFree { get { return !locked && !audioSource.isPlaying; } }
        public bool isPlaying { get { return audioSource.isPlaying; } }
        public AudioClip clip { get { return audioSource.clip; } }
        public float volume {
            get { return audioSource.volume; }
            set { audioSource.volume = value; }
        }

        internal float playTime { get; private set; } // Time.realTimeSinceStartup since the last Play call
        internal float originalVolume { get; private set; } // Volume set at the last Play call
        internal float unmodifiedVolume { get; private set; } // Volume unmodified by global and group volumes

        bool _disposed;

        // ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        // CONSTRUCTOR

        public DeAudioSource(DeAudioGroup audioGroup, GameObject container)
        {
            this.audioGroup = audioGroup;
            audioSource = container.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            audioSource.playOnAwake = false;

            // Listeners
            DeAudioNotificator.DeAudioEvent += DeAudioEventHandler;
        }

        #region Public Methods

        /// <summary>
        /// Play the given clip with the given options.
        /// Calling Play directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status will remain active if present)
        /// </summary>
        public void Play(AudioClip clip, float volume = 1, bool loop = false)
        {
            playTime = Time.realtimeSinceStartup;
            originalVolume = unmodifiedVolume = volume;
            audioSource.clip = clip;
            audioSource.volume = volume * audioGroup.fooVolume * DeAudioManager.globalVolume;
            audioSource.loop = loop;
            audioSource.Play();
        }

        /// <summary>
        /// Stops any sound connected to this source
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) {
                DeAudioNotificator.DeAudioEvent -= DeAudioEventHandler;
                Object.Destroy(audioSource);
            }
            _disposed = true;
        }

        internal void UpdateVolume()
        {
            audioSource.volume = unmodifiedVolume * audioGroup.fooVolume * DeAudioManager.globalVolume;
        }

        #endregion

        #region Event Handlers

        void DeAudioEventHandler(DeAudioEventArgs e)
        {
            switch (e.type) {
            case DeAudioEventType.GlobalVolumeChange:
                UpdateVolume();
                break;
            case DeAudioEventType.GroupVolumeChange:
                if (e.audioGroup == audioGroup) UpdateVolume();
                break;
            }
        }

        #endregion
    }
}