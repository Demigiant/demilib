// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/12 10:47
// License Copyright (c) Daniele Giardini

using System.Collections;
using System.Collections.Generic;
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
        public static DeInspektor I { get; private set; }
        static GUIStyle _arrayElementBtStyle;
        DeMethodButtonEditor _methodButtonEditor;
        int _listId;

        #region GUI

        public override void OnInspectorGUI()
        {
            I = this;
            if (_methodButtonEditor == null) _methodButtonEditor = new DeMethodButtonEditor(target);
            DeGUI.BeginGUI();
            SetStyles();

            // Draw default inspector
//            Debug.Log("<color=#00ff00>>>>>>>>> GUI > " + Event.current.type + "</color>");
            _listId = 0;
            Draw(this.serializedObject);

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
//            Debug.Log("DrawProperty > " + property.name + " > " + property.propertyType + "/" + property.type + " > " + property.isArray);
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
                    EditorGUILayout.PropertyField(iterator, new GUIContent(string.Format("{0} {1}", iterator.type, arrayElementIndex)), true, new GUILayoutOption[0]);
                } else EditorGUILayout.PropertyField(iterator, new GUIContent(""), true, new GUILayoutOption[0]);
//            } else if (iterator.propertyType == SerializedPropertyType.Generic) {
//                // Generic class/struct (expandable)
//                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            } else {
                // Regular property
                EditorGUILayout.PropertyField(iterator, true, new GUILayoutOption[0]);
            }
        }

        static void DrawList(SerializedProperty iterator)
        {
//            Debug.Log("DrawList > " + iterator.name + " > " + iterator.propertyType + "/" + iterator.type);
            EditorGUILayout.PropertyField(iterator, new GUIContent(string.Format("{0} [{1}]", iterator.displayName, iterator.arraySize)), false, new GUILayoutOption[0]);
            if (!iterator.isExpanded) return;

            I._listId++;
            FieldInfo fInfo = iterator.serializedObject.targetObject.GetType().GetField(iterator.name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            IList iList = fInfo.GetValue(iterator.serializedObject.targetObject) as IList;
            int len = iList.Count;
            Undo.RecordObject(iterator.serializedObject.targetObject, iterator.serializedObject.targetObject.name);
            for (int i = 0; i < len; ++i) {
                SerializedProperty property = iterator.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                if (DeGUILayout.PressButton(i.ToString(), _arrayElementBtStyle, GUILayout.Width(18))) {
                    DeGUIDrag.StartDrag(I._listId, I, iList, i);
                }
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
                EditorGUILayout.EndHorizontal();
                if (DeGUIDrag.Drag(I._listId, iList, i).outcome == DeDragResultType.Accepted) {
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

        #endregion
    }
}