// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/11 15:48
// License Copyright (c) Daniele Giardini

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
{
    public enum DePosition
    {
        /// <summary>Default inspector width excluding label-reserved space</summary>
        HDefault,
        /// <summary>Default inspector width including label-reserved space</summary>
        HExtended,
        /// <summary>Half of inspector width (extended or default depending on the attribute drawn), left side</summary>
        HHalfLeft,
        /// <summary>Half of inspector width (extended or default depending on the attribute drawn), right side</summary>
        HHalfRight,
        /// <summary>One third of inspector width (extended or default depending on the attribute drawn), left side</summary>
        HThirdLeft,
        /// <summary>One third of inspector width (extended or default depending on the attribute drawn), middle</summary>
        HThirdMiddle,
        /// <summary>One third of inspector width (extended or default depending on the attribute drawn), right side</summary>
        HThirdRight
    }
}