// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2017/02/04 11:56
// License Copyright (c) Daniele Giardini

using DG.DemiLib.External;
using UnityEditor;
using UnityEngine;

namespace DG.DeEditorTools.Hierarchy
{
    public class DeHierarchyContextMenu : MonoBehaviour
    {
        const int _Priority_Status = -116;
        const int _Priority_Pre = -115;
        const int _Priority = -100; // was 21
        const int _Priority_Evidence_Sub0 = _Priority + 11;
        const int _Priority_Evidence_Sub1 = _Priority_Evidence_Sub0 + 11;
        const int _Priority_Evidence_Sub2 = _Priority_Evidence_Sub1 + 11;
        const int _Priority_Evidence_Sub3 = _Priority_Evidence_Sub2 + 11;
        
        #region Mode

        const string _StatusEnabled = "GameObject/DeHierarchy/STATUS : Enabled";
        const string _StatusDisabled = "GameObject/DeHierarchy/STATUS : Disabled";
        const string _StatusDisabledAtRuntime = "GameObject/DeHierarchy/STATUS : Disabled at Runtime";
        const string _MenuEnabled = "GameObject/DeHierarchy/Enabled";
        const string _MenuDisabled = "GameObject/DeHierarchy/Disabled";
        const string _MenuDisabledAtRuntime = "GameObject/DeHierarchy/Disabled at Runtime";
        
        [MenuItem(_StatusEnabled, false, _Priority_Status)]
        static void Status_Enabled() {}
        [MenuItem(_StatusEnabled, true)]
        static bool Validate_Status_Enabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            return data == null || data.mode == DeHierarchyData.Mode.Enabled;
        }
        
