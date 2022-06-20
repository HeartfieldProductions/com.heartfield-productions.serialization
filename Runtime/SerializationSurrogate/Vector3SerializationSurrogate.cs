using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    #region Vector3
    sealed class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        readonly string[] _vector = new string[] { "x", "y", "z" };

        internal Vector3SerializationSurrogate(out Type type)
        {
            type = typeof(Vector3);
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var v3 = (Vector3)obj;

            info.AddValue(_vector[0], v3.x);
            info.AddValue(_vector[1], v3.y);
            info.AddValue(_vector[2], v3.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var v3 = (Vector3)obj;

            v3.x = (float)info.GetValue(_vector[0], typeof(float));
            v3.y = (float)info.GetValue(_vector[1], typeof(float));
            v3.z = (float)info.GetValue(_vector[2], typeof(float));

            obj = v3;

            return obj;
        }
    }
    #endregion

    #region Vector2
    sealed class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        readonly string[] _vector = new string[] { "x", "y" };

        internal Vector2SerializationSurrogate(out Type type)
        {
            type = typeof(Vector2);
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var v2 = (Vector2)obj;

            info.AddValue(_vector[0], v2.x);
            info.AddValue(_vector[1], v2.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var v2 = (Vector2)obj;

            v2.x = (float)info.GetValue(_vector[0], typeof(float));
            v2.y = (float)info.GetValue(_vector[1], typeof(float));

            obj = v2;

            return obj;
        }
    }
    #endregion

    #region Vector4
    sealed class Vector4SerializationSurrogate : ISerializationSurrogate
    {
        readonly string[] _vector = new string[] { "x", "y", "z", "w" };

        internal Vector4SerializationSurrogate(out Type type)
        {
            type = typeof(Vector4);
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var v4 = (Vector4)obj;

            info.AddValue(_vector[0], v4.x);
            info.AddValue(_vector[1], v4.y);
            info.AddValue(_vector[2], v4.z);
            info.AddValue(_vector[3], v4.w);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var v4 = (Vector4)obj;

            v4.x = (float)info.GetValue(_vector[0], typeof(float));
            v4.y = (float)info.GetValue(_vector[1], typeof(float));
            v4.z = (float)info.GetValue(_vector[2], typeof(float));
            v4.w = (float)info.GetValue(_vector[3], typeof(float));

            obj = v4;

            return obj;
        }
    }
    #endregion
}