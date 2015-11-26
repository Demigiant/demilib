// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/21 19:55
// License Copyright (c) Daniele Giardini

#pragma warning disable 1591
namespace DG.DeAudio
{
    public enum DeAudioGroupId
    {
        Ambient,
        Cutscene,
        Dialogue,
        FX,
        Music,
        Special,
        UI,
        Custom0,
        Custom1,
        Custom2,
        Custom3,
        Custom4,
        Custom5,
        Custom6,
        Custom7,
        Custom8,
        Custom9,
        Custom10,

        /// <summary>Don't use/assign this! It's assigned automatically to the global group</summary>
        INTERNAL_Global
    }
}