        [MenuItem(_StatusDisabled, false, _Priority_Status)]
        static void Status_Disabled() {}
        [MenuItem(_StatusDisabled, true)]
        static bool Validate_Status_Disabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            return data != null && data.mode == DeHierarchyData.Mode.Disabled;
        }
        
        [MenuItem(_StatusDisabledAtRuntime, false, _Priority_Status)]
        static void Status_DisabledAtRuntime() {}
        [MenuItem(_StatusDisabledAtRuntime, true)]
        static bool Validate_Status_DisabledAtRuntime()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            return data != null && data.mode == DeHierarchyData.Mode.DisabledAtRuntime;
        }
        
        [MenuItem(_MenuEnabled, false, _Priority_Pre)]
        static void Mode_Enabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(true);
            Undo.RecordObject(data, "DeHierarchy");
            data.mode = DeHierarchyData.Mode.Enabled;
            EditorUtility.SetDirty(data);
            DeHierarchy.Init();
        }
        [MenuItem(_MenuEnabled, true, _Priority_Pre)]
        static bool Validate_Mode_Enabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            Menu.SetChecked(_MenuEnabled, data == null || data.mode == DeHierarchyData.Mode.Enabled);
            return true;
        }
        
        [MenuItem(_MenuDisabled, false, _Priority_Pre + 1)]
        static void Mode_Disabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(true);
            Undo.RecordObject(data, "DeHierarchy");
            data.mode = DeHierarchyData.Mode.Disabled;
            EditorUtility.SetDirty(data);
            DeHierarchy.Init();
        }
        [MenuItem(_MenuDisabled, true, _Priority_Pre + 1)]
        static bool Validate_Mode_Disabled()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            Menu.SetChecked(_MenuDisabled, data != null && data.mode == DeHierarchyData.Mode.Disabled);
            return true;
        }
        
        [MenuItem(_MenuDisabledAtRuntime, false, _Priority_Pre + 2)]
        static void Mode_DisabledAtRuntime()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(true);
            Undo.RecordObject(data, "DeHierarchy");
            data.mode = DeHierarchyData.Mode.DisabledAtRuntime;
            EditorUtility.SetDirty(data);
            DeHierarchy.Init();
        }
        [MenuItem(_MenuDisabledAtRuntime, true, _Priority_Pre + 2)]
        static bool Validate_Mode_DisabledAtRuntime()
        {
            DeHierarchyData data = DeHierarchy.ConnectToProjectData(false);
            Menu.SetChecked(_MenuDisabledAtRuntime, data != null && data.mode == DeHierarchyData.Mode.DisabledAtRuntime);
            return true;
        }

        #endregion

        #region Reset

        [MenuItem("GameObject/DeHierarchy/Customize Panel", false, _Priority)]
        static void OpenCustomizePanel() { DeHierarchyCustomizePanel.ShowWindow(); }

        [MenuItem("GameObject/DeHierarchy/Reset", false, _Priority)]
        static void SetColorReset() { DeHierarchy.ResetSelections(); }

        #endregion

        #region Icon

        [MenuItem("GameObject/DeHierarchy/Icon/Audio", false, _Priority_Evidence_Sub0)]
        static void SetIconAudio() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Audio); }
        
        [MenuItem("GameObject/DeHierarchy/Icon/Camera", false, _Priority_Evidence_Sub0)]
        static void SetIconCamera() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Camera); }

        [MenuItem("GameObject/DeHierarchy/Icon/Cog", false, _Priority_Evidence_Sub0)]
        static void SetIconCog() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Cog); }

        [MenuItem("GameObject/DeHierarchy/Icon/Comment", false, _Priority_Evidence_Sub0)]
        static void SetIconComment() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Comment); }

        [MenuItem("GameObject/DeHierarchy/Icon/Dot", false, _Priority_Evidence_Sub0)]
        static void SetIconDot() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Dot); }

        [MenuItem("GameObject/DeHierarchy/Icon/Heart", false, _Priority_Evidence_Sub0)]
        static void SetIconHeart() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Heart); }

        [MenuItem("GameObject/DeHierarchy/Icon/Light", false, _Priority_Evidence_Sub0)]
        static void SetIconLight() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Light); }

        [MenuItem("GameObject/DeHierarchy/Icon/Play", false, _Priority_Evidence_Sub0)]
        static void SetIconPlay() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Play); }

        [MenuItem("GameObject/DeHierarchy/Icon/Skull", false, _Priority_Evidence_Sub0)]
        static void SetIconSkull() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Skull); }

        [MenuItem("GameObject/DeHierarchy/Icon/Star", false, _Priority_Evidence_Sub0)]
        static void SetIconStar() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Star); }

        [MenuItem("GameObject/DeHierarchy/Icon/UI", false, _Priority_Evidence_Sub0)]
        static void SetIconUI() { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.UI); }

        #endregion

        #region Separators

        [MenuItem("GameObject/DeHierarchy/Separator/None", false, _Priority_Evidence_Sub1)]
        static void RemoveSeparator() { DeHierarchy.ResetSeparatorsForSelections(); }

        #region Top


        [MenuItem("GameObject/DeHierarchy/Separator/Top/Red", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorRed() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Red); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Orange", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorOrange() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Orange); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Yellow", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorYellow() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Yellow); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Green", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorGreen() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Green); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Blue", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorBlue() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Blue); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Purple", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorPurple() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Purple); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Pink", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorPink() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Pink); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Bright Grey", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorBrightGrey() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.BrightGrey); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Dark Grey", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorDarkGrey() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.DarkGrey); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/Black", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorBlack() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.Black); }
        [MenuItem("GameObject/DeHierarchy/Separator/Top/White", false, _Priority_Evidence_Sub2)]
        static void SetTopSeparatorWhite() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Top, DeHierarchyComponent.HColor.White); }
        
        #endregion

        #region Bottom

        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Red", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorRed() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Red); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Orange", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorOrange() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Orange); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Yellow", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorYellow() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Yellow); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Green", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorGreen() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Green); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Blue", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorBlue() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Blue); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Purple", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorPurple() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Purple); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Pink", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorPink() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Pink); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Bright Grey", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorBrightGrey() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.BrightGrey); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Dark Grey", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorDarkGrey() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.DarkGrey); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/Black", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorBlack() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.Black); }
        [MenuItem("GameObject/DeHierarchy/Separator/Bottom/White", false, _Priority_Evidence_Sub2)]
        static void SetBottomSeparatorWhite() { DeHierarchy.SetSeparatorForSelections(DeHierarchyComponent.SeparatorType.Bottom, DeHierarchyComponent.HColor.White); }
        
        #endregion
    
        #endregion    

        #region Colors

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Red", false, _Priority_Evidence_Sub3)]
        static void SetColorRed() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Red); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Orange", false, _Priority_Evidence_Sub3)]
        static void SetColorOrange() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Orange); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Yellow", false, _Priority_Evidence_Sub3)]
        static void SetColorYellow() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Yellow); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Green", false, _Priority_Evidence_Sub3)]
        static void SetColorGreen() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Green); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Blue", false, _Priority_Evidence_Sub3)]
        static void SetColorBlue() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Blue); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Purple", false, _Priority_Evidence_Sub3)]
        static void SetColorPurple() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Purple); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Pink", false, _Priority_Evidence_Sub3)]
        static void SetColorPink() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Pink); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Bright Grey", false, _Priority_Evidence_Sub3)]
        static void SetColorBrightGrey() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.BrightGrey); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Dark Grey", false, _Priority_Evidence_Sub3)]
        static void SetColorDarkGrey() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.DarkGrey); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Black", false, _Priority_Evidence_Sub3)]
        static void SetColorBlack() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Black); }
        [MenuItem("GameObject/DeHierarchy/Set Color ▸ White", false, _Priority_Evidence_Sub3)]
        static void SetColorWhite() { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.White); }
        
        #endregion
    }
}