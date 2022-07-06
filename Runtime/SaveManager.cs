using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    public static class SaveManager
    {
        static Dictionary<object, SaveData> serializebleObjects = new Dictionary<object, SaveData>();
        static GlobalData globalData = new GlobalData();

        static HashSet<string> saveFilesPath = new HashSet<string>();

        public delegate void SerializationEvents();
        public static SerializationEvents OnPopulateSave;
        public static SerializationEvents OnLoadFromSaveData;
        public static SerializationEvents OnDeleteSaveData;

        //public static void UpdateTotalPlayedTime() => _totalPlayedTime++;
        //public static int GetTotalPlayedTime => _totalPlayedTime;

        static void RegisterSaveableObject<T>(T source) => serializebleObjects.Add(source, new SaveData());

        public static T GetSavedData<T>(this object source) => (T)serializebleObjects[source].Get(source);

        public static void GetSavedData<T>(this object source, ref T field) => field = serializebleObjects[source].Get(field);

        public static void AddDataToSave<T>(this object source, T field)
        {
            if (!serializebleObjects.ContainsKey(source))
                RegisterSaveableObject(source);

            serializebleObjects[source].Add(field);
        }

        public static void AddDataToSave<T>(T source)
        {
            if (!serializebleObjects.ContainsKey(source))
                RegisterSaveableObject(source);

            serializebleObjects[source].Add(source);
        }

        static void CreateGlobalData(string path)
        {
            globalData.lastSaveFilePath = path;
            SerializationSystem.Serialize(globalData, SaveSettings.GetFilePath("GlobalData"));
        }

        public static void Save(string name, int slot)
        {
            OnPopulateSave?.Invoke();
            CreateGlobalData(SaveSettings.GetFilePath(name, slot));
            SerializationSystem.Serialize(serializebleObjects, globalData.lastSaveFilePath);//, slot, out isNewSave);
        }

        static string GetSaveFilePath(int slot)
        {
            //CheckDirectory();
            return Directory.GetFiles(SaveSettings.directory).Where(a => a.EndsWith($"{slot:00}.sav")).ToArray()[0];
        }

        static void LoadGlobalData()
        {
            globalData = SerializationSystem.Deserialize<GlobalData>(SaveSettings.GetFilePath("GlobalData"));
        }

        public static void Load(string name, int slot)
        {
            LoadGlobalData();
            serializebleObjects = SerializationSystem.Deserialize<Dictionary<object, SaveData>>(GetSaveFilePath(slot));
            OnLoadFromSaveData?.Invoke();
        }

        public static void Delete(string name, int slot)
        {
            SerializationSystem.DeleteFile(GetSaveFilePath(slot));
            OnDeleteSaveData?.Invoke();
        }

        public static int GetSaveFilesAmount => saveFilesPath.Count;

        //public static int LastSaveDataSlot => lastSaveSlot;
    }

    [Serializable]
    class TestClass2 : ISaveable
    {
        public void LoadFromSaveData()
        {

        }

        public void PopulateSaveData()
        {

        }
    }

    [Serializable]
    class TestClass
    {
        internal float a = float.MaxValue;
        internal int b = int.MinValue;
        internal bool c = true;
        internal string d = "asdfghjk";
        internal List<int> e = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        internal Dictionary<int, int> f = new Dictionary<int, int>() { { 1, 1 }, { 2, 2 }, { 3, 3 } };

        [Serializable]
        struct Temp
        {
            int g;

            internal Temp(int g)
            {
                this.g = g;
            }
        }

        Temp tmp = new Temp(9);

        void Awake()
        {
            SaveManager.OnPopulateSave += PopulateSaveData;
        }

        void PopulateSaveData()
        {
            this.AddDataToSave(a);
            this.AddDataToSave(b);
            this.AddDataToSave(c);
            this.AddDataToSave(d);
            this.AddDataToSave(e);
            this.AddDataToSave(f);
            this.AddDataToSave(tmp);
        }

        void LoadFromSaveData()
        {
            this.GetSavedData(ref a);
            this.GetSavedData(ref b);
            this.GetSavedData(ref c);
            this.GetSavedData(ref d);
            this.GetSavedData(ref e);
            this.GetSavedData(ref f);
            this.GetSavedData(ref tmp);
        }
    }
}