// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/11 13:14
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeButtonAttribute))]
    public class DeButtonPropertyDrawer : DecoratorDrawer
    {
        const int _LineH = 16;
        const int _MarginBottom = 2;
        float _h;

        public override float GetHeight()
        {
            _h = _LineH + _MarginBottom;
            DeButtonAttribute attr = (DeButtonAttribute)attribute;
            switch (attr.position) {
            case DePosition.HHalfRight:
            case DePosition.HThirdMiddle:
            case DePosition.HThirdRight:
                return 0.0f;
            default:
                return _h;
            }
        }

        public override void OnGUI(Rect position)
        {
            DeButtonAttribute attr = (DeButtonAttribute)attribute;

            Rect r = position;
            r.height = _LineH;
            float labelW = EditorGUIUtility.labelWidth;
            switch (attr.position) {
            case DePosition.HDefault:
                r.width -= labelW;
                r.x += labelW;
                break;
            case DePosition.HHalfLeft:
            case DePosition.HHalfRight:
                r.width = r.width * 0.5f;
                r.x += attr.position == DePosition.HHalfLeft ? 0 : r.width;
                if (attr.position != DePosition.HHalfLeft) r.y -= _LineH + _MarginBottom;
                break;
            case DePosition.HThirdLeft:
            case DePosition.HThirdMiddle:
            case DePosition.HThirdRight:
                r.width = r.width * 0.333f;
                r.x += attr.position == DePosition.HThirdLeft
                    ? 0
                    : attr.position == DePosition.HThirdMiddle
                    ? r.width
                    : r.width * 2;
                if (attr.position != DePosition.HThirdLeft) r.y -= _LineH + _MarginBottom;
                break;
            }

            Color defBgColor = GUI.backgroundColor;
            Color defContentColor = GUI.contentColor;
            if (attr.bgShade != null) GUI.backgroundColor = DeEditorUtils.HexToColor(attr.bgShade);
            if (attr.textShade != null) GUI.contentColor = DeEditorUtils.HexToColor(attr.textShade);
            if (GUI.Button(r, attr.text)) {
                MethodInfo mInfo = attr.targetType.GetMethod(
                    attr.methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
                );
                if (mInfo == null) Debug.LogWarning(string.Format("Method \"{0}\" missing", attr.methodName));
                else {
                    if (mInfo.IsStatic) mInfo.Invoke(null, attr.parameters);
                    else {
                        if (attr.targetType.IsSubclassOf(typeof(Component))) {
                            // Monobehaviour > find it and call the method directly
                            Component c = Selection.activeTransform.GetComponent(attr.targetType);
                            mInfo.Invoke(c, attr.parameters);
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