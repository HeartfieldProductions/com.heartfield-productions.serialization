using System.IO;

namespace Heartfield.Serialization
{
    static class SaveSettings
    {
        internal static string path = "Saves";
        internal static string fileExtension = "sav";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <param name="slot">Slot that the file is in</param>
        /// <returns></returns>
        internal static string GetFilePath(string name, int slot) => $"{name}{slot:00}.{fileExtension}";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Name of the save file</param>
        /// <param name="slot">Slot that the file is in</param>
        /// <returns></returns>
        internal static string GetFullFilePath(string name, int slot) => Path.Combine(path, GetFilePath(name, slot));
    }
}