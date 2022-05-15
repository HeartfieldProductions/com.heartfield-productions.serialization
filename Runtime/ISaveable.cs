namespace Heartfield.Serialization
{
    public interface ISaveable
    {
        void PopulateSaveData();
        void LoadFromSaveData();
    }
}