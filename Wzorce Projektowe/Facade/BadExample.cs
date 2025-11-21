using System;

namespace DesignPatterns.Facade
{
    // ❌ BAD: Client musi znać wszystkie subsystemy

    // Complex subsystems
    public class DatabaseService
    {
        public void Connect() => Console.WriteLine("DB Connected");
        public void Disconnect() => Console.WriteLine("DB Disconnected");
        public object Query(string sql) => "data";
    }

    public class CacheService
    {
        public void Initialize() => Console.WriteLine("Cache init");
        public object Get(string key) => null;
        public void Set(string key, object value) => Console.WriteLine("Cache set");
    }

    public class LoggingService
    {
        public void Initialize() => Console.WriteLine("Logger init");
        public void Log(string message) => Console.WriteLine($"Log: {message}");
    }

    // ❌ Client musi zarządzać wszystkimi subsystemami
    public class BadClient
    {
        public void GetUserData(int userId)
        {
            // ❌ Złożona orkiestracja wielu subsystemów
            var cache = new CacheService();
            cache.Initialize();
            
            var cachedData = cache.Get($"user_{userId}");
            
            if (cachedData == null)
            {
                var db = new DatabaseService();
                db.Connect();
                var data = db.Query($"SELECT * FROM Users WHERE Id={userId}");
                db.Disconnect();
                
                cache.Set($"user_{userId}", data);
                
                var logger = new LoggingService();
                logger.Initialize();
                logger.Log($"User {userId} loaded from DB");
            }
            
            // ❌ Client musi znać szczegóły implementacji
            // ❌ Duplikacja logiki w każdym miejscu użycia
        }
    }
}
