using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    [Serializable]
    sealed class SaveDataMaster
    {
        [JsonProperty] internal string path;
        [JsonProperty] byte[] thumbnailBytes;
        [JsonProperty] Vector2Int thumbnailResolution;
        [NonSerialized] Texture2D thumbnail;
        [JsonProperty] int totalPlayedTime;

        [JsonProperty] Dictionary<int, SaveData> datas = new Dictionary<int, SaveData>();

        internal Texture2D GetThumbnail()
        {
            if (thumbnail == null)
            {
                thumbnail = new Texture2D(thumbnailResolution.x, thumbnailResolution.y);
                thumbnail.LoadRawTextureData(thumbnailBytes);
            }

            return thumbnail;
        }

        internal int GetTotalPlayedTime => totalPlayedTime;

        internal void AddData(KeyValuePair<int, SaveData> data)
        {
            var key = data.Key;
            var value = data.Value;

            if (datas.ContainsKey(key))
                datas[key] = value;
            else
                datas.Add(key, value);
        }

        internal SaveData GetData(int id)
        {
            if (datas.ContainsKey(id))
            {
                return datas[id];
            }
            else
                throw new NullReferenceException($"({id}) does not exist in this file");
        }
    }
}