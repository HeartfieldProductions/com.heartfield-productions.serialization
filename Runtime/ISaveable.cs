namespace Heartfield.SerializableData
{
    public interface ISaveable
    {
        void PopulateSaveData();
        void LoadFromSaveData();
    }
}