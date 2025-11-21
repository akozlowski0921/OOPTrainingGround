using System;

namespace SOLID.DIP.Good2
{
    public class MongoDatabase : IDatabase
    {
        public void Save(string data)
        {
            Console.WriteLine($"Saving to MongoDB: {data}");
        }

        public string Load(int id)
        {
            return $"Data from MongoDB: {id}";
        }
    }
}
