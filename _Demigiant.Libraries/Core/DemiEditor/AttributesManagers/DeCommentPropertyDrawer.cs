// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/10 15:43
// License Copyright (c) Daniele Giardini

using DG.DemiLib.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeCommentAttribute))]
    public class DeCommentPropertyDrawer : DecoratorDrawer
    {
        const float _MarginBottom = 5;
        float _positionW;

        public override float GetHeight()
        {
            DeCommentAttribute attr = (DeCommentAttribute)attribute;

            return GUI.skin.box.CalcHeight(new GUIContent(attr.text), _positionW) + _MarginBottom;
        }

        public override void OnGUI(Rect position)
        {
            _positionW = position.width;
            DeCommentAttribute attr = (DeCommentAttribute)attribute;

            Rect r = position;
            r.height -= _MarginBottom;
            EditorGUI.HelpBox(r, attr.text, (MessageType)attr.commentType);
        }
    }
}