using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Heartfield.Serialization
{
    [Serializable]
    sealed class SaveData
    {
        [JsonProperty] Dictionary<int, object> datas = new Dictionary<int, object>();

        internal void AddData<T>(T data, int id)
        {
            if (datas.ContainsKey(id))
                datas[id] = data;
            else
                datas.Add(id, data);
        }

        internal void AddData<T>(T field)
        {
            int id = field.GetType().MetadataToken;
            AddData(field, id);
        }

        internal T GetData<T>(int id)
        {
            if (datas.ContainsKey(id))
            {
                var jToken = JToken.FromObject(datas[id]);
                return jToken.ToObject<T>();
            }
            else
                throw new NullReferenceException($"data for ({id}) does not exist");
        }

        internal T GetData<T>(T field)
        {
            int id = field.GetType().MetadataToken;
            return GetData<T>(id);
        }
    }
}