namespace Heartfield.Serialization
{
    public interface ISaveable
    {
        public void OnPopulateSave();
        public void OnLoadFromSave();
    }
}