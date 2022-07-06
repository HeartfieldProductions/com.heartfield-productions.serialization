using UnityEngine;
using System;
using Heartfield.Serialization;

namespace HeartfieldEditor.Serialization
{
    [Serializable]
    sealed class SaveManagerWindowAsset : EditorWindowAsset
    {
        [SerializeField] internal bool useCustomDirectory;
        [SerializeField] internal SpecialFolders specialFolders;
        [SerializeField] internal bool includeCompanyName;
        [SerializeField] internal bool includeProductName;
        [SerializeField] internal DisplayNameMode displayNameMode;
        [SerializeField] internal string rootDirectory;
        [SerializeField] internal string finalPath;
        [SerializeField] internal string previewPath;
        [SerializeField] internal bool validFolder;

        [SerializeField] internal bool showMoreSettings;
        [SerializeField] internal bool takeScreenshot;
        [SerializeField] internal bool screenshotNativeResolution;
        [SerializeField] internal Vector2Int screenshotResolution;

        public override void RevertDefults()
        {
            useCustomDirectory = false;
            specialFolders = SpecialFolders.ApplicationData;
            includeCompanyName = true;
            includeProductName = true;
            displayNameMode = DisplayNameMode.Name_DD_MM_YYYY_hh_mm_ss;
            takeScreenshot = true;
            screenshotNativeResolution = false;
            screenshotResolution = new Vector2Int(480, 270);
        }
    }
}