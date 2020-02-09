// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2020/02/08

using System;
using System.Collections.Generic;
using System.Reflection;

namespace DG.DemiEditor
{
    /// <summary>
    /// Used code from Celtc on StackOverflow: https://stackoverflow.com/a/54044197/10151925
    /// </summary>
    public static class DeEditorReflectionUtils
    {
        /// <summary>
        /// Gets all fields from an object and its hierarchy inheritance
        /// </summary>
        public static List<FieldInfo> GetAllFields(this Type type, BindingFlags flags)
        {
            // Early exit if Object type
            if (type == typeof(System.Object)) return new List<FieldInfo>();

            // Recursive call
            List<FieldInfo> fields = type.BaseType.GetAllFields(flags);
            fields.AddRange(type.GetFields(flags | BindingFlags.DeclaredOnly));
            return fields;
        }

        /// <summary>
        /// Perform a deep copy of the class
        /// </summary>
        public static T DeepCopy<T>(T obj)
        {
            if (obj == null) throw new ArgumentNullException("Object cannot be null");
            return (T)DoCopy(obj);
        }


        /// <summary>
        /// Does the copy
        /// </summary>
        static object DoCopy(object obj)
        {
            if (obj == null) return null;

            // Value type
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string)) return obj;

            // Array
            if (type.IsArray) {
                Type elementType = type.GetElementType();
                Array array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    copied.SetValue(DoCopy(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }

            // Unity Object
            if (typeof(UnityEngine.Object).IsAssignableFrom(type)) return obj;

            // Class -> Recursion
            if (type.IsClass) {
                object copy = Activator.CreateInstance(obj.GetType());
                List<FieldInfo> fields = type.GetAllFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields) {
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue != null) field.SetValue(copy, DoCopy(fieldValue));
                }
                return copy;
            }

            // Fallback
            throw new ArgumentException("Unknown type");
        }
    }
}