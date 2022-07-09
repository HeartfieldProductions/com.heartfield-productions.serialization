using System.IO;
using UnityEngine;

namespace Heartfield.Serialization
{
    public static class SaveSettings
    {
        static SettingsSerialization json = new SettingsSerialization();

        static SaveSettings()
        {
            json.LoadAsset();
        }

        public static string Directory => json.directory;
        public static bool TakeScreenshot => json.takeScreenshot;
        public static bool UseScreenshotNativeResolution => json.useScreenshotNativeResolution;
        public static Vector2Int ScreenshotResolution => json.screenshotResolution;

        public static bool CountTotalPlayedTime => json.countTotalPlayedTime;
        public static bool CountWhilePaused => json.countWhilePaused;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <param name="slot">Slot that the file is in</param>
        /// <returns></returns>
        public static string GetFilePath(string name, int slot) => Path.Combine(Directory, $"{slot:00}{name}.sav");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <returns></returns>
        public static string GetFilePath(string name) => Path.Combine(Directory, $"{name}.sav");
    }
}