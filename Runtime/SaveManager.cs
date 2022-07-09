using System;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Heartfield.Serialization
{
    public static class SaveManager
    {
        /// <summary>
        /// TKey is slot
        /// TValue is path
        /// </summary>
        static Dictionary<int, string> savePaths = new Dictionary<int, string>(2);        
        static GlobalData globalData = new GlobalData();

        const int MAX_QUICK_SAVES_AMOUNT = 10;
        const int MAX_AUTO_SAVES_AMOUNT = 10;
        const int MAX_CHECKPOINTS_SAVES_AMOUNT = 10;

        static void LoadGlobalData()
        {
            string path = SaveSettings.GetFilePath("GlobalData");

            if (!File.Exists(path))
                return;

            globalData = SerializationSystem.Deserialize<GlobalData>(path);
        }

        static void CheckFiles()
        {
            savePaths.Clear();
            var paths = Directory.GetFiles(SaveSettings.Directory, "*.sav").
                                            Where(file => Regex.IsMatch(Path.GetFileName(file), "^[0-9]+"));

            foreach (var path in paths)
            {
                var name = Path.GetFileName(path);
                var sb = new StringBuilder(name);
                sb.Remove(2, name.Length - 2);
                int slot = int.Parse(sb.ToString());
                savePaths.Add(slot, path);
            }
        }

        static SaveManager()
        {
            LoadGlobalData();
            CheckFiles();

#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
                ResumeTotalPlayedTime();
        }

        static Dictionary<object, SaveData> serializebleObjects = new Dictionary<object, SaveData>();

        static Coroutine playedTimeCoroutine;
        static float totalPlayedTime;

        public delegate void SerializationEvents();
        public static SerializationEvents OnPopulateSave;
        public static SerializationEvents OnLoadFromSaveData;
        public static SerializationEvents OnDeleteSaveData;

        public static int GetTotalPlayedTime => Mathf.CeilToInt(totalPlayedTime);

        public static void StopTotalPlayedTime()
        {
            if (playedTimeCoroutine != null)
                MonoBehaviourHelper.StopCoroutine(playedTimeCoroutine);
        }

        public static void ResumeTotalPlayedTime()
        {
            if (playedTimeCoroutine == null)
                playedTimeCoroutine = MonoBehaviourHelper.StartCoroutine(TotalPlayedTimeCoroutine());
        }

        public static T GetData<T>(this ISaveable source) => (T)serializebleObjects[source].Get(source);

        public static void GetData<T>(this ISaveable source, ref T field) => field = serializebleObjects[source].Get(field);

        public static void AddData<T>(this ISaveable source, T field)
        {
            if (!serializebleObjects.ContainsKey(source))
                serializebleObjects.Add(source, new SaveData());

            serializebleObjects[source].Add(field);
        }

        static IEnumerator TotalPlayedTimeCoroutine()
        {
            while (true)
            {
                totalPlayedTime += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        public static void Save(int slot, SaveType type)
        {
            var fileName = new StringBuilder();
            fileName.Append(type);
            fileName.Append(DateTime.Now.ToOADate());
            fileName.Replace(',', '_');

            if (type == SaveType.Auto)
                slot += 100;
            else if (type == SaveType.Checkpoint)
                slot += 110;
            else if (type == SaveType.Quick)
                slot += 120;

            string path = Path.Combine(SaveSettings.Directory, $"{slot:00}{fileName}.sav");

            globalData.lastSaveFilePath = path;
            SerializationSystem.Serialize(globalData, Path.Combine(SaveSettings.Directory, "GlobalData.sav"));

            if (savePaths.ContainsKey(slot))
            {
                File.Move(savePaths[slot], path);
                savePaths[slot] = path;
            }
            else
                savePaths.Add(slot, path);

            OnPopulateSave?.Invoke();
            SerializationSystem.Serialize(serializebleObjects, path);

            Debug.Log($"Saved files amount: {savePaths.Count()}");
        }

        public static void Load(int slot)
        {
            CheckFiles();
            LoadGlobalData();
            serializebleObjects = SerializationSystem.Deserialize<Dictionary<object, SaveData>>(savePaths[slot]);
            OnLoadFromSaveData?.Invoke();
        }

        public static void Delete(int slot)
        {
            SerializationSystem.DeleteFile(savePaths[slot]);
            OnDeleteSaveData?.Invoke();
        }

        public static int GetSaveFilesAmount => savePaths.Count;

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
    class TestClass : ISaveable
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
            this.AddData(a);
            this.AddData(b);
            this.AddData(c);
            this.AddData(d);
            this.AddData(e);
            this.AddData(f);
            this.AddData(tmp);
        }

        void LoadFromSaveData()
        {
            this.GetData(ref a);
            this.GetData(ref b);
            this.GetData(ref c);
            this.GetData(ref d);
            this.GetData(ref e);
            this.GetData(ref f);
            this.GetData(ref tmp);
        }

        void ISaveable.PopulateSaveData()
        {
            throw new NotImplementedException();
        }

        void ISaveable.LoadFromSaveData()
        {
            throw new NotImplementedException();
        }
    }
}