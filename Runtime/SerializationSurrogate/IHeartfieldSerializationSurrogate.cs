using System;
using System.Runtime.Serialization;

namespace Heartfield.Serialization
{
    public interface IHeartfieldSerializationSurrogate : ISerializationSurrogate
    {
        public Type SurrogateType { get; }
    }
}