// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/09 15:38
// License Copyright (c) Daniele Giardini

using DG.DeAudio.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeAudioEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeAudioClipDataModeAttribute))]
    public class DeAudioClipDataModePropertyDrawer : PropertyDrawer
    {
        DeAudioClipDataPropertyDrawer _drawer;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_drawer == null) _drawer = new DeAudioClipDataPropertyDrawer();
            DeAudioClipDataPropertyDrawer.drawMode = (DeAudioClipDataModeAttribute)attribute;
            float result = _drawer.GetPropertyHeight(property, label);
            DeAudioClipDataPropertyDrawer.drawMode = null;
            return result;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_drawer == null) _drawer = new DeAudioClipDataPropertyDrawer();
            DeAudioClipDataPropertyDrawer.drawMode = (DeAudioClipDataModeAttribute)attribute;
            _drawer.OnGUI(position, property, label);
            DeAudioClipDataPropertyDrawer.drawMode = null;
        }
    }
}