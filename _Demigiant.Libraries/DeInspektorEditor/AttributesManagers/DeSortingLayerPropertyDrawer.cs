// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/02/26 18:32
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeSortingLayer))]
    public class DeSortingLayerPropertyDrawer : PropertyDrawer
    {
        static string[] _displayedOptions;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (Event.current.type == EventType.Layout) return;

            SortingLayer[] layers = SortingLayer.layers;
            if (_displayedOptions == null || layers.Length != _displayedOptions.Length) _displayedOptions = new string[layers.Length];
            int index = 0;
            for (int i = 0; i < layers.Length; ++i) {
                SortingLayer layer = layers[i];
                _displayedOptions[i] = layer.name;
                if (property.intValue == layer.id) index = i;
            }
            int result = EditorGUI.Popup(position, label.text, index, _displayedOptions);
            property.intValue = layers[result].id;
        }
    }
}