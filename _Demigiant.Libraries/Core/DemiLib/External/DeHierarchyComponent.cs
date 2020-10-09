// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 13:50
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 1591
namespace DG.DemiLib.External
{
    /// <summary>
    /// Used by DeHierarchy
    /// </summary>
    public class DeHierarchyComponent : MonoBehaviour
    {
        public enum HColor
        {
            None,
            Blue,
            Green,
            Orange,
            Purple,
            Red,
            Yellow,
            BrightGrey,
            DarkGrey,
            Black,
            White,
            Pink
        }

        public enum IcoType
        {
            Dot,
            Star,
            Cog,
            Comment,
            UI,
            Play,
            Heart,
            Skull,
            Camera,
            Light
        }

        public enum SeparatorType
        {
            None,
            Top,
            Bottom
        }

        #region Serialized

        public List<CustomizedItem> customizedItems = new List<CustomizedItem>();
        
        #endregion

        /// <summary>
        /// Returns a list of all items whose gameObject is NULL, or NULL if there's no missing gameObjects.
        /// </summary>
        public List<int> MissingItemsIndexes()
        {
            List<int> result = null;
            for (int i = customizedItems.Count - 1; i > -1; --i) {
                if (customizedItems[i].gameObject == null) {
                    if (result == null) result = new List<int>();
                    result.Add(i);
                }
            }
            return result;
        }

        /// <summary>
        /// If the item exists sets it, otherwise first creates it and then sets it
        /// </summary>
        public void StoreItemColor(GameObject go, HColor hColor)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject != go) continue;
                // Item exists, replace it
                customizedItems[i].hColor = hColor;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(go, hColor));
        }

        /// <summary>
        /// If the item exists sets it, otherwise first creates it and then sets it
        /// </summary>
        public void StoreItemIcon(GameObject go, IcoType icoType)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject != go) continue;
                // Item exists, replace it
                customizedItems[i].icoType = icoType;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(go, icoType));
        }

        /// <summary>
        /// If the item exists sets it, otherwise first creates it and then sets it
        /// </summary>
        public void StoreItemSeparator(GameObject go, SeparatorType? separatorType, HColor? separatorHColor)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject != go) continue;
                // Item exists, replace it
                if (separatorType != null) customizedItems[i].separatorType = (SeparatorType)separatorType;
                if (separatorHColor != null) customizedItems[i].separatorHColor = (HColor)separatorHColor;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(
                go,
                separatorType == null ? SeparatorType.None : (SeparatorType)separatorType,
                separatorHColor == null ? HColor.None : (HColor)separatorHColor
            ));
        }

        /// <summary>
        /// Returns TRUE if the item existed and was removed.
        /// </summary>
        public bool RemoveItemData(GameObject go)
        {
            int index = -1;
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject == go) {
                    index = i;
                    break;
                }
            }
            if (index != -1) {
                customizedItems.RemoveAt(index);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns TRUE if the item existed and was changed.
        /// </summary>
        public bool ResetSeparator(GameObject go)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject != go) continue;
                // Item exists, replace it
                customizedItems[i].separatorType = SeparatorType.None;
                customizedItems[i].separatorHColor = HColor.None;
                if (customizedItems[i].hColor == HColor.None) customizedItems.RemoveAt(i);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the customizedItem for the given gameObject, or NULL if none was found
        /// </summary>
        public CustomizedItem GetItem(GameObject go)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].gameObject == go) return customizedItems[i];
            }
            return null;
        }

        /// <summary>
        /// Returns the color corresponding to the given <see cref="HColor"/>
        /// </summary>
        public static Color GetColor(HColor color)
        {
            switch (color) {
            case HColor.Red: return new Color(0.82f, 0f, 0f);
            case HColor.Orange: return new Color(1f, 0.44f, 0f);
            case HColor.Yellow: return new Color(0.99f, 0.84f, 0.12f);
            case HColor.Green: return new Color(0.05060553f, 0.8602941f, 0.2237113f, 1f);
            case HColor.Blue: return new Color(0.21f, 0.62f, 1f);
            case HColor.Purple: return new Color(0.64f, 0.27f, 1f);
            case HColor.Pink: return new Color(1f, 0.21f, 0.82f);
            case HColor.BrightGrey: return new Color(0.6470588f, 0.6470588f, 0.6470588f, 1f);
            case HColor.DarkGrey: return new Color(0.3308824f, 0.3308824f, 0.3308824f, 1f);
            case HColor.Black: return Color.black;
            case HColor.White: return Color.white;
            default: return Color.white;
            }
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class CustomizedItem
        {
            public GameObject gameObject;
            public HColor hColor = HColor.BrightGrey;
            public IcoType icoType = IcoType.Dot;
            public SeparatorType separatorType = SeparatorType.None;
            public HColor separatorHColor = HColor.Black;

            public CustomizedItem(GameObject gameObject, HColor hColor)
            {
                this.gameObject = gameObject;
                this.hColor = hColor;
            }
            public CustomizedItem(GameObject gameObject, IcoType icoType)
            {
                this.gameObject = gameObject;
                this.icoType = icoType;
            }
            public CustomizedItem(GameObject gameObject, SeparatorType separatorType, HColor separatorHColor)
            {
                this.gameObject = gameObject;
                this.separatorType = separatorType;
                this.separatorHColor = separatorHColor;
            }

            public Color GetColor()
            { return DeHierarchyComponent.GetColor(hColor); }

            public Color GetSeparatorColor()
            { return DeHierarchyComponent.GetColor(separatorHColor); }
        }
    }
}