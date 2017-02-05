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
        #region Reset

        [MenuItem("GameObject/DeHierarchy/Reset", false, 21)]
        static void SetColorReset()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.None); }
        
        #endregion

        #region Icon

        [MenuItem("GameObject/DeHierarchy/Icon/Dot", false, 22)]
        static void SetIconDot()
        { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Dot); }

        [MenuItem("GameObject/DeHierarchy/Icon/Star", false, 22)]
        static void SetIconStar()
        { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Star); }

        [MenuItem("GameObject/DeHierarchy/Icon/Cog", false, 22)]
        static void SetIconCog()
        { DeHierarchy.SetIconForSelections(DeHierarchyComponent.IcoType.Cog); }

        #endregion

        #region Colors

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Blue", false, 23)]
        static void SetColorBlue()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Blue); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Green", false, 23)]
        static void SetColorGreen()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Green); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Orange", false, 23)]
        static void SetColorOrange()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Orange); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Purple", false, 23)]
        static void SetColorPurple()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Purple); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Red", false, 23)]
        static void SetColorRed()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Red); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Yellow", false, 23)]
        static void SetColorYellow()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Yellow); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ BrightGrey", false, 23)]
        static void SetColorBrightGrey()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.BrightGrey); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ DarkGrey", false, 23)]
        static void SetColorDarkGrey()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.DarkGrey); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ Black", false, 23)]
        static void SetColorBlack()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.Black); }

        [MenuItem("GameObject/DeHierarchy/Set Color ▸ White", false, 23)]
        static void SetColorWhite()
        { DeHierarchy.SetColorForSelections(DeHierarchyComponent.HColor.White); }
        
        #endregion
    }
}