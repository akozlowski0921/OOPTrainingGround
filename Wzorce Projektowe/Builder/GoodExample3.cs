using System;

namespace DesignPatterns.Builder.Good3
{
    // ✅ GOOD: Fluent builder for database connection
    public class DatabaseConnection
    {
        public string Host { get; private set; }
        public int Port { get; private set; }
        public string Database { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool UseSSL { get; private set; }
        public int ConnectionTimeout { get; private set; }
        public int MaxPoolSize { get; private set; }

        private DatabaseConnection() { }

        public class Builder
        {
            private string _host;
            private int _port = 5432;
            private string _database;
            private string _username;
            private string _password;
            private bool _useSSL = false;
            private int _connectionTimeout = 30;
            private int _maxPoolSize = 10;

            public Builder WithHost(string host)
            {
                _host = host;
                return this;
            }

            public Builder WithPort(int port)
            {
                _port = port;
                return this;
            }

            public Builder WithDatabase(string database)
            {
                _database = database;
                return this;
            }

            public Builder WithCredentials(string username, string password)
            {
                _username = username;
                _password = password;
                return this;
            }

            public Builder WithSSL(bool useSSL)
            {
                _useSSL = useSSL;
                return this;
            }

            public Builder WithConnectionTimeout(int seconds)
            {
                _connectionTimeout = seconds;
                return this;
            }

            public Builder WithMaxPoolSize(int size)
            {
                _maxPoolSize = size;
                return this;
            }

            public DatabaseConnection Build()
            {
                if (string.IsNullOrEmpty(_host))
                    throw new InvalidOperationException("Host is required");
                if (string.IsNullOrEmpty(_database))
                    throw new InvalidOperationException("Database is required");

                return new DatabaseConnection
                {
                    Host = _host,
                    Port = _port,
                    Database = _database,
                    Username = _username,
                    Password = _password,
                    UseSSL = _useSSL,
                    ConnectionTimeout = _connectionTimeout,
                    MaxPoolSize = _maxPoolSize
                };
            }
        }
    }

    // ✅ Clear and flexible construction
    public class Example
    {
        public void CreateConnection()
        {
            var connection = new DatabaseConnection.Builder()
                .WithHost("localhost")
                .WithDatabase("myapp")
                .WithCredentials("admin", "password")
                .WithSSL(true)
                .WithMaxPoolSize(20)
                .Build();
        }
    }
}
