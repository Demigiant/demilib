// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/29 18:54

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiEditor
{
    /// <summary>
    /// Stores a GUIStyle palette, which can be passed to default DeGUI layouts when calling <code>DeGUI.BeginGUI</code>,
    /// and changed at any time by calling <code>DeGUI.ChangePalette</code>.
    /// You can inherit from this class to create custom GUIStyle palettes with more options.
    /// Each of the sub-options require a public Init method to initialize the styles, which will be called via Reflection.
    /// </summary>
    public class DeStylePalette
    {
        public readonly BoxStyles box = new BoxStyles();
        public readonly ButtonStyles button = new ButtonStyles();
        public readonly LabelStyles label = new LabelStyles();
        public readonly ToolbarStyles toolbar = new ToolbarStyles();
        public readonly MiscStyles misc = new MiscStyles();

        protected bool initialized;

        // ADB path to Imgs directory, final slash included
        static string _adbImgsDir {
            get { if (_fooAdbImgsDir == null) _fooAdbImgsDir = Assembly.GetExecutingAssembly().ADBDir() + "/Imgs/"; return _fooAdbImgsDir; }
        }
        static string _fooAdbImgsDir;

        #region Texture2D

        public static Texture2D whiteSquare { get {
            if (_fooWhiteSquare == null) _fooWhiteSquare = LoadSquareTexture("whiteSquare");
            return _fooWhiteSquare;
        }}
        public static Texture2D whiteSquareAlpha10 { get {
            if (_fooWhiteSquareAlpha10 == null) _fooWhiteSquareAlpha10 = LoadSquareTexture("whiteSquareAlpha10");
            return _fooWhiteSquareAlpha10;
        }}
        public static Texture2D whiteSquareAlpha15 { get {
            if (_fooWhiteSquareAlpha15 == null) _fooWhiteSquareAlpha15 = LoadSquareTexture("whiteSquareAlpha15");
            return _fooWhiteSquareAlpha15;
        }}
        public static Texture2D whiteSquareAlpha25 { get {
            if (_fooWhiteSquareAlpha25 == null) _fooWhiteSquareAlpha25 = LoadSquareTexture("whiteSquareAlpha25");
            return _fooWhiteSquareAlpha25;
        }}
        public static Texture2D whiteSquareAlpha50 { get {
            if (_fooWhiteSquareAlpha50 == null) _fooWhiteSquareAlpha50 = LoadSquareTexture("whiteSquareAlpha50");
            return _fooWhiteSquareAlpha50;
        }}
        public static Texture2D whiteSquareAlpha80 { get {
            if (_fooWhiteSquareAlpha80 == null) _fooWhiteSquareAlpha80 = LoadSquareTexture("whiteSquareAlpha80");
            return _fooWhiteSquareAlpha80;
        }}
        public static Texture2D blackSquare { get {
            if (_fooBlackSquare == null) _fooBlackSquare = LoadSquareTexture("blackSquare");
            return _fooBlackSquare;
        }}
        public static Texture2D blackSquareAlpha10 { get {
            if (_fooBlackSquareAlpha10 == null) _fooBlackSquareAlpha10 = LoadSquareTexture("blackSquareAlpha10");
            return _fooBlackSquareAlpha10;
        }}
        public static Texture2D blackSquareAlpha15 { get {
            if (_fooBlackSquareAlpha15 == null) _fooBlackSquareAlpha15 = LoadSquareTexture("blackSquareAlpha15");
            return _fooBlackSquareAlpha15;
        }}
        public static Texture2D blackSquareAlpha25 { get {
            if (_fooBlackSquareAlpha25 == null) _fooBlackSquareAlpha25 = LoadSquareTexture("blackSquareAlpha25");
            return _fooBlackSquareAlpha25;
        }}
        public static Texture2D blackSquareAlpha50 { get {
            if (_fooBlackSquareAlpha50 == null) _fooBlackSquareAlpha50 = LoadSquareTexture("blackSquareAlpha50");
            return _fooBlackSquareAlpha50;
        }}
        public static Texture2D blackSquareAlpha80 { get {
            if (_fooBlackSquareAlpha80 == null) _fooBlackSquareAlpha80 = LoadSquareTexture("blackSquareAlpha80");
            return _fooBlackSquareAlpha80;
        }}
        public static Texture2D redSquare { get {
            if (_fooRedSquare == null) _fooRedSquare = LoadSquareTexture("redSquare");
            return _fooRedSquare;
        }}
        public static Texture2D orangeSquare { get {
            if (_fooOrangeSquare == null) _fooOrangeSquare = LoadSquareTexture("orangeSquare");
            return _fooOrangeSquare;
        }}
        public static Texture2D yellowSquare { get {
            if (_fooYellowSquare == null) _fooYellowSquare = LoadSquareTexture("yellowSquare");
            return _fooYellowSquare;
        }}
        public static Texture2D greenSquare { get {
            if (_fooGreenSquare == null) _fooGreenSquare = LoadSquareTexture("greenSquare");
            return _fooGreenSquare;
        }}
        public static Texture2D blueSquare { get {
            if (_fooBlueSquare == null) _fooBlueSquare = LoadSquareTexture("blueSquare");
            return _fooBlueSquare;
        }}
        public static Texture2D purpleSquare { get {
            if (_fooPurpleSquare == null) _fooPurpleSquare = LoadSquareTexture("purpleSquare");
            return _fooPurpleSquare;
        }}
        public static Texture2D squareBorderCurved { get {
            if (_fooSquareBorderCurved == null) _fooSquareBorderCurved = LoadSquareTexture("squareBorderCurved");
            return _fooSquareBorderCurved;
        }}
        public static Texture2D squareBorderCurvedEmpty { get {
            if (_fooSquareBorderCurvedEmpty == null) _fooSquareBorderCurvedEmpty = LoadSquareTexture("squareBorderCurvedEmpty");
            return _fooSquareBorderCurvedEmpty;
        }}
        public static Texture2D squareBorderCurvedAlpha { get {
            if (_fooSquareBorderCurvedAlpha == null) _fooSquareBorderCurvedAlpha = LoadSquareTexture("squareBorderCurvedAlpha");
            return _fooSquareBorderCurvedAlpha;
        }}
        public static Texture2D squareBorderCurved_darkBordersAlpha { get {
            if (_fooSquareBorderCurved_darkBordersAlpha == null) _fooSquareBorderCurved_darkBordersAlpha = LoadSquareTexture("squareBorderCurved_darkBordersAlpha");
            return _fooSquareBorderCurved_darkBordersAlpha;
        }}
        public static Texture2D whiteDot { get {
            if (_fooWhiteDot == null) _fooWhiteDot = LoadSquareTexture("whiteDot");
            return _fooWhiteDot;
        }}
        public static Texture2D whiteDot_darkBorder { get {
            if (_fooWhiteDot_darkBorder == null) _fooWhiteDot_darkBorder = LoadSquareTexture("whiteDot_darkBorder");
            return _fooWhiteDot_darkBorder;
        }}
        public static Texture2D whiteDot_whiteBorderAlpha { get {
            if (_fooWhiteDot_whiteBorderAlpha == null) _fooWhiteDot_whiteBorderAlpha = LoadSquareTexture("whiteDot_whiteBorderAlpha");
            return _fooWhiteDot_whiteBorderAlpha;
        }}
        static Texture2D _fooWhiteSquare;
        static Texture2D _fooWhiteSquareAlpha10;
        static Texture2D _fooWhiteSquareAlpha15;
        static Texture2D _fooWhiteSquareAlpha25;
        static Texture2D _fooWhiteSquareAlpha50;
        static Texture2D _fooWhiteSquareAlpha80;
        static Texture2D _fooBlackSquare;
        static Texture2D _fooBlackSquareAlpha10;
        static Texture2D _fooBlackSquareAlpha15;
        static Texture2D _fooBlackSquareAlpha25;
        static Texture2D _fooBlackSquareAlpha50;
        static Texture2D _fooBlackSquareAlpha80;
        static Texture2D _fooRedSquare;
        static Texture2D _fooOrangeSquare;
        static Texture2D _fooYellowSquare;
        static Texture2D _fooGreenSquare;
        static Texture2D _fooBlueSquare;
        static Texture2D _fooPurpleSquare;
        static Texture2D _fooSquareBorderCurved;
        static Texture2D _fooSquareBorderCurvedEmpty;
        static Texture2D _fooSquareBorderCurvedAlpha;
        static Texture2D _fooSquareBorderCurved_darkBordersAlpha;
        static Texture2D _fooWhiteDot;
        static Texture2D _fooWhiteDot_darkBorder;
        static Texture2D _fooWhiteDot_whiteBorderAlpha;
        public static Texture2D ico_alignTL { get {
            if (_fooIco_alignTL == null) _fooIco_alignTL = LoadSquareTexture("ico_alignTL");
            return _fooIco_alignTL;
        }}
        public static Texture2D ico_alignTC { get {
            if (_fooIco_alignTC == null) _fooIco_alignTC = LoadSquareTexture("ico_alignTC");
            return _fooIco_alignTC;
        }}
        public static Texture2D ico_alignTR { get {
            if (_fooIco_alignTR == null) _fooIco_alignTR = LoadSquareTexture("ico_alignTR");
            return _fooIco_alignTR;
        }}
        public static Texture2D ico_alignCL { get {
            if (_fooIco_alignCL == null) _fooIco_alignCL = LoadSquareTexture("ico_alignCL");
            return _fooIco_alignCL;
        }}
        public static Texture2D ico_alignCC { get {
            if (_fooIco_alignCC == null) _fooIco_alignCC = LoadSquareTexture("ico_alignCC");
            return _fooIco_alignCC;
        }}
        public static Texture2D ico_alignCR { get {
            if (_fooIco_alignCR == null) _fooIco_alignCR = LoadSquareTexture("ico_alignCR");
            return _fooIco_alignCR;
        }}
        public static Texture2D ico_alignBL { get {
            if (_fooIco_alignBL == null) _fooIco_alignBL = LoadSquareTexture("ico_alignBL");
            return _fooIco_alignBL;
        }}
        public static Texture2D ico_alignBC { get {
            if (_fooIco_alignBC == null) _fooIco_alignBC = LoadSquareTexture("ico_alignBC");
            return _fooIco_alignBC;
        }}
        public static Texture2D ico_alignBR { get {
            if (_fooIco_alignBR == null) _fooIco_alignBR = LoadSquareTexture("ico_alignBR");
            return _fooIco_alignBR;
        }}
        public static Texture2D ico_star { get {
            if (_fooIco_star == null) _fooIco_star = LoadSquareTexture("ico_star");
            return _fooIco_star;
        }}
        public static Texture2D ico_star_border { get {
            if (_fooIco_star_border == null) _fooIco_star_border = LoadSquareTexture("ico_star_border");
            return _fooIco_star_border;
        }}
        public static Texture2D ico_cog { get {
            if (_fooIco_cog == null) _fooIco_cog = LoadSquareTexture("ico_cog");
            return _fooIco_cog;
        }}
        public static Texture2D ico_cog_border { get {
            if (_fooIco_cog_border == null) _fooIco_cog_border = LoadSquareTexture("ico_cog_border");
            return _fooIco_cog_border;
        }}
        public static Texture2D ico_comment { get {
            if (_fooIco_comment == null) _fooIco_comment = LoadSquareTexture("ico_comment");
            return _fooIco_comment;
        }}
        public static Texture2D ico_comment_border { get {
            if (_fooIco_comment_border == null) _fooIco_comment_border = LoadSquareTexture("ico_comment_border");
            return _fooIco_comment_border;
        }}
        static Texture2D _fooIco_alignTL;
        static Texture2D _fooIco_alignTC;
        static Texture2D _fooIco_alignTR;
        static Texture2D _fooIco_alignCL;
        static Texture2D _fooIco_alignCC;
        static Texture2D _fooIco_alignCR;
        static Texture2D _fooIco_alignBL;
        static Texture2D _fooIco_alignBC;
        static Texture2D _fooIco_alignBR;
        static Texture2D _fooIco_star;
        static Texture2D _fooIco_star_border;
        static Texture2D _fooIco_cog;
        static Texture2D _fooIco_cog_border;
        static Texture2D _fooIco_comment;
        static Texture2D _fooIco_comment_border;

        #endregion

        /// <summary>
        /// Called automatically by <code>DeGUI.BeginGUI</code>.
        /// Override when adding new style subclasses.
        /// </summary>
        internal void Init()
        {
            if (initialized) return;

            initialized = true;

            // Default inits (made manually so they happen before subpalettes, which might be using them)
            box.Init();
            button.Init();
            label.Init();
            toolbar.Init();
            misc.Init();

            // Initialize custome subpalettes from inherited classes
            FieldInfo[] fieldInfos = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo fi in fieldInfos) {
                if (fi.FieldType.IsSubclassOf(typeof(DeStyleSubPalette))) ((DeStyleSubPalette)fi.GetValue(this)).Init();
            }
        }

        static Texture2D LoadSquareTexture(string name)
        {
            Texture2D tex = AssetDatabase.LoadAssetAtPath(String.Format("{0}{1}.png", _adbImgsDir, name), typeof(Texture2D)) as Texture2D;
            tex.SetGUIFormat(FilterMode.Point, 32);
            return tex;
        }
    }

    /// <summary>
    /// Extend any custom subpalettes from this, so they will be initialized correctly
    /// </summary>
    public abstract class DeStyleSubPalette
    {
        public abstract void Init();
    }

    public class BoxStyles
    {
        public GUIStyle def,
                        flat, flatAlpha10, flatAlpha25; // Flat with white background
        public GUIStyle sticky, stickyTop; // Without any margin (or only top margin)

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.box).Padding(6, 6, 6, 6);
            flat = new GUIStyle(def).Background(DeStylePalette.whiteSquare);
            flatAlpha10 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha10);
            flatAlpha25 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha25);
            sticky = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(0), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(0));
            stickyTop = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(7), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(7));
        }
    }

    public class ButtonStyles
    {
        public GUIStyle def,
                        tool, toolL, toolS, toolIco,
                        toolFoldoutClosed, toolFoldoutClosedWLabel, toolFoldoutClosedWStretchedLabel,
                        toolFoldoutOpen, toolFoldoutOpenWLabel, toolFoldoutOpenWStretchedLabel,
                        toolLFoldoutClosed, toolLFoldoutClosedWLabel, toolLFoldoutClosedWStretchedLabel,
                        toolLFoldoutOpen, toolLFoldoutOpenWLabel, toolLFoldoutOpenWStretchedLabel,
                        bBlankBorder;

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.button);
            tool = new GUIStyle(EditorStyles.toolbarButton).ContentOffsetY(-1);
            toolL = new GUIStyle(EditorStyles.toolbarButton).Height(23).ContentOffsetY(0);
            toolS = new GUIStyle(EditorStyles.toolbarButton).Height(13).ContentOffsetY(0).Padding(0);
            toolIco = new GUIStyle(tool).StretchWidth(false).Width(22).ContentOffsetX(-1);
