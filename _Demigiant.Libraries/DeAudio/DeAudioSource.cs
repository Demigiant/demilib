// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:41
// License Copyright (c) Daniele Giardini

using System;
using DG.DeAudio.Events;
using DG.Tweening;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DG.DeAudio
{
    /// <summary>
    /// Data connected to every AudioSource added by <see cref="DeAudioManager"/>
    /// </summary>
    public class DeAudioSource : IDisposable
    {
        /// <summary>If TRUE, the AudioSource won't be reused until it's unlocked</summary>
        public bool locked;
        public DeAudioGroup audioGroup { get; private set; }
        public AudioSource audioSource { get; private set; }
        /// <summary>TRUE if the audioSource is not playing and is not locked</summary>
        public bool isFree { get { return !locked && !audioSource.isPlaying; } }
        public bool isPlaying { get { return audioSource.isPlaying; } }
        public AudioClip clip { get { return audioSource.clip; } }
        public float pitch { get { return audioSource.pitch; } }
        public bool loop { get { return audioSource.loop; } }
        /// <summary>Unscaled volume (doesn't include modifiers caused by global and group volumes)</summary>
        public float unscaledVolume { get; private set; }
        /// <summary>Current volume (including modifiers caused by global and group volumes)</summary>
        public float volume {
            get { return audioSource.volume; }
            set {
                unscaledVolume = value;
                audioSource.volume = value * audioGroup.fooVolume * DeAudioManager.globalVolume;
            }
        }

        internal float playTime { get; private set; } // Time.realTimeSinceStartup since the last Play call

        bool _disposed;
        Tween _fadeTween;

        // ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        // CONSTRUCTOR

        internal DeAudioSource(DeAudioGroup audioGroup, GameObject container)
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
        /// (though the locked status won't change)
        /// </summary>
        public void Play(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
        {
            _fadeTween.Kill();
            playTime = Time.realtimeSinceStartup;
            this.volume = volume;
            if (audioGroup.mixerGroup != null) audioSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            audioSource.clip = clip;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.Play();
        }
        /// <summary>
        /// Play the given clip with the stored volume, pitch and loop settings.
        /// Calling Play directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status won't change)
        /// </summary>
        public void Play(DeAudioClipData clipData)
        {
            _fadeTween.Kill();
            playTime = Time.realtimeSinceStartup;
            this.volume = clipData.volume;
            if (audioGroup.mixerGroup != null) audioSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            audioSource.clip = clipData.clip;
            audioSource.pitch = clipData.pitch;
            audioSource.loop = clipData.loop;
            audioSource.Play();
        }

        /// <summary>
        /// Stops any sound connected to this source
        /// </summary>
        public void Stop()
        {
            _fadeTween.Kill();
            audioSource.Stop();
        }

        #region Tweens

        /// <summary>Fades out this source's volume</summary>
        public void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, bool stopOnComplete = true, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, stopOnComplete, onComplete); }
        /// <summary>Fades in this source's volume</summary>
        public void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades this source's volume to the given value</summary>
        public void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades this source's volume from the given value to its current one</summary>
        public void FadeFrom(float from, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        {
            float to = unscaledVolume;
            volume = from;
            FadeTo(to, duration, ignoreTimeScale, false, onComplete);
        }
        internal void FadeTo(float to, float duration, bool ignoreTimeScale, bool stopOnComplete, TweenCallback onComplete)
        {
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => volume, x => volume = x, to, duration)
                .SetTarget(this).SetUpdate(ignoreTimeScale).SetEase(Ease.Linear);
            if (stopOnComplete) _fadeTween.OnStepComplete(Stop);
            if (onComplete != null) _fadeTween.OnComplete(onComplete);
        }

        #endregion

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
                _fadeTween.Kill();
                Object.Destroy(audioSource);
            }
            _disposed = true;
        }

        #endregion

        #region Event Handlers

        void DeAudioEventHandler(DeAudioEventArgs e)
        {
            switch (e.type) {
            case DeAudioEventType.GlobalVolumeChange:
                volume = unscaledVolume;
                break;
            case DeAudioEventType.GroupVolumeChange:
                if (e.audioGroup == audioGroup) volume = unscaledVolume;
                break;
            }
        }

        #endregion
    }
}