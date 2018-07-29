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
        public bool isFree { get { return !locked && !audioSource.isPlaying && !isPaused; } }
        public bool isPlaying { get { return audioSource.isPlaying; } }
        /// <summary>TRUE if the clip was playing and has been paused - FALSE if it was/is stopped</summary>
        public bool isPaused { get; private set; }
        /// <summary>TRUE while the source is fading out towards 0.
        /// Sources that are fading out are ignored when calling a DeAudioGroup Fade tween</summary>
        public bool isFadingOut { get; private set; }
        public AudioClip clip { get { return audioSource.clip; } }
        public float duration { get { if (clip == null) return 0; return clip.length; } }
        public float pitch { get { return audioSource.pitch; } set { audioSource.pitch = value; } }
        public bool loop { get { return audioSource.loop; } set { audioSource.loop = value; } }
        public float time { get { return audioSource.time; } set { audioSource.time = value; } }
        /// <summary>Target volume to use when fading in (taken by DeAudioClipData, but can also be set manually)</summary>
        public float targetVolume { get; private set; }
        /// <summary>Unscaled volume (doesn't include modifiers caused by global, group and target volumes)</summary>
        public float unscaledVolume { get; private set; }
        /// <summary>Getter returns current audioSource.volume (which is influenced by global, group and target volumes).<para/>
        /// Setter automatically applies scale factors (target, audioGroup and global volumes)</summary>
        public float volume {
            get { return audioSource.volume; }
            set {
                unscaledVolume = value;
                audioSource.volume = value * targetVolume * audioGroup.fooVolume * DeAudioManager.globalVolume;
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
            targetVolume = 1;
            audioSource = container.AddComponent<AudioSource>();
            audioSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            audioSource.playOnAwake = false;

            // Listeners
            DeAudioNotificator.DeAudioEvent += DeAudioEventHandler;
        }

        #region Public Methods

        /// <summary>
        /// Play the given clip with the stored volume, pitch and loop settings.
        /// Calling Play directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status won't change)
        /// </summary>
        public void Play(DeAudioClipData clipData)
        { PlayFrom(clipData.clip, 0, clipData.volume, clipData.pitch, clipData.loop); }
        /// <summary>
        /// Play the given clip with the given options.
        /// Calling Play directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status won't change)
        /// </summary>
        public void Play(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
        { PlayFrom(clip, 0, volume, pitch, loop); }

        /// <summary>
        /// Play the given clip with the stored volume, pitch and loop settings.
        /// Calling Play directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status won't change)
        /// </summary>
        public void PlayFrom(DeAudioClipData clipData, float fromTime)
        { PlayFrom(clipData.clip, fromTime, clipData.volume, clipData.pitch, clipData.loop); }
        /// <summary>
        /// Play the given clip with the given options from the given time.
        /// Calling PlayFrom directly from a DeAudioSource overrides any lock that might've been set
        /// (though the locked status won't change)
        /// </summary>
        public void PlayFrom(AudioClip clip, float fromTime, float volume = 1, float pitch = 1, bool loop = false)
        {
            isPaused = false;
            DestroyFadeTween();
            playTime = Time.realtimeSinceStartup;
            targetVolume = volume;
            this.volume = 1;
            if (audioGroup.mixerGroup != null) audioSource.outputAudioMixerGroup = audioGroup.mixerGroup;
            audioSource.clip = clip;
            audioSource.time = fromTime;
            audioSource.pitch = pitch;
            audioSource.loop = loop;
            audioSource.Play();
        }

        /// <summary>
        /// If the source was actually playing something, pauses it and returns TRUE, otherwise returns FALSE
        /// </summary>
        public bool Pause()
        {
            if (clip == null || !audioSource.isPlaying) return false;

            isPaused = true;
            audioSource.Pause();
            if (_fadeTween != null) _fadeTween.Pause();
            return true;
        }
        // Here so it can be called from fade tweens without using lambdas (they require a void method)
        void PauseDirect()
        {
            Pause();
        }

        /// <summary>
        /// Resumes playing if paused
        /// </summary>
        public void Resume()
        {
            if (clip == null || !isPaused) return;

            isPaused = false;
            if (_fadeTween != null) _fadeTween.Play();
            audioSource.Play();
        }

        /// <summary>
        /// Stops any sound connected to this source
        /// </summary>
        public void Stop()
        {
            isPaused = false;
            audioSource.time = 0; // Reset time to beginning
            DestroyFadeTween();
            audioSource.Stop();
        }

        /// <summary>
        /// If this source was paused, stops any sound connected to it
        /// </summary>
        public void StopIfPaused()
        {
            if (isPaused) Stop();
        }

        /// <summary>
        /// Sends this source's clip to the given time position
        /// </summary>
        public void Seek(float time)
        {
            if (clip == null) return;

            if (time >= clip.length) {
                Debug.LogWarning(string.Format("{0}DeAudioSource.GotoTime({1}): invalid seek position (length of clip is {2}), going to nearest one", DeAudioManager.LogPrefix, time, clip.length));
                time = clip.length - 0.05f;
            }
            audioSource.time = time;
        }

        /// <summary>
        /// Sends this source's clip to the given percentage position, 0 to 1 (but beware: 1 can't be seeked because it's the end of the clip),
        /// and returns the actual time position where the source went.
        /// </summary>
        public void SeekPercentage(float percentage)
        {
            if (clip == null) return;

            if (percentage >= 1) {
                Debug.LogWarning(string.Format("{0}DeAudioSource.GotoTimePercentage({1}): invalid seek position (can't seek to end of clip), going to nearest one", DeAudioManager.LogPrefix, percentage));
                audioSource.time = clip.length - 0.05f;
            } else audioSource.time = clip.length * percentage;
        }

        #region Tweens

        /// <summary>Fades out this source's volume</summary>
        public void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in this source's volume</summary>
        public void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades this source's volume to the given value</summary>
        public void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades this source's volume from the given value to its current one</summary>
        public void FadeFrom(float from, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        {
            float to = unscaledVolume;
            volume = from;
            FadeTo(to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete);
        }
        internal void FadeTo(float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            if (to <= 0) to = 0;
            else {
                float maxScaledVolume = 1 / targetVolume / audioGroup.fooVolume / DeAudioManager.globalVolume;
                if (to > maxScaledVolume) to = maxScaledVolume;
            }
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => unscaledVolume, x => volume = x, to, duration)
                .SetTarget(this).SetUpdate(ignoreTimeScale).SetEase(DeAudioManager.I.fadeEase);
            if (to <= 0) isFadingOut = true;
            switch (onCompleteBehaviour) {
            case FadeBehaviour.Stop:
                _fadeTween.OnStepComplete(OnFadeToInternalComplete_Stop);
                break;
            case FadeBehaviour.Pause:
                _fadeTween.OnStepComplete(OnFadeToInternalComplete_Pause);
                break;
            default:
                _fadeTween.OnStepComplete(OnFadeToInternalComplete_Default);
                break;
            }
            if (onComplete != null) _fadeTween.OnComplete(onComplete);
        }
        void OnFadeToInternalComplete_Default()
        {
            isFadingOut = false;
        }
        void OnFadeToInternalComplete_Stop()
        {
            isFadingOut = false;
            Stop();
        }
        void OnFadeToInternalComplete_Pause()
        {
            isFadingOut = false;
            PauseDirect();
        }

        #endregion

        #endregion

        #region Methods

        internal void Reset()
        {
            targetVolume = 1;
        }

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
                DestroyFadeTween();
                Object.Destroy(audioSource);
            }
            _disposed = true;
        }

        // Kills _fadeTween and sets it as null
        void DestroyFadeTween()
        {
            if (_fadeTween == null) return;
            _fadeTween.Kill();
            _fadeTween = null;
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