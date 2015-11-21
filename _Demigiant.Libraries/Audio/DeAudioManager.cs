// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 18:39
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.Audio.Core;
using UnityEngine;

namespace DG.Audio
{
    /// <summary>
    /// Global AudioManager.
    /// Must be instantiated only once per project (either manually or via code).
    /// </summary>
    public class DeAudioManager : MonoBehaviour
    {
        // SERIALIZED ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬

#pragma warning disable 1591
        public float masterVolume = 1;
        public float fxVolume = 1;
        public float ambientVolume = 1;
        public float musicVolume = 1;
        public int maxFxSources = -1; // -1 for unlimited - can't be changed after startup
        public int maxAmbientSources = -1; // -1 for unlimited - can't be changed after startup
        public int maxMusicSources = -1; // -1 for unlimited - can't be changed after startup
        public bool culling = true; // If TRUE, when a new sound wants to play and there's too many sources, the oldest playing one will be used. Otherwise the sound won't play
        public bool logInfo = false;
#pragma warning restore 1591

        static DeAudioManager I;
        static List<DeAudioSource> _sources, _fxSources, _ambientSources, _musicSources;

        #region Unity Methods

        void Awake()
        {
            I = this;
            _sources = maxFxSources > 0 && maxAmbientSources > 0 && maxMusicSources > 0
                ? new List<DeAudioSource>(maxFxSources + maxAmbientSources + maxMusicSources)
                : new List<DeAudioSource>();
            _fxSources = maxFxSources > 0 ? new List<DeAudioSource>(maxFxSources) : new List<DeAudioSource>();
            _ambientSources = maxAmbientSources > 0 ? new List<DeAudioSource>(maxAmbientSources) : new List<DeAudioSource>();
            _musicSources = maxMusicSources > 0 ? new List<DeAudioSource>(maxMusicSources) : new List<DeAudioSource>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the given amount of sources, if within the set max limits
        /// </summary>
        public static void PrecacheSources(DeAudioType type, int cacheAmount)
        {
            I.DOPrecacheSources(type, cacheAmount);
        }

        #region Public Play Methods

        /// <summary>
        /// Plays the given clip with the given options
        /// </summary>
        public static void Play(DeAudioType type, AudioClip clip, bool loop)
        { DoPlay(type, clip, 1, loop); }
        /// <summary>
        /// Plays the given clip with the given options
        /// </summary>
        public static void Play(DeAudioType type, AudioClip clip, float volume = 1, bool loop = false)
        { DoPlay(type, clip, volume, loop); }

        /// <summary>
        /// Fades out all playing audios of the given type
        /// </summary>
        public static void FadeOutAllOfType(DeAudioType type, float fadeTime = 1, bool ignoreTimeScale = true)
        {
            List<DeAudioSource> li = GetSourceListByType(type);
            int len = li.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = li[i];
                if (s.isPlaying) s.StartCoroutine(s.FadeOutCoroutine(fadeTime, ignoreTimeScale));
            }
        }

        /// <summary>
        /// Fades out all playing instances of the given clip
        /// </summary>
        public static void FadeOut(AudioClip clip, float fadeTime = 1, bool ignoreTimeScale = true)
        {
            int len = _sources.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = _sources[i];
                if (s.clip == clip && s.isPlaying) s.StartCoroutine(s.FadeOutCoroutine(fadeTime, ignoreTimeScale));
            }
        }

        #endregion

        #endregion

        #region Internal Methods

        internal static float GetVolumeMultiplierByType(DeAudioType type)
        {
            switch (type) {
            case DeAudioType.Ambient:
                return I.masterVolume * I.ambientVolume;
            case DeAudioType.Music:
                return I.masterVolume * I.musicVolume;
            default:
                return I.masterVolume * I.fxVolume;
            }
        }

        #endregion

        #region Methods

        void DOPrecacheSources(DeAudioType type, int cacheAmount)
        {
            List<DeAudioSource> li = null;
            switch (type) {
            case DeAudioType.FX:
                if (maxFxSources > -1) cacheAmount = Mathf.Min(cacheAmount, maxFxSources);
                li = _fxSources;
                break;
            case DeAudioType.Ambient:
                if (maxAmbientSources > -1) cacheAmount = Mathf.Min(cacheAmount, maxAmbientSources);
                li = _ambientSources;
                break;
            case DeAudioType.Music:
                if (maxMusicSources > -1) cacheAmount = Mathf.Min(cacheAmount, maxMusicSources);
                li = _musicSources;
                break;
            }
            if (li == null) return;

            cacheAmount -= li.Count;
            while (cacheAmount > 0) {
                cacheAmount--;
                DeAudioSource s = CreateSource(type);
                li.Add(s);
                _sources.Add(s);
            }
        }

        static void DoPlay(DeAudioType type, AudioClip clip, float volume, bool loop)
        {
            DeAudioSource src = CheckForFreeSource(type);
            if (src == null) {
                List<DeAudioSource> li = GetSourceListByType(type);
                int maxSrcs = GetMaxSourcesByType(type);
                if (maxSrcs == -1 || li.Count < maxSrcs) {
                    // Add new src
                    src = I.CreateSource(type);
                    li.Add(src);
                    _sources.Add(src);
                } else {
                    // Too many sources already
                    if (I.culling) {
                        // Find oldest src and use that
                        src = FindOldestPlayingSourceInList(li);
                    } else {
                        // Don't play the clip
                        if (I.logInfo) Debug.LogWarning("Clip couldn't be played because max clips for " + type + " have been reached (activate culling to reuse the oldest clip instead)");
                        return;
                    }
                }
            }
            src.Play(clip, volume, loop);
        }

        #region Utils

        // NOTE: doesn't add the source to the corresponding list
        DeAudioSource CreateSource(DeAudioType type)
        {
            DeAudioSource src = DeAudioSource.Instantiate(type, type.ToString());
            src.transform.parent = transform;
            return src;
        }

        // Returns a non-playing existing source, or NULL if none was found for the given type
        static DeAudioSource CheckForFreeSource(DeAudioType type)
        {
            List<DeAudioSource> li = GetSourceListByType(type);
            if (li == null) return null;
            int len = li.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource s = li[i];
                if (!s.isPlaying) return s;
            }
            return null;
        }

        static List<DeAudioSource> GetSourceListByType(DeAudioType type)
        {
            switch (type) {
            case DeAudioType.Ambient:
                return _ambientSources;
            case DeAudioType.Music:
                return _musicSources;
            default:
                return _fxSources;
            }
        }

        static int GetMaxSourcesByType(DeAudioType type)
        {
            switch (type) {
            case DeAudioType.Ambient:
                return I.maxAmbientSources;
            case DeAudioType.Music:
                return I.maxMusicSources;
            default:
                return I.maxFxSources;
            }
        }

        static DeAudioSource FindOldestPlayingSourceInList(List<DeAudioSource> list)
        {
            DeAudioSource res = null;
            float time = float.MaxValue;
            int len = list.Count;
            for (int i = 0; i < len; ++i) {
                DeAudioSource src = list[i];
                if (src.startTime < time) {
                    res = src;
                    time = src.startTime;
                }
            }
            return res;
        }

        #endregion

        #endregion
    }
}