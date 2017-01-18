// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/11 13:14
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using DG.DemiLib;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeButtonAttribute))]
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