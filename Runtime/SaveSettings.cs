using System.IO;

namespace Heartfield.Serialization
{
    internal static class SaveSettings
    {
        internal static string directory = "Saves";
        internal static DisplayNameMode displayNameMode = DisplayNameMode.Name_DD_MM_YYYY_hh_mm_ss;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <param name="slot">Slot that the file is in</param>
        /// <returns></returns>
        internal static string GetFilePath(string name, int slot) => Path.Combine(directory, $"{name}{slot:00}.sav");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <returns></returns>
        internal static string GetFilePath(string name) => Path.Combine(directory, $"{name}.sav");
    }
}