// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/16 11:31
// License Copyright (c) Daniele Giardini

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    public static class SerializedPropertyExtensions
    {
        #region Public Methods

        /// <summary>
        /// Returns the value of the given property (works like a cast to type).
        /// <para>
        /// Improved from HiddenMonk's functions (http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html)
        /// </para>
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

        #endregion

        #region Methods

        static T GetFieldOrPropertyValue<T>(string fieldName, object obj)
        {
            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            Type t = obj.GetType();

            FieldInfo field = t.GetField(fieldName, bindings);
            if (field != null) return (T)field.GetValue(obj);
 
            PropertyInfo property = t.GetProperty(fieldName, bindings);
            if (property != null) return (T)property.GetValue(obj, null);
 
            return default(T);
        }

        #endregion
    }
}