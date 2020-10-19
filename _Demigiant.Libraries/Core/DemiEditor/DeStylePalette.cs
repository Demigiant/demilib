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
        protected bool initializedAsInterfont;

        // ADB path to Imgs directory, final slash included
        static string _adbImgsDir {
            get { if (_fooAdbImgsDir == null) _fooAdbImgsDir = Assembly.GetExecutingAssembly().ADBDir() + "/Imgs/"; return _fooAdbImgsDir; }
        }
        static string _fooAdbImgsDir;

        #region Texture2D

        public static Texture2D transparent { get { return LoadTexture(ref _transparent, "transparentSquare"); } }
        public static Texture2D whiteSquare { get { return LoadTexture(ref _whiteSquare, "whiteSquare"); } }
        public static Texture2D whiteSquareCurved { get { return LoadTexture(ref _whiteSquareCurved, "whiteSquareCurved"); } }
        public static Texture2D whiteSquareCurved02 { get { return LoadTexture(ref _whiteSquareCurved02, "whiteSquareCurved02"); } }
        public static Texture2D whiteSquareAlpha10 { get { return LoadTexture(ref _whiteSquareAlpha10, "whiteSquareAlpha10"); } }
        public static Texture2D whiteSquareAlpha15 { get { return LoadTexture(ref _whiteSquareAlpha15, "whiteSquareAlpha15"); } }
        public static Texture2D whiteSquareAlpha25 { get { return LoadTexture(ref _whiteSquareAlpha25, "whiteSquareAlpha25"); } }
        public static Texture2D whiteSquareAlpha50 { get { return LoadTexture(ref _whiteSquareAlpha50, "whiteSquareAlpha50"); } }
        public static Texture2D whiteSquareAlpha80 { get { return LoadTexture(ref _whiteSquareAlpha80, "whiteSquareAlpha80"); } }
        public static Texture2D whiteSquare_fadeOut_bt { get { return LoadTexture(ref _whiteSquare_fadeOut_bt, "whiteSquare_fadeOut_bt", FilterMode.Bilinear); } }
        public static Texture2D blackSquare { get { return LoadTexture(ref _blackSquare, "blackSquare"); } }
        public static Texture2D blackSquareAlpha10 { get { return LoadTexture(ref _blackSquareAlpha10, "blackSquareAlpha10"); } }
        public static Texture2D blackSquareAlpha15 { get { return LoadTexture(ref _blackSquareAlpha15, "blackSquareAlpha15"); } }
        public static Texture2D blackSquareAlpha25 { get { return LoadTexture(ref _blackSquareAlpha25, "blackSquareAlpha25"); } }
        public static Texture2D blackSquareAlpha50 { get { return LoadTexture(ref _blackSquareAlpha50, "blackSquareAlpha50"); } }
        public static Texture2D blackSquareAlpha80 { get { return LoadTexture(ref _blackSquareAlpha80, "blackSquareAlpha80"); } }
        public static Texture2D redSquare { get { return LoadTexture(ref _redSquare, "redSquare"); } }
        public static Texture2D orangeSquare { get { return LoadTexture(ref _orangeSquare, "orangeSquare"); } }
        public static Texture2D yellowSquare { get { return LoadTexture(ref _yellowSquare, "yellowSquare"); } }
        public static Texture2D greenSquare { get { return LoadTexture(ref _greenSquare, "greenSquare"); } }
        public static Texture2D blueSquare { get { return LoadTexture(ref _blueSquare, "blueSquare"); } }
        public static Texture2D purpleSquare { get { return LoadTexture(ref _purpleSquare, "purpleSquare"); } }
        public static Texture2D squareBorder { get { return LoadTexture(ref _squareBorder, "squareBorder"); } }
        public static Texture2D squareBorderEmpty01 { get { return LoadTexture(ref _squareBorderEmpty01, "squareBorderEmpty01"); } }
        public static Texture2D squareBorderEmpty02 { get { return LoadTexture(ref _squareBorderEmpty02, "squareBorderEmpty02"); } }
        public static Texture2D squareBorderEmpty03 { get { return LoadTexture(ref _squareBorderEmpty03, "squareBorderEmpty03"); } }
        public static Texture2D squareBorderAlpha15 { get { return LoadTexture(ref _squareBorderAlpha15, "squareBorderAlpha15"); } }
        public static Texture2D squareBorderCurved { get { return LoadTexture(ref _squareBorderCurved, "squareBorderCurved"); } }
        public static Texture2D squareBorderCurved02 { get { return LoadTexture(ref _squareBorderCurved02, "squareBorderCurved02"); } }
        public static Texture2D squareBorderCurvedEmpty { get { return LoadTexture(ref _squareBorderCurvedEmpty, "squareBorderCurvedEmpty"); } }
        public static Texture2D squareBorderCurvedEmptyThick { get { return LoadTexture(ref _squareBorderCurvedEmptyThick, "squareBorderCurvedEmptyThick"); } }
        public static Texture2D squareBorderCurvedEmpty02 { get { return LoadTexture(ref _squareBorderCurvedEmpty02, "squareBorderCurvedEmpty02"); } }
        public static Texture2D squareBorderCurvedAlpha { get { return LoadTexture(ref _squareBorderCurvedAlpha, "squareBorderCurvedAlpha"); } }
        public static Texture2D squareBorderCurved_darkBorders { get { return LoadTexture(ref _squareBorderCurved_darkBorders, "squareBorderCurved_darkBorders"); } }
        public static Texture2D squareBorderCurved_darkBordersAlpha { get { return LoadTexture(ref _squareBorderCurved_darkBordersAlpha, "squareBorderCurved_darkBordersAlpha"); } }
        public static Texture2D squareBorderCurved02_darkBorders { get { return LoadTexture(ref _squareBorderCurved02_darkBorders, "squareBorderCurved02_darkBorders"); } }
        public static Texture2D squareCornersEmpty02 { get { return LoadTexture(ref _squareCornersEmpty02, "squareCornersEmpty02"); } }
        public static Texture2D whiteDot { get { return LoadTexture(ref _whiteDot, "whiteDot"); } }
        public static Texture2D whiteDot_darkBorder { get { return LoadTexture(ref _whiteDot_darkBorder, "whiteDot_darkBorder"); } }
        public static Texture2D whiteDot_whiteBorderAlpha { get { return LoadTexture(ref _whiteDot_whiteBorderAlpha, "whiteDot_whiteBorderAlpha"); } }
        public static Texture2D circle { get { return LoadTexture(ref _circle, "circle", FilterMode.Bilinear); } }
        static Texture2D _transparent;
        static Texture2D _whiteSquare;
        static Texture2D _whiteSquareCurved;
        static Texture2D _whiteSquareCurved02;
        static Texture2D _whiteSquareAlpha10;
        static Texture2D _whiteSquareAlpha15;
        static Texture2D _whiteSquareAlpha25;
        static Texture2D _whiteSquareAlpha50;
        static Texture2D _whiteSquareAlpha80;
        static Texture2D _whiteSquare_fadeOut_bt; // Bottom to top
        static Texture2D _blackSquare;
        static Texture2D _blackSquareAlpha10;
        static Texture2D _blackSquareAlpha15;
        static Texture2D _blackSquareAlpha25;
        static Texture2D _blackSquareAlpha50;
        static Texture2D _blackSquareAlpha80;
        static Texture2D _redSquare;
        static Texture2D _orangeSquare;
        static Texture2D _yellowSquare;
        static Texture2D _greenSquare;
        static Texture2D _blueSquare;
        static Texture2D _purpleSquare;
        static Texture2D _squareBorder;
        static Texture2D _squareBorderEmpty01;
        static Texture2D _squareBorderEmpty02;
        static Texture2D _squareBorderEmpty03;
        static Texture2D _squareBorderAlpha15;
        static Texture2D _squareBorderCurved;
        static Texture2D _squareBorderCurved02;
        static Texture2D _squareBorderCurvedEmpty;
        static Texture2D _squareBorderCurvedEmptyThick;
        static Texture2D _squareBorderCurvedEmpty02; // More curved
        static Texture2D _squareBorderCurvedAlpha;
        static Texture2D _squareBorderCurved_darkBorders;
        static Texture2D _squareBorderCurved_darkBordersAlpha;
        static Texture2D _squareBorderCurved02_darkBorders;
        static Texture2D _squareCornersEmpty02;
        static Texture2D _whiteDot;
        static Texture2D _whiteDot_darkBorder;
        static Texture2D _whiteDot_whiteBorderAlpha;
        static Texture2D _circle;
        public static Texture2D ico_demigiant { get { return LoadTexture(ref _ico_demigiant, "ico_demigiant", FilterMode.Bilinear, 16); } }
        public static Texture2D ico_lock { get { return LoadTexture(ref _ico_lock, "ico_lock"); } }
        public static Texture2D ico_lock_open { get { return LoadTexture(ref _ico_lock_open, "ico_lock_open"); } }
        public static Texture2D ico_visibility { get { return LoadTexture(ref _ico_visibility_on, "ico_visibility"); } }
        public static Texture2D ico_visibility_off { get { return LoadTexture(ref _ico_visibility_off, "ico_visibility_off"); } }
        public static Texture2D ico_flipV { get { return LoadTexture(ref _ico_flipV, "ico_flipV"); } }
        public static Texture2D ico_optionsDropdown { get { return LoadTexture(ref _ico_optionsDropdown, "ico_optionsDropdown"); } }
        public static Texture2D ico_foldout_open { get { return LoadTexture(ref _ico_foldout_open, "ico_foldout_open"); } }
        public static Texture2D ico_foldout_closed { get { return LoadTexture(ref _ico_foldout_closed, "ico_foldout_closed"); } }
        public static Texture2D ico_nodeArrow { get { return LoadTexture(ref _ico_nodeArrow, "ico_nodeArrow", FilterMode.Bilinear, 16); } }
        public static Texture2D ico_delete { get { return LoadTexture(ref _ico_delete, "ico_delete", FilterMode.Bilinear, 16); } }
        public static Texture2D ico_end { get { return LoadTexture(ref _ico_end, "ico_end", FilterMode.Bilinear); } }
        public static Texture2D ico_alert { get { return LoadTexture(ref _ico_alert, "ico_alert", FilterMode.Bilinear); } }
        public static Texture2D ico_ok { get { return LoadTexture(ref _ico_ok, "ico_ok", FilterMode.Bilinear); } }
        public static Texture2D ico_alignTL { get { return LoadTexture(ref _ico_alignTL, "ico_alignTL"); } }
        public static Texture2D ico_alignTC { get { return LoadTexture(ref _ico_alignTC, "ico_alignTC"); } }
        public static Texture2D ico_alignTR { get { return LoadTexture(ref _ico_alignTR, "ico_alignTR"); } }
        public static Texture2D ico_alignCL { get { return LoadTexture(ref _ico_alignCL, "ico_alignCL"); } }
        public static Texture2D ico_alignCC { get { return LoadTexture(ref _ico_alignCC, "ico_alignCC"); } }
        public static Texture2D ico_alignCR { get { return LoadTexture(ref _ico_alignCR, "ico_alignCR"); } }
        public static Texture2D ico_alignBL { get { return LoadTexture(ref _ico_alignBL, "ico_alignBL"); } }
        public static Texture2D ico_alignBC { get { return LoadTexture(ref _ico_alignBC, "ico_alignBC"); } }
        public static Texture2D ico_alignBR { get { return LoadTexture(ref _ico_alignBR, "ico_alignBR"); } }
        public static Texture2D ico_alignL { get { return LoadTexture(ref _ico_alignL, "ico_alignL"); } }
        public static Texture2D ico_alignHC { get { return LoadTexture(ref _ico_alignHC, "ico_alignHC"); } }
        public static Texture2D ico_alignR { get { return LoadTexture(ref _ico_alignR, "ico_alignR"); } }
        public static Texture2D ico_alignT { get { return LoadTexture(ref _ico_alignT, "ico_alignT"); } }
        public static Texture2D ico_alignVC { get { return LoadTexture(ref _ico_alignVC, "ico_alignVC"); } }
        public static Texture2D ico_alignB { get { return LoadTexture(ref _ico_alignB, "ico_alignB"); } }
        public static Texture2D ico_distributeHAlignT { get { return LoadTexture(ref _ico_distributeHAlignT, "ico_distributeHAlignT"); } }
        public static Texture2D ico_distributeVAlignL { get { return LoadTexture(ref _ico_distributeVAlignL, "ico_distributeVAlignL"); } }
        public static Texture2D ico_star { get { return LoadTexture(ref _ico_star, "ico_star"); } }
        public static Texture2D ico_star_border { get { return LoadTexture(ref _ico_star_border, "ico_star_border"); } }
        public static Texture2D ico_play { get { return LoadTexture(ref _ico_play, "ico_play"); } }
        public static Texture2D ico_play_border { get { return LoadTexture(ref _ico_play_border, "ico_play_border"); } }
        public static Texture2D ico_cog { get { return LoadTexture(ref _ico_cog, "ico_cog"); } }
        public static Texture2D ico_cog_border { get { return LoadTexture(ref _ico_cog_border, "ico_cog_border"); } }
        public static Texture2D ico_comment { get { return LoadTexture(ref _ico_comment, "ico_comment"); } }
        public static Texture2D ico_comment_border { get { return LoadTexture(ref _ico_comment_border, "ico_comment_border"); } }
        public static Texture2D ico_ui { get { return LoadTexture(ref _ico_ui, "ico_ui"); } }
        public static Texture2D ico_ui_border { get { return LoadTexture(ref _ico_ui_border, "ico_ui_border"); } }
        public static Texture2D ico_heart { get { return LoadTexture(ref _ico_heart, "ico_heart"); } }
        public static Texture2D ico_heart_border { get { return LoadTexture(ref _ico_heart_border, "ico_heart_border"); } }
        public static Texture2D ico_skull { get { return LoadTexture(ref _ico_skull, "ico_skull"); } }
        public static Texture2D ico_skull_border { get { return LoadTexture(ref _ico_skull_border, "ico_skull_border"); } }
        public static Texture2D ico_camera { get { return LoadTexture(ref _ico_camera, "ico_camera"); } }
        public static Texture2D ico_camera_border { get { return LoadTexture(ref _ico_camera_border, "ico_camera_border"); } }
        public static Texture2D ico_light { get { return LoadTexture(ref _ico_light, "ico_light"); } }
        public static Texture2D ico_light_border { get { return LoadTexture(ref _ico_light_border, "ico_light_border"); } }
        public static Texture2D grid_dark { get { return LoadTexture(ref _grid_dark, "grid_dark", FilterMode.Point, 64, TextureWrapMode.Repeat); } }
        public static Texture2D grid_bright { get { return LoadTexture(ref _grid_bright, "grid_bright", FilterMode.Point, 64, TextureWrapMode.Repeat); } }
        public static Texture2D tileBars_empty { get { return LoadTexture(ref _tileBars_empty, "tileBars_empty", FilterMode.Point, 32, TextureWrapMode.Repeat); } }
        public static Texture2D tileBars_slanted { get { return LoadTexture(ref _tileBars_slanted, "tileBars_slanted", FilterMode.Point, 32, TextureWrapMode.Repeat); } }
        public static Texture2D tileBars_slanted_alpha { get { return LoadTexture(ref _tileBars_slanted_alpha, "tileBars_slanted_alpha", FilterMode.Point, 32, TextureWrapMode.Repeat); } }
        static Texture2D _ico_demigiant;
        static Texture2D _ico_lock;
        static Texture2D _ico_lock_open;
        static Texture2D _ico_visibility_on;
        static Texture2D _ico_visibility_off;
        static Texture2D _ico_flipV;
        static Texture2D _ico_optionsDropdown;
        static Texture2D _ico_foldout_open;
        static Texture2D _ico_foldout_closed;
        static Texture2D _ico_nodeArrow;
        static Texture2D _ico_delete;
        static Texture2D _ico_end;
        static Texture2D _ico_alert;
        static Texture2D _ico_ok;
        static Texture2D _ico_alignTL; // Icons with square with corners and center
        static Texture2D _ico_alignTC;
        static Texture2D _ico_alignTR;
        static Texture2D _ico_alignCL;
        static Texture2D _ico_alignCC;
        static Texture2D _ico_alignCR;
        static Texture2D _ico_alignBL;
        static Texture2D _ico_alignBC;
        static Texture2D _ico_alignBR;
        static Texture2D _ico_alignL; // Simpler align icons
        static Texture2D _ico_alignHC;
        static Texture2D _ico_alignR;
        static Texture2D _ico_alignT;
        static Texture2D _ico_alignVC;
        static Texture2D _ico_alignB;
        static Texture2D _ico_distributeHAlignT;
        static Texture2D _ico_distributeVAlignL;
        static Texture2D _ico_star;
        static Texture2D _ico_star_border;
        static Texture2D _ico_cog;
        static Texture2D _ico_cog_border;
        static Texture2D _ico_play;
        static Texture2D _ico_play_border;
        static Texture2D _ico_comment;
        static Texture2D _ico_comment_border;
        static Texture2D _ico_ui;
        static Texture2D _ico_ui_border;
        static Texture2D _ico_heart;
        static Texture2D _ico_heart_border;
        static Texture2D _ico_skull;
        static Texture2D _ico_skull_border;
        static Texture2D _ico_camera;
        static Texture2D _ico_camera_border;
        static Texture2D _ico_light;
        static Texture2D _ico_light_border;
        static Texture2D _grid_dark;
        static Texture2D _grid_bright;
        static Texture2D _tileBars_empty;
        static Texture2D _tileBars_slanted;
        static Texture2D _tileBars_slanted_alpha;

        public static Texture2D proj_folder { get { return LoadTexture(ref _proj_folder, "project/ico_folder"); } }
        public static Texture2D proj_atlas { get { return LoadTexture(ref _proj_atlas, "project/ico_atlas"); } }
        public static Texture2D proj_audio { get { return LoadTexture(ref _proj_audio, "project/ico_audio"); } }
        public static Texture2D proj_bundle { get { return LoadTexture(ref _proj_bundle, "project/ico_bundle"); } }
        public static Texture2D proj_cog { get { return LoadTexture(ref _proj_cog, "project/ico_cog"); } }
        public static Texture2D proj_cross { get { return LoadTexture(ref _proj_cross, "project/ico_cross"); } }
        public static Texture2D proj_demigiant { get { return LoadTexture(ref _proj_demigiant, "project/ico_demigiant"); } }
        public static Texture2D proj_fonts { get { return LoadTexture(ref _proj_fonts, "project/ico_fonts"); } }
        public static Texture2D proj_heart { get { return LoadTexture(ref _proj_heart, "project/ico_heart"); } }
        public static Texture2D proj_materials { get { return LoadTexture(ref _proj_materials, "project/ico_materials"); } }
        public static Texture2D proj_models { get { return LoadTexture(ref _proj_models, "project/ico_models"); } }
        public static Texture2D proj_particles { get { return LoadTexture(ref _proj_particles, "project/ico_particles"); } }
        public static Texture2D proj_play { get { return LoadTexture(ref _proj_play, "project/ico_play"); } }
        public static Texture2D proj_prefab { get { return LoadTexture(ref _proj_prefab, "project/ico_prefab"); } }
        public static Texture2D proj_shaders { get { return LoadTexture(ref _proj_shaders, "project/ico_shaders"); } }
        public static Texture2D proj_scripts { get { return LoadTexture(ref _proj_scripts, "project/ico_scripts"); } }
        public static Texture2D proj_skull { get { return LoadTexture(ref _proj_skull, "project/ico_skull"); } }
        public static Texture2D proj_star { get { return LoadTexture(ref _proj_star, "project/ico_star"); } }
        public static Texture2D proj_terrains { get { return LoadTexture(ref _proj_terrains, "project/ico_terrains"); } }
        public static Texture2D proj_textures { get { return LoadTexture(ref _proj_textures, "project/ico_textures"); } }
        static Texture2D _proj_folder;
        static Texture2D _proj_atlas;
        static Texture2D _proj_audio;
        static Texture2D _proj_bundle;
        static Texture2D _proj_cog;
        static Texture2D _proj_cross;
        static Texture2D _proj_demigiant;
        static Texture2D _proj_fonts;
        static Texture2D _proj_heart;
        static Texture2D _proj_materials;
        static Texture2D _proj_models;
        static Texture2D _proj_particles;
        static Texture2D _proj_play;
        static Texture2D _proj_prefab;
        static Texture2D _proj_shaders;
        static Texture2D _proj_scripts;
        static Texture2D _proj_skull;
        static Texture2D _proj_star;
        static Texture2D _proj_terrains;
        static Texture2D _proj_textures;

        #endregion

        /// <summary>
        /// Called automatically by <code>DeGUI.BeginGUI</code>.
        /// Override when adding new style subclasses.
        /// Returns TRUE if the styles were initialized or re-initialized
        /// </summary>
        internal bool Init()
        {
            if (initialized && initializedAsInterfont == DeGUI.usesInterFont) return false;

            initialized = true;
            initializedAsInterfont = DeGUI.usesInterFont;

            // Unity 2018 (bug with non-centered content) ► change default styles then reapply them after init
            Vector2 def_toolbarBtContentOffset = GUI.skin.button.contentOffset;
            if (DeUnityEditorVersion.MajorVersion >= 2018) {
                EditorStyles.toolbarButton.contentOffset = new Vector2(1, 0);
            }

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

            if (DeUnityEditorVersion.MajorVersion >= 2018) {
                // Unity 2018 ► Reassign default styles
                EditorStyles.toolbarButton.contentOffset = def_toolbarBtContentOffset;
            }

            return true;
        }

        static Texture2D LoadTexture(ref Texture2D property, string name, FilterMode filterMode = FilterMode.Point, int maxTextureSize = 32, TextureWrapMode wrapMode = TextureWrapMode.Clamp)
        {
            if (property == null) {
                property = AssetDatabase.LoadAssetAtPath(string.Format("{0}{1}.png", _adbImgsDir, name), typeof(Texture2D)) as Texture2D;
                if (property == null) {
                    // Look inside package manager folder
                    property = AssetDatabase.LoadAssetAtPath(string.Format(
                        "Packages/com.demigiant.demilib/Demigiant/DemiLib/Core/Editor/Imgs/{0}.png",
                        name
                    ), typeof(Texture2D)) as Texture2D;
                }
                property.SetGUIFormat(filterMode, maxTextureSize, wrapMode);
            }
            return property;
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
                        flat, flatAlpha10, flatAlpha25, // Flat with white background
                        sticky, stickyTop, // Without any margin (or only top margin)
                        outline01, outline02, outline03, roundOutline01, roundOutline02;

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.box).Padding(6, 6, 6, 6);
            flat = new GUIStyle(def).Background(DeStylePalette.whiteSquare);
            flatAlpha10 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha10);
            flatAlpha25 = new GUIStyle(def).Background(DeStylePalette.whiteSquareAlpha25);
            sticky = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(0), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(0));
            stickyTop = new DeSkinStyle(new GUIStyle(flatAlpha25).MarginTop(-2).MarginBottom(7), new GUIStyle(flatAlpha10).MarginTop(-2).MarginBottom(7));
            outline01 = DeGUI.styles.box.flat.Clone().Background(DeStylePalette.squareBorderEmpty01);
            outline02 = outline01.Clone().Border(new RectOffset(5, 5, 5, 5)).Background(DeStylePalette.squareBorderEmpty02);
            outline03 = outline01.Clone().Border(new RectOffset(7, 7, 7, 7)).Background(DeStylePalette.squareBorderEmpty03);
            roundOutline01 = outline02.Clone().Background(DeStylePalette.squareBorderCurvedEmpty);
            roundOutline02 = outline02.Clone().Background(DeStylePalette.squareBorderCurvedEmptyThick);
        }
    }

    public class ButtonStyles
    {
        public GUIStyle def,
                        tool, toolNoFixedH, toolL, toolS, toolIco,
                        toolFoldoutClosed, toolFoldoutClosedWLabel, toolFoldoutClosedWStretchedLabel,
                        toolFoldoutOpen, toolFoldoutOpenWLabel, toolFoldoutOpenWStretchedLabel,
                        toolLFoldoutClosed, toolLFoldoutClosedWLabel, toolLFoldoutClosedWStretchedLabel,
                        toolLFoldoutOpen, toolLFoldoutOpenWLabel, toolLFoldoutOpenWStretchedLabel,
                        foldoutClosedWLabel, foldoutOpenWLabel,
                        bBlankBorder, bBlankBorderCompact,
                        flatWhite, transparent;

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.button);
            tool = new GUIStyle(EditorStyles.toolbarButton).ContentOffsetY(-1);
            if (DeGUI.usesInterFont) tool.Height((int)(tool.fixedHeight - 3));
            toolNoFixedH = new GUIStyle(EditorStyles.toolbarButton).ContentOffsetY(-1).Height(0);
            toolL = new GUIStyle(EditorStyles.toolbarButton).Height(23).ContentOffsetY(0);
            toolS = new GUIStyle(EditorStyles.toolbarButton).Height(13).ContentOffsetY(0).Padding(0);
            toolIco = new GUIStyle(tool).StretchWidth(false).Width(22).ContentOffsetX(-1);
            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleLeft,
                active = { background = DeStylePalette.transparent, scaledBackgrounds = new Texture2D[0] },
                fixedWidth = 14,
