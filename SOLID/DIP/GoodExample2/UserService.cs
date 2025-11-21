namespace SOLID.DIP.Good2
{
    // ✅ GOOD: Depends on abstraction
    public class UserService
    {
        private readonly IDatabase _database;

        public UserService(IDatabase database)
        {
            _database = database;
        }

        public void SaveUser(string userData)
        {
            _database.Save(userData);
        }

        public string GetUser(int id)
        {
            return _database.Load(id);
        }
    }
}
