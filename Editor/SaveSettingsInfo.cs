using System;
using UnityEngine;
using Heartfield.Serialization;

namespace HeartfieldEditor.Serialization
{
    [Serializable]
    struct SaveSettingsInfo
    {
        [SerializeField] internal bool useCustomDirectory;
        [SerializeField] internal SpecialFolders specialFolders;
        [SerializeField] internal bool includeCompanyName;
        [SerializeField] internal bool includeProductName;
        [SerializeField] internal DisplayNameMode displayNameMode;
        [SerializeField] internal bool takeScreenshot;
        [SerializeField] internal bool screenshotNativeResolution;
        [SerializeField] internal Vector2Int screenshotResolution;
    }
}