//                normal = { background = EditorStyles.foldout.normal.background },
                normal = { background = DeStylePalette.ico_foldout_closed, scaledBackgrounds = new Texture2D[0] },
                border = EditorStyles.foldout.border,
                padding = new RectOffset(14, 0, 0, 0),
                margin = new RectOffset(0, 3, 0, 0),
                overflow = new RectOffset(-2, 0, -2, 0),
                stretchHeight = true,
                contentOffset = new Vector2(2, -1),
                richText = true
            };
            toolFoldoutClosedWLabel = toolFoldoutClosed.Clone(DeGUI.usesInterFont ? 11 : 9).Width(0).StretchWidth(false);
            toolFoldoutClosedWStretchedLabel = toolFoldoutClosedWLabel.Clone().StretchWidth();
            toolFoldoutOpen = new GUIStyle(toolFoldoutClosed) {
//                normal = { background = EditorStyles.foldout.onNormal.background }
                normal = { background = DeStylePalette.ico_foldout_open, scaledBackgrounds = new Texture2D[0] }
            };
            toolFoldoutOpenWLabel = new GUIStyle(toolFoldoutClosedWLabel) {
//                normal = { background = EditorStyles.foldout.onNormal.background }
                normal = { background = DeStylePalette.ico_foldout_open, scaledBackgrounds = new Texture2D[0] }
            };
            toolFoldoutOpenWStretchedLabel = toolFoldoutOpenWLabel.Clone().StretchWidth();
            // Large
            toolLFoldoutClosed = toolFoldoutClosed.Clone().OverflowTop(-4);
            toolLFoldoutClosedWLabel = toolFoldoutClosedWLabel.Clone().OverflowTop(-4);
            toolLFoldoutClosedWStretchedLabel = toolFoldoutClosedWStretchedLabel.Clone().OverflowTop(-4);
            toolLFoldoutOpen = toolFoldoutOpen.Clone().OverflowTop(-4);
            toolLFoldoutOpenWLabel = toolFoldoutOpenWLabel.Clone().OverflowTop(-4);
            toolLFoldoutOpenWStretchedLabel = toolFoldoutOpenWStretchedLabel.Clone().OverflowTop(-4);
            //
            foldoutOpenWLabel = toolFoldoutOpenWStretchedLabel.Clone(12);
            foldoutClosedWLabel = toolFoldoutClosedWStretchedLabel.Clone(12);
            // Custom using squareBorder
            bBlankBorder = new GUIStyle(GUI.skin.button).Add(TextAnchor.MiddleCenter, Color.white).Background(DeStylePalette.squareBorderCurved)
                .Padding(5, 4, 1, 2).Border(new RectOffset(4, 4, 4, 4)).Overflow(-1, -1, 0, 0).ContentOffsetX(-1);
            bBlankBorderCompact = bBlankBorder.Clone().Padding(5, 4, 0, 0).ContentOffsetY(-1);
            flatWhite = DeGUI.styles.button.tool.Clone(TextAnchor.MiddleCenter).Background(DeStylePalette.whiteSquare).Margin(0).Padding(0)
                .Border(0, 0, 0, 0).Overflow(0).Height(0).ContentOffset(0, 0);
            transparent = flatWhite.Clone().Background(null);
        }
    }

    public class LabelStyles
    {
        public GUIStyle bold, rightAligned,
                        wordwrap, wordwrapRichtText,
                        toolbar, toolbarRightAligned, toolbarL, toolbarS, toolbarBox;

        internal void Init()
        {
            bold = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold);
            rightAligned = new GUIStyle(GUI.skin.label).Add(TextAnchor.MiddleRight);
            wordwrap = new GUIStyle(GUI.skin.label).Add(Format.WordWrap);
            wordwrapRichtText = wordwrap.Clone(Format.RichText);
            toolbar = new GUIStyle(GUI.skin.label).Add(DeGUI.usesInterFont ? 11 : 9)
                .ContentOffset(new Vector2(-2, DeGUI.usesInterFont ? -1 : 1));
            toolbarRightAligned = toolbar.Clone(TextAnchor.MiddleRight);
            toolbarL = new GUIStyle(toolbar).ContentOffsetY(3);
            toolbarS = new GUIStyle(toolbar).Add(DeGUI.usesInterFont ? 10 : 8, FontStyle.Bold).ContentOffsetY(-2);
            toolbarBox = new GUIStyle(toolbar).ContentOffsetY(0);
        }
    }

    public class ToolbarStyles
    {
        public GUIStyle def, defNoPadding,
                        large, small,
                        stickyTop,
                        box,
                        flat; // Flat with white background

        internal void Init()
        {
            def = new GUIStyle(EditorStyles.toolbar).Height(18).StretchWidth();
            defNoPadding = def.Clone().Padding(0);
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