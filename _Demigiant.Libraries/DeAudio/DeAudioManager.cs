// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 18:29
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
    /// Global AudioManager. Only use its static methods.
    /// <para>Must be instantiated only once per project (either manually or via code), and its GameObject is set automatically to DontDestroyOnLoad.</para>
    /// </summary>
    public class DeAudioManager : MonoBehaviour
    {
        public bool logInfo = false;
        public DeAudioGroup[] fooAudioGroups;
        public float fooGlobalVolume = 1;
        public Ease fadeEase = Ease.Linear;
        /// <summary>Used internally inside Unity Editor, as a trick to update DeAudioManager's inspector at every frame</summary>
        public int inspectorUpdater;

        public static float globalVolume {
            get { return I.fooGlobalVolume; }
            set { SetVolume(value); }
        }

        internal static DeAudioManager I;
        public const string Version = "0.6.150";
        internal const string LogPrefix = "DeAudio :: ";
        static bool _isInitializing; // If TRUE skips audioGroups initialization at Awake
        internal static DeAudioGroup[] audioGroups; // Internal so Inspector can read it
        internal static DeAudioGroup globalGroup; // Group created when playing a clip without any group indication. Also stored as the final _audioGroups value
        static Tween _fadeTween;


        #region Unity Methods

        void Awake()
        {
            if (I != null) {
                Debug.LogWarning(LogPrefix + "Multiple DeAudioManager instances were found. The newest one will be destroyed");
                Destroy(this.gameObject);
                return;
            }

            I = this;
            DontDestroyOnLoad(this.gameObject);

            if (!_isInitializing) InitializeAudioGroups();
        }

        void Update()
        {
            if (Application.isEditor) inspectorUpdater++;
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

        #region Init

        /// <summary>
        /// Creates a DeAudioManager instance (if it's not present already) and sets it as DontDestroyOnLoad.
        /// Returns TRUE if the initialization is successful, FALSE otherwise.
        /// <para>Use this method if you want to use DeAudioManager without setting up any DeAudioGroup.
        /// Though the recommended way is to create a prefab with the required settings and instantiate it once at startup.</para>
        /// </summary>
        public static bool Init()
        {
            if (I != null) {
                Debug.Log(LogPrefix + "DeAudioManager won't be initialized because another DeAudioManager is already present");
                return false;
            }

            GameObject go = new GameObject("[DeAudioManager]");
            go.AddComponent<DeAudioManager>();
            return true;
        }
        /// <summary>
        /// Instantiates the DeAudioManager prefab at the given Resources path (if it's not present already) and sets it as DontDestroyOnLoad.
        /// Returns TRUE if the initialization is successful, FALSE otherwise.
        /// </summary>
        public static bool Init(string resourcePath)
        {
            if (I != null) {
                Debug.Log(LogPrefix + "DeAudioManager won't be initialized because another DeAudioManager is already present");
                return false;
            }

            GameObject go = Instantiate(Resources.Load<GameObject>(resourcePath));
            int index = go.name.LastIndexOf("(");
            if (index != -1) go.name = go.name.Substring(0, index);
            return true;
        }
        /// <summary>
        /// Creates a DeAudioManager instance (if it's not present already), sets it as DontDestroyOnLoad, and sets it with the given groups.
        /// Returns TRUE if the initialization is successful, FALSE otherwise.
        /// </summary>
        public static bool Init(DeAudioGroup[] audioGroups)
        {
            _isInitializing = true;
            if (!Init()) return false;

            I.fooAudioGroups = audioGroups;
            I.InitializeAudioGroups();
            _isInitializing = false;
            return true;
        }

        #endregion

        /// <summary>
        /// Plays the given <see cref="DeAudioClipData"/> on the stored group,
        /// with the stored volume, pitch and loop settings (unless set otherwise).
        /// A <see cref="DeAudioGroup"/> with the given ID must exist in order for the sound to actually play.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Play(DeAudioClipData clipData, float? volume = null, float? pitch = null, bool? loop = null)
        {
            return PlayFrom(clipData, 0,
                volume == null ? clipData.volume : (float)volume,
                pitch == null ? clipData.pitch : (float)pitch,
                loop == null ? clipData.loop : (bool)loop
            );
        }
        /// <summary>
        /// Plays the given sound with the given options and using the given group id.
        /// A <see cref="DeAudioGroup"/> with the given ID must exist in order for the sound to actually play.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Play(DeAudioGroupId groupId, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
        { return PlayFrom(groupId, clip, 0, volume, pitch, loop); }
        /// <summary>
        /// Plays the given sound external to any group, using the given options.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Play(AudioClip clip, float volume = 1, float pitch = 1, bool loop = false)
        { return PlayFrom(clip, 0, volume, pitch, loop); }

        /// <summary>
        /// Plays the given <see cref="DeAudioClipData"/> on the stored group,
        /// with the stored volume, pitch, loop settings (unless set otherwise), and from the given time.
        /// A <see cref="DeAudioGroup"/> with the given ID must exist in order for the sound to actually play.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource PlayFrom(DeAudioClipData clipData, float fromTime, float? volume = null, float? pitch = null, bool? loop = null)
        {
            DeAudioSource src = PlayFrom(clipData.groupId, clipData.clip, fromTime,
                volume == null ? clipData.volume : (float)volume,
                pitch == null ? clipData.pitch : (float)pitch,
                loop == null ? clipData.loop : (bool)loop
            );
            src.targetVolume = clipData.volume;
            return src;
        }
        /// <summary>
        /// Plays the given sound with the given options, using the given group id, and from the given time.
        /// A <see cref="DeAudioGroup"/> with the given ID must exist in order for the sound to actually play.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource PlayFrom(DeAudioGroupId groupId, AudioClip clip, float fromTime, float volume = 1, float pitch = 1, bool loop = false)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group == null) {
                Debug.LogWarning(LogPrefix + "Clip can't be played because no group with the given groupId (" + groupId + ") was created");
                return null;
            }
            return group.PlayFrom(clip, fromTime, volume, pitch, loop);
        }
        /// <summary>
        /// Plays the given sound external to any group, using the given options and from the given time.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource PlayFrom(AudioClip clip, float fromTime, float volume = 1, float pitch = 1, bool loop = false)
        {
            return globalGroup.PlayFrom(clip, fromTime, volume, pitch, loop);
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

        /// <summary>Stops all paused sounds</summary>
        public static void StopAllPaused()
        { IterateOnAllGroups(OperationType.StopIfPaused); }
        /// <summary>Stops all paused sounds for the given group</summary>
        public static void StopAllPaused(DeAudioGroupId groupId)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.StopAllPaused();
        }

        /// <summary>Pauses all sounds</summary>
        public static void Pause()
        { IterateOnAllGroups(OperationType.Pause); }
        /// <summary>Pauses all sounds for the given group</summary>
        public static void Pause(DeAudioGroupId groupId)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.Pause();
        }
        /// <summary>Pauses all sounds for the given clip</summary>
        public static void Pause(AudioClip clip)
        { IterateOnAllGroups(OperationType.PauseByClip, clip); }

        /// <summary>Resumes all paused sounds</summary>
        public static void Resume()
        { IterateOnAllGroups(OperationType.Resume); }
        /// <summary>Resumes all paused sounds for the given group</summary>
        /// <param name="groupId">Group ID</param>
        /// <param name="volume">If >=0 also sets the group volume, otherwise leaves as it was</param>
        public static void Resume(DeAudioGroupId groupId, float volume = -1)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.Resume(volume);
        }
        /// <summary>Resumes all paused sounds for the given clip</summary>
        /// <param name="clip">Clip</param>
        /// <param name="volume">If >= 0 also sets the volume for the sources resuming the given clip, otherwise leaves as it was</param>
        public static void Resume(AudioClip clip, float volume = -1)
        { IterateOnAllGroups(OperationType.ResumeByClip, clip, volume); }

        /// <summary>Changes the pitch for the given group's existing sources</summary>
        public static void ChangePitch(DeAudioGroupId groupId, float pitch)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.ChangePitch(pitch);
        }
        /// <summary>Changes the pitch for all audio groups' existing sources</summary>
        public static void ChangePitch(float pitch)
        {
            for (int i = 0; i < audioGroups.Length; ++i) audioGroups[i].ChangePitch(pitch);
        }

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
        public static void FadeOut(float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeTo(0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in the global volume</summary>
        public static void FadeIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades the global volume to the given value</summary>
        public static void FadeTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        static void FadeTo(float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            _fadeTween.Kill();
            _fadeTween = DOTween.To(() => globalVolume, x => globalVolume = x, to, duration)
                .SetTarget(I).SetUpdate(ignoreTimeScale).SetEase(I.fadeEase);
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
        /// <summary>Fades out the volume of every source without touching global and group volumes</summary>
        public static void FadeSourcesOut(float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeSourcesTo(0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in the volume of every source without touching global and group volumes</summary>
        public static void FadeSourcesIn(float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeSourcesTo(1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades the volume of every source to the given value without touching global and group volumes</summary>
        public static void FadeSourcesTo(float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeSourcesTo(to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        static void FadeSourcesTo(float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) audioGroups[i].FadeSourcesTo(to, duration, ignoreTimeScale, onCompleteBehaviour, onComplete);
        }

        /// <summary>Fades out the given group's volume</summary>
        public static void FadeOut(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeTo(groupId, 0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in the given group's volume</summary>
        public static void FadeIn(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(groupId, 1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades the given group's volume to the given value</summary>
        public static void FadeTo(DeAudioGroupId groupId, float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(groupId, to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        static void FadeTo(DeAudioGroupId groupId, float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.FadeTo(to, duration, ignoreTimeScale, onCompleteBehaviour, onComplete);
        }
        /// <summary>Fades out the volume of each source in the given group (not the given group's volume)</summary>
        public static void FadeSourcesOut(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeSourcesTo(groupId, 0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Fades in the volume of each source in the given group (not the given group's volume)</summary>
        public static void FadeSourcesIn(DeAudioGroupId groupId, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeSourcesTo(groupId, 1, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades the volume of each source in the given group (not the given group's volume) to the given value</summary>
        public static void FadeSourcesTo(DeAudioGroupId groupId, float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeSourcesTo(groupId, to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        static void FadeSourcesTo(DeAudioGroupId groupId, float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group != null) group.FadeSourcesTo(to, duration, ignoreTimeScale, onCompleteBehaviour, onComplete);
        }

        /// <summary>Fades out the given clip's volume</summary>
        public static void FadeOut(AudioClip clip, float duration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onCompleteBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { FadeTo(clip, 0, duration, ignoreTimeScale, onCompleteBehaviour, onComplete); }
        /// <summary>Starts playing the given clip with a fade-in volume effect</summary>
        public static void FadeIn(DeAudioGroupId groupId, AudioClip clip, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { Play(groupId, clip, 0).FadeTo(1, duration, ignoreTimeScale, onComplete); }
        /// <summary>Starts playing the given clip external to any group, with a fade-in volume effect</summary>
        public static void FadeIn(AudioClip clip, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { Play(clip, 0).FadeTo(1, duration, ignoreTimeScale, onComplete); }
        /// <summary>Starts playing the given <see cref="DeAudioClipData"/> with a fade-in volume effect</summary>
        public static void FadeIn(DeAudioClipData clipData, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { Play(clipData.groupId, clipData.clip, 0, clipData.pitch, clipData.loop).FadeTo(clipData.volume, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        /// <summary>Fades the given clip's volume to the given value</summary>
        public static void FadeTo(AudioClip clip, float to, float duration = 1.5f, bool ignoreTimeScale = true, TweenCallback onComplete = null)
        { FadeTo(clip, to, duration, ignoreTimeScale, FadeBehaviour.None, onComplete); }
        static void FadeTo(AudioClip clip, float to, float duration, bool ignoreTimeScale, FadeBehaviour onCompleteBehaviour, TweenCallback onComplete)
        {
            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup group = audioGroups[i];
                int slen = group.sources.Count;
                for (int c = 0; c < slen; c++) {
                    DeAudioSource s = group.sources[c];
                    if (s.clip == clip) s.FadeTo(to, duration, ignoreTimeScale, onCompleteBehaviour, onComplete);
                }
            }
        }

        /// <summary>
        /// Fades out then stops all sources in the given <see cref="DeAudioClipData"/> group,
        /// while starting the given <see cref="DeAudioClipData"/> with a fade-in effect.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Crossfade(DeAudioClipData clipData, float fadeDuration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onfadeOutBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        { return Crossfade(clipData.groupId, clipData.clip, clipData.volume, clipData.pitch, clipData.loop, fadeDuration, ignoreTimeScale, onfadeOutBehaviour, onComplete); }
        /// <summary>
        /// Fades out then stops all sources in the given group, while starting the given clip with a fade-in effect.
        /// <para>Returns the <see cref="DeAudioSource"/> instance used to play, or NULL if the clip couldn't be played</para>
        /// </summary>
        public static DeAudioSource Crossfade(DeAudioGroupId groupId, AudioClip clip, float volume = 1, float pitch = 1, bool loop = false, float fadeDuration = 1.5f, bool ignoreTimeScale = true, FadeBehaviour onfadeOutBehaviour = FadeBehaviour.Stop, TweenCallback onComplete = null)
        {
            DeAudioGroup group = GetAudioGroup(groupId);
            if (group == null) {
                Debug.LogWarning(LogPrefix + "Crossfade can't happend and clip can't be played because no group with the given groupId (" + groupId + ") was created");
                return null;
            }
            return group.Crossfade(clip, volume, pitch, loop, fadeDuration, ignoreTimeScale, onfadeOutBehaviour, onComplete);
        }

        #endregion

        #endregion

        #region Methods

        void InitializeAudioGroups()
        {
            globalGroup = new DeAudioGroup(DeAudioGroupId.INTERNAL_Global);
            globalGroup.Init(this.transform, "Global [auto]");
            int len = I.fooAudioGroups == null ? 0 : I.fooAudioGroups.Length;
            audioGroups = new DeAudioGroup[len + 1];
            for (int i = 0; i < len; ++i) {
                DeAudioGroup g = I.fooAudioGroups[i];
                g.Init(this.transform);
                audioGroups[i] = g;
            }
            audioGroups[len] = globalGroup;
            fooAudioGroups = null;
        }

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
                case OperationType.StopIfPaused:
                    group.StopAllPaused();
                    break;
                case OperationType.Pause:
                    group.Pause();
                    break;
                case OperationType.PauseByClip:
                    group.Pause(clip);
                    break;
                case OperationType.Resume:
                    group.Resume();
                    break;
                case OperationType.ResumeByClip:
                    group.Resume(clip, floatValue);
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