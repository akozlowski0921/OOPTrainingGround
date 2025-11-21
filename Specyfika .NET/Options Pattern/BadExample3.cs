using Microsoft.Extensions.Configuration;
using System;

namespace SpecyfikaDotNet.OptionsPattern.Bad3
{
    // ❌ BAD: Ostatnie problemy z Options Pattern

    // BŁĄD 1: Parsing w każdym użyciu
    public class BadRepeatedParsing
    {
        private readonly IConfiguration _config;

        public BadRepeatedParsing(IConfiguration config)
        {
            _config = config;
        }

        public void Method1()
        {
            var timeout = int.Parse(_config["Timeout"]); // ❌ Parsing
        }

        public void Method2()
        {
            var timeout = int.Parse(_config["Timeout"]); // ❌ Znowu parsing
        }
    }

    // BŁĄD 2: ConnectionString jako plain string
    public class BadConnectionStringHandling
    {
        private readonly string _connectionString;

        public BadConnectionStringHandling(IConfiguration config)
        {
            // ❌ Direct access, brak type safety
            _connectionString = config["ConnectionStrings:Default"];
        }
    }

    // BŁĄD 3: Brak separation of concerns
    public class BadServiceWithConfig
    {
        private readonly IConfiguration _config;

        public BadServiceWithConfig(IConfiguration config)
        {
            _config = config;
        }

        public void DoWork()
        {
            // ❌ Business logic mixed with config access
            var setting1 = _config["Setting1"];
            var setting2 = _config["Setting2"];
            
            // Work with settings
        }
    }
}
