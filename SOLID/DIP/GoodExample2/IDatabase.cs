namespace SOLID.DIP.Good2
{
    // ✅ Abstraction
    public interface IDatabase
    {
        void Save(string data);
        string Load(int id);
    }
}
