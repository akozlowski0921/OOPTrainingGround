using System;
using System.Collections.Generic;

namespace DesignPatterns.Singleton.Good3
{
    // ✅ GOOD: Singleton connection pool
    public sealed class DatabaseConnectionPool
    {
        private static readonly Lazy<DatabaseConnectionPool> _instance =
            new Lazy<DatabaseConnectionPool>(() => new DatabaseConnectionPool());

        private readonly int _maxConnections = 10;
        private readonly Queue<Connection> _availableConnections;
        private readonly object _lock = new object();

        private DatabaseConnectionPool()
        {
            _availableConnections = new Queue<Connection>();
            for (int i = 0; i < _maxConnections; i++)
            {
                _availableConnections.Enqueue(new Connection());
            }
        }

        public static DatabaseConnectionPool Instance => _instance.Value;

        public Connection GetConnection()
        {
            lock (_lock)
            {
                if (_availableConnections.Count > 0)
                {
                    return _availableConnections.Dequeue();
                }
                throw new InvalidOperationException("No connections available");
            }
        }

        public void ReleaseConnection(Connection conn)
        {
            lock (_lock)
            {
                _availableConnections.Enqueue(conn);
            }
        }
    }

    public class Connection { }

    // ✅ Single pool manages all connections
    public class Example
    {
        public void Run()
        {
            var pool1 = DatabaseConnectionPool.Instance;
            var pool2 = DatabaseConnectionPool.Instance;

            // Same instance
            Console.WriteLine(ReferenceEquals(pool1, pool2)); // True

            // Only 10 connections total
            var connections = new List<Connection>();
            for (int i = 0; i < 10; i++)
            {
                connections.Add(pool1.GetConnection());
            }

            // This would throw - no more connections
            // pool2.GetConnection();
        }
    }
}
