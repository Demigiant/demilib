// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/13 18:11
// License Copyright (c) Daniele Giardini

using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Project
{
    public class DeProjectContextMenu : MonoBehaviour
    {
        const int _Priority = 18;

        #region Reset

        [MenuItem("Assets/DeProject/Reset", false, _Priority + 1)]
        static void SetColorReset() { DeProject.SetColorForSelections(DeProjectData.HColor.None); }

        [MenuItem("Assets/DeProject/Custom", false, _Priority + 1)]
        static void SetColorCustom() { DeProject.CustomizeSelections(); }

        #endregion

        #region Icon

        [MenuItem("Assets/DeProject/Icon/Audio", false, _Priority + 2)]
        static void SetIconAudio() { DeProject.SetIconForSelections(DeProjectData.IcoType.Audio); }

        [MenuItem("Assets/DeProject/Icon/Fonts", false, _Priority + 2)]
        static void SetIconFonts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Fonts); }

        [MenuItem("Assets/DeProject/Icon/Prefabs", false, _Priority + 2)]
        static void SetIconPrefabs() { DeProject.SetIconForSelections(DeProjectData.IcoType.Prefab); }

        [MenuItem("Assets/DeProject/Icon/Scripts", false, _Priority + 2)]
        static void SetIconScripts() { DeProject.SetIconForSelections(DeProjectData.IcoType.Scripts); }

        [MenuItem("Assets/DeProject/Icon/Textures", false, _Priority + 2)]
        static void SetIconTextures() { DeProject.SetIconForSelections(DeProjectData.IcoType.Textures); }

        [MenuItem("Assets/DeProject/Icon/Cog", false, _Priority + 2)]
        static void SetIconCog() { DeProject.SetIconForSelections(DeProjectData.IcoType.Cog); }

        [MenuItem("Assets/DeProject/Icon/Demigiant", false, _Priority + 2)]
        static void SetIconDemigiant() { DeProject.SetIconForSelections(DeProjectData.IcoType.Demigiant); }

        [MenuItem("Assets/DeProject/Icon/Play", false, _Priority + 2)]
        static void SetIconPlay() { DeProject.SetIconForSelections(DeProjectData.IcoType.Play); }

        [MenuItem("Assets/DeProject/Icon/Heart", false, _Priority + 2)]
        static void SetIconHeart() { DeProject.SetIconForSelections(DeProjectData.IcoType.Heart); }

        [MenuItem("Assets/DeProject/Icon/Skull", false, _Priority + 2)]
        static void SetIconSkull() { DeProject.SetIconForSelections(DeProjectData.IcoType.Skull); }

        [MenuItem("Assets/DeProject/Icon/Star", false, _Priority + 2)]
        static void SetIconStar() { DeProject.SetIconForSelections(DeProjectData.IcoType.Star); }

        #endregion

        #region Colors

        [MenuItem("Assets/DeProject/Set Color ▸ Blue", false, _Priority + 3)]
        static void SetColorBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.Blue); }

        [MenuItem("Assets/DeProject/Set Color ▸ Bright Blue", false, _Priority + 3)]
        static void SetColorBrightBlue() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightBlue); }

        [MenuItem("Assets/DeProject/Set Color ▸ Green", false, _Priority + 3)]
        static void SetColorGreen() { DeProject.SetColorForSelections(DeProjectData.HColor.Green); }

        [MenuItem("Assets/DeProject/Set Color ▸ Orange", false, _Priority + 3)]
        static void SetColorOrange() { DeProject.SetColorForSelections(DeProjectData.HColor.Orange); }

        [MenuItem("Assets/DeProject/Set Color ▸ Purple", false, _Priority + 3)]
        static void SetColorPurple() { DeProject.SetColorForSelections(DeProjectData.HColor.Purple); }

        [MenuItem("Assets/DeProject/Set Color ▸ Red", false, _Priority + 3)]
        static void SetColorRed() { DeProject.SetColorForSelections(DeProjectData.HColor.Red); }

        [MenuItem("Assets/DeProject/Set Color ▸ Yellow", false, _Priority + 3)]
        static void SetColorYellow() { DeProject.SetColorForSelections(DeProjectData.HColor.Yellow); }

        [MenuItem("Assets/DeProject/Set Color ▸ Bright Grey", false, _Priority + 3)]
        static void SetColorBrightGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.BrightGrey); }

        [MenuItem("Assets/DeProject/Set Color ▸ Dark Grey", false, _Priority + 3)]
        static void SetColorDarkGrey() { DeProject.SetColorForSelections(DeProjectData.HColor.DarkGrey); }

        [MenuItem("Assets/DeProject/Set Color ▸ Black", false, _Priority + 3)]
        static void SetColorBlack() { DeProject.SetColorForSelections(DeProjectData.HColor.Black); }

        [MenuItem("Assets/DeProject/Set Color ▸ White", false, _Priority + 3)]
        static void SetColorWhite() { DeProject.SetColorForSelections(DeProjectData.HColor.White); }
        
        #endregion
    }
}