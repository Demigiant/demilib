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
            Violet
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
            Demigiant,
            AssetBundle,
            Cross,
            Atlas,
            Shaders,
            Models,
            Materials,
            Terrains,
            Particles
        }

        public enum EvidenceType
        {
            None,
            Default
        }

        #region Serialized

        public EvidenceType evidenceType = EvidenceType.None;
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
        public void StoreItemColor(string guid, HColor hColor, Color? customColor = null)
        {
            for (int i = 0; i < customizedItems.Count; ++i) {
                if (customizedItems[i].guid != guid) continue;
                // Item exists, replace it
                customizedItems[i].hColor = hColor;
                if (customColor != null) customizedItems[i].customColor = (Color)customColor;
                return;
            }
            // Item doesn't exist, add it
            customizedItems.Add(new CustomizedItem(guid, hColor, customColor));
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

        public static Texture2D GetIcon(IcoType forType, Texture2D customIcon = null)
        {
            switch (forType) {
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
            case IcoType.AssetBundle:
                return DeStylePalette.proj_bundle;
            case IcoType.Cross:
                return DeStylePalette.proj_cross;
            case IcoType.Atlas:
                return DeStylePalette.proj_atlas;
            case IcoType.Shaders:
                return DeStylePalette.proj_shaders;
            case IcoType.Models:
                return DeStylePalette.proj_models;
            case IcoType.Materials:
                return DeStylePalette.proj_materials;
            case IcoType.Terrains:
                return DeStylePalette.proj_terrains;
            case IcoType.Particles:
                return DeStylePalette.proj_particles;
            }
            return null;
        }

        public static Color GetColor(HColor hColor, Color? customColor = null)
        {
            return DeProjectUtils.HColorToColor(hColor, customColor);
        }

        #endregion

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███ INTERNAL CLASSES ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        [Serializable]
        public class CustomizedItem
        {
            public string guid;
            public HColor hColor = HColor.None;
            public IcoType icoType = IcoType.None;
            public Color customColor = Color.white;
            public Texture2D customIcon = null;
            public int customIconOffsetX = 2;
            public int customIconOffsetY = 2;
            public int customIconMaxSize = 16;
            public bool hasColor { get { return hColor != HColor.None; } }
            public bool hasIcon { get { return icoType != IcoType.None; } }

            public CustomizedItem(string guid)
            {
                this.guid = guid;
            }

            public CustomizedItem(string guid, HColor hColor, Color? customColor = null)
            {
                this.guid = guid;
                this.hColor = hColor;
                if (customColor != null) this.customColor = (Color)customColor;
            }
            public CustomizedItem(string guid, IcoType icoType)
            {
                this.guid = guid;
                this.icoType = icoType;
            }

            public CustomizedItem Clone()
            {
                return new CustomizedItem(guid, hColor) {
                    icoType = icoType,
                    customColor = customColor,
                    customIcon = customIcon,
                    customIconOffsetX = customIconOffsetX,
                    customIconOffsetY = customIconOffsetY,
                    customIconMaxSize = customIconMaxSize
                };
            }

            public void Paste(CustomizedItem item)
            {
                this.hColor = item.hColor;
                this.icoType = item.icoType;
                this.customColor = item.customColor;
                this.customIcon = item.customIcon;
                this.customIconOffsetX = item.customIconOffsetX;
                this.customIconOffsetY = item.customIconOffsetY;
                this.customIconMaxSize = item.customIconMaxSize;
            }

            public Color GetColor()
            {
                return DeProjectData.GetColor(hColor, customColor);
            }

            public Texture2D GetIcon()
            {
                return DeProjectData.GetIcon(icoType, customIcon);
            }

            public Color GetIconColor()
            {
                return Color.white;
            }

            public Vector2 GetIconOffset()
            {
                switch (icoType) {
                case IcoType.Custom:
                    return new Vector2(customIconOffsetX, customIconOffsetY);
                case IcoType.AssetBundle:
                    return new Vector2(4, 4);
                default:
                    return new Vector2(2, 2);
                }
            }
        }
    }
}