using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SpecyfikaDotNet.OptionsPattern.Good3
{
    // ✅ GOOD: Best practices Options Pattern

    // ✅ Cached options - parsed once
    public class GoodCachedOptions
    {
        public class AppSettings
        {
            public int Timeout { get; set; }
            public string ApiKey { get; set; }
        }

        private readonly AppSettings _settings;

        public GoodCachedOptions(IOptions<AppSettings> options)
        {
            _settings = options.Value; // Cached
        }

        public void Method1()
        {
            var timeout = _settings.Timeout; // No parsing
        }

        public void Method2()
        {
            var timeout = _settings.Timeout; // Same cached value
        }
    }

    // ✅ Strongly-typed connection strings
    public class ConnectionStringOptions
    {
        public const string SectionName = "ConnectionStrings";

        public string Default { get; set; }
        public string ReadOnly { get; set; }
    }

    public class GoodDatabaseService
    {
        private readonly string _connectionString;

        public GoodDatabaseService(IOptions<ConnectionStringOptions> options)
        {
            _connectionString = options.Value.Default;
        }

        public void Connect()
        {
            Console.WriteLine($"Connecting to: {_connectionString}");
        }
    }

    // ✅ Clean separation of concerns
    public class GoodBusinessService
    {
        private readonly BusinessOptions _options;

        public GoodBusinessService(IOptions<BusinessOptions> options)
        {
            _options = options.Value;
        }

        public void DoWork()
        {
            // ✅ Business logic, options already parsed
            var setting1 = _options.Setting1;
            var setting2 = _options.Setting2;
            
            // Work with strongly-typed settings
        }
    }

    public class BusinessOptions
    {
        public string Setting1 { get; set; }
        public string Setting2 { get; set; }
    }

    // ✅ PostConfigure dla runtime modifications
    public static class OptionsSetup
    {
        public static IServiceCollection AddBusinessOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<BusinessOptions>(
                configuration.GetSection("Business"));

            services.PostConfigure<BusinessOptions>(options =>
            {
                // Modyfikacja po załadowaniu
                if (string.IsNullOrEmpty(options.Setting1))
                    options.Setting1 = "default";
            });

            return services;
        }
    }
}
