namespace Project.Core.SaveSystem
{
    public interface ISaveable
    {
        void SaveState(SaveData saveData);
        void LoadState(SaveData saveData);
    }
}
