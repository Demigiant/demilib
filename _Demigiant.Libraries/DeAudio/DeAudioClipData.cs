// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 16:26
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeAudio
{
    [System.Serializable]
    public class DeAudioClipData
    {
        public DeAudioGroupId groupId;
        public AudioClip clip;
        public float volume = 1;
        public float pitch = 1;
        public bool loop = false;

        public DeAudioClipData(DeAudioGroupId groupId, float volume = 1, float pitch = 1, bool loop = false)
        {
            this.groupId = groupId;
            this.volume = volume;
            this.pitch = pitch;
            this.loop = loop;
        }

        public DeAudioClipData(DeAudioGroupId groupId, bool loop)
        {
            this.groupId = groupId;
            this.loop = loop;
        }

        public DeAudioClipData()
        {
            volume = 1;
            pitch = 1;
            loop = false;
        }

        #region Public Methods

        /// <summary>
        /// Plays this DeAudioClipData
        /// </summary>
        public DeAudioSource Play()
        {
            return DeAudioManager.Play(this);
        }

        /// <summary>
        /// Stops all sounds using this DeAudioClipData clip
        /// </summary>
        public void Stop()
        {
            DeAudioManager.Stop(this.clip);
        }

        /// <summary>
        /// Pauses all sounds using this DeAudioClipData clip
        /// </summary>
        public void Pause()
        {
            DeAudioManager.Pause(this.clip);
        }

        /// <summary>
        /// If the clip is active in an AudioSource (even if not playing) returns it,
        /// otherwise returns NULL
        /// </summary>
        /// <param name="ignorePaused">If TRUE ignores sources that are not playing</param>
        /// <param name="ignoreFadingOut">If TRUE ignores sources that are fading out</param>
        public DeAudioSource GetAudioSource(bool ignorePaused = false, bool ignoreFadingOut = true)
        {
            return DeAudioManager.GetAudioGroup(groupId).GetExistingAudioSourceFor(clip, ignorePaused, ignoreFadingOut);
        }

        #region Info Methods

        /// <summary>
        /// Returns TRUE if this clip is currently playing
        /// </summary>
        public bool IsPlaying()
        {
            if (clip == null) return false;
            return DeAudioManager.GetAudioGroup(groupId).IsPlaying(clip);
        }

        #endregion

        #endregion
    }
}