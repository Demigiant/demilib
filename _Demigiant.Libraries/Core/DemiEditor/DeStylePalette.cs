// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/29 18:54

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
        public readonly Box box = new Box();
        public readonly Button button = new Button();
        public readonly Label label = new Label();
        public readonly Toolbar toolbar = new Toolbar();
        public readonly Misc misc = new Misc();

        protected bool initialized;

        // ADB path to Imgs directory, final slash included
        static string _adbImgsDir {
            get { if (_fooAdbImgsDir == null) _fooAdbImgsDir = Assembly.GetExecutingAssembly().ADBDir() + "/Imgs/"; return _fooAdbImgsDir; }
        }
        static string _fooAdbImgsDir;

        #region Texture2D

        public static Texture2D whiteSquare {
            get {
                if (_fooWhiteSquare == null) {
                    _fooWhiteSquare = AssetDatabase.LoadAssetAtPath("Assets/" + _adbImgsDir + "whiteSquare.png", typeof(Texture2D)) as Texture2D;
                    _fooWhiteSquare.SetFormat(FilterMode.Point, 16);
                }
                return _fooWhiteSquare;
            }
        }
        public static Texture2D whiteSquareAlpha10 {
            get {
                if (_fooWhiteSquareAlpha10 == null) {
                    _fooWhiteSquareAlpha10 = AssetDatabase.LoadAssetAtPath("Assets/" + _adbImgsDir + "whiteSquareAlpha10.png", typeof(Texture2D)) as Texture2D;
                    _fooWhiteSquareAlpha10.SetFormat(FilterMode.Point, 16);
                }
                return _fooWhiteSquareAlpha10;
            }
        }
        public static Texture2D whiteSquareAlpha25 {
            get {
                if (_fooWhiteSquareAlpha25 == null) {
                    _fooWhiteSquareAlpha25 = AssetDatabase.LoadAssetAtPath("Assets/" + _adbImgsDir + "whiteSquareAlpha25.png", typeof(Texture2D)) as Texture2D;
                    _fooWhiteSquareAlpha25.SetFormat(FilterMode.Point, 16);
                }
                return _fooWhiteSquareAlpha25;
            }
        }
        public static Texture2D whiteSquareAlpha50 {
            get {
                if (_fooWhiteSquareAlpha50 == null) {
                    _fooWhiteSquareAlpha50 = AssetDatabase.LoadAssetAtPath("Assets/" + _adbImgsDir + "whiteSquareAlpha50.png", typeof(Texture2D)) as Texture2D;
                    _fooWhiteSquareAlpha50.SetFormat(FilterMode.Point, 16);
                }
                return _fooWhiteSquareAlpha50;
            }
        }
        static Texture2D _fooWhiteSquare;
        static Texture2D _fooWhiteSquareAlpha10;
        static Texture2D _fooWhiteSquareAlpha25;
        static Texture2D _fooWhiteSquareAlpha50;

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
                if (fi.FieldType.BaseType == typeof(DeStyleSubPalette)) ((DeStyleSubPalette)fi.GetValue(this)).Init();
            }
        }
    }

    /// <summary>
    /// Extend any custom subpalettes from this, so they will be initialized correctly
    /// </summary>
    public abstract class DeStyleSubPalette
    {
        public abstract void Init();
    }

    public class Box
    {
        public GUIStyle def,
                        flat, flatAlpha10, flatAlpha25; // Flat with white background
        public DeSkinStyle sticky, stickyTop; // Without any margin (or only top margin)

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

    public class Button
    {
        public GUIStyle def,
                        tool, toolL, toolIco,
                        toolFoldoutClosed, toolFoldoutClosedWLabel, toolFoldoutOpen, toolFoldoutOpenWLabel;

        internal void Init()
        {
            def = new GUIStyle(GUI.skin.button);
            tool = new GUIStyle(EditorStyles.toolbarButton).ContentOffsetY(-1);
            toolL = new GUIStyle(EditorStyles.toolbarButton).Height(23).ContentOffsetY(0);
            toolIco = new GUIStyle(tool).StretchWidth(false).Width(22).ContentOffsetX(-1);
            toolFoldoutClosed = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.UpperLeft,
                active = { background = null },
                fixedWidth = 14,
                normal = { background = EditorStyles.foldout.normal.background },
                border = EditorStyles.foldout.border,
                padding = new RectOffset(14, 0, 0, 0)
            }.MarginTop(2);
            toolFoldoutClosedWLabel = new GUIStyle(toolFoldoutClosed).Width(0).StretchWidth(false);
            toolFoldoutOpen = new GUIStyle(toolFoldoutClosed) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
            toolFoldoutOpenWLabel = new GUIStyle(toolFoldoutClosedWLabel) {
                normal = { background = EditorStyles.foldout.onNormal.background }
            };
        }
    }

    public class Label
    {
        public GUIStyle bold,
                        wordwrap,
                        toolbar, toolbarL, toolbarBox;

        internal void Init()
        {
            bold = new GUIStyle(GUI.skin.label).Add(FontStyle.Bold);
            wordwrap = new GUIStyle(GUI.skin.label).Add(Format.WordWrap);
            toolbar = new GUIStyle(GUI.skin.label).Add(10).ContentOffset(new Vector2(-2, -1));
            toolbarL = new GUIStyle(toolbar).ContentOffsetY(2);
            toolbarBox = new GUIStyle(toolbar).ContentOffsetY(0);
        }
    }

    public class Toolbar
    {
        public GUIStyle def,
                        large,
                        stickyTop,
                        box,
                        flat; // Flat with white background

        internal void Init()
        {
            def = new GUIStyle(EditorStyles.toolbar).Height(18).StretchWidth();
            large = new GUIStyle(def).Height(23);
            stickyTop = new GUIStyle(def).MarginTop(0);
            box = new GUIStyle(GUI.skin.box).Height(20).StretchWidth().Padding(5, 6, 1, 0).Margin(0, 0, 0, 0);
            flat = new GUIStyle(GUI.skin.box).Height(18).StretchWidth().Padding(5, 6, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }

    public class Misc
    {
        public GUIStyle line; // Flat line with no margin

        internal void Init()
        {
            line = new GUIStyle(GUI.skin.box).Padding(0, 0, 0, 0).Margin(0, 0, 0, 0).Background(DeStylePalette.whiteSquare);
        }
    }
}