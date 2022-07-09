using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Heartfield.Serialization
{
    public struct SettingsSerialization
    {
        public string directory;
        public bool takeScreenshot;
        public bool useScreenshotNativeResolution;
        public Vector2Int screenshotResolution;

        public bool countTotalPlayedTime;
        public bool countWhilePaused;

        const string JSON_PATH = "Packages/com.heartfield-productions.serialization/Runtime/Settings/SaveSettings.json";

#if UNITY_EDITOR
        public void CreateAsset()
        {
            File.WriteAllText(JSON_PATH, JsonConvert.SerializeObject(this));
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        public void LoadAsset()
        {
            if (!File.Exists(JSON_PATH))
                return;

            string text = File.ReadAllText(JSON_PATH);
            this = JsonConvert.DeserializeObject<SettingsSerialization>(text);
        }
    }
}