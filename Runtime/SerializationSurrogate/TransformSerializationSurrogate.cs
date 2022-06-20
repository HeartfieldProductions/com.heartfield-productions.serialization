using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    sealed class TransformSerializationSurrogate : ISerializationSurrogate
    {
        const string p = "position";
        const string r = "rotation";
        const string s = "scale";

        internal TransformSerializationSurrogate(out Type type)
        {
            type = typeof(Transform);
        }

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var tr = (Transform)obj;

            info.AddValue(p, tr.position, typeof(Vector3));
            info.AddValue(r, tr.rotation, typeof(Quaternion));
            info.AddValue(s, tr.localScale, typeof(Vector3));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var tr = (Transform)obj;
            var pos = (Vector3)info.GetValue(p, typeof(Vector3));
            var rot = (Quaternion)info.GetValue(r, typeof(Quaternion));
            var scale = (Vector3)info.GetValue(s, typeof(Vector3));

            tr.SetPositionAndRotation(pos, rot);
            tr.localScale = scale;

            obj = tr;
            return obj;
        }
    }
}