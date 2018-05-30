// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/05/30 10:23
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeEditorTools.Project
{
    internal static class DeProjectUtils
    {
        #region Public Methods

        public static Color HColorToColor(DeProjectData.HColor color, Color? customColor = null)
        {
            switch (color) {
            case DeProjectData.HColor.Custom: return customColor == null ? Color.white : (Color)customColor;
            case DeProjectData.HColor.Blue: return new Color(0.22f, 0.47f, 0.96f);
            case DeProjectData.HColor.BrightBlue: return new Color(0.27f, 0.68f, 1f);
            case DeProjectData.HColor.Green: return new Color(0.05060553f, 0.8602941f, 0.2237113f, 1f);
            case DeProjectData.HColor.Orange: return new Color(0.9558824f, 0.4471125f, 0.05622837f, 1f);
            case DeProjectData.HColor.Purple: return new Color(0.907186f, 0.05406574f, 0.9191176f, 1f);
            case DeProjectData.HColor.Violet: return new Color(0.5797163f, 0.1764706f, 1f, 1f);
            case DeProjectData.HColor.Red: return new Color(0.9191176f, 0.1617312f, 0.07434041f, 1f);
            case DeProjectData.HColor.Yellow: return new Color(1f, 0.853854f, 0.03676468f, 1f);
            case DeProjectData.HColor.BrightGrey: return new Color(0.6470588f, 0.6470588f, 0.6470588f, 1f);
            case DeProjectData.HColor.DarkGrey: return new Color(0.3308824f, 0.3308824f, 0.3308824f, 1f);
            case DeProjectData.HColor.Black: return Color.black;
            case DeProjectData.HColor.White: return Color.white;
            default: return Color.white;
            }
        }

        #endregion
    }
}