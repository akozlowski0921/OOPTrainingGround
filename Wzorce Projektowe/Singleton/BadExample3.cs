using System;

namespace DesignPatterns.Singleton.Bad3
{
    // ❌ BAD: Database connection pool without singleton
    public class DatabaseConnectionPool
    {
        private int _maxConnections = 10;
        private int _currentConnections = 0;

        public Connection GetConnection()
        {
            if (_currentConnections < _maxConnections)
            {
                _currentConnections++;
                return new Connection();
            }
            throw new InvalidOperationException("No connections available");
        }

        public void ReleaseConnection(Connection conn)
        {
            _currentConnections--;
        }
    }

    public class Connection { }

    // Problem: Multiple pools defeating the purpose
    public class Example
    {
        public void Run()
        {
            var pool1 = new DatabaseConnectionPool();
            var pool2 = new DatabaseConnectionPool();

            // Problem: Each pool thinks it has 10 connections available
            // But we might exceed database limit!
            for (int i = 0; i < 10; i++)
                pool1.GetConnection();

            for (int i = 0; i < 10; i++)
                pool2.GetConnection(); // 20 connections total!
        }
    }
}
