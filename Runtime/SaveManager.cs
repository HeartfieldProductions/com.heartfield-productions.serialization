using System;
using UnityEngine;
using System.Collections.Generic;

namespace Heartfield.SerializableData
{
    [Serializable]
    sealed class Data
    {
        Dictionary<Hash128, object> _data = new Dictionary<Hash128, object>();

        internal void ClearData() => _data.Clear();        
        internal void AddData<T>(Hash128 hash, T data) => _data.Add(hash, data);
        internal T GetData<T>(Hash128 hash) => (T)_data[hash];
    }

    public static class SaveManager
    {
        static Data _data = new Data();

        static List<ISaveable> _saveableObjects = new List<ISaveable>();

        static int _totalPlayedTime = 0;
        static int _lastPlayedTime = 0;

        static int _currentSaveableId = 0;
        static ISaveable _lastISaveable;

        public static void UpdateTotalPlayedTime() => _totalPlayedTime++;
        public static int GetTotalPlayedTime => _totalPlayedTime;

        public static void RegisterSaveableObject(ISaveable obj) => _saveableObjects.Add(obj);

        public static void AddData<T>(ISaveable saveable, T data) => _data.AddData(GetPropertyHash(saveable, data), data);

        public static T GetData<T>(ISaveable saveable, T data) => _data.GetData<T>(GetPropertyHash(saveable, data));
        
        static Hash128 GetPropertyHash<T>(ISaveable saveable, T property)
        {
            Hash128 hash = new Hash128();

            if (_lastISaveable == saveable)
                _currentSaveableId++;
            else
                _currentSaveableId = 0;

            hash.Append(saveable.GetType().MetadataToken + property.GetType().MetadataToken + _currentSaveableId);

            _lastISaveable = saveable;

            return hash;
        }

        public static void OnSave(string name, int slot, out bool isNewSave)
        {
            _currentSaveableId = 0;

            PlayerPrefs.SetInt(PlayedTimeKey(slot), _totalPlayedTime + _lastPlayedTime);

            for (int i = 0; i < _saveableObjects.Count; i++)
            {
                _saveableObjects[i].PopulateSaveData();
            }

            SerializationSystem.Save(_data, name, slot, out isNewSave);

            _data.ClearData();
        }

        public static void OnLoad(int slot)
        {
            _currentSaveableId = 0;

            _data = SerializationSystem.Load(slot);

            for (int i = 0; i < _saveableObjects.Count; i++)
            {
                _saveableObjects[i].LoadFromSaveData();
            }

            _lastPlayedTime = PlayerPrefs.GetInt(PlayedTimeKey(slot));
            _totalPlayedTime = _lastPlayedTime;

            _data.ClearData();
        }

        static string PlayedTimeKey(int slot) => string.Format("PlayedTime_{0:00}", slot);

        public static void OnDelete(int slot) => SerializationSystem.DeleteSaveFile(slot);

        public static int SaveFilesAmount => SerializationSystem.SaveFilesAmount();
        public static bool HasSaves => SaveFilesAmount > 0;
        public static int LastSaveDataSlot => SerializationSystem.GetLastSaveFileSlot();
    }
}