//            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
//                alignment = TextAnchor.UpperLeft,
//                active = { background = null },
//                fixedWidth = 14,
//                normal = { background = EditorStyles.foldout.normal.background },
//                border = EditorStyles.foldout.border,
//                padding = new RectOffset(14, 0, 0, 0)
//            }.MarginTop(2);
            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleLeft,
                active = { background = null },
                fixedWidth = 14,
                normal = { background = EditorStyles.foldout.normal.background },
                border = EditorStyles.foldout.border,
                padding = new RectOffset(14, 0, 0, 0),
                margin = new RectOffset(0, 3, 0, 0),
                overflow = new RectOffset(-2, 0, -2, 0),
                stretchHeight = true,
                contentOffset = new Vector2(2, -1)
            };
            toolFoldoutClosedWLabel = toolFoldoutClosed.Clone(9).Width(0).StretchWidth(false);
            toolFoldoutClosedWStretchedLabel = toolFoldoutClosedWLabel.Clone().StretchWidth();
            toolFoldoutOpen = new GUIStyle(toolFoldoutClosed) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
            toolFoldoutOpenWLabel = new GUIStyle(toolFoldoutClosedWLabel) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
            toolFoldoutOpenWStretchedLabel = toolFoldoutOpenWLabel.Clone().StretchWidth();
            // Large
            toolLFoldoutClosed = toolFoldoutClosed.Clone().OverflowTop(-4);
            toolLFoldoutClosedWLabel = toolFoldoutClosedWLabel.Clone().OverflowTop(-4);
            toolLFoldoutClosedWStretchedLabel = toolFoldoutClosedWStretchedLabel.Clone().OverflowTop(-4);
            toolLFoldoutOpen = toolFoldoutOpen.Clone().OverflowTop(-4);
            toolLFoldoutOpenWLabel = toolFoldoutOpenWLabel.Clone().OverflowTop(-4);
            toolLFoldoutOpenWStretchedLabel = toolFoldoutOpenWStretchedLabel.Clone().OverflowTop(-4);
            // Custom using squareBorder
            bBlankBorder = new GUIStyle(GUI.skin.button).Add(TextAnchor.MiddleCenter, Color.white).Background(DeStylePalette.squareBorderCurved)
                .Padding(0, 1, 0, 0).ContentOffsetY(-1)
                .Border(new RectOffset(4, 4, 4, 4)).Overflow(-1, -1, 0, 0);
        }
    }

    public class LabelStyles
    {
        public GUIStyle bold,
                        wordwrap, wordwrapRichtText,
                        toolbar, toolbarL, toolbarS, toolbarBox;

        internal void Init()
        {
            bold = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold);
            wordwrap = new GUIStyle(GUI.skin.label).Add(Format.WordWrap);
            wordwrapRichtText = wordwrap.Clone(Format.RichText);
            toolbar = new GUIStyle(GUI.skin.label).Add(9).ContentOffset(new Vector2(-2, 0));
            toolbarL = new GUIStyle(toolbar).ContentOffsetY(3);
            toolbarS = new GUIStyle(toolbar).Add(8, FontStyle.Bold).ContentOffsetY(-2);
            toolbarBox = new GUIStyle(toolbar).ContentOffsetY(0);
        }
    }

    public class ToolbarStyles
    {
        public GUIStyle def,
                        large, small,
                        stickyTop,
                        box,
                        flat; // Flat with white background

        internal void Init()
        {
            def = new GUIStyle(EditorStyles.toolbar).Height(18).StretchWidth();
            large = new GUIStyle(def).Height(23);
            small = new GUIStyle(def).Height(13);
            stickyTop = new GUIStyle(def).MarginTop(0);
            box = new GUIStyle(GUI.skin.box).Height(20).StretchWidth().Padding(5, 6, 1, 0).Margin(0, 0, 0, 0);
            flat = new GUIStyle(GUI.skin.box).Height(18).StretchWidth().Padding(5, 6, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }

    public class MiscStyles
    {
        public GUIStyle line; // Flat line with no margin

        internal void Init()
        {
            line = new GUIStyle(GUI.skin.box).Padding(0, 0, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }
}