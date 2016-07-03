// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/03 10:20
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Reflection;
using DG.De2D;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    [CustomEditor(typeof(De2DAutosorter))]
    public class De2DAutosorterInspector : Editor
    {
        De2DAutosorter _src;
        static readonly List<De2DAutosorter> _Subscribed = new List<De2DAutosorter>();
        static EditorWindow _gameView;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as De2DAutosorter;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "De2DAutosorter");

            GUILayout.Space(4);

            _src.sortMode = (SortMode)EditorGUILayout.EnumPopup("Sort Mode", _src.sortMode);
            if (_src.sortMode == SortMode.LocalZAxis) {
                _src.zShift = EditorGUILayout.Slider("Z Shift", _src.zShift, 0.0001f, 3f);
            }
            EditorGUILayout.HelpBox(
                "OrderInLayer: Top sprite will have 0 sorting order, the others will be decreased from top to bottom\nLocalZAxis: Top sprite will have 0 local Z depth, the others will be decreased from top to bottom"
            , MessageType.Info);

            if (GUI.changed) Reorder();
        }

        #endregion

        public static void Subscribe(De2DAutosorter src)
        {
            if (_Subscribed.Contains(src)) return;

            _Subscribed.Add(src);
            EditorApplication.hierarchyWindowChanged -= Reorder;
            EditorApplication.hierarchyWindowChanged += Reorder;
        }

        public static void Unsubscribe(De2DAutosorter src)
        {
            _Subscribed.Remove(src);
            if (_Subscribed.Count == 0) EditorApplication.hierarchyWindowChanged -= Reorder;
        }

        static void Reorder()
        {
            foreach (De2DAutosorter de2DAutosorter in _Subscribed) {
                SpriteRenderer[] srs = de2DAutosorter.GetComponentsInChildren<SpriteRenderer>(true);
                switch (de2DAutosorter.sortMode) {
                case SortMode.OrderInLayer:
                    for (int i = 0; i < srs.Length; ++i) {
                        SpriteRenderer sr = srs[i];
                        sr.sortingOrder = i;
                        SetLocalZ(sr.transform, 0);
                    }
                    break;
                case SortMode.LocalZAxis:
                    for (int i = 0; i < srs.Length; ++i) {
                        SpriteRenderer sr = srs[i];
                        sr.sortingOrder = 0;
                        SetLocalZ(sr.transform, -i * de2DAutosorter.zShift);
                    }
                    break;
                }
            }

            // Repaint game view
            if (_gameView == null) {
                Type type = Type.GetType("UnityEditor.GameView, UnityEditor");
                _gameView = EditorWindow.GetWindow(type);
            }
            _gameView.Repaint();
        }

        static void SetLocalZ(Transform t, float value)
        {
            Vector3 localP = t.localPosition;
            localP.z = value;
            t.localPosition = localP;
        }
    }
}