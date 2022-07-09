using System;
using System.IO;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Heartfield.Serialization
{
    public static class SerializationSystem
    {
        static readonly byte[] KEY_BYTES = { 0x02, 0x03, 0x01, 0x03, 0x03, 0x07, 0x07, 0x08, 0x09, 0x09, 0x11, 0x11, 0x16, 0x17, 0x19, 0x16 };

        static void CheckDirectory(string path)
        {
            string directory = Directory.GetParent(path).FullName;

            if (Directory.Exists(directory))
                return;

            Directory.CreateDirectory(directory);
        }

        public static SerializedFileInfo Serialize<T>(T data, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path should not be null or empty");

            CheckDirectory(path);

            try
            {
                using var algorithm = Rijndael.Create();
                algorithm.Key = KEY_BYTES;

                var encryptor = algorithm.CreateEncryptor();

                using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
                using var cryptStream = new CryptoStream(fileStream, encryptor, CryptoStreamMode.Write);
                using var streamWriter = new StreamWriter(cryptStream);
                fileStream.Write(algorithm.IV, 0, algorithm.IV.Length);
                string jsonData = JsonConvert.SerializeObject(data);
                streamWriter.Write(jsonData);
            }
            catch
            {
                throw;
            }

//#if UNITY_EDITOR
//            Debug.Log($"{Path.GetFileName(path)} saved succesfully at: {Directory.GetParent(path)}");
//#endif

            var info = new SerializedFileInfo()
            {
                newFile = true,
                overrided = false
            };

            return info;
        }

        public static T Deserialize<T>(string path)
        {
            CheckDirectory(path);

            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} doesn't exist");

            try
            {
                var decodedData = string.Empty;

                using (var algorithm = Rijndael.Create())
                {
                    algorithm.Key = KEY_BYTES;

                    using var fileStream = new FileStream(path, FileMode.Open);
                    byte[] iv = new byte[16];
                    fileStream.Read(iv, 0, iv.Length);

                    algorithm.IV = iv;

                    var decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV);

                    using var cryptStream = new CryptoStream(fileStream, decryptor, CryptoStreamMode.Read);
                    using var streamReader = new StreamReader(cryptStream);
                    decodedData = streamReader.ReadToEnd();
                }

//#if UNITY_EDITOR
//                Debug.Log($"{Path.GetFileName(path)} loaded succesfully");
//#endif

                return JsonConvert.DeserializeObject<T>(decodedData);
            }
            catch
            {
                throw;
            }
        }

        internal static void DeleteFile(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException($"{path} doesn't exist");

            File.Delete(path);
        }
    }
}