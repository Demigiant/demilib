// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/04/24 12:40
// License Copyright (c) Daniele Giardini

using DG.DemiEditor.DeGUINodeSystem;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Returns key modifiers currently pressed.
    /// Requires to be updated at the beginning of every GUI call.
    /// </summary>
    public static class DeGUIKey
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
        public static void Refresh()
        {
            // Evaluate softControl and space keys
            if (Event.current.type == EventType.KeyDown) {
                if (Event.current.keyCode == KeyCode.Space) Extra.space = true;
            } else if (Event.current.rawType == EventType.KeyUp) {
                switch (Event.current.keyCode) {
                case KeyCode.LeftControl:
                case KeyCode.RightControl:
                case KeyCode.LeftCommand:
                case KeyCode.RightCommand:
                    _timeAtControlKeyRelease = Time.realtimeSinceStartup;
                    break;
                case KeyCode.Space:
                    Extra.space = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Returns the given <see cref="KeyCode"/> as an int, or -1 if it's not a number
        /// </summary>
        public static int ToInt(KeyCode keycode)
        {
            switch (keycode) {
            case KeyCode.Keypad0:
            case KeyCode.Alpha0:
                return 0;
            case KeyCode.Keypad1:
            case KeyCode.Alpha1:
                return 1;
            case KeyCode.Keypad2:
            case KeyCode.Alpha2:
                return 2;
            case KeyCode.Keypad3:
            case KeyCode.Alpha3:
                return 3;
            case KeyCode.Keypad4:
            case KeyCode.Alpha4:
                return 4;
            case KeyCode.Keypad5:
            case KeyCode.Alpha5:
                return 5;
            case KeyCode.Keypad6:
            case KeyCode.Alpha6:
                return 6;
            case KeyCode.Keypad7:
            case KeyCode.Alpha7:
                return 7;
            case KeyCode.Keypad8:
            case KeyCode.Alpha8:
                return 8;
            case KeyCode.Keypad9:
            case KeyCode.Alpha9:
                return 9;
            default:
                return -1;
            }
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public static class Extra
        {
            public static bool space { get; internal set; }
        }

        public static class Exclusive
        {
            public static bool ctrl { get { return DeGUIKey.ctrl && !DeGUIKey.shift && !DeGUIKey.alt; }}
            public static bool softCtrl { get { return DeGUIKey.softCtrl && !DeGUIKey.shift && !DeGUIKey.alt; }}
            public static bool shift { get { return DeGUIKey.shift && !DeGUIKey.ctrl && !DeGUIKey.alt; }}
            public static bool alt { get { return DeGUIKey.alt && !DeGUIKey.ctrl && !DeGUIKey.shift; }}

            public static bool ctrlShiftAlt { get { return DeGUIKey.ctrl && DeGUIKey.shift && DeGUIKey.alt; }}
            public static bool ctrlShift { get { return DeGUIKey.ctrl && DeGUIKey.shift && !DeGUIKey.alt; }}
            public static bool ctrlAlt { get { return DeGUIKey.ctrl && DeGUIKey.alt && !DeGUIKey.shift; }}
            public static bool shiftAlt { get { return DeGUIKey.shift && DeGUIKey.alt && !DeGUIKey.ctrl; }}
        }
    }
}