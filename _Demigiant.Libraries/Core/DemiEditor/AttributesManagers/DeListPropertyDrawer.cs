// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/19 11:19
// License Copyright (c) Daniele Giardini

using System.Collections;
using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    /// <summary>
    /// Note: this propertyDrawer is called only if the list is expanded in the array and has at least one element, and is called for each list element
    /// </summary>
    [CustomPropertyDrawer(typeof(DeListAttribute))]
    public class DeListPropertyDrawer : PropertyDrawer
    {
        const int _LineH = 16;
        const int _SpaceH = 2;
        static GUIStyle _btStyle;
        int _listId = -1;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty li = property.serializedObject.FindProperty(fieldInfo.Name);
            if (IsFirstLIstElement(li, property)) return li.arraySize * _LineH + li.arraySize * _SpaceH;
            return 0;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (position.height < 1) return; // Not first list element

            DeGUI.BeginGUI();
            SetStyles();
            if (_listId == -1) _listId = UnityEngine.Random.Range(0, int.MaxValue);

            SerializedProperty li = property.serializedObject.FindProperty(fieldInfo.Name);

            EditorGUI.BeginProperty(position, label, li);
            Undo.RecordObject(property.serializedObject.targetObject, property.serializedObject.targetObject.name);
            IList iList = fieldInfo.GetValue(li.serializedObject.targetObject) as IList;
            int listLen = iList.Count;
            if (listLen > 0) GUILayout.Space(-position.height - listLen * 2 - _SpaceH);
            for (int i = 0; i < listLen; ++i) {
                SerializedProperty el = li.GetArrayElementAtIndex(i);
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel + 16);
                if (DeGUILayout.PressButton(i.ToString(), _btStyle, GUILayout.Width(20))) {
                    DeGUIDrag.StartDrag(_listId, DeGlobalInspector.I, iList, i);
                }
                GUILayout.Space(-12);
                Rect r = GUILayoutUtility.GetRect(position.width - 66, _LineH);
                r.y += 2;
                DeGUI.DefaultPropertyField(r, el, new GUIContent(""));
                GUILayout.Space(2);
                if (GUILayout.Button("+", _btStyle, GUILayout.Width(16))) {
                    li.InsertArrayElementAtIndex(i);
                    break;
                }
                GUILayout.Space(2);
                if (DeGUILayout.ShadedButton(Color.red, "x", _btStyle, GUILayout.Width(16))) {
                    if (el.objectReferenceValue != null) li.DeleteArrayElementAtIndex(i); // Delete twice otherwise first time it just becomes NULL
                    li.DeleteArrayElementAtIndex(i);
                    break;
                }
                GUILayout.Space(1);
                GUILayout.EndHorizontal();
                GUILayout.Space(2);
                GUILayout.EndVertical();
                if (DeGUIDrag.Drag(_listId, iList, i).outcome == DeDragResultType.Accepted) {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        static bool IsFirstLIstElement(SerializedProperty list, SerializedProperty element)
        {
            return list.GetArrayElementAtIndex(0).displayName == element.displayName;
        }

        static void SetStyles()
        {
            if (_btStyle != null) return;

            _btStyle = DeGUI.styles.button.tool.Clone(9).Height(_LineH).StretchHeight(false).MarginTop(2).PaddingLeft(0).PaddingRight(0);
        }
    }
}