// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/01/07 18:44
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DemiLib
{
    /// <summary>
    /// Various Input utils
    /// </summary>
    public static class DeInputUtils
    {
        #region Public Methods

        /// <summary>
        /// Returns a number key int if a number key was pressed in this frame, or -1 otherwise
        /// </summary>
        /// <returns></returns>
        public static int IsNumKeyDown()
        {
            if (Input.inputString == "") return -1;
            if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0)) return 0;
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)) return 1;
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)) return 2;
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)) return 3;
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)) return 4;
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)) return 5;
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)) return 6;
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)) return 7;
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)) return 8;
            if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9)) return 9;
            return -1;
        }

        #endregion
    }
}