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

        internal void AddData<T>(T field)
        {
            int id = field.GetType().MetadataToken;

            if (datas.ContainsKey(id))
                datas[id] = field;
            else
                datas.Add(field.GetType().MetadataToken, field);
        }

        internal T GetData<T>(T field)
        {
            int id = field.GetType().MetadataToken;

            if (datas.ContainsKey(id))
            {
                var jToken = JToken.FromObject(datas[id]);
                return jToken.ToObject<T>();
            }
            else
                throw new NullReferenceException($"data for {field} does not exist");
        }
    }
}