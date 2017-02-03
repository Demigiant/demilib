// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/03 19:58
// License Copyright (c) Daniele Giardini

using System.Reflection;
using UnityEditor;

namespace DG.DemiEditor
{
    public static class DeEditorGUIUtils
    {
        static FieldInfo _fi_lastControlId;

        /// <summary>
        /// Precisely returns the last controlId assigned to a GUI element
        /// </summary>
        public static int GetLastControlId()
        {
            if (_fi_lastControlId == null) {
                _fi_lastControlId = typeof(EditorGUIUtility).GetField("s_LastControlID", BindingFlags.NonPublic | BindingFlags.Static);
                if (_fi_lastControlId == null) {
                    // Legacy
                    _fi_lastControlId = typeof(EditorGUI).GetField("lastControlID", BindingFlags.NonPublic | BindingFlags.Static);
                }
            }
            return (int)_fi_lastControlId.GetValue(null);
        }
    }
}