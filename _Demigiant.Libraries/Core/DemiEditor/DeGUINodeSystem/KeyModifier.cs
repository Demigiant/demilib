// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/24 12:40
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiEditor.DeGUINodeSystem
{
    /// <summary>
    /// Returns key modifiers currently pressed
    /// </summary>
    public static class KeyModifier
    {
        public static bool ctrl { get { return Event.current.control || Event.current.command; }}
        public static bool softCtrl { get {
            return (Event.current.control || Event.current.command || Time.realtimeSinceStartup - _timeAtControlKeyRelease < 0.2f);
        }}
        public static bool shift { get { return Event.current.shift; }}
        public static bool alt { get { return Event.current.alt; }}
        public static bool none { get { return !ctrl && !shift && !alt; } }

        public static bool ctrlShiftAlt { get { return ctrl && shift && alt; }}
        public static bool ctrlShift { get { return ctrl && shift; }}
        public static bool ctrlAlt { get { return ctrl && alt; }}
        public static bool shiftAlt { get { return shift && alt; }}

        static float _timeAtControlKeyRelease;

        #region Internal Methods

        /// <summary>
        /// Call this method to update data required by softCtrl calculations.
        /// Automatically called from within a <see cref="NodeProcessScope{T}"/>.
        /// </summary>
        public static void Update()
        {
            // Evaluate control key
            if (Event.current.type == EventType.KeyUp && (Event.current.keyCode == KeyCode.LeftControl || Event.current.keyCode == KeyCode.RightControl || Event.current.keyCode == KeyCode.LeftCommand || Event.current.keyCode == KeyCode.RightCommand)) {
                _timeAtControlKeyRelease = Time.realtimeSinceStartup;
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public static class Exclusive
        {
            public static bool ctrl { get { return KeyModifier.ctrl && !KeyModifier.shift && !KeyModifier.alt; }}
            public static bool softCtrl { get { return KeyModifier.softCtrl && !KeyModifier.shift && !KeyModifier.alt; }}
            public static bool shift { get { return KeyModifier.shift && !KeyModifier.ctrl && !KeyModifier.alt; }}
            public static bool alt { get { return KeyModifier.alt && !KeyModifier.ctrl && !KeyModifier.shift; }}

            public static bool ctrlShiftAlt { get { return KeyModifier.ctrl && KeyModifier.shift && KeyModifier.alt; }}
            public static bool ctrlShift { get { return KeyModifier.ctrl && KeyModifier.shift && !KeyModifier.alt; }}
            public static bool ctrlAlt { get { return KeyModifier.ctrl && KeyModifier.alt && !KeyModifier.shift; }}
            public static bool shiftAlt { get { return KeyModifier.shift && KeyModifier.alt && !KeyModifier.ctrl; }}
        }
    }
}