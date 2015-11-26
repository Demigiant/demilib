// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:29
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DeAudio.Core;
using DG.DeAudio.Events;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

namespace DG.DeAudio
{
    /// <summary>
    /// Global AudioManager.
    /// Must be instantiated only once per project (either manually or via code).
    /// Its GameObject is set automatically to DontDestroyOnLoad.
    /// </summary>
    public class DeAudioManager : MonoBehaviour
    {
        public bool logInfo = false;
        public DeAudioGroup[] fooAudioGroups;
        public float fooGlobalVolume = 1;

        internal static DeAudioManager I;
        public const string Version = "0.5.100";
        internal const string LogPrefix = "DAM :: ";
        static Tween _fadeTween;

        public static DeAudioGroup[] audioGroups { get { return I.fooAudioGroups; } }
        public static float globalVolume {
            get { return I.fooGlobalVolume; }
            set { SetVolume(value); }
        }

        #region Unity Methods

        void Awake()
        {
            if (I != null) {
                Debug.LogWarning(LogPrefix + "Multiple DeAudioManager instances were found. The newest ones will be destroyed");
                Destroy(this.gameObject);
            }

            I = this;
            DontDestroyOnLoad(this.gameObject);

            // Initialize audioGroups
            foreach (DeAudioGroup group in audioGroups) group.Init(this.transform);
        }

        void OnDestroy()
        {
            if (I != this) return;
            _fadeTween.Kill();
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) audioGroups[i].Dispose();
            I = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Plays the given sound with the given options and using the given group id.
        /// A <see cref="DeAudioGroup"/> with the given ID must exist in order for the sound to actually play.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Play(DeAudioGroupId groupId, AudioClip clip, float volume = 1, bool loop = false)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group == null) {
                Debug.LogWarning(LogPrefix + "Clip can't be played because no group with the given groupId (" + groupId + ") was created");
                return null;
            }
            return group.Play(clip, volume, loop);
        }

        /// <summary>Stops all sounds</summary>
        public static void Stop()
        { IterateOnAllGroups(OperationType.Stop); }
        /// <summary>Stops all sounds for the given group</summary>
        public static void Stop(DeAudioGroupId groupId)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.Stop();
        }
        /// <summary>Stops all sounds for the given clip</summary>
        public static void Stop(AudioClip clip)
        { IterateOnAllGroups(OperationType.StopByClip, clip); }

        /// <summary>Sets the global volume (same as setting <see cref="globalVolume"/> directly</summary>
        public static void SetVolume(float volume)
        {
            I.fooGlobalVolume = volume;
            DeAudioNotificator.DispatchDeAudioEvent(DeAudioEventType.GlobalVolumeChange);
        }
        /// <summary>Sets the volume for the given group</summary>
        public static void SetVolume(DeAudioGroupId groupId, float volume)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.SetVolume(volume);
        }
        /// <summary>Sets the volume for the given clip</summary>
        public static void SetVolume(AudioClip clip, float volume)
        { IterateOnAllGroups(OperationType.SetVolumeByClip, clip, volume); }

        /// <summary>Unlocks all <see cref="DeAudioSource"/> instances</summary>
        public static void Unlock()
        { IterateOnAllGroups(OperationType.Unlock); }
        /// <summary>Unlocks all <see cref="DeAudioSource"/> instances for the given group</summary>
        public static void Unlock(DeAudioGroupId groupId)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.Unlock();
        }
        /// <summary>Unlocks all <see cref="DeAudioSource"/> instances for the given clip</summary>
        public static void Unlock(AudioClip clip)
        { IterateOnAllGroups(OperationType.UnlockByClip, clip); }

        /// <summary>
        /// Returns the <see cref="DeAudioGroup"/> with the given ID, or NULL if there is none
        /// </summary>
        public static DeAudioGroup GetAudioGroup(DeAudioGroupId groupId)
        {
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup g = audioGroups[i];
                if (g.id == groupId) return g;
            }
            return null;
        }

        /// <summary>
        /// Returns the AudioMixerGroup for <see cref="DeAudioGroup"/> with the given ID, or null if there is none
        /// </summary>
        public static AudioMixerGroup GetMixerGroup(DeAudioGroupId groupId)
        {
            DeAudioGroup g = GetAudioGroup(groupId);
            if (g == null) return null;
            return g.mixerGroup;
        }

        #region Tweens

        /// <summary>Fades out the global volume</summary>
        public static void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, bool stopOnComplete = true, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, stopOnComplete, onComplete); }
        /// <summary>Fades in the global volume</summary>
        public static void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades the global volume to the given value</summary>
        public static void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, false, onComplete); }
        static void FadeTo(float to, float duration, bool ignoreTimeScale, bool stopOnComplete, TweenCallback onComplete)
        {
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => globalVolume, x => globalVolume = x, to, duration)
                .SetTarget(I).SetUpdate(ignoreTimeScale).SetEase(Ease.Linear);
            if (stopOnComplete) _fadeTween.OnStepComplete(Stop);
            if (onComplete != null) _fadeTween.OnComplete(onComplete);
        }

        /// <summary>Fades out the given group's volume</summary>
        public static void FadeOut(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, bool stopOnComplete = true, TweenCallback onComplete = null)
        { FadeTo(groupId, 0, duration, ignoreTimeScale, stopOnComplete, onComplete); }
        /// <summary>Fades in the given group's volume</summary>
        public static void FadeIn(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(groupId, 1, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades the given group's volume to the given value</summary>
        public static void FadeTo(DeAudioGroupId groupId, float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(groupId, to, duration, ignoreTimeScale, false, onComplete); }
        static void FadeTo(DeAudioGroupId groupId, float to, float duration, bool ignoreTimeScale, bool stopOnComplete, TweenCallback onComplete)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.FadeTo(to, duration, ignoreTimeScale, stopOnComplete, onComplete);
        }

        /// <summary>Fades out the given clip's volume</summary>
        public static void FadeOut(AudioClip clip, float duration = 1.5f, bool ignoreTimeScale = true, bool stopOnComplete = true, TweenCallback onComplete = null)
        { FadeTo(clip, 0, duration, ignoreTimeScale, stopOnComplete, onComplete); }
        /// <summary>Fades in the given clip's volume</summary>
        public static void FadeIn(AudioClip clip, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(clip, 1, duration, ignoreTimeScale, false, onComplete); }
        /// <summary>Fades the given clip's volume to the given value</summary>
        public static void FadeTo(AudioClip clip, float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(clip, to, duration, ignoreTimeScale, false, onComplete); }
        static void FadeTo(AudioClip clip, float to, float duration, bool ignoreTimeScale, bool stopOnComplete, TweenCallback onComplete)
        {
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup group = audioGroups[i];
                int slen = group.sources.Count;
                for (int c = 0; c < slen; c++) {
                    DeAudioSource s = group.sources[c];
                    if (s.clip == clip) s.FadeTo(to, duration, ignoreTimeScale, stopOnComplete, onComplete);
                }
            }
        }

        #endregion

        #endregion

        #region Methods

        static void IterateOnAllGroups(OperationType operationType, AudioClip clip = null, float floatValue = 0)
        {
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup group = audioGroups[i];
                switch (operationType) {
                case OperationType.Stop:
                    group.Stop();
                    break;
                case OperationType.StopByClip:
                    group.Stop(clip);
                    break;
                case OperationType.SetVolumeByClip:
                    group.SetVolume(clip, floatValue);
                    break;
                case OperationType.Unlock:
                    group.Unlock();
                    break;
                case OperationType.UnlockByClip:
                    group.Unlock(clip);
                    break;
                }
            }
        }

        #endregion
    }
}