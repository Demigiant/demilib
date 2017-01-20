// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/16 17:54
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeBeginGroupAttribute))]
    public class DeBeginGroupPropertyDrawer : DecoratorDrawer
    {
        int _count;

        // Used to BeginVertical, because starting it during OnGUI will otherwise begin after this property has been drawm
        public override float GetHeight()
        {
            _count++;
            if (_count == 1) EditorGUILayout.BeginVertical(GUI.skin.box);
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            _count = 0;
        }
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [CustomPropertyDrawer(typeof(DeEndGroupAttribute))]
    public class DeEndGroupPropertyDrawer : DecoratorDrawer
    {
        int _count;

        public override float GetHeight()
        {
            _count++;
            if (_count == 2) EditorGUILayout.EndVertical();
            return 0;
        }

        public override void OnGUI(Rect position)
        {
            _count = 0;
        }
    }
}