// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 18:11
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    public class DeProjectContextMenu : MonoBehaviour
    {
        const int _Priority = 8; // Was 18 + 1
        const int _Priority_Evidence = _Priority + 1;
        const int _Priority_Evidence_Sub0 = _Priority_Evidence + 11;
        const int _Priority_Custom = _Priority_Evidence + 11;
        const int _Priority_CopyPaste = _Priority_Custom + 11;
        const int _Priority_Color = _Priority_CopyPaste + 11;
        const int _Priority_Color_Sub0 = _Priority_Color + 11;
        const int _Priority_Color_Sub1 = _Priority_Color_Sub0 + 11;
        const int _Priority_Icon = _Priority_Color + 0;
        const int _Priority_Icon_Sub0 = _Priority_Icon + 11;
        const int _Priority_Icon_Sub1 = _Priority_Icon_Sub0 + 11;
        const int _Priority_Preset = _Priority_Color + 0;

        #region Tree Evidence + Reset + Custom + Copy/Paste

        [MenuItem("Assets/DeProject/Global Tree Evidence/None", false, _Priority_Evidence)]
        static void SetEvidenceNone() { DeProject.SetEvidence(DeProjectData.EvidenceType.None); }

        [MenuItem("Assets/DeProject/Customize Panel", false, _Priority_Evidence)]
        static void OpenCustomizePanel() { DeProjectCustomizePanel.ShowWindow(); }

        [MenuItem("Assets/DeProject/Global Tree Evidence/Default", false, _Priority_Evidence_Sub0)]
        static void SetEvidenceWChildren() { DeProject.SetEvidence(DeProjectData.EvidenceType.Default); }

        [MenuItem("Assets/DeProject/Reset", false, _Priority_Custom)]
        static void Reset() { DeProject.ResetSelections(Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Custom", false, _Priority_Custom)]
        static void SetCustom() { DeProject.CustomizeSelections(Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Copy Customization", false, _Priority_CopyPaste)]
        static void Copy() { DeProject.CopyDataFromSelection(Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Copy Customization", true)]
        static bool Copy_Validate() { return DeProject.CanCopyDataFromSelection(Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Paste Customization", false, _Priority_CopyPaste)]
        static void Paste() { DeProject.PasteDataToSelections(Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Paste Customization", true)]
        static bool Paste_Validate() { return DeProjectClipboard.hasStoreData; }

        #endregion

        #region Colors

        [MenuItem("Assets/DeProject/Color/None", false, _Priority_Color)]
        static void SetColorNone() { DeProject.SetColorForSelections(DeProjectData.HColor.None, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Red", false, _Priority_Icon_Sub0)]
        static void SetColorRed() { DeProject.SetColorForSelections(DeProjectData.HColor.Red, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Yellow", false, _Priority_Icon_Sub0)]
        static void SetColorYellow() { DeProject.SetColorForSelections(DeProjectData.HColor.Yellow, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Orange", false, _Priority_Icon_Sub0)]
        static void SetColorOrange() { DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Green", false, _Priority_Icon_Sub0)]
        static void SetColorGreen() { DeProject.SetColorForSelections(DeProjectData.HColor.Green, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Blue", false, _Priority_Icon_Sub0)]
        static void SetColorBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.Blue, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Blue (Bright)", false, _Priority_Icon_Sub0)]
        static void SetColorBrightBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Violet", false, _Priority_Icon_Sub0)]
        static void SetColorViolet() { DeProject.SetColorForSelections(DeProjectData.HColor.Violet, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Purple", false, _Priority_Icon_Sub0)]
        static void SetColorPurple() { DeProject.SetColorForSelections(DeProjectData.HColor.Purple, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Black", false, _Priority_Color_Sub1)]
        static void SetColorBlack() { DeProject.SetColorForSelections(DeProjectData.HColor.Black, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/White", false, _Priority_Color_Sub1)]
        static void SetColorWhite() { DeProject.SetColorForSelections(DeProjectData.HColor.White, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Grey (Bright)", false, _Priority_Color_Sub1)]
        static void SetColorBrightGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightGrey, Selection.assetGUIDs); }
        [MenuItem("Assets/DeProject/Color/Grey (Dark)", false, _Priority_Color_Sub1)]
        static void SetColorDarkGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.DarkGrey, Selection.assetGUIDs); }

        #endregion

        #region Icon

        [MenuItem("Assets/DeProject/Icon/None", false, _Priority_Icon)]
        static void SetIconNone() { DeProject.SetIconForSelections(DeProjectData.IcoType.None, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/AssetBundle", false, _Priority_Icon_Sub0)]
        static void SetIconAssetBundle() { DeProject.SetIconForSelections(DeProjectData.IcoType.AssetBundle, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Atlas", false, _Priority_Icon_Sub0)]
        static void SetIconAtlas() { DeProject.SetIconForSelections(DeProjectData.IcoType.Atlas, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Audio", false, _Priority_Icon_Sub0)]
        static void SetIconAudio() { DeProject.SetIconForSelections(DeProjectData.IcoType.Audio, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Fonts", false, _Priority_Icon_Sub0)]
        static void SetIconFonts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Fonts, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Materials", false, _Priority_Icon_Sub0)]
        static void SetIconMaterials() { DeProject.SetIconForSelections(DeProjectData.IcoType.Materials, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Particles", false, _Priority_Icon_Sub0)]
        static void SetIconParticles() { DeProject.SetIconForSelections(DeProjectData.IcoType.Particles, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Prefabs", false, _Priority_Icon_Sub0)]
        static void SetIconPrefabs() { DeProject.SetIconForSelections(DeProjectData.IcoType.Prefab, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Scripts", false, _Priority_Icon_Sub0)]
        static void SetIconScripts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Scripts, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Shaders", false, _Priority_Icon_Sub0)]
        static void SetIconShaders() { DeProject.SetIconForSelections(DeProjectData.IcoType.Shaders, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Terrains", false, _Priority_Icon_Sub0)]
        static void SetIconTerrains() { DeProject.SetIconForSelections(DeProjectData.IcoType.Terrains, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Textures", false, _Priority_Icon_Sub0)]
        static void SetIconTextures() { DeProject.SetIconForSelections(DeProjectData.IcoType.Textures, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Cog", false, _Priority_Icon_Sub1)]
        static void SetIconCog() { DeProject.SetIconForSelections(DeProjectData.IcoType.Cog, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Cross", false, _Priority_Icon_Sub1)]
        static void SetIconCross() { DeProject.SetIconForSelections(DeProjectData.IcoType.Cross, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Demigiant", false, _Priority_Icon_Sub1)]
        static void SetIconDemigiant() { DeProject.SetIconForSelections(DeProjectData.IcoType.Demigiant, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Heart", false, _Priority_Icon_Sub1)]
        static void SetIconHeart() { DeProject.SetIconForSelections(DeProjectData.IcoType.Heart, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Play", false, _Priority_Icon_Sub1)]
        static void SetIconPlay() { DeProject.SetIconForSelections(DeProjectData.IcoType.Play, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Skull", false, _Priority_Icon_Sub1)]
        static void SetIconSkull() { DeProject.SetIconForSelections(DeProjectData.IcoType.Skull, Selection.assetGUIDs); }

        [MenuItem("Assets/DeProject/Icon/Star", false, _Priority_Icon_Sub1)]
        static void SetIconStar() { DeProject.SetIconForSelections(DeProjectData.IcoType.Star, Selection.assetGUIDs); }

        #endregion

        #region Presets

        [MenuItem("Assets/DeProject/Preset/3D Models", false, _Priority_Preset)]
        static void SetPresetModels()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Models, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/AssetBundle", false, _Priority_Preset)]
        static void SetPresetAssetBundle()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.AssetBundle, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Atlas", false, _Priority_Preset)]
        static void SetPresetAtlas()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Atlas, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Audio", false, _Priority_Preset)]
        static void SetPresetAudio()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Audio, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Fonts", false, _Priority_Preset)]
        static void SetPresetFonts()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Fonts, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Materials", false, _Priority_Preset)]
        static void SetPresetMaterials()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Materials, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Particles", false, _Priority_Preset)]
        static void SetPresetParticles()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Particles, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Plugins + Standard Assets", false, _Priority_Preset)]
        static void SetPresetPlugins()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Cog, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.None, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Prefabs", false, _Priority_Preset)]
        static void SetPresetPrefabs()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Prefab, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Resources", false, _Priority_Preset)]
        static void SetPresetResources()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Play, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Green, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Scenes", false, _Priority_Preset)]
        static void SetPresetScenes()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Play, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Green, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Scripts", false, _Priority_Preset)]
        static void SetPresetScripts()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Scripts, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Purple, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Shaders", false, _Priority_Preset)]
        static void SetPresetShaders()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Shaders, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/StreamingAssets", false, _Priority_Preset)]
        static void SetPresetStreamingAssets()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Play, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Green, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Terrains", false, _Priority_Preset)]
        static void SetPresetTerrains()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Terrains, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue, Selection.assetGUIDs);
        }

        [MenuItem("Assets/DeProject/Preset/Textures + Sprites", false, _Priority_Preset)]
        static void SetPresetTextures()
        {
            DeProject.SetIconForSelections(DeProjectData.IcoType.Textures, Selection.assetGUIDs);
            DeProject.SetColorForSelections(DeProjectData.HColor.Orange, Selection.assetGUIDs);
        }

        #endregion
    }
}