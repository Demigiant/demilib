// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/04/26 13:29

using DG.DeAudio;
using DG.DeAudioEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;
using DG.DemiEditor;
using _Examples.DeGUI.Editor.DeGUINode;

[CustomEditor(typeof(DeSampler))]
public class DeSamplerInspector : Editor
{
    enum DrawMode
    {
        Default,
        ColorPalette
    }

    const int _DragID0 = 0;
    DeSampler _src;
    DrawMode _drawMode;
    DeColorPalette _toolbarButtonsColors;
    float _vSpace = 8;
    DeAudioClipGUIMode _deAudioGUIMode = DeAudioClipGUIMode.Full;
    bool _allowDeAudioGroupIdChange = true;

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

        if (GUILayout.Button("Open DeGUINodeSampler")) DeGUINodeSampler.ShowWindow(_src);
        GUILayout.Space(4);

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

        // Press button

        DeGUILayout.PressButton("Press Button", DeGUI.styles.button.def);

        // Toolbars + toolbar content

        using (new DeGUILayout.ToolbarScope()) {
            GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbar);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        }
        GUILayout.Space(4);
        using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.box)) {
        GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbarBox);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        }
        GUILayout.Space(4);
        using (new DeGUILayout.ToolbarScope(Color.black, DeGUI.styles.toolbar.flat)) {
        GUILayout.Label("Toolbar | UPPERCASE", DeGUI.styles.label.toolbarBox.Clone(Color.white));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("▲", DeGUI.styles.button.toolIco)) Debug.Log("▲ pressed");
            if (GUILayout.Button("▼", DeGUI.styles.button.toolIco)) Debug.Log("▼ pressed");
            if (DeGUILayout.ColoredButton(DeGUI.colors.bg.critical, DeGUI.colors.content.critical, "X", DeGUI.styles.button.toolIco.Clone(FontStyle.Bold))) Debug.Log("x pressed");
        }
        GUILayout.Space(_vSpace);

        // Draggable list

        for (int i = 0; i < _src.strList0.Count; ++i) {
            DeGUILayout.BeginToolbar();
            if (DeGUILayout.PressButton("Drag me " + _src.strList0[i], DeGUI.styles.button.tool)) {
                DeGUIDrag.StartDrag(this, _src.strList0, i);
            }
            DeGUILayout.EndToolbar();
            if (DeGUIDrag.Drag(_src.strList0, i).outcome == DeDragResultType.Accepted) GUI.changed = true;
        }
        GUILayout.Space(3);
        for (int i = 0; i < _src.strList1.Count; ++i) {
            DeGUILayout.BeginToolbar();
            if (DeGUILayout.PressButton("Drag me " + _src.strList1[i], DeGUI.styles.button.tool)) {
                DeGUIDrag.StartDrag(this, _src.strList1, i);
            }
            DeGUILayout.EndToolbar();
            if (DeGUIDrag.Drag(_src.strList1, i).outcome == DeDragResultType.Accepted) GUI.changed = true;
        }
        GUILayout.Space(3); // Both vertical and horizontal
        float h = (int)(_src.strListAlt.Count * 0.5f) * EditorGUIUtility.singleLineHeight;
        Rect areaR = GUILayoutUtility.GetRect(0, float.MaxValue, h, h);
        float elW = (int)(areaR.width * 0.5f);
        Rect elR = areaR.SetWidth(elW).SetHeight(EditorGUIUtility.singleLineHeight);
        for (int i = 0; i < _src.strListAlt.Count; ++i) {
            if (i % 2 == 0) {
                elR = elR.SetX(areaR.x);
                if (i > 0) elR.y += elR.height;
            } else elR = elR.SetX(areaR.x + elR.width);
            if (DeGUI.PressButton(elR, "Drag me " + _src.strListAlt[i], DeGUI.styles.button.tool)) {
                DeGUIDrag.StartDrag(this, _src.strListAlt, i);
            }
            if (DeGUIDrag.Drag(_src.strListAlt, i, elR).outcome == DeDragResultType.Accepted) GUI.changed = true;
        }

        // Container

        DeGUILayout.Toolbar("Sticky Container");
        DeGUILayout.BeginVBox(DeGUI.styles.box.stickyTop);
        GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
        DeGUILayout.EndVBox();
        GUILayout.Space(_vSpace);

        // Toggle buttons

        using (new DeGUILayout.ToolbarScope(_src.palette.toolbarBg, DeGUI.styles.toolbar.large.Clone())) {
            GUILayout.Label("Toggle Buttons", DeGUI.styles.label.toolbarL.Clone(_src.palette.toolbarContent));
            _src.toggles[0] = DeGUILayout.ToggleButton(_src.toggles[0], string.Format("Toggle (0)"), _toolbarButtonsColors, DeGUI.styles.button.toolL);
        }

        using (new DeGUILayout.VBoxScope(DeGUI.styles.box.stickyTop)) {
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
        }
        GUILayout.Space(_vSpace);

        // Foldout

        using (new DeGUILayout.ToolbarScope()) {
            _src.foldoutOpen = DeGUILayout.ToolbarFoldoutButton(_src.foldoutOpen, "Sticky foldout toolbars");
        }
        if (_src.foldoutOpen) {
            using (new DeGUILayout.VBoxScope(DeGUI.styles.box.sticky)) {
                GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
            }
        }

        using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.stickyTop)) {
            _src.foldoutOpen = DeGUILayout.ToolbarFoldoutButton(_src.foldoutOpen, GUIContent.none);
        }
        if (_src.foldoutOpen) {
            using (new DeGUILayout.VBoxScope(DeGUI.styles.box.stickyTop)) {
                GUILayout.Label("Blah blah blah here's a label inside a VBox with some wordwrapping just to be on the safe side", DeGUI.styles.label.wordwrap);
            }
        }
        GUILayout.Space(_vSpace);

        // Toggles

        GUILayout.BeginHorizontal();
            _src.toggles[0] = EditorGUILayout.Toggle(_src.toggles[0], GUILayout.Width(16));
            GUILayout.Button("Some Button");
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
            _src.toggles[0] = EditorGUILayout.Toggle(_src.toggles[0], GUILayout.Width(16));
            GUILayout.Label("Some label");
        GUILayout.EndHorizontal();
        GUILayout.Space(_vSpace);

        // TODO Draggable elements
        using (new DeGUILayout.ToolbarScope(DeGUI.styles.toolbar.stickyTop)) {
            GUILayout.Label("Draggable editable textFields");
        }
        using (new DeGUILayout.VBoxScope(DeGUI.styles.box.stickyTop)) {
            EditorGUILayout.HelpBox("Double click a label to edit it, drag it to change its position (works with both arrays and lists)", MessageType.Info);
            for (int i = 0; i < _src.draggableLabels.Length; ++i) {
                string l = _src.draggableLabels[i];
                _src.draggableLabels[i] = DeGUILayout.DoubleClickDraggableTextField(this, i.ToString(), l, _src.draggableLabels, i, DeGUI.styles.label.toolbar);
                if (DeGUIDrag.Drag(_src.draggableLabels, i).outcome == DeDragResultType.Accepted) EditorUtility.SetDirty(_src);
            }
        }

        // DeAudio

        _deAudioGUIMode = (DeAudioClipGUIMode)EditorGUILayout.EnumPopup("DeAudioGUIMode", _deAudioGUIMode);
        _allowDeAudioGroupIdChange = EditorGUILayout.Toggle("Allow Group Change", _allowDeAudioGroupIdChange);
        _src.deAudioClip = DeAudioGUILayout.DeAudioClip("Some Clip", _src.deAudioClip, _allowDeAudioGroupIdChange, _deAudioGUIMode);
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