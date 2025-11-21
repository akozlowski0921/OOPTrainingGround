using System;

namespace SOLID.DIP.Bad2
{
    // ❌ BAD: High-level module depends on low-level module
    public class SqlDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"Saving to SQL: {data}");
        }

        public string Load(int id)
        {
            return $"Data from SQL: {id}";
        }
    }

    public class UserService
    {
        // Problem: Direct dependency on concrete implementation
        private SqlDatabase _database = new SqlDatabase();

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
