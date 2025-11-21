using Microsoft.Extensions.Configuration;
using System;

namespace SpecyfikaDotNet.OptionsPattern
{
    // ❌ BAD: Nieprawidłowe zarządzanie konfiguracją

    // BŁĄD 1: Bezpośredni dostęp do IConfiguration w logice biznesowej
    public class BadEmailService
    {
        private readonly IConfiguration _configuration;

        public BadEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string to, string subject, string body)
        {
            // ❌ Magic strings, brak type safety
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];

            // ❌ Co jeśli klucz nie istnieje? NullReferenceException!
            // ❌ Co jeśli parsing się nie powiedzie? FormatException!
            
            Console.WriteLine($"Sending email via {smtpHost}:{smtpPort}");
        }
    }

    // BŁĄD 2: Statyczne ustawienia - niemożliwe do przetestowania
    public static class BadAppSettings
    {
        public static string SmtpHost { get; set; } = "smtp.gmail.com";
        public static int SmtpPort { get; set; } = 587;
        public static string ApiKey { get; set; } = "default-key";
    }

    public class BadNotificationService
    {
        public void SendNotification()
        {
            // ❌ Statyczne ustawienia - coupling, nie testowalne
            var host = BadAppSettings.SmtpHost;
            var port = BadAppSettings.SmtpPort;
            
            Console.WriteLine($"Notification via {host}:{port}");
        }
    }

    // BŁĄD 3: Brak walidacji konfiguracji
    public class BadDatabaseSettings
    {
        public string ConnectionString { get; set; }
        public int CommandTimeout { get; set; }
        public int MaxRetryCount { get; set; }
    }

    public class BadDatabaseService
    {
        private readonly BadDatabaseSettings _settings;

        public BadDatabaseService(BadDatabaseSettings settings)
        {
            _settings = settings;
            // ❌ Brak walidacji - może być null, pusty, nieprawidłowy
        }

        public void Connect()
        {
            // ❌ ConnectionString może być null lub pusty
            Console.WriteLine($"Connecting to: {_settings.ConnectionString}");
        }
    }

    // BŁĄD 4: Pobieranie konfiguracji w każdym wywołaniu
    public class BadApiClient
    {
        private readonly IConfiguration _configuration;

        public BadApiClient(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void MakeRequest()
        {
            // ❌ Parsing przy każdym wywołaniu - performance hit
            var baseUrl = _configuration["ApiSettings:BaseUrl"];
            var timeout = int.Parse(_configuration["ApiSettings:Timeout"]);
            var apiKey = _configuration["ApiSettings:ApiKey"];

            Console.WriteLine($"Request to {baseUrl} with timeout {timeout}");
        }
    }

    // BŁĄD 5: Mieszanie konfiguracji z logiką
    public class BadPaymentProcessor
    {
        private readonly IConfiguration _configuration;

        public BadPaymentProcessor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public decimal ProcessPayment(decimal amount)
        {
            // ❌ Logika biznesowa zależna od IConfiguration
            var commissionRate = decimal.Parse(_configuration["Payment:CommissionRate"]);
            var minAmount = decimal.Parse(_configuration["Payment:MinAmount"]);

            if (amount < minAmount)
                throw new InvalidOperationException("Amount too low");

            return amount * (1 + commissionRate);
        }
    }

    // BŁĄD 6: Brak użycia IOptionsSnapshot dla zmieniającej się konfiguracji
    public class BadFeatureFlagService
    {
        private readonly bool _newFeatureEnabled;

        public BadFeatureFlagService(IConfiguration configuration)
        {
            // ❌ Wartość pobrana raz przy starcie - nie reaguje na zmiany
            _newFeatureEnabled = bool.Parse(configuration["FeatureFlags:NewFeature"]);
        }

        public void UseFeature()
        {
            if (_newFeatureEnabled)
                Console.WriteLine("Using new feature");
        }
    }

    // BŁĄD 7: Brak named options dla wielu konfiguracji tego samego typu
    public class BadMultiTenantService
    {
        private readonly IConfiguration _configuration;

        public BadMultiTenantService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ProcessForTenant(string tenantId)
        {
            // ❌ Manual handling różnych tenantów
            var configSection = $"Tenants:{tenantId}";
            var apiKey = _configuration[$"{configSection}:ApiKey"];
            var baseUrl = _configuration[$"{configSection}:BaseUrl"];

            Console.WriteLine($"Processing for {tenantId}");
        }
    }
}
