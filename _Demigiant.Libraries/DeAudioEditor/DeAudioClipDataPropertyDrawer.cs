// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/24 18:26
// License Copyright (c) Daniele Giardini

using DG.DeAudio;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeAudioEditor
{
    [CustomPropertyDrawer(typeof (DeAudioClipData))]
    public class DeAudioClipDataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            DeGUI.BeginGUI();
            int orIndent = EditorGUI.indentLevel;

            float lineH = EditorGUIUtility.singleLineHeight;
            float listVSpacer = 1f;
            float btW = 25;
            Rect clipRect = new Rect(position.x, position.y, position.width - btW * 2 - 4, lineH);
            Rect btPlayRect = new Rect(position.x + (position.width - btW * 2), clipRect.y - 1, btW, lineH);
            Rect btStopRect = new Rect(position.x + (position.width - btW), clipRect.y - 1, btW, lineH);
            Rect volumeRect = new Rect(position.x, position.y + lineH + listVSpacer, position.width, lineH);

            EditorGUI.PropertyField(clipRect, property.FindPropertyRelative("clip"), label);
            EditorGUI.indentLevel = 1;
            EditorGUI.Slider(volumeRect, property.FindPropertyRelative("volume"), 0, 1, "└ Volume");
            if (GUI.Button(btPlayRect, "►", DeGUI.styles.button.tool)) DeSoundUtils.Play(property.FindPropertyRelative("clip").objectReferenceValue as AudioClip);
            if (GUI.Button(btStopRect, "■", DeGUI.styles.button.tool)) DeSoundUtils.StopAll();

            EditorGUI.indentLevel = orIndent;
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 3;
        }
    }
}