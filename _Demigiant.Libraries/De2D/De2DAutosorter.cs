// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2016/07/02 18:56
// License Copyright (c) Daniele Giardini

using System;
using System.Reflection;
using UnityEngine;

namespace DG.De2D
{
    public enum SortMode
    {
        OrderInLayer,
        LocalZAxis
    }

    /// <summary>
    /// Automatically sorts all its SpriteRenderer children based on their position in the Hierarchy
    /// (like UI: top is behind, bottom is frontal).
    /// Only applies sorting in the editor
    /// </summary>
    [ExecuteInEditMode]
    public class De2DAutosorter : MonoBehaviour
    {
        public SortMode sortMode = SortMode.OrderInLayer;
        public bool ignoreEditorOnly = false;
        public bool invert = false; // If TRUE first lower elements' sorting oeder decreases instead of increasing
        public int sortFrom = 0;
        public float zShift = 0.001f;

        MethodInfo _miSubscribe, _miUnsubscribe;

        void Awake()
        {
            if (Application.isPlaying) {
                this.enabled = false;
                return;
            }

            Subscribe();
        }

        void OnDestroy()
        {
            if (Application.isPlaying) return;

            Unsubscribe();
        }

        void Update() // Used to resubscribe after compilation happened - Disabled at runtime
        {
            Subscribe();
        }

        void Subscribe()
        {
            if (_miSubscribe == null) {
                Type t = Type.GetType("DG.De2DEditor.De2DAutosorterInspector, De2DEditor");
                _miSubscribe = t.GetMethod("Subscribe", BindingFlags.Static | BindingFlags.Public);
            }
            _miSubscribe.Invoke(null, new[]{ this });
        }

        void Unsubscribe()
        {
            if (_miUnsubscribe == null) {
                Type t = Type.GetType("DG.De2DEditor.De2DAutosorterInspector, De2DEditor");
                _miUnsubscribe = t.GetMethod("Unsubscribe", BindingFlags.Static | BindingFlags.Public);
            }
            _miUnsubscribe.Invoke(null, new[]{ this });
        }
    }
}