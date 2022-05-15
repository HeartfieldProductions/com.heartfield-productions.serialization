using System;

namespace Heartfield.SerializableData
{
    [Serializable]
    public class SaveData
    {
        object _data;
        Type _type;

        internal object data => _data;
        internal Type type => _type;

        //internal SaveData(ISaveable data)
        //{
        //    _data = data.PopulateSaveData();
        //    _type = data.GetType();
        //}
    }
}