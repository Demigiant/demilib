// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/04/13 15:45
// License Copyright (c) Daniele Giardini
namespace DG.DeAudio
{
    /// <summary>
    /// Display mode used when drawing <see cref="DeAudioClipData"/> objects in a custom editor.
    /// Public so it can be serialized for advanced editors.
    /// </summary>
    public enum DeAudioClipGUIMode
    {
        /// <summary>1 row > AudioClip only</summary>
        ClipOnly, // Only clip, no groupId
        /// <summary>1 row > AudioClip and GroupId</summary>
        Compact,
        /// <summary>1 row > AudioClip and play/stop buttons</summary>
        CompactPreviewOnly,
        /// <summary>1 row > AudioClip, GroupId and play/stop buttons</summary>
        CompactWithGroupAndPreview,
        /// <summary>2 rows > AudioClip, GroupId, Volume, play/stop buttons</summary>
        VolumeWithPreview,
        /// <summary>2 rows > AudioClip, GroupId, Volume, play/stop/loops buttons</summary>
        VolumeAndLoopsWithPreview,
        /// <summary>3 rows > All options except GroupId</summary>
        FullNoGroup,
        /// <summary>3 rows > All options</summary>
        Full
    }
}