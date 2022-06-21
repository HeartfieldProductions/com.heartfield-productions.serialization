using UnityEngine;
using System;

namespace HeartfieldEditor.Serialization
{
    [Serializable]
    sealed class SaveManagerWindowAsset
    {
        [SerializeField] internal bool useCustomDirectory;
        [SerializeField] internal SpecialFolders specialFolders;
        [SerializeField] internal bool includeCompanyName;
        [SerializeField] internal bool includeProductName;
        [SerializeField] internal string path = "Saves";
        [SerializeField] internal string extension = "sav";
        [SerializeField] internal string rootDirectory;
        [SerializeField] internal string finalPath;
        [SerializeField] internal string previewPath;
        [SerializeField] internal bool validFolder;
    }
}