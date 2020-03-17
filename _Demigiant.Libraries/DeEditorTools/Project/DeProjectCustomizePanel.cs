// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/03/17

using System;
using System.Reflection;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    class DeProjectCustomizePanel : EditorWindow
    {
        public static void ShowWindow()
        {
            EditorWindow win = GetWindow(typeof(DeProjectCustomizePanel), true, _Title);
            win.maxSize = win.minSize = new Vector2(_MaxW, _MaxH);
        }
		
        const string _Title = "DeProject";
        const int _MaxW = 640, _MaxH = 160;
        static DeProjectData.HColor[] _hColors;
        static DeProjectData.IcoType[] _icoTypes;
        static Texture2D[] _icoTextures;
        Rect _labelR, _icosR, _colorsR;
        Preset[] _presets;

        #region Unity and GUI Methods

        void OnEnable()
        {
            Undo.undoRedoPerformed += Repaint;
            _hColors = (DeProjectData.HColor[])Enum.GetValues(typeof(DeProjectData.HColor));
            _icoTypes = (DeProjectData.IcoType[])Enum.GetValues(typeof(DeProjectData.IcoType));
            _icoTextures = new Texture2D[_icoTypes.Length];
            for (int i = 0; i < _icoTypes.Length; ++i) {
                DeProjectData.IcoType icoType = _icoTypes[i];
                _icoTextures[i] = DeProjectData.GetIcon(icoType);
            }
            // Rects
            _labelR = new Rect(2, 2, _MaxW, EditorGUIUtility.singleLineHeight);
            _icosR = _labelR.Shift(0, _labelR.height + 4, 0, 0).SetHeight(24);
            _colorsR = _icosR.Shift(0, _icosR.height + 4, 0, 0);
            _presets = new Preset[16];
            int presetsPerRow = 4;
            int presetsW = (_MaxW - 4) / presetsPerRow;
            int presetsH = (int)EditorGUIUtility.singleLineHeight;
            float x = 2;
            float y = _colorsR.yMax + 4;
            for (int i = 0; i < _presets.Length; ++i) {
                if (i > 0 && i % presetsPerRow == 0) {
                    x = 2;
                    y += presetsH + 2;
                }
                Rect r = new Rect(x, y, presetsW, presetsH);
                switch (i) {
                case 0: _presets[i] = new Preset(r, "3D Models", DeProjectData.IcoType.Models, DeProjectData.HColor.BrightBlue);
                    break;
                case 1: _presets[i] = new Preset(r, "Asset Bundle", DeProjectData.IcoType.AssetBundle, DeProjectData.HColor.BrightBlue);
                    break;
                case 2: _presets[i] = new Preset(r, "Atlas", DeProjectData.IcoType.Atlas, DeProjectData.HColor.Orange);
                    break;
                case 3: _presets[i] = new Preset(r, "Audio", DeProjectData.IcoType.Audio, DeProjectData.HColor.Orange);
                    break;
                case 4: _presets[i] = new Preset(r, "Fonts", DeProjectData.IcoType.Fonts, DeProjectData.HColor.Orange);
                    break;
                case 5: _presets[i] = new Preset(r, "Material", DeProjectData.IcoType.Materials, DeProjectData.HColor.Orange);
                    break;
                case 6: _presets[i] = new Preset(r, "Particles", DeProjectData.IcoType.Particles, DeProjectData.HColor.Orange);
                    break;
                case 7: _presets[i] = new Preset(r, "Plugins + Standard Assets", DeProjectData.IcoType.Cog, DeProjectData.HColor.None);
                    break;
                case 8: _presets[i] = new Preset(r, "Prefabs", DeProjectData.IcoType.Prefab, DeProjectData.HColor.BrightBlue);
                    break;
                case 9: _presets[i] = new Preset(r, "Resources", DeProjectData.IcoType.Play, DeProjectData.HColor.Green);
                    break;
                case 10: _presets[i] = new Preset(r, "Scenes", DeProjectData.IcoType.Play, DeProjectData.HColor.Green);
                    break;
                case 11: _presets[i] = new Preset(r, "Scripts", DeProjectData.IcoType.Scripts, DeProjectData.HColor.Purple);
                    break;
                case 12: _presets[i] = new Preset(r, "Shaders", DeProjectData.IcoType.Shaders, DeProjectData.HColor.Orange);
                    break;
                case 13: _presets[i] = new Preset(r, "StreamingAssets", DeProjectData.IcoType.Play, DeProjectData.HColor.Green);
                    break;
                case 14: _presets[i] = new Preset(r, "Terrains", DeProjectData.IcoType.Terrains, DeProjectData.HColor.BrightBlue);
                    break;
                case 15: _presets[i] = new Preset(r, "Textures + Sprites", DeProjectData.IcoType.Textures, DeProjectData.HColor.Orange);
                    break;
                }
                x += presetsW;
            }

            Selection.selectionChanged += Repaint;
        }

        void OnDisable()
        {
            Undo.undoRedoPerformed -= Repaint;
            Selection.selectionChanged -= Repaint;
        }

        void OnGUI()
        {
            DeGUI.BeginGUI();
            Styles.Init();

            GUI.Label(_labelR, string.Format("{0} assets selected", Selection.assetGUIDs.Length));

            // Icons
            Rect icoR = _icosR.SetWidth(_icosR.height);
            using (new DeGUI.ColorScope(new DeSkinColor(0.5f, 0.4f))) {
                for (int i = 1; i < _icoTextures.Length; ++i) { // Skip none (assigned manually) and custom
                    if (i == 1) {
                        if (DeGUI.ActiveButton(icoR, new GUIContent(DeStylePalette.ico_delete), Styles.bt)) DeProject.ResetSelections(Selection.assetGUIDs);
                    } else {
                        if (DeGUI.ActiveButton(icoR, new GUIContent(_icoTextures[i]), Styles.bt)) {
                            DeProject.SetIconForSelections(_icoTypes[i], Selection.assetGUIDs);
                        }
                    }
                    icoR = icoR.Shift(icoR.width + 1, 0, 0, 0);
                }
            }
            // Colors
            Rect colorR = _colorsR.SetWidth(_colorsR.height);
            for (int i = 0; i < _hColors.Length; ++i) {
                bool noColor = _hColors[i] == DeProjectData.HColor.None;
                using (new DeGUI.ColorScope(null, null, noColor ? new DeSkinColor(0.5f, 0.4f) : new DeSkinColor(1)))
                using (new DeGUI.ColorScope(DeProjectData.GetColor(_hColors[i]))) {
                    if (GUI.Button(colorR, "", noColor ? Styles.btNoColor : Styles.btColor)) {
                        DeProject.SetColorForSelections(_hColors[i], Selection.assetGUIDs);
                    }
                }
                colorR = colorR.Shift(colorR.width + 1, 0, 0, 0);
            }
            // Presets
            int totPresets = _presets.Length;
            for (int i = 0; i < totPresets; ++i) {
                Preset preset = _presets[i];
                if (DeGUI.ActiveButton(preset.rect, preset.label, Styles.bt)) {
                    DeProject.SetIconForSelections(preset.icoType, Selection.assetGUIDs);
                    DeProject.SetColorForSelections(preset.hColor, Selection.assetGUIDs);
                }
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        static class Styles
        {
            static bool _initialized;
            public static GUIStyle bt;
            public static GUIStyle btColor;
            public static GUIStyle btNoColor;

            public static void Init()
            {
                if (_initialized) return;

                _initialized = true;

                bt = DeGUI.styles.button.bBlankBorder.Clone().Background(DeStylePalette.squareBorderCurvedEmpty);
                btColor = DeGUI.styles.button.flatWhite.Clone();
                btNoColor = DeGUI.styles.button.flatWhite.Clone().Background(DeStylePalette.squareCornersEmpty02);
            }
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        class Preset
        {
            public readonly Rect rect;
            public readonly GUIContent label;
            public readonly DeProjectData.IcoType icoType;
            public readonly DeProjectData.HColor hColor;

            public Preset(Rect rect, string label, DeProjectData.IcoType icoType, DeProjectData.HColor hColor)
            {
                this.rect = rect;
                this.label = new GUIContent(label);
                this.icoType = icoType;
                this.hColor = hColor;
            }
        }
    }
}