using System;
using System.Data.SqlClient;

namespace SpecyfikaDotNet.DependencyInjection.Bad3
{
    // ❌ BAD: Singleton service z połączeniem do bazy
    public class BadDatabaseService
    {
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public BadDatabaseService(string connectionString)
        {
            // PROBLEM: Współdzielone połączenie między wszystkimi requestami!
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public void BeginTransaction()
        {
            // PROBLEM: Transakcja współdzielona między użytkownikami!
            _transaction = _connection.BeginTransaction();
        }

        public void ExecuteCommand(string sql)
        {
            var command = new SqlCommand(sql, _connection, _transaction);
            command.ExecuteNonQuery();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
        }
    }

    // ❌ BAD: Scoped service z heavy initialization
    public class BadReportGeneratorService
    {
        private byte[] _largeTemplate;
        private ComplexLibrary _library;

        public BadReportGeneratorService()
        {
            // PROBLEM: Kosztowna inicjalizacja wykonywana przy każdym requeście
            _largeTemplate = LoadLargeTemplate(); // 50MB
            _library = new ComplexLibrary(); // Długa inicjalizacja
        }

        private byte[] LoadLargeTemplate()
        {
            // Symulacja ładowania dużego pliku
            return new byte[50 * 1024 * 1024];
        }

        public void GenerateReport()
        {
            // Używa template i library
        }
    }

    public class ComplexLibrary { }
}
