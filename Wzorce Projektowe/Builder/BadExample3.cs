using System;
using System.Collections.Generic;

namespace DesignPatterns.Builder.Bad3
{
    // ❌ BAD: Multiple constructors for different configurations
    public class DatabaseConnection
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MaxPoolSize { get; set; }

        // Problem: Explosion of constructors
        public DatabaseConnection(string host, string database)
        {
            Host = host;
            Port = 5432;
            Database = database;
            UseSSL = false;
            ConnectionTimeout = 30;
            MaxPoolSize = 10;
        }

        public DatabaseConnection(string host, int port, string database)
        {
            Host = host;
            Port = port;
            Database = database;
            UseSSL = false;
            ConnectionTimeout = 30;
            MaxPoolSize = 10;
        }

        public DatabaseConnection(string host, string database, string username, string password)
        {
            Host = host;
            Port = 5432;
            Database = database;
            Username = username;
            Password = password;
            UseSSL = false;
            ConnectionTimeout = 30;
            MaxPoolSize = 10;
        }

        public DatabaseConnection(string host, int port, string database, 
            string username, string password, bool useSSL)
        {
            Host = host;
            Port = port;
            Database = database;
            Username = username;
            Password = password;
            UseSSL = useSSL;
            ConnectionTimeout = 30;
            MaxPoolSize = 10;
        }

        // Need even more constructors for other combinations!
    }
}
