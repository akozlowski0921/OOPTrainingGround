using System;

namespace SOLID.DIP.Good2
{
    public class SqlDatabase : IDatabase
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
}
