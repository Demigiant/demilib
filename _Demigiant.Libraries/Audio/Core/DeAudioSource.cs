// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 18:45
// License Copyright (c) Daniele Giardini

using System.Collections;
using UnityEngine;

namespace DG.Audio.Core
{
    /// <summary>
    /// Must be instantiate via DeAudioSource.Instantiate()
    /// </summary>
    internal class DeAudioSource : MonoBehaviour
    {
        internal DeAudioType type;
        internal float startTime { get; private set; } // Time since the last Play call
        internal float originalVolume { get; private set; } // Last volume set (source only, ignores global volumes)
        AudioSource _src;

        internal AudioClip clip { get { return _src.clip; } }
        internal bool isPlaying { get { return _src.isPlaying; } }

        // ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        // INIT

        internal static DeAudioSource Instantiate(DeAudioType type, string name)
        {
            GameObject go = new GameObject(name);
            DeAudioSource res = go.AddComponent<DeAudioSource>();
            res.type = type;
            res._src = go.AddComponent<AudioSource>();
            return res;
        }

        #region Unity Methods

        void OnDestroy()
        {
            this.StopAllCoroutines();
        }

        #endregion

        #region Internal Methods

        internal void Play(AudioClip clip, float volume, bool loop)
        {
            this.StopAllCoroutines();
            startTime = Time.realtimeSinceStartup;
            originalVolume = volume;
            _src.clip = clip;
            _src.volume = volume * DeAudioManager.GetVolumeMultiplierByType(type);
            _src.loop = loop;
            _src.Play();
        }

        #endregion

        #region Coroutines

        internal IEnumerator FadeOutCoroutine(float fadeTime, bool ignoreTimeScale)
        {
            float fromVolume = _src.volume;
            float elapsed = 0;
            while (_src.isPlaying && _src.volume > 0) {
                elapsed += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                float toVolume = fromVolume - ((fromVolume * elapsed) / fadeTime);
                if (toVolume < 0) toVolume = 0;
                _src.volume = toVolume;
                yield return null;
            }
        }

        #endregion
    }
}