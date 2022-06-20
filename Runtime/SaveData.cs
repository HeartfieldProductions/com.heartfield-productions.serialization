using System;
using UnityEngine;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    [Serializable]
    sealed class SaveData
    {
        Dictionary<Hash128, object> _data = new Dictionary<Hash128, object>();

        internal void Clear() => _data.Clear();
        internal void Add<T>(Hash128 hash, T data) => _data.Add(hash, data);
        internal T Get<T>(Hash128 hash) => (T)_data[hash];
    }
}