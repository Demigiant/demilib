// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/19 12:31
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiLib;
using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// You can attach to this
    /// </summary>
    public class HelpPanel
    {
        public bool isOpen { get; private set; }

        const int _InnerPadding = 11;
        static readonly Styles _Styles = new Styles();
        static readonly Color _EvidenceColor = new Color(0.05490196f, 0.5960785f, 0.9725491f, 1f);
        static readonly Color _DescriptionColor = new Color(0.5998054f, 0.7537874f, 0.9485294f, 1f);
        static readonly Color _DescriptionBGColor = new Color(0.1386786f, 0.2625088f, 0.4191176f, 1f);
        static readonly Color _KeysColor = new Color(1f, 0.8068966f, 0f, 1f);
        static readonly Color _RowColor0 = new Color(0.08f,0.08f, 0.08f, 0.94f);
        static readonly Color _RowColor1 = new Color(0.16f, 0.16f, 0.16f, 0.94f);
        NodeProcess _process;
        readonly List<ContentGroup> _ContentGroups = new List<ContentGroup>();
        bool _layoutReady; // Used to prevent a repaint until a layout has happened
        Vector2 _scroll;

        #region Constructor

        public HelpPanel(NodeProcess nodeProcess)
        {
            _process = nodeProcess;

            // Write down help content
            // GENERAL
            ContentGroup general = AddContentGroup("General");
            general.AppendDefinition("Open/Close Help Panel").AddKey("F1");
            general.AppendDefinition("Pan area").AddKey("MMB → Drag");
            general.AppendDefinition("Zoom in/out (if allowed)").AddKey("CTRL+Scrollwheel");
            general.AppendDefinition("Show extra UI buttons").AddKey("ALT");
            general.AppendDefinition("Background context menu").AddKey("RMB");
            // SELECTION
            ContentGroup selection = AddContentGroup("Selection");
            selection.AppendDefinition("Select all nodes").AddKey("CTRL+A");
            selection.AppendDefinition("Draw selection rect").AddKey("LMB → Drag").AddKeyTarget("on background");
            selection.AppendDefinition("Draw selection rect (add)").AddKey("SHIFT+LMB → Drag").AddKeyTarget("on background");
            selection.AppendDefinition("Add/Remove node from selection").AddKey("SHIFT+LMB").AddKeyTarget("on node");
            selection.AppendDefinition("Add node plus all forward connected nodes to selection").AddKey("SHIFT+ALT+LMB").AddKeyTarget("on node");
            // NODE MANIPULATION
            ContentGroup nodes = AddContentGroup("Nodes Manipulation");
            nodes.AppendDefinition("Delete selected nodes").AddKey("DELETE").AddKey("BACKSPACE");
            nodes.AppendDefinition("Copy selected nodes").AddKey("CTRL+C");
            nodes.AppendDefinition("Paste nodes").AddKey("CTRL+V");
            nodes.AppendDefinition("Move selected nodes by 1 pixel").AddKey("ARROWS");
            nodes.AppendDefinition("Move selected nodes by 10 pixel").AddKey("SHIFT+ARROWS");
            nodes.AppendDefinition("Disable snapping while dragging nodes").AddKey("ALT");
            nodes.AppendDefinition("Drag new connection from node (if allowed)").AddKey("CTRL+LMB → Drag");
            nodes.AppendDefinition("Clone selected nodes and drag them").AddKey("SHIFT+CTRL+LMB → Drag");
            nodes.AppendDefinition("Node context menu").AddKey("RMB");
        }

        #endregion

        #region GUI Methods

        // Returns FALSE if the help panel should be closed
        public bool Draw()
        {
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == KeyCode.F1) {
                Event.current.Use();
                return false;
            }

            // Block and event until the first layout has happened
            if (!_layoutReady) {
                if (Event.current.type == EventType.Layout) _layoutReady = true;
                else return true;
            }

            _Styles.Init();
            Matrix4x4 prevGuiMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.TRS(_process.position.min - _process.position.min / _process.guiScale, Quaternion.identity, Vector3.one);

            // Background (adapt area to counter GUI scale)
            Rect area = new Rect(
                _process.relativeArea.x, _process.relativeArea.y,
                _process.relativeArea.width * _process.guiScale, _process.relativeArea.height * _process.guiScale
            );
            using (new DeGUI.ColorScope(null, null, new Color(0.14f, 0.14f, 0.14f, 0.6f))) GUI.DrawTexture(area, DeStylePalette.whiteSquare);

            // Content
            GUILayout.BeginArea(area);
            _scroll = GUILayout.BeginScrollView(_scroll);
            GUILayout.Label("Help Panel", _Styles.titleLabel);
            DeGUILayout.HorizontalDivider(_EvidenceColor, 1, 0, 0);
            int descriptionWidth = (int)Mathf.Min(area.width * 0.6f, 350);
            foreach (ContentGroup contentGroup in _ContentGroups) {
                GUILayout.Label(contentGroup.title, _Styles.groupTitleLabel);
                if (!string.IsNullOrEmpty(contentGroup.description)) {
                    using (new DeGUI.ColorScope(_DescriptionBGColor)) GUILayout.Label(contentGroup.description, _Styles.descriptionLabel);
                }
                for (int r = 0; r < contentGroup.definitions.Count; ++r) {
                    Definition definition = contentGroup.definitions[r];
                    using (new DeGUI.ColorScope(r % 2 == 0 ? _RowColor0 : _RowColor1)) GUILayout.BeginHorizontal(_Styles.rowBox);
                    GUILayout.Label(definition.definition, _Styles.definitionLabel, GUILayout.Width(descriptionWidth));
                    GUILayout.Label(definition.keys, _Styles.keysLabel);
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            GUI.matrix = prevGuiMatrix;
            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Use this to add a content group to the Help Panel
        /// </summary>
        public ContentGroup AddContentGroup(string title, string description = null)
        {
            ContentGroup result = new ContentGroup(title, description);
            _ContentGroups.Add(result);
            return result;
        }

        #endregion

        #region Internal Methods

        internal void Open(bool doOpen)
        {
            isOpen = doOpen;
            if (doOpen) _layoutReady = false;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public class ContentGroup
        {
            internal readonly string title;
            internal readonly string description;
            internal readonly List<Definition> definitions = new List<Definition>();
            internal ContentGroup(string title, string description)
            {
                this.title = title;
                this.description = description;
            }
            public Definition AppendDefinition(string value)
            {
                Definition definition = new Definition(value);
                definitions.Add(definition);
                return definition;
            }
        }

        public class Definition
        {
            internal readonly string definition;
            internal string keys;
            internal Definition(string value)
            {
                definition = value;
                keys = "";
            }
            /// <summary>
            /// Add key on new line, automatically formatting these special keys:<para/>
            /// /<para/>
            /// +<para/>
            /// →
            /// </summary>
            public Definition AddKey(string value)
            {
                if (string.IsNullOrEmpty(keys)) keys = value;
                else keys += ",\n" + value;
                keys = keys.Replace("/", "<color=#ffffff>/</color>");
                keys = keys.Replace("+", "<color=#ffffff>+</color>");
                keys = keys.Replace("→", "<color=#ffffff><b>→</b></color>");
                keys = keys.Replace(",", "<color=#ffffff>,</color>");
                return this;
            }

            public Definition AddKeyTarget(string value)
            {
                keys = string.Format("{0} <color=#ffffff>{1}</color>", keys, value);
                return this;
            }
        }

        class Styles
        {
            public GUIStyle rowBox, titleLabel, groupTitleLabel, descriptionLabel, definitionLabel, keysLabel;
            bool _initialized;

            public void Init()
            {
                if (_initialized) return;

                _initialized = true;
                rowBox = new GUIStyle().Margin(0).Padding(0).Background(DeStylePalette.whiteSquare).StretchWidth();
                titleLabel = new GUIStyle(GUI.skin.label).Add(16, Color.white, Format.RichText).Background(DeStylePalette.blueSquare)
                    .Padding(_InnerPadding, _InnerPadding, 4, 4).Margin(0).StretchWidth();
                groupTitleLabel = new GUIStyle(GUI.skin.label).Add(Format.WordWrap, Color.white).Background(DeStylePalette.blueSquare)
                    .Padding(_InnerPadding, 4, 8, 4).Margin(0).StretchWidth();
                descriptionLabel = new GUIStyle(GUI.skin.label).Add(Format.WordWrap, _DescriptionColor).Background(DeStylePalette.whiteSquare)
                    .Padding(_InnerPadding, 4, 4, 4).Margin(0).StretchWidth();
                definitionLabel = new GUIStyle(GUI.skin.label).Add(Format.WordWrap, new DeSkinColor(0.75f))
                    .Padding(_InnerPadding, 4, 4, 4).Margin(0);
                keysLabel = new GUIStyle(GUI.skin.label).Add(Format.WordWrap, Format.RichText, _KeysColor)
                    .Padding(0, _InnerPadding, 4, 4).Margin(0);
            }
        }
    }
}