// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/11 13:14
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using DG.DeInspektor.Attributes;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeButtonAttribute), true)]
    public class DeButtonPropertyDrawer : DecoratorDrawer
    {
        const int _LineH = 18;
        const int _MarginBottom = 2;

        public override float GetHeight()
        {
            DeButtonAttribute attr = (DeButtonAttribute)attribute;
            switch (attr.position) {
            case DePosition.HHalfRight:
            case DePosition.HThirdMiddle:
            case DePosition.HThirdRight:
                return 0.0f;
            default:
                return _LineH + _MarginBottom;
            }
        }

        public override void OnGUI(Rect position)
        {
            DeButtonAttribute attr = (DeButtonAttribute)attribute;

            Rect r = AttributesManagersUtils.AdaptRectToDePosition(true, position, attr.position, _LineH, _MarginBottom);

            bool disabled = attr.mode == DeButtonMode.NoPlayMode && EditorApplication.isPlayingOrWillChangePlaymode
                            || attr.mode == DeButtonMode.PlayModeOnly && !EditorApplication.isPlaying;
            using (new EditorGUI.DisabledScope(disabled)) {
                Color defBgColor = GUI.backgroundColor;
                Color defContentColor = GUI.contentColor;
                if (attr.bgShade != null) GUI.backgroundColor = DeColorPalette.HexToColor(attr.bgShade);
                if (attr.textShade != null) GUI.contentColor = DeColorPalette.HexToColor(attr.textShade);
                if (GUI.Button(r, attr.text)) {
                    MethodInfo mInfo = attr.targetType.GetMethod(
                        attr.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
                        );
                    if (mInfo == null) Debug.LogWarning(string.Format("Method \"{0}\" missing", attr.methodName));
                    else {
                        if (mInfo.IsStatic) mInfo.Invoke(null, attr.parameters);
                        else {
                            if (attr.targetType.IsSubclassOf(typeof(ScriptableObject))) {
                                // ScriptableObject > call the method directly from it
                                if (Selection.activeObject == null) {
                                    Debug.LogWarning("DeButton ► When calling a non-static Component or ScriptableObject method" +
                                                     " the instance must be selected in Unity's project/hierarchy");
                                } else {
                                    mInfo.Invoke(Selection.activeObject, attr.parameters);
                                    EditorUtility.SetDirty(Selection.activeObject);
                                }
                            } else if (attr.targetType.IsSubclassOf(typeof(Component))) {
                                // Monobehaviour > find it and call the method directly
                                Transform t = Selection.activeTransform;
                                Component c = t == null ? null : t.GetComponent(attr.targetType);
                                if (c == null) {
                                    Debug.LogWarning("DeButton ► When calling a non-static Component or ScriptableObject method" +
                                                     " the instance must be selected in Unity's project/hierarchy");
                                } else {
                                    mInfo.Invoke(c, attr.parameters);
                                }
                            } else {
                                // Normal class > create instance and call method from it
                                mInfo.Invoke(Activator.CreateInstance(attr.targetType), attr.parameters);
                            }
                        }
                    }
                }
                GUI.backgroundColor = defBgColor;
                GUI.contentColor = defContentColor;
            }
        }
    }
}