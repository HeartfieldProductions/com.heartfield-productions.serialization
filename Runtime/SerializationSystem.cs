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
            if (!Directory.Exists(SaveSettings.path))
                Directory.CreateDirectory(SaveSettings.path);
        }

        /// <summary>
        /// ..."slot"."SaveSettings.fileExtension"
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        static string GetFileSlotAndExtension(int slot) => $"{slot:0:00}.{SaveSettings.fileExtension}";

        static FileInfo GetFileInfo(int slot) => _dirInfo.GetFiles().Where(a => a.Name.EndsWith(GetFileSlotAndExtension(slot), StringComparison.OrdinalIgnoreCase)).ToArray()[0];

        static void PopulateFilesInfo(int slot)
        {
            _filesInfo.Add(slot, GetFileInfo(slot));
        }

        static SerializationSystem()
        {
            //_saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Application.companyName, Application.productName, "Saves");

            CreateDirectory();

            _dirInfo = new DirectoryInfo(SaveSettings.path);

            if (_dirInfo.GetFiles().Length > 0)
            {
                int lastSaveSlot = PlayerPrefs.GetInt(_lastSaveSlotKey);

                for (int i = 1; i < lastSaveSlot; i++)
                    PopulateFilesInfo(i);
            }
        }

        //internal static void OverrideDirectory(Environment.SpecialFolder environmentFolder, string path) => _saveDirectory = string.Format("{0}/{1}/Saves/", Environment.GetFolderPath(environmentFolder), path);

        //internal static void OverrideDirectory(string path) => _saveDirectory = string.Format("{0}/Saves/", path);

        //internal static void OverrideSaveFileExtension(string type) => _saveFileExtension = type;

        static SurrogateSelector SetupSurrogates()
        {
            var selector = new SurrogateSelector();
            var streamingCtx = new StreamingContext(StreamingContextStates.All);

            var v2 = new Vector2SerializationSurrogate(out var v2Type);
            selector.AddSurrogate(v2Type, streamingCtx, v2);

            var v3 = new Vector3SerializationSurrogate(out var v3Type);
            selector.AddSurrogate(v3Type, streamingCtx, v3);

            var v4 = new Vector4SerializationSurrogate(out var v4Type);
            selector.AddSurrogate(v4Type, streamingCtx, v4);

            var qt = new QuaternionSerializationSurrogate(out var qtType);
            selector.AddSurrogate(qtType, streamingCtx, qt);

            var tr = new TransformSerializationSurrogate(out var trType);
            selector.AddSurrogate(trType, streamingCtx, tr);

            return selector;
        }

        static BinaryFormatter GetBinaryFormatter()
        {
            if (_binaryFormatter == null)
            {
                _binaryFormatter = new BinaryFormatter
                {
                    SurrogateSelector = SetupSurrogates()
                };
            }

            return _binaryFormatter;
        }

        //static string CreateSaveFilePath(string name, int slot) => Path.Combine(Settings.path, string.Format("{0}{1:00}.{2}", name, slot, Settings.fileExtension));

        internal static bool Serialize(SaveData data, string saveName, int slot, out bool isNewSave)
        {
            var formatter = GetBinaryFormatter();

            CreateDirectory();

            _filesInfo.Clear();

            string path = SaveSettings.GetFullFilePath(saveName, slot);

            isNewSave = !File.Exists(path);

            var file = File.Create(path);
            formatter.Serialize(file, data);
            file.Close();

            //_dirInfo = new DirectoryInfo(SaveSettings.path);

            PopulateFilesInfo(slot);

            PlayerPrefs.SetInt(_lastSaveSlotKey, slot);
            PlayerPrefs.Save();

            Debug.Log($"Game saved succesfully at: {path}");

            return true;
        }

        internal static FileInfo GetSaveDataInfo(int slot) => _filesInfo[slot];

        internal static int GetLastSaveFileSlot() => PlayerPrefs.HasKey(_lastSaveSlotKey) ? PlayerPrefs.GetInt(_lastSaveSlotKey) : 0;

        static string GetSaveFilePath(int slot)
        {
            CreateDirectory();
            return Directory.GetFiles(SaveSettings.path).Where(a => a.EndsWith(GetFileSlotAndExtension(slot))).ToArray()[0];
        }

        internal static SaveData Deserialize(int slot)
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
                return save as SaveData;
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
            if (!Directory.Exists(SaveSettings.path))
                return 0;

            return Directory.GetFiles(SaveSettings.path).Length;
        }
    }
}