using UnityEngine;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    class TransformSerializationSurrogate : ISerializationSurrogate
    {
        const string p = "position";
        const string r = "rotation";
        const string s = "scale";

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Transform tr = (Transform)obj;

            info.AddValue(p, tr.position, typeof(Vector3));
            info.AddValue(r, tr.rotation, typeof(Quaternion));
            info.AddValue(s, tr.localScale, typeof(Vector3));
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            Transform tr = (Transform)obj;
            Vector3 pos = (Vector3)info.GetValue(p, typeof(Vector3));
            Quaternion rot = (Quaternion)info.GetValue(r, typeof(Quaternion));
            Vector3 scale = (Vector3)info.GetValue(s, typeof(Vector3));

            tr.SetPositionAndRotation(pos, rot);
            tr.localScale = scale;

            obj = tr;
            return obj;
        }
    }
}