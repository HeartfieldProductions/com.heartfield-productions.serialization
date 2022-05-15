using System.IO;

namespace Heartfield.Serialization
{
    //[CreateAssetMenu(fileName = "Save Data Settings", menuName = "Heartfield Productions/Serialization/Save Data Settings", order = 100)]
    static class Settings
    {
        internal static string path = "";
        internal static string fileExtension = "sav";

        internal static string GetSaveFilePath(string name, int slot) => Path.Combine(path, string.Format("{0}{1:00}.{2}", name, slot, fileExtension));
    }
}