// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/04/08 12:41
// License Copyright (c) Daniele Giardini

using UnityEngine;

namespace DG.DeExtensions
{
    public static class SpriteExtensions
    {
        public static void SetAlpha(this SpriteRenderer t, float alpha)
        {
            Color c = t.color;
            c.a = alpha;
            t.color = c;
        }

        public static void SetColor(this SpriteRenderer t, Color color, float alpha)
        {
            color.a = alpha;
            t.color = color;
        }
    }
}