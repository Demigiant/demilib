// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/10 14:23
// License Copyright (c) Daniele Giardini
namespace DG.DeInspektor.Attributes
{
    /// <summary>
    /// Appearance of button attributes
    /// </summary>
    public enum DeButtonMode
    {
        /// <summary>Enabled always</summary>
        Default,
        /// <summary>Enabled only if application is playing</summary>
        PlayModeOnly,
        /// <summary>Enabled only if application is not playing</summary>
        NoPlayMode
    }
}