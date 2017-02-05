// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/01 10:44
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Scene
{
    public class DeSceneMainPopup : PopupWindowContent
    {
        static DeSceneMainPopup I;
        bool _isStartup;
        Vector2 _popupSize = new Vector2(-1, -1);
        readonly Dictionary<int, Rect> _menuItemToRect = new Dictionary<int, Rect>();
        int _hoverItemIndex = -1;
        PopupWindowContent _currSubpopup;
        Rect _subpopupRect;
        bool _subpopupOpenRequired;

        const int _ItemH = 20;
        static GUIStyle _menuItemBox, _menuItemLabel, _menuItemArrow;
        const string _EvidenceColor = "#f348bd";

        #region GUI

        public override void OnOpen()
        {
            I = this;
            _isStartup = true;
        }

        public override void OnClose()
        {
            if (I == this) I = null;
            _menuItemToRect.Clear();
            _currSubpopup = null;
            _subpopupOpenRequired = false;
        }

        public override Vector2 GetWindowSize()
        {
            _popupSize.y = DeScene.OrthoCams.Count * _ItemH + 1;
            return new Vector2(_isStartup ? 400 : _popupSize.x, _popupSize.y);
        }

        public override void OnGUI(Rect rect)
        {
            SetStyles();
            PopupWindowContent prevSubpopup = null;

            // MenuItems > Align to camera
            for (int i = 0; i < DeScene.OrthoCams.Count; ++i) {
                Camera cam = DeScene.OrthoCams[i];
                bool isOver = !_isStartup && _menuItemToRect[i].Contains(Event.current.mousePosition);
                if (isOver && _hoverItemIndex != i) {
                    _hoverItemIndex = i;
                    prevSubpopup = _currSubpopup;
                    _currSubpopup = new DeSceneAlignSubpopup(cam, DeScene.SelectedSpriteRenderers);
                    _subpopupRect = _menuItemToRect[i];
                    _subpopupRect.x += _popupSize.x - 1;
                    _subpopupRect.y -= _subpopupRect.height;
                    _subpopupOpenRequired = true;
                    this.editorWindow.Repaint();
                }
                using (new DeGUI.ColorScope(isOver ? new Color(0.2043685f, 0.3257368f, 0.7720588f, 1f) : (Color)new DeSkinColor(0.15f))) {
                    using (new GUILayout.HorizontalScope(_menuItemBox)) {
                        GUILayout.Label(string.Format("<color=\"{0}\"><b>Align To → </b></color>{1}    ", _EvidenceColor, cam.name), _menuItemLabel, GUILayout.ExpandWidth(!_isStartup));
                    }
                }
                if (Event.current.type == EventType.Repaint) {
                    if (_isStartup) {
                        Rect r = GUILayoutUtility.GetLastRect();
                        _menuItemToRect.Add(i, r);
                        if (_popupSize.x < r.width) _popupSize.x = (int)r.width;
                    } else {
                        Rect r = _menuItemToRect[i];
                        Rect arrowR = new Rect(r.xMax - 17, r.y, 17, r.height);
                        GUI.Label(arrowR, "▸", _menuItemArrow);
                    }
                }
            }

            if (_isStartup && Event.current.type == EventType.Repaint) {
                _isStartup = false;
                // Set max width to all menuItems rects
                List<int> keys = new List<int>(_menuItemToRect.Keys);
                foreach (int key in keys) {
                    Rect r = _menuItemToRect[key];
                    r.width = _popupSize.x;
                    _menuItemToRect[key] = r;
                }
                this.editorWindow.Repaint();
            }
            if (_subpopupOpenRequired) {
                if (prevSubpopup != null) prevSubpopup.editorWindow.Close();
                _subpopupOpenRequired = false;
                PopupWindow.Show(_subpopupRect, _currSubpopup);
            }
        }

        static void SetStyles()
        {
            if (_menuItemBox != null) return;

            _menuItemBox = DeGUI.styles.button.bBlankBorder.Clone().StretchWidth(false).Background(DeStylePalette.whiteSquare)
                .Height(_ItemH).Margin(0);
            _menuItemLabel = new GUIStyle(GUI.skin.label).Add(Format.RichText, new DeSkinColor(0.9f));
            _menuItemArrow = _menuItemLabel.Clone(14);
        }

        #endregion

        #region Public Methods

        public static void CloseAll()
        {
            EditorWindow.FocusWindowIfItsOpen<SceneView>();
        }

        #endregion
    }
}