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
        const int _Priority_Custom = _Priority + 11;
        const int _Priority_CopyPaste = _Priority_Custom + 11;
        const int _Priority_Icon = _Priority_CopyPaste + 11;
        const int _Priority_Icon_Sub = _Priority_Icon + 11;
        const int _Priority_Color = _Priority_Icon + 11;

        #region Reset

        [MenuItem("Assets/DeProject/Reset", false, _Priority_Custom)]
        static void Reset() { DeProject.SetColorForSelections(DeProjectData.HColor.None); }

        [MenuItem("Assets/DeProject/Custom", false, _Priority_Custom)]
        static void SetCustom() { DeProject.CustomizeSelections(); }

        [MenuItem("Assets/DeProject/Copy Customization", false, _Priority_CopyPaste)]
        static void Copy() { DeProject.CopyDataFromSelection(); }
        [MenuItem("Assets/DeProject/Copy Customization", true)]
        static bool Copy_Validate() { return DeProject.CanCopyDataFromSelection(); }

        [MenuItem("Assets/DeProject/Paste Customization", false, _Priority_CopyPaste)]
        static void Paste() { DeProject.PastDataToSelections(); }
        [MenuItem("Assets/DeProject/Paste Customization", true)]
        static bool Paste_Validate() { return DeProjectClipboard.hasStoreData; }

        #endregion

        #region Icon

        [MenuItem("Assets/DeProject/Icon/AssetBundle", false, _Priority_Icon)]
        static void SetIconAssetBundle() { DeProject.SetIconForSelections(DeProjectData.IcoType.AssetBundle); }

        [MenuItem("Assets/DeProject/Icon/Audio", false, _Priority_Icon)]
        static void SetIconAudio() { DeProject.SetIconForSelections(DeProjectData.IcoType.Audio); }

        [MenuItem("Assets/DeProject/Icon/Fonts", false, _Priority_Icon)]
        static void SetIconFonts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Fonts); }

        [MenuItem("Assets/DeProject/Icon/Prefabs", false, _Priority_Icon)]
        static void SetIconPrefabs() { DeProject.SetIconForSelections(DeProjectData.IcoType.Prefab); }

        [MenuItem("Assets/DeProject/Icon/Scripts", false, _Priority_Icon)]
        static void SetIconScripts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Scripts); }

        [MenuItem("Assets/DeProject/Icon/Textures", false, _Priority_Icon)]
        static void SetIconTextures() { DeProject.SetIconForSelections(DeProjectData.IcoType.Textures); }

        [MenuItem("Assets/DeProject/Icon/Cog", false, _Priority_Icon_Sub)]
        static void SetIconCog() { DeProject.SetIconForSelections(DeProjectData.IcoType.Cog); }

        [MenuItem("Assets/DeProject/Icon/Demigiant", false, _Priority_Icon_Sub)]
        static void SetIconDemigiant() { DeProject.SetIconForSelections(DeProjectData.IcoType.Demigiant); }

        [MenuItem("Assets/DeProject/Icon/Heart", false, _Priority_Icon_Sub)]
        static void SetIconHeart() { DeProject.SetIconForSelections(DeProjectData.IcoType.Heart); }

        [MenuItem("Assets/DeProject/Icon/Play", false, _Priority_Icon_Sub)]
        static void SetIconPlay() { DeProject.SetIconForSelections(DeProjectData.IcoType.Play); }

        [MenuItem("Assets/DeProject/Icon/Skull", false, _Priority_Icon_Sub)]
        static void SetIconSkull() { DeProject.SetIconForSelections(DeProjectData.IcoType.Skull); }

        [MenuItem("Assets/DeProject/Icon/Star", false, _Priority_Icon_Sub)]
        static void SetIconStar() { DeProject.SetIconForSelections(DeProjectData.IcoType.Star); }

        #endregion

        #region Colors

        [MenuItem("Assets/DeProject/Set Color ▸ Blue", false, _Priority_Color)]
        static void SetColorBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.Blue); }

        [MenuItem("Assets/DeProject/Set Color ▸ Bright Blue", false, _Priority_Color)]
        static void SetColorBrightBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue); }

        [MenuItem("Assets/DeProject/Set Color ▸ Green", false, _Priority_Color)]
        static void SetColorGreen() { DeProject.SetColorForSelections(DeProjectData.HColor.Green); }

        [MenuItem("Assets/DeProject/Set Color ▸ Orange", false, _Priority_Color)]
        static void SetColorOrange() { DeProject.SetColorForSelections(DeProjectData.HColor.Orange); }

        [MenuItem("Assets/DeProject/Set Color ▸ Purple", false, _Priority_Color)]
        static void SetColorPurple() { DeProject.SetColorForSelections(DeProjectData.HColor.Purple); }

        [MenuItem("Assets/DeProject/Set Color ▸ Red", false, _Priority_Color)]
        static void SetColorRed() { DeProject.SetColorForSelections(DeProjectData.HColor.Red); }

        [MenuItem("Assets/DeProject/Set Color ▸ Yellow", false, _Priority_Color)]
        static void SetColorYellow() { DeProject.SetColorForSelections(DeProjectData.HColor.Yellow); }

        [MenuItem("Assets/DeProject/Set Color ▸ Bright Grey", false, _Priority_Color)]
        static void SetColorBrightGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightGrey); }

        [MenuItem("Assets/DeProject/Set Color ▸ Dark Grey", false, _Priority_Color)]
        static void SetColorDarkGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.DarkGrey); }

        [MenuItem("Assets/DeProject/Set Color ▸ Black", false, _Priority_Color)]
        static void SetColorBlack() { DeProject.SetColorForSelections(DeProjectData.HColor.Black); }

        [MenuItem("Assets/DeProject/Set Color ▸ White", false, _Priority_Color)]
        static void SetColorWhite() { DeProject.SetColorForSelections(DeProjectData.HColor.White); }
        
        #endregion
    }
}