using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace Heartfield.Serialization
{
    internal struct SettingsSerialization
    {
        [SerializeField] internal string directory;
        [SerializeField] internal bool takeScreenshot;
        [SerializeField] internal bool useScreenshotNativeResolution;
        [SerializeField] internal Vector2Int screenshotResolution;

        [SerializeField] internal bool countTotalPlayedTime;
        [SerializeField] internal bool countWhilePaused;

        const string JSON_PATH = "Packages/com.heartfield-productions.serialization/Runtime/Settings/SaveSettings.json";

#if UNITY_EDITOR
        internal void CreateAsset()
        {
            File.WriteAllText(JSON_PATH, JsonConvert.SerializeObject(this));
            UnityEditor.AssetDatabase.Refresh();
        }
#endif

        internal void LoadAsset()
        {
            if (!File.Exists(JSON_PATH))
                return;

            string text = File.ReadAllText(JSON_PATH);
            this = JsonConvert.DeserializeObject<SettingsSerialization>(text);
        }
    }
}