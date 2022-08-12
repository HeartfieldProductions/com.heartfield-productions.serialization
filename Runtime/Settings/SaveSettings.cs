using System.IO;
using UnityEngine;

namespace Heartfield.Serialization
{
    internal static class SaveSettings
    {
        static SettingsSerialization json = new SettingsSerialization();

        static SaveSettings()
        {
            LoadAsset();
        }

        internal static void LoadAsset()
        {
            json.LoadAsset();
        }

        internal static string Directory => json.directory;
        internal static bool TakeScreenshot => json.takeScreenshot;
        internal static bool UseScreenshotNativeResolution => json.useScreenshotNativeResolution;
        internal static Vector2Int ScreenshotResolution => json.screenshotResolution;

        internal static bool CountTotalPlayedTime => json.countTotalPlayedTime;
        internal static bool CountWhilePaused => json.countWhilePaused;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <param name="slot">Slot that the file is in</param>
        /// <returns></returns>
        internal static string GetFilePath(string name, int slot) => Path.Combine(Directory, $"{slot:00}{name}.sav");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <returns></returns>
        internal static string GetFilePath(string name) => Path.Combine(Directory, $"{name}.sav");
    }
}