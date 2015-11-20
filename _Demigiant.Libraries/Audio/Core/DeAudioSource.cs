// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/20 18:45
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.Audio.Core
{
    /// <summary>
    /// Must be instantiate via DeAudioSource.Instantiate()
    /// </summary>
    internal class DeAudioSource : MonoBehaviour
    {
        internal DeAudioType type;
        internal float startTime; // Time since the last Play call
        AudioSource _src;

        // ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬
        // INIT

        public static DeAudioSource Instantiate(DeAudioType type, string name)
        {
            GameObject go = new GameObject(name);
            DeAudioSource res = go.AddComponent<DeAudioSource>();
            res.type = type;
            res._src = go.AddComponent<AudioSource>();
            return res;
        }

        #region Public Methods

        public void Play(AudioClip clip, float volume, bool loop)
        {
            startTime = Time.realtimeSinceStartup;
            _src.clip = clip;
            _src.volume = volume;
            _src.loop = loop;
            _src.Play();
        }

        #region Info Methods

        public bool IsPlaying()
        {
            return _src.isPlaying;
        }

        #endregion

        #endregion
    }
}