// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/05/06 12:02

using DG.DemiLib.Core;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiEditor
{
    /// <summary>
    /// Contains both free and pro skins GUIStyle variations,
    /// and automatically returns the correct one when converted to GUIStyle
    /// </summary>
    public struct DeSkinStyle
    {
        public GUIStyle free, pro;

        public DeSkinStyle(GUIStyle free, GUIStyle pro)
        {
            this.free = free;
            this.pro = pro;
        }

        public DeSkinStyle(GUIStyle style) : this()
        {
            free = style;
            pro = new GUIStyle(style);
        }

        public static implicit operator GUIStyle(DeSkinStyle v)
        {
            return GUIUtils.isProSkin ? v.pro : v.free;
        }

        public static implicit operator DeSkinStyle(GUIStyle v)
        {
            return new DeSkinStyle(v);
        }
    }
}