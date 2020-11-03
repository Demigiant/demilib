// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/03 10:20
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
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
            _src.ignoreEditorOnly = EditorGUILayout.Toggle("Ignore EditorOnly", _src.ignoreEditorOnly);
            _src.invert = EditorGUILayout.Toggle("Invert", _src.invert);
            switch (_src.sortMode) {
            case SortMode.OrderInLayer:
                _src.sortFrom = EditorGUILayout.IntField("Starting Order", _src.sortFrom);
                break;
            case SortMode.LocalZAxis:
                _src.zShift = EditorGUILayout.Slider("Z Shift", _src.zShift, 0.0001f, 3f);
                break;
            }

            if (GUI.changed) {
                // if (_src.sortFrom < 0) _src.sortFrom = 0;
                Reorder();
            }
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
            for (int i = _Subscribed.Count - 1; i > -1; --i) {
                De2DAutosorter de2DAutosorter = _Subscribed[i];
                if (de2DAutosorter == null) {
                    // Happens when undoing a De2DAutoSorter AddComponent
                    _Subscribed.RemoveAt(i);
                    if (i == 0 && _Subscribed.Count == 0) EditorApplication.hierarchyWindowChanged -= Reorder;
                    continue;
                }
                Renderer[] rs = de2DAutosorter.GetComponentsInChildren<Renderer>(true);
                int sortIncrement = 0;
                switch (de2DAutosorter.sortMode) {
                case SortMode.OrderInLayer:
                    foreach (Renderer r in rs) {
                        if (de2DAutosorter.ignoreEditorOnly && IsEditorOnly(r.transform)) continue;
                        r.sortingOrder = de2DAutosorter.sortFrom + (de2DAutosorter.invert ? -sortIncrement : sortIncrement);
                        SetLocalZ(r.transform, 0);
                        sortIncrement++;
                    }
                    break;
                case SortMode.LocalZAxis:
                    foreach (Renderer r in rs) {
                        if (de2DAutosorter.ignoreEditorOnly && IsEditorOnly(r.transform)) continue;
                        r.sortingOrder = de2DAutosorter.sortFrom;
                        SetLocalZ(r.transform, (de2DAutosorter.invert ? sortIncrement : -sortIncrement) * de2DAutosorter.zShift);
                        sortIncrement++;
                    }
                    break;
                }
            }

            // Repaint game view and Scene
            if (_gameView == null) {
                Type type = Type.GetType("UnityEditor.GameView, UnityEditor");
                _gameView = EditorWindow.GetWindow(type);
            }
            _gameView.Repaint();
            SceneView.RepaintAll();
        }

        static void SetLocalZ(Transform t, float value)
        {
            Vector3 localP = t.localPosition;
            localP.z = value;
            t.localPosition = localP;
        }

        static bool IsEditorOnly(Transform t)
        {
            const string editorOnly = "EditorOnly";
            if (t.tag == editorOnly) return true;
            while (t.parent != null) {
                t = t.parent;
                if (t.tag == editorOnly) return true;
            }
            return false;
        }
    }
}