using System;
using System.IO;
using System.Net.Http;

namespace SpecyfikaDotNet.ResourceManagement
{
    // ❌ BAD: Brak zarządzania zasobami niezarządzanymi
    public class BadFileLogger
    {
        private FileStream _fileStream;

        public BadFileLogger(string logFilePath)
        {
            // FileStream trzyma handle do pliku - zasób niezarządzany
            _fileStream = new FileStream(logFilePath, FileMode.Append);
        }

        public void Log(string message)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(message + Environment.NewLine);
            _fileStream.Write(bytes, 0, bytes.Length);
            _fileStream.Flush();
        }

        // BŁĄD: Brak Dispose() - FileStream nigdy nie zostanie zamknięty
        // Plik pozostanie zablokowany, handle nie zostanie zwolniony
    }

    public class BadHttpClientUsage
    {
        public string FetchData(string url)
        {
            // BŁĄD: Nowa instancja HttpClient przy każdym wywołaniu
            // Prowadzi do wyczerpania portów sieciowych (socket exhaustion)
            var client = new HttpClient();
            var response = client.GetStringAsync(url).Result;
            
            // Brak Dispose - socket nie jest zwalniany natychmiast
            return response;
        }
    }

    public class BadDatabaseConnection
    {
        private readonly string _connectionString;

        public BadDatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void ExecuteQuery(string query)
        {
            var connection = new System.Data.SqlClient.SqlConnection(_connectionString);
            connection.Open();
            
            var command = new System.Data.SqlClient.SqlCommand(query, connection);
            command.ExecuteNonQuery();
            
            // BŁĄD: Connection i Command nie są zwalniane
            // Połączenia DB są ograniczonym zasobem
        }
    }
}
