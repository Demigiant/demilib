// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:41
// License Copyright (c) Daniele Giardini

using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.Attributes
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
    /// </summary>
    public class DeCommentAttribute : PropertyAttribute
    {
        internal string text;
        internal CommentType commentType;

        public DeCommentAttribute(string text, CommentType commentType = CommentType.Simple)
        {
            this.text = text;
            this.commentType = commentType;
        }
    }
}