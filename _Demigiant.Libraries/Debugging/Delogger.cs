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
        /// <summary>Log everything except logs marked as verbose</summary>
        Normal,
        /// <summary>Log only errors</summary>
        Errors,
        /// <summary>Log only errors and warnings</summary>
        ErrorsAndWarnings,
        /// <summary>Log everything, verbose logs included</summary>
        Verbose
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
        /// <summary>Sets the verbosity value, which indicates the type of messages that will be logged</summary>
        public static Verbosity verbosity = Verbosity.Normal;
        /// <summary>If FALSE doesn't log anything</summary>
        public static bool enabled = true;

        static readonly StringBuilder _Strb = new StringBuilder();

        #region Public Methods

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void Log(object message, bool isVerbose = false, string hexColor = null)
        { Log(LogType.Normal, message, isVerbose, hexColor); }

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void LogWarning(object message)
        { Log(LogType.Warning, message, false, null); }

        /// <summary>
        /// Logs the given message with the given options
        /// </summary>
        public static void LogError(object message)
        { Log(LogType.Error, message, false, null); }

        #endregion

        #region Methods

        static void Log(LogType logType, object message, bool isVerbose, string hexColor)
        {
            if (!enabled) return;

            switch (verbosity) {
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

            bool colored = !string.IsNullOrEmpty(hexColor);
            if (colored) _Strb.Append("<color=").Append(hexColor).Append(">");
            _Strb.Append(message);
            if (colored) _Strb.Append("</color>");
            switch (logType) {
            case LogType.Warning:
                Debug.LogWarning(_Strb.ToString());
                break;
            case LogType.Error:
                Debug.LogError(_Strb.ToString());
                break;
            default:
                Debug.Log(_Strb.ToString());
                break;
            }

            _Strb.Length = 0;
        }

        #endregion
    }
}