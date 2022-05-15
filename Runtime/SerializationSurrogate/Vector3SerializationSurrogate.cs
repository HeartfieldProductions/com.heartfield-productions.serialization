using UnityEngine;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        readonly string[] _vector = new string[] { "x", "y", "z" };

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 v3 = (Vector3)obj;

            info.AddValue(_vector[0], v3.x);
            info.AddValue(_vector[1], v3.y);
            info.AddValue(_vector[2], v3.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Vector3 v3 = (Vector3)obj;

            v3.x = (float)info.GetValue(_vector[0], typeof(float));
            v3.y = (float)info.GetValue(_vector[1], typeof(float));
            v3.z = (float)info.GetValue(_vector[2], typeof(float));

            obj = v3;

            return obj;
        }
    }
}