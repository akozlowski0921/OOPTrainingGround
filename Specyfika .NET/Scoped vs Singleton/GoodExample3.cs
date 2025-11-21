using System;
using System.Data.SqlClient;

namespace SpecyfikaDotNet.DependencyInjection.Good3
{
    // ✅ GOOD: Scoped service z własnym połączeniem na request
    public class GoodDatabaseService
    {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public GoodDatabaseService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private void EnsureConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
        }

        public void BeginTransaction()
        {
            EnsureConnection();
            _transaction = _connection.BeginTransaction();
        }

        public void ExecuteCommand(string sql)
        {
            EnsureConnection();
            using var command = new SqlCommand(sql, _connection, _transaction);
            command.ExecuteNonQuery();
        }

        public void CommitTransaction()
        {
            _transaction?.Commit();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }
    }

    // ✅ GOOD: Singleton dla heavy resources, Scoped dla per-request logic
    public class ReportTemplateCache
    {
        private readonly byte[] _largeTemplate;
        private readonly ComplexLibrary _library;

        public ReportTemplateCache()
        {
            // Inicjalizacja raz przy starcie aplikacji
            _largeTemplate = LoadLargeTemplate();
            _library = new ComplexLibrary();
        }

        private byte[] LoadLargeTemplate()
        {
            return new byte[50 * 1024 * 1024];
        }

        public byte[] GetTemplate() => _largeTemplate;
        public ComplexLibrary GetLibrary() => _library;
    }

    public class GoodReportGeneratorService
    {
        private readonly ReportTemplateCache _cache;
        private string _userSpecificData;

        public GoodReportGeneratorService(ReportTemplateCache cache)
        {
            // Otrzymuje Singleton cache - nie trzeba każdorazowo ładować
            _cache = cache;
        }

        public void SetUserData(string data)
        {
            _userSpecificData = data;
        }

        public void GenerateReport()
        {
            // Używa cached template i library
            var template = _cache.GetTemplate();
            var library = _cache.GetLibrary();
            // ... generowanie raportu z user-specific data
        }
    }

    public class ComplexLibrary { }

    // Konfiguracja DI
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Singleton - heavy resources, loaded once
            services.AddSingleton<ReportTemplateCache>();

            // Scoped - per-request logic with user data
            services.AddScoped<GoodDatabaseService>();
            services.AddScoped<GoodReportGeneratorService>();
        }
    }

    public interface IServiceCollection
    {
        void AddScoped<T>() where T : class;
        void AddSingleton<T>() where T : class;
    }

    public static class ServiceCollectionExtensions
    {
        public static void AddScoped<T>(this IServiceCollection services) where T : class { }
        public static void AddSingleton<T>(this IServiceCollection services) where T : class { }
    }
}
