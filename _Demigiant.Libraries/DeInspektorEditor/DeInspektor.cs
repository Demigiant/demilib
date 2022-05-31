﻿// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 10:47
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Reflection;
using DG.DeInspektorEditor.AttributesManagers;
using DG.DemiEditor;
using DG.DemiLib;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor
{
    /// <summary>
    /// Used to draw special method attributes.
    /// If you need another generic inspector, inherit from this and override DoEnable, DoDisable and OnInspectorGUI.
    /// </summary>
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)] [CanEditMultipleObjects]
    public class DeInspektor : Editor
    {
        public const string Version = "0.5.300";
        public static DeInspektor I { get; private set; }
        static GUIStyle _arrayElementBtStyle;
        DeMethodButtonEditor _methodButtonEditor;
        DeComponentDescriptionEditor _componentDescriptionEditor;

        #region GUI

        public override void OnInspectorGUI()
        {
            I = this;
            if (_methodButtonEditor == null) _methodButtonEditor = new DeMethodButtonEditor(target);
            if (_componentDescriptionEditor == null) _componentDescriptionEditor = new DeComponentDescriptionEditor(target);
            DeGUI.BeginGUI();
            SetStyles();

            if (DeInspektorPrefs.componentsReordering && !(target is ScriptableObject)) DrawReorderingButtons();
            InjectGUITop();

            _componentDescriptionEditor.Draw();

            switch (DeInspektorPrefs.mode) {
            case DeInspektorPrefs.Mode.Full:
                // Inspector with special features like custom lists
                Draw(this.serializedObject);
                break;
            default:
                // Default inspector with features necessary only to attributes
                base.OnInspectorGUI();
                break;
            }

            _methodButtonEditor.Draw();
            if (I == this) I = null;
        }

        static bool Draw(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
	        obj.Update();
	        SerializedProperty iterator = obj.GetIterator();

	        bool enterChildren = true;
	        while (iterator.NextVisible(enterChildren)) {
		        using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath)) {
                    DrawProperty(iterator);
		        }
		        enterChildren = false;
	        }

	        obj.ApplyModifiedProperties();
	        return EditorGUI.EndChangeCheck();
        }

        static void DrawProperty(SerializedProperty property, bool isArrayElement = false, int arrayElementIndex = -1)
        {
            if (property.isArray && property.propertyType == SerializedPropertyType.Generic) DrawList(property);
            else DrawDefault(property, isArrayElement, arrayElementIndex);
        }

        static void DrawDefault(SerializedProperty iterator, bool isArrayElement, int arrayElementIndex)
        {
            if (isArrayElement) {
                // Array element
                if (iterator.propertyType == SerializedPropertyType.Generic) {
                    // Struct/class as array element: add space to show expand button + use propertyType as label
                    GUILayout.Space(8);
                    EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
                } else EditorGUILayout.PropertyField(iterator, new GUIContent(""), true, new GUILayoutOption[0]);
            } else {
                // Regular property
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
        }

        static void DrawList(SerializedProperty iterator)
        {
            // Header
            // ► Main
            EditorGUILayout.PropertyField(iterator, new GUIContent(string.Format("[{0}] {1}", iterator.arraySize, iterator.displayName)), false, new GUILayoutOption[0]);
            if (DeUnityEditorVersion.MajorVersion >= 2021) {
                // ► Force-add an expand arrow because newer versions of Unity removed them from top headers (why?!?)
                Rect headerRect = GUILayoutUtility.GetLastRect();
                Rect foldoutRect = new Rect(headerRect.x - 15, headerRect.yMax - 18, 20, 20);
                GUIStyle style = iterator.isExpanded ? DeGUI.styles.button.toolFoldoutOpen : DeGUI.styles.button.toolFoldoutClosed;
                if (DeGUI.PressButton(foldoutRect, GUIContent.none, style)) {
                    iterator.isExpanded = !iterator.isExpanded;
                    DeGUI.ExitCurrentEvent();
                    return;
                }
            }
            // Header buttons (to draw them overlayed correctly even if there's decorators assigned to the current property)
            GUILayout.Space(-18);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DeGUILayout.PressButton("+", _arrayElementBtStyle, GUILayout.Width(30))) {
                bool wasExpanded = iterator.isExpanded;
                iterator.isExpanded = true;
                iterator.InsertArrayElementAtIndex(iterator.arraySize);
                RepaintMe();
                if (!wasExpanded) {
                    GUILayout.EndHorizontal();
                    return;
                }
            }
            GUILayout.Space(2);
            Color currColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.red;
            if (DeGUILayout.PressButton("x", _arrayElementBtStyle, GUILayout.Width(30))) {
                iterator.isExpanded = true;
                iterator.ClearArray();
                DeGUI.ExitCurrentEvent();
                RepaintMe();
            }
            GUI.backgroundColor = currColor;
            GUILayout.EndHorizontal();

            if (!iterator.isExpanded || iterator.serializedObject == null) return;

            // List contents
            IList iList = null;
            int minListLen = int.MaxValue;
            Type type = iterator.serializedObject.targetObject.GetType();
            FieldInfo fInfo = type.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            while (fInfo == null && type != typeof(UnityEngine.Object)) {
                // Find property in upper inheritance
                type = type.BaseType;
                if (type == null) break;
                fInfo = type.GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            }
            foreach (UnityEngine.Object o in I.targets) {
                IList li = fInfo.GetValue(o) as IList;
                int l = li == null ? 0 : li.Count;
                if (l >= minListLen) continue;
                iList = li;
                minListLen = l;
            }
            int len = Mathf.Min(minListLen, iterator.arraySize);
            Undo.RecordObject(iterator.serializedObject.targetObject, iterator.serializedObject.targetObject.name);
            for (int i = 0; i < len; ++i) {
                SerializedProperty property = iterator.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(-5);
                float currLabelW = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth -= 25;
                bool wasEnabled = GUI.enabled;
                if (I.targets.Length > 1) GUI.enabled = false;
                if (DeGUILayout.PressButton(i.ToString(), _arrayElementBtStyle, GUILayout.Width(18))) {
                    DeGUIDrag.StartDrag(I, iList, i);
                }
                GUI.enabled = wasEnabled;
                DrawProperty(property, true, i); // Property
                GUILayout.Space(-1);
                if (GUILayout.Button("+", _arrayElementBtStyle, GUILayout.Width(14))) {
                    iterator.InsertArrayElementAtIndex(i);
                    break;
                }
                GUILayout.Space(2);
                if (DeGUILayout.ShadedButton(Color.red, "x", _arrayElementBtStyle, GUILayout.Width(14))) {
                    if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue != null) {
                        // Delete twice otherwise first time it just becomes NULL
                        iterator.DeleteArrayElementAtIndex(i);
                    }
                    iterator.DeleteArrayElementAtIndex(i);
                    break;
                }
                EditorGUIUtility.labelWidth = currLabelW;
                EditorGUILayout.EndHorizontal();
                if (DeGUIDrag.Drag(iList, i).outcome == DeDragResultType.Accepted) {
                    GUI.changed = true;
                }
            }
        }

        static void SetStyles()
        {
            if (_arrayElementBtStyle != null) return;

            _arrayElementBtStyle = DeGUI.styles.button.tool.Clone(9, new DeSkinColor(0.4f, 0.9f)).Height(16).StretchHeight(false)
                .PaddingLeft(0).PaddingRight(0).MarginTop(2)
                .Background(DeGUI.IsProSkin ? DeStylePalette.squareBorderCurvedAlpha : DeStylePalette.squareBorderCurved_darkBordersAlpha);
        }

        #endregion

        #region Public Methods

        public static void RepaintMe()
        {
            if (I == null) return;
            I.Repaint();
        }

        /// <summary>
        /// Can be used to inject GUI elements before any other default GUI calls.
        /// </summary>
        public virtual void InjectGUITop() {}

        /// <summary>
        /// Draws buttons to reorder the component on its header
        /// </summary>
        public static void DrawReorderingButtons()
        {
            GUILayout.Space(-16);
            bool wasGuiEnabled = GUI.enabled;
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (DeGUILayout.PressButton("▲", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                Reorder(true);
            }
            if (DeGUILayout.PressButton("▼", DeGUI.styles.button.tool, GUILayout.Width(16))) {
                Reorder(false);
            }
            GUILayout.Space(32);
            GUILayout.EndHorizontal();
            GUI.enabled = wasGuiEnabled;
        }
        static void Reorder(bool up)
        {
            foreach (UnityEngine.Object o in I.targets) {
                Component c = o as Component;
                Component[] comps = c.GetComponents<Component>();
                int index = Array.IndexOf(comps, c);
                if (up && index > 1) {
                    Undo.RecordObject(c.gameObject, "Move Component Up");
                    UnityEditorInternal.ComponentUtility.MoveComponentUp(c);
                    EditorUtility.SetDirty(c.gameObject);
                } else if (!up && index < comps.Length - 1) {
                    Undo.RecordObject(c.gameObject, "Move Component Down");
                    UnityEditorInternal.ComponentUtility.MoveComponentDown(c);
                    EditorUtility.SetDirty(c.gameObject);
                }
            }
        }

        #endregion
    }
}