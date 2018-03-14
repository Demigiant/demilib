// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/14 11:00
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    internal class DeProjectCustomizeItemWindow : EditorWindow
    {
        public static readonly List<DeProjectData.CustomizedItem> Items = new List<DeProjectData.CustomizedItem>();

        const string _Title = "DeProject Customize";
        const int _Width = 250;
        const int _Height = 100;
        static DeProjectData _src;

        #region Unity and GUI Methods

        void OnEnable()
        { Undo.undoRedoPerformed += Repaint; }

        void OnDisable()
        { Undo.undoRedoPerformed -= Repaint; }

        void OnGUI()
        {
            this.position = new Rect(position.x, position.y, _Width, _Height);

            Undo.RecordObject(_src, "DeProject");
            DeGUI.BeginGUI();
            bool changed = false;
            DeProjectData.CustomizedItem sampleItem = Items[0];

            if (Event.current.type == EventType.KeyDown) {
                switch (Event.current.keyCode) {
                case KeyCode.Escape:
                    this.Close();
                    return;
                }
            }

            using (var check = new EditorGUI.ChangeCheckScope()) {
                Color color = EditorGUILayout.ColorField(sampleItem.GetColor());
                if (check.changed) {
                    changed = true;
                    for (int i = 0; i < Items.Count; ++i) {
                        DeProjectData.CustomizedItem item = Items[i];
                        item.hColor = DeProjectData.HColor.Custom;
                        item.customColor = color;
                    }
                }
            }
            using (var check = new EditorGUI.ChangeCheckScope()) {
                Texture2D icon = (Texture2D)EditorGUILayout.ObjectField(sampleItem.customIcon, typeof(Texture2D), false);
                int maxSize = sampleItem.customIconMaxSize;
                int offsetX = sampleItem.customIconOffsetX;
                int offsetY = sampleItem.customIconOffsetY;
                if (icon != null) {
                    using (new DeGUI.LabelFieldWidthScope(86)) {
                        maxSize = (int)EditorGUILayout.Slider("Icon Max Size", maxSize, 5, 30);
                        offsetX = (int)EditorGUILayout.Slider("Icon Offset X", offsetX, -16, 16);
                        offsetY = (int)EditorGUILayout.Slider("Icon Offset Y", offsetY, -16, 16);
                    }
                }
                if (check.changed) {
                    changed = true;
                    for (int i = 0; i < Items.Count; ++i) {
                        DeProjectData.CustomizedItem item = Items[i];
                        item.icoType = icon == null ? DeProjectData.IcoType.None : DeProjectData.IcoType.Custom;
                        item.customIcon = icon;
                        if (icon != null) {
                            item.customIconMaxSize = maxSize;
                            item.customIconOffsetX = offsetX;
                            item.customIconOffsetY = offsetY;
                        }
                    }
                }
            }

            if (changed) {
                EditorApplication.RepaintProjectWindow();
                EditorUtility.SetDirty(_src);
            }
        }

        #endregion

        #region Public Methods

        public static void Open(DeProjectData src)
        {
            _src = src;
            GetWindow(typeof(DeProjectCustomizeItemWindow), true, _Title);
        }

        #endregion
    }
}