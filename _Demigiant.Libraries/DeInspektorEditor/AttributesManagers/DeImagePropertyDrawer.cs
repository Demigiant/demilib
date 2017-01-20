// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/01/17 13:21
// License Copyright (c) Daniele Giardini

using DG.DeInspektor.Attributes;
using UnityEditor;
using UnityEngine;

namespace DG.DeInspektorEditor.AttributesManagers
{
    [CustomPropertyDrawer(typeof(DeImageAttribute))]
    public class DeImagePropertyDrawer : DecoratorDrawer
    {
        Texture2D _img;
        bool _imgNotFound;
        Rect _imgRect;

        public override float GetHeight()
        {
            if (_imgNotFound) return 0;

            DeImageAttribute attr = (DeImageAttribute)attribute;
            if (_img != null) return _imgRect.height + attr.marginTop + attr.marginBottom;

            _img = AssetDatabase.LoadAssetAtPath(attr.adbFilePath, typeof(Texture2D)) as Texture2D;
            if (_img == null) {
                _imgNotFound = true;
                return 0;
            }

            _imgRect = new Rect(0, 0, _img.width, _img.height);
            if (attr.maxWidth > -1) {
                _imgRect = Fit(_imgRect, attr.maxWidth, attr.maxHeight > -1 ? attr.maxHeight : _img.height);
            } else if (attr.maxHeight > -1) _imgRect = Fit(_imgRect, _img.width, attr.maxHeight);
            return _imgRect.height + attr.marginTop + attr.marginBottom;
        }

        public override void OnGUI(Rect position)
        {
            if (_imgNotFound) return;

            DeImageAttribute attr = (DeImageAttribute)attribute;
            Rect r = position;
            r.width = _imgRect.width;
            r.height = _imgRect.height;
            r.y += attr.marginTop;

            GUI.DrawTexture(r, _img);
        }

        static Rect Fit(Rect r, float w, float h)
        {
            if (r.width <= w && r.height <= h) return r;
            float wPerc = w / r.width;
            float hPerc = h / r.height;
            float perc = wPerc < hPerc ? wPerc : hPerc;
            r.width *= perc;
            r.height *= perc;
            return r;
        }
    }
}