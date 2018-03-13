// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 17:14
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    /// <summary>
    /// Stored in main Assets folder
    /// </summary>
    public class DeProjectData : ScriptableObject
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
            BrightBlue
        }

        public enum IcoType
        {
            None,
            Scripts,
            Prefab,
            Cog,
            Play,
            Star,
            Heart,
            Skull,
        }

        #region Serialized

        public List<CustomizedItem> customizedItems = new List<CustomizedItem>();

        #endregion

        #region Public Methods

        /// <summary>
        /// If the item exists sets it, otherwise first creates it and then sets it
        /// </summary>
        public void StoreItemColor(string guid, HColor hColor)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid != guid) continue;
                // Item exists, replace it
                customizedItems[i].hColor = hColor;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(guid, hColor));
        }

        /// <summary>
        /// If the item exists sets it, otherwise first creates it and then sets it
        /// </summary>
        public void StoreItemIcon(string guid, IcoType icoType)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid != guid) continue;
                // Item exists, replace it
                customizedItems[i].icoType = icoType;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(guid, icoType));
        }

        /// <summary>
        /// Returns TRUE if the item existed and was removed.
        /// </summary>
        public bool RemoveItemData(string guid)
        {
            int index = -1;
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid == guid) {
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
        /// Returns the customizedItem for the given gameObject, or NULL if none was found
        /// </summary>
        public CustomizedItem GetItem(string guid)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid == guid) return customizedItems[i];
            }
            return null;
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class CustomizedItem
        {
            public string guid;
            public HColor hColor = HColor.BrightGrey;
            public IcoType icoType = IcoType.None;

            public CustomizedItem(string guid, HColor hColor)
            {
                this.guid = guid;
                this.hColor = hColor;
            }
            public CustomizedItem(string guid, IcoType icoType)
            {
                this.guid = guid;
                this.icoType = icoType;
            }

            public Color GetColor()
            {
                switch (hColor) {
                case HColor.Blue: return new Color(0.2145329f, 0.4501492f, 0.9117647f, 1f);
                case HColor.BrightBlue: return new Color(0.42f, 0.74f, 0.99f);
                case HColor.Green: return new Color(0.05060553f, 0.8602941f, 0.2237113f, 1f);
                case HColor.Orange: return new Color(0.9558824f, 0.4471125f, 0.05622837f, 1f);
                case HColor.Purple: return new Color(0.907186f, 0.05406574f, 0.9191176f, 1f);
                case HColor.Red: return new Color(0.9191176f, 0.1617312f, 0.07434041f, 1f);
                case HColor.Yellow: return new Color(1f, 0.853854f, 0.03676468f, 1f);
                case HColor.BrightGrey: return new Color(0.6470588f, 0.6470588f, 0.6470588f, 1f);
                case HColor.DarkGrey: return new Color(0.3308824f, 0.3308824f, 0.3308824f, 1f);
                case HColor.Black: return Color.black;
                case HColor.White: return Color.white;
                default: return Color.white;
                }
            }

            public Color GetIconColor()
            {
                return Color.white;
            }
        }
    }
}