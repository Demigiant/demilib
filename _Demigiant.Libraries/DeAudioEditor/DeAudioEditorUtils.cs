// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 19:59
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DeAudio;

namespace DG.DeAudioEditor
{
    internal static class DeAudioEditorUtils
    {
        /// <summary>
        /// Check if there are duplicate DeAudioGroupIds in the DeAudioGroups added to DeAudioManager
        /// </summary>
        /// <returns></returns>
        internal static void CheckForDuplicateAudioGroupIds(DeAudioGroup[] audioGroups, List<DeAudioGroupId> fillWithDuplicates)
        {
            fillWithDuplicates.Clear();
            if (audioGroups == null) return;

            int len = audioGroups.Length;
            for (int i = 0; i < len; ++i) {
                DeAudioGroup groupA = audioGroups[i];
                for (int c = i + 1; c < len; ++c) {
                    DeAudioGroup groupB = audioGroups[c];
                    if (groupA.id != groupB.id) continue;
                    if (!fillWithDuplicates.Contains(groupA.id)) fillWithDuplicates.Add(groupA.id);
                }
            }
        }
    }
}