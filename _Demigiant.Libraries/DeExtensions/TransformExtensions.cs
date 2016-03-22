// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2015/11/27 21:24
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class TransformExtensions
    {
        public static void SetX(this Transform t, float value)
        {
            Vector3 v = t.position;
            v.x = value;
            t.position = v;
        }
        public static void SetY(this Transform t, float value)
        {
            Vector3 v = t.position;
            v.y = value;
            t.position = v;
        }
        public static void SetZ(this Transform t, float value)
        {
            Vector3 v = t.position;
            v.z = value;
            t.position = v;
        }

        public static void SetLocalX(this Transform t, float value)
        {
            Vector3 v = t.localPosition;
            v.x = value;
            t.localPosition = v;
        }
        public static void SetLocalY(this Transform t, float value)
        {
            Vector3 v = t.localPosition;
            v.y = value;
            t.localPosition = v;
        }
        public static void SetLocalZ(this Transform t, float value)
        {
            Vector3 v = t.localPosition;
            v.z = value;
            t.localPosition = v;
        }

        public static void SetLocalScale(this Transform t, float value)
        {
            t.localScale = new Vector3(value, value, value);
        }
        public static void SetLocalScaleX(this Transform t, float value)
        {
            Vector3 v = t.localScale;
            v.x = value;
            t.localScale = v;
        }
        public static void SetLocalScaleY(this Transform t, float value)
        {
            Vector3 v = t.localScale;
            v.y = value;
            t.localScale = v;
        }
        public static void SetLocalScaleZ(this Transform t, float value)
        {
            Vector3 v = t.localScale;
            v.z = value;
            t.localScale = v;
        }
    }
}