using System;
using System.IO;
using System.Net.Http;

namespace SpecyfikaDotNet.ResourceManagement
{
    // ✅ GOOD: Prawidłowa implementacja IDisposable
    public class GoodFileLogger : IDisposable
    {
        private FileStream _fileStream;
        private bool _disposed = false;

        public GoodFileLogger(string logFilePath)
        {
            _fileStream = new FileStream(logFilePath, FileMode.Append);
        }

        public void Log(string message)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(GoodFileLogger));

            var bytes = System.Text.Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); // Zapobiega wywołaniu finalizera
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Zwalnianie managed resources
                _fileStream?.Dispose();
            }

            // Tutaj zwalniamy unmanaged resources (jeśli są)
            
            _disposed = true;
        }

        ~GoodFileLogger()
        {
            // Finalizer jako safety net
            Dispose(false);
        }
    }

    public class GoodHttpClientUsage
    {
        // Statyczny HttpClient współdzielony - zalecane podejście
        private static readonly HttpClient _sharedClient = new HttpClient();

        public string FetchData(string url)
        {
            // Reużywamy tego samego klienta - nie wyczerpujemy portów
            return _sharedClient.GetStringAsync(url).Result;
        }

        // Alternatywnie: IHttpClientFactory w ASP.NET Core
    }

    public class GoodDatabaseConnection
    {
        private readonly string _connectionString;

        public GoodDatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            // using zapewnia wywołanie Dispose() nawet w przypadku wyjątku
            using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                connection.Open();
                
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                // Command.Dispose() wywoływane automatycznie
            }
            // Connection.Dispose() wywoływane automatycznie
        }

        // C# 8+ using declaration - jeszcze krótsza składnia
        public void ExecuteQueryModern(string query)
        {
            using var connection = new System.Data.SqlClient.SqlConnection(_connectionString);
            connection.Open();
            
            using var command = new System.Data.SqlClient.SqlCommand(query, connection);
            command.ExecuteNonQuery();
            
            // Dispose wywoływane na końcu scope'a metody
        }
    }
}
