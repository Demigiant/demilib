// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/15 12:54
// License Copyright (c) Daniele Giardini

using System.Text;
using UnityEngine;

namespace DG.Debugging
{
    // ENUMS ▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬▬

    /// <summary>Verbosity</summary>
    public enum Verbosity
    {
        /// <summary>Log everything, verbose logs included</summary>
        Verbose,
        /// <summary>Log everything except logs marked as verbose</summary>
        Normal,
        /// <summary>Log only errors</summary>
        Errors,
        /// <summary>Log only errors and warnings</summary>
        ErrorsAndWarnings
    }

    enum LogType
    {
        Normal,
        Warning,
        Error
    }

    /// <summary>
    /// Logs messages using the given options
    /// </summary>
    public static class Delogger
    {
        /// <summary>Sets the runtime verbosity value, which indicates the type of messages that will be logged</summary>
        public static Verbosity verbosity = Verbosity.Normal;
        /// <summary>Sets the editor verbosity value, which indicates the type of messages that will be logged</summary>
        public static Verbosity editorVerbosity = Verbosity.Normal;
        /// <summary>If FALSE doesn't log anything, neither in editor nor at runtime</summary>
        public static bool enabled = true;

        const string _EditorPrefix = "[Editor] ";
        const string _VerboseColor = "<color=#666666>";
        const string _SenderColor = "<color=#6b854a>";
        const string _VerboseSenderColor = "<color=#6b854a>";
        static readonly StringBuilder _Strb = new StringBuilder();

        #region Public Methods

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void Log(object sender, object message, Object context = null, string hexColor = null)
        { Log(-1, false, LogType.Normal, sender, message, context, false, hexColor); }

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void LogVerbose(object sender, object message, Object context = null)
        { Log(-1, false, LogType.Normal, sender, message, context, true, null); }

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void LogWarning(object sender, object message, Object context = null)
        { Log(-1, false, LogType.Warning, sender, message, context, false, null); }

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void LogError(object sender, object message, Object context = null)
        { Log(-1, false, LogType.Error, sender, message, context, false, null); }

        #endregion

        #region Methods

        static void Log(int importance, bool isEditorLog, LogType logType, object sender, object message, Object context, bool isVerbose, string hexColor)
        {
            if (!enabled) return;

            Verbosity targetVerbosity = isEditorLog ? editorVerbosity : verbosity;
            switch (targetVerbosity) {
            case Verbosity.Errors:
                if (logType != LogType.Error) return;
                break;
            case Verbosity.ErrorsAndWarnings:
                if (logType != LogType.Error || logType != LogType.Warning) return;
                break;
            case Verbosity.Normal:
                if (isVerbose) return;
                break;
            }

            bool isImportant = importance > -1;
            bool colored = isVerbose || !string.IsNullOrEmpty(hexColor);
            string importantLogEnd = "";
            if (isImportant) {
                _Strb.Append("<b><color=#f4c560><size=14>★ </size></color></b>");
                switch (importance) {
                case 1:
                    _Strb.Append("<b>");
                    importantLogEnd = "</b>";
                    break;
                default:
                    _Strb.Append("<size=14><b>");
                    importantLogEnd = "</b></size>";
                    break;
                }
            }
            if (isVerbose) _Strb.Append(_VerboseColor);
            else if (colored) {
                _Strb.Append("<color=");
                if (hexColor[0] != '#') _Strb.Append('#');
                _Strb.Append(hexColor).Append(">");
            }
            if (isEditorLog) _Strb.Append(_EditorPrefix);
            if (sender != null) {
                _Strb.Append(isVerbose ? _VerboseSenderColor : _SenderColor).Append(sender).Append("</color> ► ");
            }
            _Strb.Append(message);
            if (colored) _Strb.Append("</color>");
            if (isImportant) _Strb.Append(importantLogEnd);
            switch (logType) {
            case LogType.Warning:
                Debug.LogWarning(_Strb.ToString(), context);
                break;
            case LogType.Error:
                Debug.LogError(_Strb.ToString(), context);
                break;
            default:
                Debug.Log(_Strb.ToString(), context);
                break;
            }

            _Strb.Length = 0;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        public static class Important
        {
            /// <summary>
            /// Logs the given important message with the given options
            /// </summary>
            /// <param name="importance">0 to 1 (0 being more important than 1)</param>
            public static void Log(int importance, object sender, object message, Object context = null, string hexColor = null)
            { Delogger.Log(importance, false, LogType.Normal, sender, message, context, false, hexColor); }
        }

        public static class Editor
        {
            /// <summary>
            /// Logs the given editor-only message with the given options
            /// </summary>
            public static void Log(object sender, object message, Object context = null, string hexColor = null)
            { Delogger.Log(-1, true, LogType.Normal, sender, message, context, false, hexColor); }

            /// <summary>
            /// Logs the given editor-only message with the given options
            /// </summary>
            public static void LogVerbose(object sender, object message, Object context = null)
            { Delogger.Log(-1, true, LogType.Normal, sender, message, context, true, null); }

            /// <summary>
            /// Logs the given editor-only message with the given options
            /// </summary>
            public static void LogWarning(object sender, object message, Object context = null)
            { Delogger.Log(-1, true, LogType.Warning, sender, message, context, false, null); }

            /// <summary>
            /// Logs the given editor-only message with the given options
            /// </summary>
            public static void LogError(object sender, object message, Object context = null)
            { Delogger.Log(-1, true, LogType.Error, sender, message, context, false, null); }
        }
    }
}