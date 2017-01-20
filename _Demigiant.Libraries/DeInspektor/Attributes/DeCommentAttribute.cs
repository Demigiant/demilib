// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:41
// License Copyright (c) Daniele Giardini

using System;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DeInspektor.Attributes
{
    public enum CommentType
    {
        Simple,
        Info,
        Warning,
        Error
    }

    /// <summary>
    /// <code>Decorator</code>
    /// <para>Shows a comment in the Inspector</para>
    /// Extra properties which can be set directly:
    /// <code>fontSize</code>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DeCommentAttribute : PropertyAttribute
    {
        /// <summary>Font size (default = 9)</summary>
        public int fontSize = 9;

        internal string text;
        internal string textColor, bgColor;
        internal int marginBottom;

        public DeCommentAttribute(string text, int marginBottom = 3)
        {
            this.text = text;
            this.marginBottom = marginBottom;
        }

        public DeCommentAttribute(string text, string textColor, string bgColor, int marginBottom = 3)
        {
            this.text = text;
            this.textColor = textColor;
            this.bgColor = bgColor;
            this.marginBottom = marginBottom;
        }
    }
}