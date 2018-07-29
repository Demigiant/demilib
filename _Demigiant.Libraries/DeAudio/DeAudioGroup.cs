// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:43
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using DG.DeAudio.Core;
using DG.DeAudio.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace DG.DeAudio
{
    /// <summary>
    /// Audio group, connected to an AudioMixerGroup.
    /// Create it using the DeAudioManager Inspector
    /// </summary>
    [System.Serializable]
    public class DeAudioGroup : IDisposable
    {
        public AudioMixerGroup mixerGroup;
        public DeAudioGroupId id;
        /// <summary>Max AudioSources for this group. DO NOT change this at runtime</summary>
        public int maxSources = -1;
        public int preallocate = 0;
        public bool recycle = true;
        public float fooVolume = 1;

        [System.NonSerialized] public List<DeAudioSource> sources; // Sources per each group
        public float volume {
            get { return fooVolume; }
            set { SetVolume(value); }
        }

        bool _disposed;
        GameObject _sourcesContainer;
        Tween _fadeTween;

        #region Constructor + Init

        internal DeAudioGroup() {}

        public DeAudioGroup(DeAudioGroupId id)
        {
            this.id = id;
        }
        public DeAudioGroup(DeAudioGroupId id, float volume, int maxSources = -1, int preallocate = 0, bool recycle = true, AudioMixerGroup mixerGroup = null)
        {
            this.id = id;
            this.volume = volume;
            this.maxSources = maxSources;
            this.preallocate = preallocate;
            this.recycle = recycle;
            this.mixerGroup = mixerGroup;
        }

        internal void Init(Transform container, string name = null)
        {
            _sourcesContainer = new GameObject(name == null ? id.ToString() : name);
            _sourcesContainer.transform.parent = container;
            sources = maxSources >= 0 ? new List<DeAudioSource>(maxSources) : new List<DeAudioSource>();
            if (preallocate > 0) {
                for (int i = 0; i < preallocate; ++i) sources.Add(new DeAudioSource(this, _sourcesContainer));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays the given <see cref="DeAudioClipData"/> with the stored volume, pitch and loop settings (unless set otherwise).
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource Play(DeAudioClipData clipData, float? volume = null, float? pitch = null, bool? loop = null)
        {
            DeAudioSource src = PlayFrom(clipData.clip, 0,
                volume == null ? clipData.volume : (float)volume,
                pitch == null ? clipData.pitch : (float)pitch,
                loop == null ? clipData.loop : (bool)loop
            );
            return src;
        }
        /// <summary>
        /// Plays the given sound with the given options.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource Play(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
        { return PlayFrom(clip, 0, volume, pitch, loop); }
        /// <summary>
        /// Plays the given sound with the stored volume, pitch and loop settings (unless set otherwise).
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource PlayFrom(DeAudioClipData clipData, float fromTime, float? volume = null, float? pitch = null, bool? loop = null)
        {
            DeAudioSource src = PlayFrom(clipData.clip, fromTime,
                volume == null ? clipData.volume : (float)volume,
                pitch == null ? clipData.pitch : (float)pitch,
                loop == null ? clipData.loop : (bool)loop
            );
            return src;
        }
        /// <summary>
        /// Plays the given sound with the given options from the given time.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource PlayFrom(AudioClip clip, float fromTime, float volume = 1, float pitch = 1, bool loop = false)
        {
            DeAudioSource src = GetAvailableSource();
            if (src == null) {
                if (DeAudioManager.I.logInfo) Debug.Log(DeAudioManager.LogPrefix + "Clip can't be played because all sources are busy");
                return null;
            }
            src.PlayFrom(clip, fromTime, volume, pitch, loop);
            return src;
        }

        /// <summary>Stops all sounds for this group</summary>
        public void Stop()
        { IterateOnAllSources(OperationType.Stop); }
        /// <summary>Stops all sources for this group that are using the given clip</summary>
        public void Stop(AudioClip clip)
        { IterateOnAllSources(OperationType.StopByClip, clip); }

        /// <summary>Stops all sounds for this group that are in a paused state</summary>
        public void StopAllPaused()
        { IterateOnAllSources(OperationType.StopIfPaused); }

        /// <summary>Pauses all sounds for this group</summary>
        public void Pause()
        { IterateOnAllSources(OperationType.Pause); }
        /// <summary>Pauses all sources for this group that are using the given clip</summary>
        public void Pause(AudioClip clip)
        { IterateOnAllSources(OperationType.PauseByClip, clip); }

        /// <summary>Resumes all paused sounds for this group</summary>
        /// <param name="volume">If >=0 also sets the group volume, otherwise leaves as it was</param>
        public void Resume(float volume = -1)
        {
            if (volume >= 0) SetVolume(volume);
            IterateOnAllSources(OperationType.Resume);
        }
        /// <summary>Resumes all paused sources for this group that are using the given clip</summary>
        /// <param name="volume">If >=0 also sets the volume for the sources resuming the clip, otherwise leaves as it was</param>
        public void Resume(AudioClip clip, float volume = -1)
        { IterateOnAllSources(OperationType.ResumeByClip, clip, volume); }

        /// <summary>Changes the pitch for all existing sources of this group</summary>
        public void ChangePitch(float pitch)
        {
            for (int i = 0; i < sources.Count; i++) sources[i].audioSource.pitch = pitch;
        }

        /// <summary>Sets the volume for this group (same as setting <see cref="volume"/> directly)</summary>
        public void SetVolume(float volume)
        {
            fooVolume = volume;
            DeAudioNotificator.DispatchDeAudioEvent(DeAudioEventType.GroupVolumeChange, this);
        }
        /// <summary>Sets the volume for all sources in this group that are using the given clip</summary>
        public void SetVolume(AudioClip clip, float volume)
        { IterateOnAllSources(OperationType.SetVolumeByClip, clip, volume); }

        /// <summary>Unlocks all <see cref="DeAudioSource"/> instances for this group</summary>
        public void Unlock()
        { IterateOnAllSources(OperationType.Unlock); }
        /// <summary>Unlocks all <see cref="DeAudioSource"/> instances for this group that are using the given clip</summary>
        public void Unlock(AudioClip clip)
        { IterateOnAllSources(OperationType.UnlockByClip, clip); }

        #region Info Methods

        public bool IsPlaying(AudioClip clip)
        {
            if (clip == null) return false;
            for (int i = 0; i < sources.Count; ++i) {
                if (sources[i].clip != clip) continue;
                return sources[i].isPlaying;
            }
            return false;
        }

        #endregion

        #region Tweens

        /// <summary>Fades out this group's volume</summary>
        public void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in this group's volume</summary>
        public void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades this group's volume to the given value</summary>
        public void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        internal void FadeTo(float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => volume, x => volume = x, to, duration)
                .SetTarget(this).SetUpdate(ignoreTimeScale).SetEase(DeAudioManager.I.fadeEase);
            switch (onCompleteBehaviour) {
            case FadeBehaviour.Stop:
                _fadeTween.OnStepComplete(Stop);
                break;
            case FadeBehaviour.Pause:
                _fadeTween.OnStepComplete(Pause);
                break;
            }
            if (onComplete != null) _fadeTween.OnComplete(onComplete);
        }

        /// <summary>Fades out the volume of each source in this group (not this group's volume)</summary>
        public void FadeSourcesOut(float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop)
        { FadeSourcesTo(0, duration, ignoreTimeScale, onCompleteBehaviour, null); }
        /// <summary>Fades in the volume of each source in this group (not this group's volume)</summary>
        public void FadeSourcesIn(float duration = 1.5f, bool ignoreTimeScale = true)
        { FadeSourcesTo(1, duration, ignoreTimeScale, FadeBehaviour.None, null); }
        /// <summary>Fades the volume of each source in this group (not this group's volume) to the given value</summary>
        public void FadeSourcesTo(float to, float duration = 1.5f, bool ignoreTimeScale = true)
        { FadeSourcesTo(to, duration, ignoreTimeScale, FadeBehaviour.None, null); }
        internal void FadeSourcesTo(float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            int len = sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource src = sources[i];
                if (!src.isPlaying || src.isFadingOut) continue; // Ignore sources that are fading to 0 or that are not playing at all
                sources[i].FadeTo(to, duration, ignoreTimeScale, onCompleteBehaviour, onComplete);
            }
        }

        /// <summary>
        /// Fades out then stops all sources in this group, while starting the given <see cref="DeAudioClipData"/> with a fade-in effect.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource Crossfade(DeAudioClipData clipData, float fadeDuration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onfadeOutBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { return Crossfade(clipData.clip, clipData.volume, clipData.pitch, clipData.loop, fadeDuration, ignoreTimeScale, onfadeOutBehaviour, onComplete); }
        /// <summary>
        /// Fades out then stops all sources in this group, while starting the given clip with a fade-in effect.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public DeAudioSource Crossfade(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, float fadeDuration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onfadeOutBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        {
            FadeSourcesTo(0, fadeDuration, ignoreTimeScale, onfadeOutBehaviour, null);
            DeAudioSource s = Play(clip, volume, pitch, loop);
            if (s != null) s.FadeFrom(0, fadeDuration, ignoreTimeScale, onComplete);
            return s;
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
                _fadeTween.Kill();
                int len = sources.Count;
                for (int i = 0; i < len; ++i) sources[i].Dispose();
                sources.Clear();
            }
            _disposed = true;
        }

        // Returns the eventual existing source for the given clip, or NULL if there is none
        internal DeAudioSource GetExistingAudioSourceFor(AudioClip clip)
        {
            if (clip == null) return null;
            for (int i = 0; i < sources.Count; ++i) {
                if (sources[i].clip == clip) return sources[i];
            }
            return null;
        }

        // Either:
        // - returns an existing non playing source if available
        // - creates a new source if all existing are busy and maxSources allows it
        // - return NULL if none of the previous options work
        // Also sets the src targetVolume to 1
        DeAudioSource GetAvailableSource()
        {
            int len = sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource src = sources[i];
                if (src.isFree) {
                    src.Reset();
                    return src;
                }
            }
            // No free sources...
            if (maxSources < 0 || len < maxSources) {
                // Create new source
                DeAudioSource src = new DeAudioSource(this, _sourcesContainer);
                src.Reset();
                sources.Add(src);
                return src;
            } else if (recycle) {
                // Recycle oldest source
                DeAudioSource src = GetOldestSource();
                src.Reset();
                return src;
            } else {
                // All sources busy and can't be recycled
                return null;
            }
        }

        void IterateOnAllSources(OperationType operationType, AudioClip clip = null, float floatValue = 0)
        {
            int len = sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = sources[i];
                switch (operationType) {
                case OperationType.Stop:
                    s.Stop();
                    break;
                case OperationType.StopByClip:
                    if (s.clip == clip) s.Stop();
                    break;
                case OperationType.StopIfPaused:
                    if (s.isPaused) s.Stop();
                    break;
                case OperationType.Pause:
                    s.Pause();
                    break;
                case OperationType.PauseByClip:
                    if (s.clip == clip) s.Pause();
                    break;
                case OperationType.Resume:
                    s.Resume();
                    break;
                case OperationType.ResumeByClip:
                    if (s.clip == clip) {
                        if (floatValue >= 0) s.volume = floatValue;
                        s.Resume();
                    }
                    break;
                case OperationType.SetVolumeByClip:
                    if (s.clip == clip) s.volume = floatValue;
                    break;
                case OperationType.Unlock:
                    s.locked = false;
                    break;
                case OperationType.UnlockByClip:
                    if (s.clip == clip) s.locked = false;
                    break;
                }
            }
        }

        #region Helpers

        DeAudioSource GetOldestSource()
        {
            int len = sources.Count;
            float time = float.MaxValue;
            DeAudioSource res = null;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = sources[i];
                if (s.playTime >= time) continue;
                res = s;
                time = s.playTime;
            }
            return res;
        }

        #endregion

        #endregion
    }
}