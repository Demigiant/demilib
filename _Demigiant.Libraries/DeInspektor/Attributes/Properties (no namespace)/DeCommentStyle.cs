// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/12 13:33
// License Copyright (c) Daniele Giardini
namespace DG.DeInspektor.Attributes
{
    /// <summary>Comment style</summary>
    public enum DeCommentStyle
    {
        /// <summary>Text inside a box</summary>
        Box,
        /// <summary>Text inside a box, extended to encompass the full width of the Inspector</summary>
        BoxExtended,
        /// <summary>Text only</summary>
        TextOnly,
        /// <summary>Text only, placed inside the value area and not encompassing the label area</summary>
        TextInValueArea,
        /// <summary>Box that wraps the next line</summary>
        WrapNextLine
    }
}