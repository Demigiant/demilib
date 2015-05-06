// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 20:17

using DG.DemiLib;
using UnityEngine;

[System.Serializable]
public class CustomColorPalette : DeColorPalette
{
    public DeSkinColor toolbarBg = new DeSkinColor(new Color(0.4f, 0.4f, 0.4f, 1), new Color(0.4f, 0.4f, 0.4f, 1));
    public DeSkinColor toolbarContent = new DeSkinColor(new Color(0.9f, 0.9f, 0.9f, 1), new Color(0.7f, 0.7f, 0.7f, 1));
}