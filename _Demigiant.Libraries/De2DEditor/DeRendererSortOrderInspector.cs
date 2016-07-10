// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/06 10:56
// License Copyright (c) Daniele Giardini

using DG.De2D;
using UnityEditor;
using UnityEngine;

namespace DG.De2DEditor
{
    [CustomEditor(typeof(DeRendererSortOrder))]
    public class DeRendererSortOrderInspector : Editor
    {
        DeRendererSortOrder _src;

        #region Unity and GUI Methods

        void OnEnable()
        {
            _src = target as DeRendererSortOrder;
        }

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(_src, "DeRendererSortOrder");

            _src.fooSortingOrder = EditorGUILayout.IntField("Sorting Order", _src.fooSortingOrder);

            if (GUI.changed) _src.sortingOrder = _src.fooSortingOrder;
        }

        #endregion
    }
}