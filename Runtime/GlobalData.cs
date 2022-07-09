using UnityEngine;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    class GlobalData
    {
        [SerializeField] internal int lastSaveSlot;
        [SerializeField] internal string lastSaveFilePath;

        [SerializeField] internal HashSet<int> populatedSlots = new HashSet<int>();
    }
}