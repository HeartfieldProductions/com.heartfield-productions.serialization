using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    sealed class QuaternionSerializationSurrogate : ISerializationSurrogate
    {
        readonly string[] a = new string[] { "x", "y", "z", "w" };

        internal QuaternionSerializationSurrogate(out Type type)
        {
            type = typeof(Quaternion);
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var qt = (Quaternion)obj;

            info.AddValue(a[0], qt.x);
            info.AddValue(a[1], qt.y);
            info.AddValue(a[2], qt.z);
            info.AddValue(a[3], qt.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var qt = (Quaternion)obj;

            qt.x = (float)info.GetValue(a[0], typeof(float));
            qt.y = (float)info.GetValue(a[1], typeof(float));
            qt.z = (float)info.GetValue(a[2], typeof(float));
            qt.w = (float)info.GetValue(a[3], typeof(float));

            obj = qt;

            return obj;
        }
    }
}