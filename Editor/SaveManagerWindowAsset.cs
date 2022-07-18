using UnityEngine;
using System;
using Heartfield.Serialization;

namespace HeartfieldEditor.Serialization
{
    [Serializable]
    sealed class SaveManagerWindowAsset : EditorAsset
    {
        [SerializeField] internal bool useCustomDirectory;
        [SerializeField] internal SpecialFolders specialFolders;
        [SerializeField] internal bool includeCompanyName;
        [SerializeField] internal bool includeProductName;
        [SerializeField] internal string rootDirectory;
        [SerializeField] internal string finalDirectory;
        [SerializeField] internal string previewPath;
        [SerializeField] internal bool validFolder;

        [SerializeField] internal bool showMoreSettings;
        [SerializeField] internal bool takeScreenshot;
        [SerializeField] internal bool screenshotNativeResolution;
        [SerializeField] internal Vector2Int screenshotResolution;
        
        [SerializeField] internal bool countTotalPlayedTime;
        [SerializeField] internal bool countWhilePaused;

        public override void RevertDefults()
        {
            useCustomDirectory = false;
            specialFolders = SpecialFolders.ApplicationData;
            includeCompanyName = true;
            includeProductName = true;
            takeScreenshot = true;
            screenshotNativeResolution = false;
            screenshotResolution = new Vector2Int(480, 270);

            countTotalPlayedTime = true;
            countWhilePaused = true;
        }
    }
}