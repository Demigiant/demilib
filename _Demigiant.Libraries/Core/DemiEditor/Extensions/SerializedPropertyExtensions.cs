// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/16 11:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class SerializedPropertyExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns the value of the given property (works like a cast to type).<para/>
        /// Improved from HiddenMonk's functions (http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html)
        /// </summary>
        public static T CastTo<T>(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string[] paths = property.propertyPath.Split('.');
            int len = paths.Length;
            for (int i = 0; i < len; i++) {
                string part = paths[i];
                if (part == "Array") {
                    // Array or list
                    if (i < len - 2) {
                        // Mid element array, store obj and continue
                        string strIndex = paths[i + 1];
                        strIndex = strIndex.Substring(strIndex.IndexOf("[") + 1);
                        strIndex = strIndex.Substring(0, strIndex.Length - 1);
                        obj = ((IList)obj)[Convert.ToInt32(strIndex)];
                        i++;
                        continue;
                    }
                    // Final obj is list element, find and return it
                    if (obj == null) return default(T);
                    int index = property.GetIndexInArray(); // Can be out of range in case the element had just been created
                    IList<T> iList = (IList<T>)obj;
                    return iList.Count - 1 < index ? default(T) : iList[index];
                }
                obj = GetFieldOrPropertyValue<object>(part, obj);
            }
            return (T)obj;
        }

        /// <summary>
        /// Returns TRUE if this property is inside an array
        /// </summary>
        public static bool IsArrayElement(this SerializedProperty property)
        {
            return property.propertyPath.Contains("Array");
        }

        /// <summary>
        /// Returns -1 if the property is not inside an array, otherwise returns its index inside the array
        /// </summary>
        public static int GetIndexInArray(this SerializedProperty property)
        {
            if (!property.IsArrayElement()) return -1;

            int cutFrom = property.propertyPath.LastIndexOf('[') + 1;
            int cutLen = property.propertyPath.LastIndexOf(']') - cutFrom;
            return int.Parse(property.propertyPath.Substring(cutFrom, cutLen));
        }

        /// <summary>
        /// Returns the height of a UnityEvent serializedProperty
        /// </summary>
        public static float GetUnityEventHeight(this SerializedProperty property)
        {
            if (property.propertyType != SerializedPropertyType.Generic) return 18; // Not a UnityEvent
            ReorderableList li = new ReorderableList(
                property.serializedObject, property.FindPropertyRelative("m_PersistentCalls.m_Calls"),
                draggable: false, displayHeader: true, displayAddButton: true, displayRemoveButton: true
            );
            return li.GetHeight() + Mathf.Max(1, li.count) * 26;
        }

        #region Get/Set

        /// <summary>
        /// Uses code from FlaShG's GitMerge: https://github.com/FlaShG/GitMerge-for-Unity/blob/master/Editor/SerializedPropertyExtensions.cs
        /// </summary>
        public static object GetValue(this SerializedProperty p)
        {
            switch (p.propertyType) {
            case SerializedPropertyType.AnimationCurve:
                return p.animationCurveValue;
            case SerializedPropertyType.ArraySize:
                return p.intValue;
            case SerializedPropertyType.Boolean:
                return p.boolValue;
            case SerializedPropertyType.Bounds:
                return p.boundsValue;
            case SerializedPropertyType.Character:
                return p.stringValue; //TODO: might be bullshit
            case SerializedPropertyType.Color:
                return p.colorValue;
            case SerializedPropertyType.Enum:
                return p.enumValueIndex;
            case SerializedPropertyType.Float:
                return p.floatValue;
            case SerializedPropertyType.Generic:
                // Usually arrays, but also more complex UnityEvents which would be too complex to support
                Debug.LogWarning("Get/Set of Generic SerializedProperty not supported");
                return null;
            case SerializedPropertyType.Gradient:
                Debug.LogWarning("Get/Set of Gradient SerializedProperty not supported");
                return 0;
            case SerializedPropertyType.Integer:
                return p.intValue;
            case SerializedPropertyType.LayerMask:
                return p.intValue;
            case SerializedPropertyType.ObjectReference:
                return p.objectReferenceValue;
            case SerializedPropertyType.Quaternion:
                return p.quaternionValue;
            case SerializedPropertyType.Rect:
                return p.rectValue;
            case SerializedPropertyType.String:
                return p.stringValue;
            case SerializedPropertyType.Vector2:
                return p.vector2Value;
            case SerializedPropertyType.Vector3:
                return p.vector3Value;
            case SerializedPropertyType.Vector4:
                return p.vector4Value;
            default:
                return 0;
            }
        }

        /// <summary>
        /// Uses code from FlaShG's GitMerge: https://github.com/FlaShG/GitMerge-for-Unity/blob/master/Editor/SerializedPropertyExtensions.cs
        /// </summary>
        public static void SetValue(this SerializedProperty p, object value)
        {
            switch (p.propertyType) {
            case SerializedPropertyType.AnimationCurve:
                p.animationCurveValue = value as AnimationCurve;
                break;
            case SerializedPropertyType.ArraySize:
                //TODO: erm
                p.intValue = (int)value;
                break;
            case SerializedPropertyType.Boolean:
                p.boolValue = (bool)value;
                break;
            case SerializedPropertyType.Bounds:
                p.boundsValue = (Bounds)value;
                break;
            case SerializedPropertyType.Character:
                p.stringValue = (string)value; //TODO: might be bullshit
                break;
            case SerializedPropertyType.Color:
                p.colorValue = (Color)value;
                break;
            case SerializedPropertyType.Enum:
                p.enumValueIndex = (int)value;
                break;
            case SerializedPropertyType.Float:
                p.floatValue = (float)value;
                break;
            case SerializedPropertyType.Generic:
                // Usually arrays, but also more complex UnityEvents which would be too complex to support
                Debug.LogWarning("Get/Set of Generic SerializedProperty not supported");
                break;
            case SerializedPropertyType.Gradient:
                Debug.LogWarning("Get/Set of Gradient SerializedProperty not supported");
                break;
            case SerializedPropertyType.Integer:
                p.intValue = (int)value;
                break;
            case SerializedPropertyType.LayerMask:
                p.intValue = (int)value;
                break;
            case SerializedPropertyType.ObjectReference:
                p.objectReferenceValue = value as UnityEngine.Object;
                break;
            case SerializedPropertyType.Quaternion:
                p.quaternionValue = (Quaternion)value;
                break;
            case SerializedPropertyType.Rect:
                p.rectValue = (Rect)value;
                break;
            case SerializedPropertyType.String:
                p.stringValue = (string)value;
                break;
            case SerializedPropertyType.Vector2:
                p.vector2Value = (Vector2)value;
                break;
            case SerializedPropertyType.Vector3:
                p.vector3Value = (Vector3)value;
                break;
            case SerializedPropertyType.Vector4:
                p.vector4Value = (Vector4)value;
                break;
            }
        }

        #endregion

        #endregion

        #region Methods

        static T GetFieldOrPropertyValue<T>(string fieldName, object obj)
        {
            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type t = obj.GetType();

            // Look also up hierarchy
            while (t != null) {
                FieldInfo field = t.GetField(fieldName, bindings);
                if (field != null) return (T)field.GetValue(obj);
                PropertyInfo property = t.GetProperty(fieldName, bindings);
                if (property != null) return (T)property.GetValue(obj, null);
                t = t.BaseType;
            }

            return default(T);
        }

        #endregion
    }
}