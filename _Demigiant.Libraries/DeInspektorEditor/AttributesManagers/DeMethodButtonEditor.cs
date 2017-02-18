// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 12:02
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DG.DeInspektor.Attributes;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    /// <summary>
    /// Implemented by <see cref="DeInspektor"/>
    /// </summary>
    public class DeMethodButtonEditor
    {
        readonly UnityEngine.Object _target;
        readonly List<MethodInfo> _buttonMethods;
        readonly Dictionary<MethodInfo, DeMethodButtonAttribute[]> _mInfoToAttributes;

        public DeMethodButtonEditor(UnityEngine.Object target)
        {
            _target = target;
            _buttonMethods = target.GetType().GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(
                m => Attribute.IsDefined(m, typeof(DeMethodButtonAttribute), true)
            ).ToList();
            if (_buttonMethods.Count <= 0) return;

            _mInfoToAttributes = new Dictionary<MethodInfo, DeMethodButtonAttribute[]>(_buttonMethods.Count);
            foreach (MethodInfo mInfo in _buttonMethods) {
                DeMethodButtonAttribute[] attributes = mInfo.GetCustomAttributes(typeof(DeMethodButtonAttribute), true) as DeMethodButtonAttribute[];
                Array.Sort(attributes, (a, b) => a.order.CompareTo(b.order));
                _mInfoToAttributes.Add(mInfo, attributes);
            }
        }

        public void Draw()
        {
            if (_buttonMethods.Count == 0) return;

            foreach (MethodInfo mInfo in _buttonMethods) {
                foreach (DeMethodButtonAttribute attr in _mInfoToAttributes[mInfo]) {
                    bool disabled = attr.mode == DeButtonMode.NoPlayMode && EditorApplication.isPlayingOrWillChangePlaymode
                                    || attr.mode == DeButtonMode.PlayModeOnly && !EditorApplication.isPlaying;
                    using (new EditorGUI.DisabledScope(disabled)) {
                        Color defBgColor = GUI.backgroundColor;
                        Color defContentColor = GUI.contentColor;
                        if (attr.bgShade != null) GUI.backgroundColor = DeColorPalette.HexToColor(attr.bgShade);
                        if (attr.textShade != null) GUI.contentColor = DeColorPalette.HexToColor(attr.textShade);
                        if (GUILayout.Button(string.IsNullOrEmpty(attr.text) ? DeButtonAttribute.NicifyMethodName(mInfo.Name) : attr.text)) {
                            // Add default value where optional parameters are not present
                            ParameterInfo[] parmInfos = mInfo.GetParameters();
                            object[] parameters = new object[parmInfos.Length];
                            for (int i = 0; i < parameters.Length; ++i) {
                                if (i < attr.parameters.Length) parameters[i] = attr.parameters[i];
                                else parameters[i] = parmInfos[i].DefaultValue;
                            }
                            // Invoke
                            mInfo.Invoke(_target, parameters);
                        }
                        GUI.backgroundColor = defBgColor;
                        GUI.contentColor = defContentColor;
                    }
                }
            }
        }
    }
}