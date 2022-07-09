using System;
using UnityEngine;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    [Serializable]
    public sealed class SaveData
    {
        Dictionary<int, object> data = new Dictionary<int, object>();
        const string idKey = "lastsaveddataid";
        int id = 0;

        internal byte[] thumbnail;
        internal float totalPlayedTime;

        internal SaveData()
        {
            id = PlayerPrefs.GetInt(idKey);
            PlayerPrefs.SetInt(idKey, id + 1);
        }

        internal Texture2D GetThumbnail(int width, int height)
        {
            var tex = new Texture2D(width, height);
            tex.LoadRawTextureData(thumbnail);
            return tex;
        }

        internal int GetID => id;

        internal int GetDataIdentifier<T>(T data) => data.GetType().MetadataToken;

        internal void Add<T>(T data)
        {
            int id = GetDataIdentifier(data);

            Debug.Log(id);

            if (!this.data.ContainsKey(id))
                this.data.Add(id, data);
            else
                this.data[id] = data;
        }

        internal T Get<T>(T data)
        {
            int id = GetDataIdentifier(data);
            Debug.Log(id);
            return (T)this.data[id];
        }

        internal void Reset()
        {
            data.Clear();
            PlayerPrefs.DeleteKey(idKey);
        }
    }
}