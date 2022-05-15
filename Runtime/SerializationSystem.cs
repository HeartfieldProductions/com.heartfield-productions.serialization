using System;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace Heartfield.Serialization
{
    internal static class SerializationSystem
    {
        static DirectoryInfo _dirInfo;

        static Dictionary<int, FileInfo> _filesInfo = new Dictionary<int, FileInfo>();

        const string _lastSaveSlotKey = "LastSaveSlot";

        static BinaryFormatter _binaryFormatter;

        static void CreateDirectory()
        {
            if (!Directory.Exists(Settings.path))
                Directory.CreateDirectory(Settings.path);
        }

        static FileInfo GetFileInfo(int slot) => _dirInfo.GetFiles().Where(a => a.Name.EndsWith(string.Format("{0:00}.{1}", slot, Settings.fileExtension), StringComparison.OrdinalIgnoreCase)).ToArray()[0];

        static SerializationSystem()
        {
            //_saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.companyName, Application.productName, "Saves");

            CreateDirectory();

            _dirInfo = new DirectoryInfo(Settings.path);

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

        //static string CreateSaveFilePath(string name, int slot) => Path.Combine(Settings.path, string.Format("{0}{1:00}.{2}", name, slot, Settings.fileExtension));

        internal static bool Serialize(Data data, string saveName, int slot, out bool isNewSave)
        {
            var formatter = GetBinaryFormatter();

            CreateDirectory();

            _filesInfo.Clear();

            string path = Settings.GetSaveFilePath(saveName, slot);

            isNewSave = !File.Exists(path);

            var file = File.Create(path);
            formatter.Serialize(file, data);
            file.Close();

            _dirInfo = new DirectoryInfo(Settings.path);

            _filesInfo.Add(slot, GetFileInfo(slot));

            PlayerPrefs.SetInt(_lastSaveSlotKey, slot);
            PlayerPrefs.Save();

            Debug.Log(string.Format("Game saved succesfully at: {0}", path));

            return true;
        }

        internal static FileInfo GetSaveDataInfo(int slot) => _filesInfo[slot];

        internal static int GetLastSaveFileSlot() => PlayerPrefs.HasKey(_lastSaveSlotKey) ? PlayerPrefs.GetInt(_lastSaveSlotKey) : 0;

        static string GetSaveFilePath(int slot)
        {
            CreateDirectory();
            return Directory.GetFiles(Settings.path).Where(a => a.EndsWith(string.Format("{0:00}.{1}", slot, Settings.fileExtension))).ToArray()[0];
        }

        internal static Data Deserialize(int slot)
        {
            string path = GetSaveFilePath(slot);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"{path} doesn't exist");
                return null;
            }

            var formatter = GetBinaryFormatter();
            var file = File.Open(path, FileMode.Open);

            try
            {
                var save = formatter.Deserialize(file);
                //Debug.Log(string.Format("Game loaded succesfully at: {0}", path));
                file.Close();
                return save as Data;
            }
            catch
            {
                Debug.LogError($"Failed to load file at: {path}");
                file.Close();
                return null;
            }
        }

        internal static void DeleteSaveFile(int slot)
        {
            if (PlayerPrefs.HasKey(_lastSaveSlotKey) && slot == PlayerPrefs.GetInt(_lastSaveSlotKey))
                PlayerPrefs.DeleteKey(_lastSaveSlotKey);

            string path = GetSaveFilePath(slot);

            if (File.Exists(path))
                File.Delete(path);
        }

        internal static int SaveFilesAmount()
        {
            if (!Directory.Exists(Settings.path))
                return 0;

            return Directory.GetFiles(Settings.path).Length;
        }
    }
}