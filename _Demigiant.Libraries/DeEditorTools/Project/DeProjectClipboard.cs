// Author: Daniele Giardini - http://www.demigiant.com
// Created: 2018/03/24 14:14
// License Copyright (c) Daniele Giardini

namespace DG.DeEditorTools.Project
{
    public static class DeProjectClipboard
    {
        public static bool hasStoreData { get { return storedItem != null; } }
        public static DeProjectData.CustomizedItem storedItem;
    }
}