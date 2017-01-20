// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/15 20:16
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeBeginDisabledAttribute))]
    public class DeBeginDisabledPropertyDrawer : DecoratorDrawer
    {
        public override float GetHeight() { return 0; }

        public override void OnGUI(Rect position)
        {
            DeBeginDisabledAttribute attr = (DeBeginDisabledAttribute)attribute;
            bool isTrue = attr.condition.IsTrue(DeGlobalInspector.I.serializedObject);

            bool wasGUIEnabled = GUI.enabled;
            EditorGUI.EndDisabledGroup();
            EditorGUI.BeginDisabledGroup(isTrue || !wasGUIEnabled);
            EditorGUI.BeginDisabledGroup(false);
        }
    }

    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
    // ███ CLASS ███████████████████████████████████████████████████████████████████████████████████████████████████████████
    // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

    [CustomPropertyDrawer(typeof(DeEndDisabledAttribute))]
    public class DeEndDisabledPropertyDrawer : DecoratorDrawer
    {
        public override float GetHeight() { return 0; }

        public override void OnGUI(Rect position)
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}