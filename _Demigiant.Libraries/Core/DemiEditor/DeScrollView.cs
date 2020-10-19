// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/01/14 12:40
// License Copyright (c) Daniele Giardini

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DG.DemiEditor
{
    /// <summary>
    /// Returns by <see cref="DeGUI.BeginScrollView"/>.
    /// Contains properties and methods to manage non-layout scrollview better.<para/>
    /// Remember to use <see cref="IncreaseContentHeightBy"/> or <see cref="SetContentHeight"/> to increase or set the full content height
    /// </summary>
    public struct DeScrollView
    {
        /// <summary>Area used by ScrollView and its content</summary>
        public Rect area { get; private set; }
        /// <summary>Full content area regardless if visible or not. Its height should be set manually based on the contents' height</summary>
        public Rect fullContentArea { get; private set; }
        /// <summary>Content area currently visible (scroll bars excluded)</summary>
        public Rect visibleContentArea { get; private set; }
        /// <summary>Current scrollPosition</summary>
        public Vector2 scrollPosition { get; private set; }

        static readonly Stack<DeScrollView> _CurrScrollViews = new Stack<DeScrollView>();

        #region Public Methods

        /// <summary>
        /// Returns the current <see cref="DeScrollView"/> open, or an empty one if none is open.
        /// </summary>
        public static DeScrollView Current()
        {
            return _CurrScrollViews.Count == 0 ? new DeScrollView() : _CurrScrollViews.Peek();
        }

        /// <summary>
        /// Sets the <see cref="fullContentArea"/> width
        /// </summary>
        public void SetContentWidth(float width)
        {
            fullContentArea = new Rect(fullContentArea.x, fullContentArea.y, width, fullContentArea.height);
        }

        /// <summary>
        /// Sets the <see cref="fullContentArea"/> height
        /// </summary>
        public void SetContentHeight(float height)
        {
            fullContentArea = new Rect(fullContentArea.x, fullContentArea.y, fullContentArea.width, height);
        }

        /// <summary>
        /// Increase the <see cref="fullContentArea"/> height by the given amount
        /// </summary>
        /// <param name="value"></param>
        public void IncreaseContentHeightBy(float value)
        {
            fullContentArea = new Rect(fullContentArea.x, fullContentArea.y, fullContentArea.width, fullContentArea.height + value);
        }

        /// <summary>
        /// Returns a Rect for a single line at the current scrollView yMax
        /// </summary>
        /// <param name="height">If less than 0 uses default line height, otherwise the value passed</param>
        /// <param name="increaseScrollViewHeight">if TRUE (default) automatically increases the height of the <see cref="fullContentArea"/> accordingly</param>
        /// <returns></returns>
        public Rect GetSingleLineRect(float height = -1, bool increaseScrollViewHeight = true)
        {
            Rect r = new Rect(fullContentArea.x, fullContentArea.yMax, fullContentArea.width, height < 0 ? EditorGUIUtility.singleLineHeight : height);
            if (increaseScrollViewHeight) IncreaseContentHeightBy(r.height);
            return r;
        }
        /// <summary>
        /// Returns a Rect for a single line at the current scrollView yMax, as wide as the max visible rect width
        /// </summary>
        /// <param name="height">If less than 0 uses default line height, otherwise the value passed</param>
        /// <param name="increaseScrollViewHeight">if TRUE (default) automatically increases the height of the <see cref="fullContentArea"/> accordingly</param>
        /// <returns></returns>
        public Rect GetWideSingleLineRect(float height = -1, bool increaseScrollViewHeight = true)
        {
            Rect r = GetSingleLineRect(height, increaseScrollViewHeight);
            r.xMax = visibleContentArea.xMax;
            return r;
        }

        /// <summary>
        /// Returns TRUE if the given rect is at least partially visible in the displayed scroll area
        /// </summary>
        public bool IsVisible(Rect r)
        {
            return visibleContentArea.Overlaps(r);
        }

        #endregion

        #region Internal Methods

        internal void Begin(Rect viewArea, bool resetContentHeightToZero)
        {
            area = viewArea;
            fullContentArea = new Rect(
                fullContentArea.x,
                fullContentArea.y,
                // fullContentArea.height > area.height ? area.width - 16 : area.width,
                fullContentArea.width,
                fullContentArea.height
            );
            scrollPosition = GUI.BeginScrollView(area, scrollPosition, fullContentArea);
            // if (resetContentHeightToZero) fullContentArea = new Rect(fullContentArea.x, fullContentArea.y, fullContentArea.width, 0);
            visibleContentArea = new Rect(
                area.x, scrollPosition.y,
                fullContentArea.height > area.height ? area.width - 16 : area.width,
                fullContentArea.width > area.width ? area.height - 16 : area.height
            );
            fullContentArea = new Rect(
                fullContentArea.x, fullContentArea.y,
                fullContentArea.height > area.height ? area.width - 16 : area.width,
                resetContentHeightToZero ? 0 : fullContentArea.height
            );
            _CurrScrollViews.Push(this);
        }

        internal void End()
        {
            _CurrScrollViews.Pop();
            GUI.EndScrollView();
        }

        #endregion
    }
}