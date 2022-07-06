using UnityEngine;

namespace Heartfield.Serialization
{
    struct GlobalData
    {
        [SerializeField] internal int lastSaveSlot;
        [SerializeField] internal string lastSaveFilePath;

        [SerializeField] internal int totalPlayedTime;
        [SerializeField] internal int lastPlayedTime;
    }
}