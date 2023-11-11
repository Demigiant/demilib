// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2023/11/11

using System;

namespace DG.DeAudio
{
    /// <summary>
    /// Used to store variants for the same sfx and return a smart random one when needed
    /// </summary>
    [Serializable]
    public class DeAudioClipDataVariant
    {
        public DeAudioClipData[] variants;

        int _lastReturnedIndex = -1;

        #region Public Methods

        /// <summary>
        /// Returns a random <see cref="DeAudioClipData"/> among its variants,
        /// but never returns the same one twice in a row (unless there's only one variant)
        /// </summary>
        /// <returns></returns>
        public DeAudioClipData GetClipData()
        {
            int len = variants.Length;
            if (len == 0) return null;
            if (len == 1) return variants[0];
            int index = UnityEngine.Random.Range(0, len);
            while (index == _lastReturnedIndex) index = UnityEngine.Random.Range(0, len);
            _lastReturnedIndex = index;
            return variants[index];
        }

        #endregion
    }
}