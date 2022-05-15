using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Heartfield.SerializableData
{
    internal static class SerializationSystem
    {
        static DirectoryInfo _dirInfo;

        static Dictionary<int, FileInfo> _filesInfo = new Dictionary<int, FileInfo>();

        static string _saveDirectory;

        const string _lastSaveSlotKey = "LastSaveSlot";
        const string _saveFileExtension = ".soul";

        static BinaryFormatter _binaryFormatter;

        static SerializationSystem()
        {
            _saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.companyName, Application.productName, "Saves");

            CreateSaveFolder();

            _dirInfo = new DirectoryInfo(_saveDirectory);

            if (_dirInfo.GetFiles().Length > 0)
            {
                int lastSaveSlot = PlayerPrefs.GetInt(_lastSaveSlotKey);

                for (int i = 1; i < lastSaveSlot; i++)
                    _filesInfo.Add(i, GetFileInfo(i));
            }
        }

        //internal static void OverrideDirectory(Environment.SpecialFolder environmentFolder, string path) => _saveDirectory = string.Format("{0}/{1}/Saves/", Environment.GetFolderPath(environmentFolder), path);

        //internal static void OverrideDirectory(string path) => _saveDirectory = string.Format("{0}/Saves/", path);

        //internal static void OverrideSaveFileExtension(string type) => _saveFileExtension = type;

        internal static bool Save(Data data, string saveName, int slot, out bool isNewSave)
        {
            BinaryFormatter formatter = GetBinaryFormatter();

            CreateSaveFolder();

            _filesInfo.Clear();

            string path = CreateSaveFilePath(saveName, slot);

            isNewSave = !File.Exists(path);

            FileStream file = File.Create(path);
            formatter.Serialize(file, data);
            file.Close();

            _dirInfo = new DirectoryInfo(_saveDirectory);

            _filesInfo.Add(slot, GetFileInfo(slot));

            PlayerPrefs.SetInt(_lastSaveSlotKey, slot);
            PlayerPrefs.Save();

            Debug.Log(string.Format("Game saved succesfully at: {0}", path));

            return true;
        }

        internal static FileInfo GetSaveDataInfo(int slot) => _filesInfo[slot];

        static FileInfo GetFileInfo(int slot) => _dirInfo.GetFiles().Where(a => a.Name.EndsWith(string.Format("{0:00}{1}", slot, _saveFileExtension), StringComparison.OrdinalIgnoreCase)).ToArray()[0];

        internal static int GetLastSaveFileSlot() => PlayerPrefs.HasKey(_lastSaveSlotKey) ? PlayerPrefs.GetInt(_lastSaveSlotKey) : 0;

        internal static Data Load(int slot)
        {
            string path = GetSaveFilePath(slot);

            if (!File.Exists(path))
            {
                Debug.LogWarning(path + " is null");
                return null;
            }

            BinaryFormatter formatter = GetBinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                object save = formatter.Deserialize(file);
                //Debug.Log(string.Format("Game loaded succesfully at: {0}", path));
                file.Close();
                return save as Data;
            }
            catch
            {
                //Debug.LogErrorFormat("Failed to load file at: {0}", path);
                file.Close();
                return null;
            }
        }

        internal static void DeleteSaveFile(int slot)
        {
            if (PlayerPrefs.HasKey(_lastSaveSlotKey) & slot == PlayerPrefs.GetInt(_lastSaveSlotKey))
                PlayerPrefs.DeleteKey(_lastSaveSlotKey);

            string path = GetSaveFilePath(slot);

            if (File.Exists(path))
                File.Delete(path);
        }

        static string CreateSaveFilePath(string name, int slot) => Path.Combine(_saveDirectory, string.Format("{0}{1:00}{2}", name, slot, _saveFileExtension));

        internal static int SaveFilesAmount() => GetSaveFilesPath().Length;

        internal static string[] GetSaveFilesPath()
        {
            CreateSaveFolder();
            return Directory.GetFiles(_saveDirectory).Where(a => a.EndsWith(_saveFileExtension, StringComparison.OrdinalIgnoreCase)).ToArray();
        }

        static string GetSaveFilePath(int slot)
        {
            CreateSaveFolder();
            return Directory.GetFiles(_saveDirectory).Where(a => a.EndsWith(string.Format("{0:00}{1}", slot, _saveFileExtension), StringComparison.OrdinalIgnoreCase)).ToArray()[0];
        }

        static BinaryFormatter GetBinaryFormatter()
        {
            if (_binaryFormatter != null)
                return _binaryFormatter;
            else
            {
                _binaryFormatter = new BinaryFormatter
                {
                    SurrogateSelector = SetupSurrogates()
                };

                return _binaryFormatter;
            }
        }

        static void CreateSaveFolder()
        {
            if (!Directory.Exists(_saveDirectory))
                Directory.CreateDirectory(_saveDirectory);
        }

        static SurrogateSelector SetupSurrogates()
        {
            SurrogateSelector selector = new SurrogateSelector();

            Vector3SerializationSurrogate v3 = new Vector3SerializationSurrogate();
            selector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), v3);

            QuaternionSerializationSurrogate qt = new QuaternionSerializationSurrogate();
            selector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), qt);

            TransformSerializationSurrogate tr = new TransformSerializationSurrogate();
            selector.AddSurrogate(typeof(Transform), new StreamingContext(StreamingContextStates.All), tr);

            return selector;
        }
    }
}