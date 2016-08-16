// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/08/16 11:31
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEditor;

namespace DG.DemiEditor
{
    public static class SerializedPropertyExtensions
    {
        /// <summary>
        /// Returns the value of the given property (works like a cast to type).
        /// <para>
        /// Based on HiddenMonk's function (http://answers.unity3d.com/questions/627090/convert-serializedproperty-to-custom-class.html)
        /// </para>
        /// </summary>
        public static T CastTo<T>(this SerializedProperty property)
        {
            object obj = property.serializedObject.targetObject;
            string[] paths = property.propertyPath.Split('.');
            foreach (string part in paths) obj = GetFieldOrPropertyValue<object>(part, obj);
            return (T)obj;
        }

        #region Methods

        static T GetFieldOrPropertyValue<T>(string fieldName, object obj)
        {
            const BindingFlags bindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
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