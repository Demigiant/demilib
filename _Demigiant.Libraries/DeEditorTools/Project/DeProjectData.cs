// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 17:14
// License Copyright (c) Daniele Giardini

using System;
using System.Collections.Generic;
using System.IO;
using DG.DemiEditor;
using UnityEditor;
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
            Custom,
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
            BrightBlue,
        }

        public enum IcoType
        {
            None,
            Custom,
            Scripts,
            Prefab,
            Cog,
            Play,
            Star,
            Heart,
            Skull,

            Audio,
            Textures,
            Fonts,
            Demigiant
        }

        #region Serialized

        public List<CustomizedItem> customizedItems = new List<CustomizedItem>();

        #endregion

        #region Public Methods

        /// <summary>
        /// If the item exists returns it, otherwise first creates it and then returns it
        /// </summary>
        public CustomizedItem StoreItem(string guid)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid == guid) return customizedItems[i];
            }
            // Item doesn't exist, add it
            CustomizedItem item = new CustomizedItem(guid);
            customizedItems.Add(item);
            return item;
        }

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
                customizedItems[i].customIcon = null;
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

        /// <summary>
        /// Removes any leftover items that don't exist in the Project anymore
        /// </summary>
        public void Clean()
        {
            int totRemoved = 0;
            for (int i = customizedItems.Count - 1; i > -1; --i) {
                string guid = customizedItems[i].guid;
                string adbPath = AssetDatabase.GUIDToAssetPath(guid);
                if (!string.IsNullOrEmpty(adbPath) && Directory.Exists(DeEditorFileUtils.ADBPathToFullPath(adbPath))) continue;
                totRemoved++;
                customizedItems.RemoveAt(i);
            }
            if (totRemoved > 0) {
                EditorUtility.SetDirty(this);
                EditorUtility.DisplayDialog("DeProject Clean", totRemoved + " leftover items removed from the \"-DeProjectData.asset\"", "Ok");
            } else {
                EditorUtility.DisplayDialog("DeProject Clean", "No leftovers found", "Ok");
            }
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
            public Color customColor = Color.white;
            public Texture2D customIcon = null;
            public int customIconOffsetX = 2;
            public int customIconOffsetY = 2;
            public int customIconMaxSize = 16;

            public CustomizedItem(string guid)
            {
                this.guid = guid;
            }

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
                case HColor.Custom: return customColor;
                case HColor.Blue: return new Color(0.22f, 0.47f, 0.96f);
                case HColor.BrightBlue: return new Color(0.27f, 0.68f, 1f);
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

            public Texture2D GetIcon()
            {
                switch (icoType) {
                case IcoType.Custom:
                    return customIcon;
                case IcoType.Scripts:
                    return DeStylePalette.proj_scripts;
                case IcoType.Prefab:
                    return DeStylePalette.proj_prefab;
                case IcoType.Cog:
                    return DeStylePalette.proj_cog;
                case IcoType.Demigiant:
                    return DeStylePalette.proj_demigiant;
                case IcoType.Play:
                    return DeStylePalette.proj_play;
                case IcoType.Star:
                    return DeStylePalette.proj_star;
                case IcoType.Heart:
                    return DeStylePalette.proj_heart;
                case IcoType.Skull:
                    return DeStylePalette.proj_skull;
                case IcoType.Audio:
                    return DeStylePalette.proj_audio;
                case IcoType.Textures:
                    return DeStylePalette.proj_textures;
                case IcoType.Fonts:
                    return DeStylePalette.proj_fonts;
                }
                return null;
            }

            public Color GetIconColor()
            {
                return Color.white;
            }
        }
    }
}