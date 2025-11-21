using Microsoft.Extensions.Configuration;
using System;

namespace SpecyfikaDotNet.OptionsPattern.Bad2
{
    // ❌ BAD: Więcej anty-wzorców Options Pattern

    // BŁĄD 1: Hard-coded values
    public class BadHardCodedSettings
    {
        public void SendEmail()
        {
            // ❌ Hard-coded values
            var smtpHost = "smtp.gmail.com";
            var smtpPort = 587;
            
            Console.WriteLine($"Sending via {smtpHost}:{smtpPort}");
        }
    }

    // BŁĄD 2: Mieszanie config sources
    public class BadConfigMixing
    {
        private readonly IConfiguration _config;

        public BadConfigMixing(IConfiguration config)
        {
            _config = config;
        }

        public void Process()
        {
            // ❌ Mieszanie appsettings i environment variables
            var setting1 = _config["AppSettings:Key1"];
            var setting2 = Environment.GetEnvironmentVariable("KEY2");
            
            // Niespójny sposób dostępu do konfiguracji
        }
    }

    // BŁĄD 3: Brak default values
    public class BadNoDefaults
    {
        private readonly IConfiguration _config;

        public BadNoDefaults(IConfiguration config)
        {
            _config = config;
        }

        public int GetTimeout()
        {
            // ❌ Crash jeśli brak w config
            return int.Parse(_config["Timeout"]);
        }
    }

    // BŁĄD 4: Mutable options
    public class BadMutableOptions
    {
        public string ApiKey { get; set; }
        
        public void UpdateKey(string newKey)
        {
            // ❌ Modyfikacja options w runtime
            ApiKey = newKey;
        }
    }
}
