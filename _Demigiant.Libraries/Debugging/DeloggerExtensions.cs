// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2019/02/10 09:58
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.Text;

namespace DG.Debugging
{
    /// <summary>
    /// Extensions used to format objects for logging
    /// </summary>
    public static class DeloggerExtensions
    {
        static readonly StringBuilder _Strb = new StringBuilder();

        #region Public Methods

        /// <summary>
        /// Converts the given list into a log-formatted string
        /// </summary>
        /// <param name="list">List to log</param>
        /// <param name="lineBreakWIndex">If TRUE starts each element with a lineBreak and prefixes it with its index</param>
        public static string ToLogString<T>(this IList<T> list, bool lineBreakWIndex = false)
        { return ToLogString(list, null, lineBreakWIndex); }

        /// <summary>
        /// Converts the given list into a log-formatted string
        /// </summary>
        /// <param name="list">List to log</param>
        /// <param name="elaborate">Customized formatting function.
        /// <para><code>Example:</code></para>
        /// <para>Assuming a list of 4 gameObjects named "Gino", "Pino", "Lino", "Mandarino":</para>
        /// <para><code>Debug.Log("A sample GameObject list:" + _sampleObjList.ToLogString(x => "My name is " + x.name, true));</code></para>
        /// <para>Outputs:</para>
        /// <code><para>A sample GameObject list:</para>
        /// <para>0: My name is Gino</para>
        /// <para>1: My name is Pino</para>
        /// <para>2: My name is Lino</para>
        /// <para>3: My name is Mandarino</para></code>
        /// </param>
        /// <param name="lineBreakWIndex">If TRUE starts each element with a lineBreak and prefixes it with its index</param>
        public static string ToLogString<T>(this IList<T> list, Func<T,string> elaborate, bool lineBreakWIndex = false)
        {
            for (int i = 0; i < list.Count; ++i) {
                if (lineBreakWIndex) _Strb.Append('\n').Append(i).Append(": ");
                else if (i > 0) _Strb.Append(" - ");
                if (list[i] == null) _Strb.Append(elaborate == null ? "NULL" : "[null list element]");
                else _Strb.Append(elaborate == null ? list[i].ToString() : elaborate(list[i]));
            }
            string result = _Strb.ToString();
            _Strb.Length = 0;
            return result;
        }

        #endregion
    }
}
