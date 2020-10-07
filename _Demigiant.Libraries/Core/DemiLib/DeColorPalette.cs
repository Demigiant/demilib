// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 19:25

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib
{
    public enum ToggleColors
    {
        DefaultOn, DefaultOff, Critical, Yellow, Orange, Blue, Cyan, Purple
    }

    /// <summary>
    /// Stores a color palette, which can be passed to default DeGUI layouts when calling <code>DeGUI.BeginGUI</code>,
    /// and changed at any time by calling <code>DeGUI.ChangePalette</code>.
    /// You can inherit from this class to create custom color palettes with more hColor options.
    /// </summary>
    [System.Serializable]
    public class DeColorPalette
    {
        public DeColorGlobal global = new DeColorGlobal();
        public DeColorBG bg = new DeColorBG();
        public DeColorContent content = new DeColorContent();
        public DeToggleColors toggle = new DeToggleColors();

        #region Public Methods

        /// <summary>
        /// Converts a HEX color to a Unity Color and returns it
        /// </summary>
        /// <param name="hex">The HEX color, either with or without the initial # (accepts both regular and short format)</param>
        public static Color HexToColor(string hex)
        {
            if (hex[0] == '#') hex = hex.Substring(1);
            int len = hex.Length;
            bool isShortFormat = len < 6;
            if (isShortFormat) {
                float r = (HexToInt(hex[0]) + HexToInt(hex[0]) * 16f) / 255f;
                float g = (HexToInt(hex[1]) + HexToInt(hex[1]) * 16f) / 255f;
                float b = (HexToInt(hex[2]) + HexToInt(hex[2]) * 16f) / 255f;
                float a = len == 4 ? (HexToInt(hex[3]) + HexToInt(hex[3]) * 16f) / 255f : 1;
                return new Color(r, g, b, a);
            } else {
                float r = (HexToInt(hex[1]) + HexToInt(hex[0]) * 16f) / 255f;
                float g = (HexToInt(hex[3]) + HexToInt(hex[2]) * 16f) / 255f;
                float b = (HexToInt(hex[5]) + HexToInt(hex[4]) * 16f) / 255f;
                float a = len == 8 ? (HexToInt(hex[7]) + HexToInt(hex[6]) * 16f) / 255f : 1;
                return new Color(r, g, b, a);
            }
        }

        #endregion

        #region Methods

        static int HexToInt(char hexVal)
        {
            return int.Parse(hexVal.ToString(), System.Globalization.NumberStyles.HexNumber);
        }

        #endregion
    }

    /// <summary>
    /// Global colors
    /// </summary>
    [System.Serializable]
    public class DeColorGlobal
    {
        public Color black = Color.black;
        public Color white = Color.white;
        public Color blue = new Color(0f, 0.4f, 0.91f);
        public Color green = new Color(0.11f, 0.84f, 0.02f);
        public Color orange = new Color(0.98f, 0.44f, 0f);
        public Color purple = new Color(0.67f, 0.17f, 0.87f);
        public Color red = new Color(0.93f, 0.04f, 0.04f);
        public Color yellow = new Color(0.93f, 0.77f, 0.04f);
    }

    /// <summary>
    /// Background colors
    /// </summary>
    [System.Serializable]
    public class DeColorBG
    {
        /// <summary>Editor background color</summary>
        public DeSkinColor editor = new DeSkinColor((Color)new Color32(194, 194, 194, 255), (Color)new Color32(56, 56, 56, 255));
        public DeSkinColor def = Color.white;
        public DeSkinColor critical = new DeSkinColor(new Color(0.9411765f, 0.2388736f, 0.006920422f, 1f), new Color(1f, 0.2482758f, 0f, 1f));
        public DeSkinColor divider = new DeSkinColor(0.6f, 0.3f);
        public DeSkinColor toggleOn = new DeSkinColor(new Color(0.3158468f, 0.875f, 0.1351103f, 1f), new Color(0.2183823f, 0.7279412f, 0.09099264f, 1f));
        public DeSkinColor toggleOff = new DeSkinColor(0.75f, 0.4f);
        public DeSkinColor toggleCritical = new DeSkinColor(new Color(0.9411765f, 0.2388736f, 0.006920422f, 1f), new Color(1f, 0.2482758f, 0f, 1f));
    }

    /// <summary>
    /// Content colors
    /// </summary>
    [System.Serializable]
    public class DeColorContent
    {
        public DeSkinColor def = new DeSkinColor(Color.black, new Color(0.7f, 0.7f, 0.7f, 1));
        public DeSkinColor critical = new DeSkinColor(new Color(1f, 0.9148073f, 0.5588235f, 1f), new Color(1f, 0.3881846f, 0.3014706f, 1f));
        public DeSkinColor toggleOn = new DeSkinColor(new Color(1f, 0.9686275f, 0.6980392f, 1f), new Color(0.8025267f, 1f, 0.4705882f, 1f));
        public DeSkinColor toggleOff = new DeSkinColor(new Color(0.4117647f, 0.3360727f, 0.3360727f, 1f), new Color(0.6470588f, 0.5185986f, 0.5185986f, 1f));
        public DeSkinColor toggleCritical = new DeSkinColor(new Color(1f, 0.84f, 0.62f), new Color(1f, 0.84f, 0.62f));
    }

    /// <summary>
    /// Toggle button specific colors
    /// </summary>
    [System.Serializable]
    public class DeToggleColors
    {
        public DeSkinColor bgOn = new DeSkinColor(new Color(0.3158468f, 0.875f, 0.1351103f, 1f), new Color(0.2183823f, 0.7279412f, 0.09099264f, 1f));
        public DeSkinColor bgOff = new DeSkinColor(0.75f, 0.4f);
        public DeSkinColor bgCritical = new DeSkinColor(new Color(0.9411765f, 0.2388736f, 0.006920422f, 1f), new Color(1f, 0.2482758f, 0f, 1f));
        public DeSkinColor bgYellow = new DeSkinColor(new Color(0.93f, 0.77f, 0.04f));
        public DeSkinColor bgOrange = new DeSkinColor(new Color(0.98f, 0.44f, 0f));
        public DeSkinColor bgBlue = new DeSkinColor(new Color(0f, 0.4f, 0.91f));
        public DeSkinColor bgCyan = new DeSkinColor(new Color(0f, 0.79f, 1f));
        public DeSkinColor bgPurple = new DeSkinColor(new Color(0.67f, 0.17f, 0.87f));

        public DeSkinColor contentOn = new DeSkinColor(new Color(1f, 0.9686275f, 0.6980392f, 1f), new Color(0.8025267f, 1f, 0.4705882f, 1f));
        public DeSkinColor contentOff = new DeSkinColor(new Color(0.4117647f, 0.3360727f, 0.3360727f, 1f), new Color(0.6470588f, 0.5185986f, 0.5185986f, 1f));
        public DeSkinColor contentCritical = new DeSkinColor(new Color(1f, 0.84f, 0.62f), new Color(1f, 0.84f, 0.62f));
        public DeSkinColor contentYellow = new DeSkinColor(new Color(1f, 1f, 0.64f));
        public DeSkinColor contentOrange = new DeSkinColor(new Color(1f, 0.96f, 0.57f));
        public DeSkinColor contentBlue = new DeSkinColor(new Color(0.35f, 0.96f, 0.94f));
        public DeSkinColor contentCyan = new DeSkinColor(new Color(0.62f, 1f, 0.89f));
        public DeSkinColor contentPurple = new DeSkinColor(new Color(1f, 0.81f, 0.98f));

        public ColorCombination GetColors(ToggleColors offType, ToggleColors onType)
        {
            return new ColorCombination(GetColor2(offType), GetColor2(onType));
        }

        Color2 GetColor2(ToggleColors type)
        {
            Color2 result = new Color2();
            switch (type) {
            case ToggleColors.DefaultOn:
                result.bg = bgOn;
                result.content = contentOn;
                break;
            case ToggleColors.DefaultOff:
                result.bg = bgOff;
                result.content = contentOff;
                break;
            case ToggleColors.Critical:
                result.bg = bgCritical;
                result.content = contentCritical;
                break;
            case ToggleColors.Yellow:
                result.bg = bgYellow;
                result.content = contentYellow;
                break;
            case ToggleColors.Orange:
                result.bg = bgOrange;
                result.content = contentOrange;
                break;
            case ToggleColors.Blue:
                result.bg = bgBlue;
                result.content = contentBlue;
                break;
            case ToggleColors.Cyan:
                result.bg = bgCyan;
                result.content = contentCyan;
                break;
            case ToggleColors.Purple:
                result.bg = bgPurple;
                result.content = contentPurple;
                break;
            }
            return result;
        }

        public class ColorCombination
        {
            public Color2 offCols, onCols;

            public ColorCombination(Color2 offCombination, Color2 onCombination)
            {
                this.offCols = offCombination;
                this.onCols = onCombination;
            }
        }

        public class Color2
        {
            public Color bg, content;
        }
    }
}