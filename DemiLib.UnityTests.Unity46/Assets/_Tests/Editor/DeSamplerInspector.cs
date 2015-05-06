// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 13:29

using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using DG.DemiEditor;

[CustomEditor(typeof(DeSampler))]
public class DeSamplerInspector : Editor
{
    enum DrawMode
    {
        Default,
        ColorPalette
    }

    DeSampler _src;
    DrawMode _drawMode;
    DeColorPalette _toolbarButtonsColors;
    float _vSpace = 8;

    // ===================================================================================
    // MONOBEHAVIOUR METHODS -------------------------------------------------------------

    void OnEnable()
    {
        _src = target as DeSampler;

        if (_toolbarButtonsColors == null) {
            _toolbarButtonsColors = new DeColorPalette() {
                bg = { toggleOff = new DeSkinColor(new Color(0.2f, 0.2f, 0.2f, 1), new Color(0.1f, 0.1f, 0.1f, 1)) },
                content = { toggleOff = new DeSkinColor(new Color(0.7f, 0.7f, 0.7f, 1), new Color(0.6f, 0.6f, 0.6f, 1)) }
            };
        }
    }

    override public void OnInspectorGUI()
    {
        DeGUI.BeginGUI(_src.palette);
        Undo.RecordObject(_src, "DeSampler");

        GUILayout.Space(8);

        switch (_drawMode) {
        case DrawMode.Default:
            DrawDefault();
            break;
        case DrawMode.ColorPalette:
            DrawColorPalette();
            break;
        }
    }

    // ===================================================================================
    // DRAW METHODS ----------------------------------------------------------------------

    void DrawDefault()
    {
        if (GUILayout.Button("Color Palette")) _drawMode = DrawMode.ColorPalette;
        GUILayout.Space(_vSpace);
        if (_drawMode == DrawMode.ColorPalette) return;

        // Labels

        GUILayout.BeginVertical(DeGUI.styles.box.def);
        GUILayout.Label("Bold Label", DeGUI.styles.label.bold);
        GUILayout.Label("Toolbar Label", DeGUI.styles.label.toolbar);
        GUILayout.EndVertical();
        GUILayout.Space(_vSpace);

        // Toolbars + toolbar content

        DeGUILayout.BeginToolbar();
            GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        DeGUILayout.EndToolbar();
        GUILayout.Space(4);
        DeGUILayout.BeginToolbar(DeGUI.styles.toolbar.box);
        GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbarBox);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        DeGUILayout.EndToolbar();
        GUILayout.Space(4);
        DeGUILayout.BeginToolbar(Color.black, DeGUI.styles.toolbar.flat);
        GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbarBox.Clone(Color.white));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        DeGUILayout.EndToolbar();
        GUILayout.Space(_vSpace);

        // Container

        DeGUILayout.Toolbar("Sticky Container");
        DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
        GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
        DeGUILayout.EndVBox();
        GUILayout.Space(_vSpace);

        // Toggle buttons

        DeGUILayout.BeginToolbar(_src.palette.toolbarBg, DeGUI.styles.toolbar.large.Clone());
        GUILayout.Label("Toggle Buttons", DeGUI.styles.label.toolbarL.Clone(_src.palette.toolbarContent));
        _src.toggles[0] = DeGUILayout.ToggleButton(_src.toggles[0], string.Format("Toggle (0)"), _toolbarButtonsColors, DeGUI.styles.button.toolL);
        DeGUILayout.EndToolbar();

        DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
        GUILayout.Label("Toggle buttons (tool)");
        GUILayout.BeginHorizontal();
            for (int i = 0; i < _src.toggles.Length; ++i) {
                _src.toggles[i] = DeGUILayout.ToggleButton(_src.toggles[i], string.Format("Toggle ({0})", i), DeGUI.styles.button.tool);
            }
        GUILayout.EndHorizontal();
        GUILayout.Space(3);
        GUILayout.Label("Toggle buttons (toolL)");
        GUILayout.BeginHorizontal();
            for (int i = 0; i < _src.toggles.Length; ++i) {
                _src.toggles[i] = DeGUILayout.ToggleButton(_src.toggles[i], string.Format("Toggle ({0})", i), DeGUI.styles.button.toolL);
            }
        GUILayout.EndHorizontal();
        GUILayout.Space(3);
        GUILayout.Label("Toggle buttons (normal)");
        GUILayout.BeginHorizontal();
            for (int i = 0; i < _src.toggles.Length; ++i) {
                _src.toggles[i] = DeGUILayout.ToggleButton(_src.toggles[i], string.Format("Toggle ({0})", i));
            }
        GUILayout.EndHorizontal();
        DeGUILayout.EndVBox();
        GUILayout.Space(_vSpace);

        // Foldout

        DeGUILayout.BeginToolbar();
        _src.foldoutOpen = DeGUILayout.ToolbarFoldoutButton(_src.foldoutOpen, "Sticky foldout toolbars");
        DeGUILayout.EndToolbar();
        if (_src.foldoutOpen) {
            DeGUILayout.BeginVBox(DeGUI.styles.box.sticky);
            GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
            DeGUILayout.EndVBox();
        }

        DeGUILayout.BeginToolbar(DeGUI.styles.toolbar.stickyTop);
        _src.foldoutOpen = DeGUILayout.ToolbarFoldoutButton(_src.foldoutOpen);
        DeGUILayout.EndToolbar();
        if (_src.foldoutOpen) {
            DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
            GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
            DeGUILayout.EndVBox();
        }
        GUILayout.Space(_vSpace);
    }

    void DrawColorPalette()
    {
        GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(86));
            GUILayout.Label("Free");
            GUILayout.Label("Pro");
        GUILayout.EndHorizontal();
        SetColor("BG", ref _src.palette.bg.def);
        SetColor("BG On", ref _src.palette.bg.toggleOn);
        SetColor("BG Off", ref _src.palette.bg.toggleOff);
        SetColor("Content", ref _src.palette.content.def);
        SetColor("Content On", ref _src.palette.content.toggleOn);
        SetColor("Content Off", ref _src.palette.content.toggleOff);
        GUILayout.Space(_vSpace);

        if (GUILayout.Button("BACK")) _drawMode = DrawMode.Default;
    }

    void SetColor(string label, ref DeSkinColor skinColor)
    {
        GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(86));
            skinColor.free = EditorGUILayout.ColorField(skinColor.free);
            skinColor.pro = EditorGUILayout.ColorField(skinColor.pro);
        GUILayout.EndHorizontal();
    }
}