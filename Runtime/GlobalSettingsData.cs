using UnityEngine;

namespace Heartfield.Serialization
{
    class GlobalSettingsData
    {
        [SerializeField] internal string lastManualSaveFilePath;

        [SerializeField] internal string lastAutoSavePath;
        [SerializeField] internal int lastAutoSaveSlot;

        [SerializeField] internal string lastQuickSavePath;
        [SerializeField] internal int lastQuickSaveSlot;

        internal void Reset()
        {
            lastManualSaveFilePath = string.Empty;
            lastAutoSavePath = string.Empty;
            lastAutoSaveSlot = 0;
            lastQuickSavePath = string.Empty;
            lastQuickSaveSlot = 0;
        }
    }
}