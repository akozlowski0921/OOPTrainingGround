using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace SpecyfikaDotNet.OptionsPattern
{
    // ✅ GOOD: Options Pattern z walidacją

    // ✅ Strongly-typed configuration class
    public class EmailOptions
    {
        public const string SectionName = "Email";

        [Required]
        public string SmtpHost { get; set; } = string.Empty;

        [Range(1, 65535)]
        public int SmtpPort { get; set; }

        [Required]
        [EmailAddress]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        public bool EnableSsl { get; set; } = true;
    }

    // ✅ Service używa IOptions<T>
    public class GoodEmailService
    {
        private readonly EmailOptions _options;

        public GoodEmailService(IOptions<EmailOptions> options)
        {
            // Wartości pobrane raz i cached
            _options = options.Value;
        }

        public void SendEmail(string to, string subject, string body)
        {
            // ✅ Type-safe, brak magic strings
            Console.WriteLine($"Sending email via {_options.SmtpHost}:{_options.SmtpPort}");
            Console.WriteLine($"SSL: {_options.EnableSsl}");
        }
    }

    // ✅ Custom validation
    public class DatabaseOptions : IValidatableObject
    {
        public const string SectionName = "Database";

        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        [Range(1, 300)]
        public int CommandTimeout { get; set; } = 30;

        [Range(0, 10)]
        public int MaxRetryCount { get; set; } = 3;

        // ✅ Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                yield return new ValidationResult(
                    "ConnectionString cannot be empty",
                    new[] { nameof(ConnectionString) });
            }

            if (CommandTimeout < 1)
            {
                yield return new ValidationResult(
                    "CommandTimeout must be positive",
                    new[] { nameof(CommandTimeout) });
            }
        }
    }

    // ✅ Service z validated options
    public class GoodDatabaseService
    {
        private readonly DatabaseOptions _options;

        public GoodDatabaseService(IOptions<DatabaseOptions> options)
        {
            // Walidacja wykonana przy resolved przez DI
            _options = options.Value;
        }

        public void Connect()
        {
            // ✅ Gwarantujemy że _options jest valid
            Console.WriteLine($"Connecting to: {_options.ConnectionString}");
            Console.WriteLine($"Timeout: {_options.CommandTimeout}s");
        }
    }

    // ✅ IOptionsSnapshot dla hot-reload configuration
    public class FeatureFlagOptions
    {
        public const string SectionName = "FeatureFlags";

        public bool NewFeatureEnabled { get; set; }
        public bool BetaFeaturesEnabled { get; set; }
    }

    public class GoodFeatureFlagService
    {
        private readonly IOptionsSnapshot<FeatureFlagOptions> _options;

        // ✅ IOptionsSnapshot - reloaded per request
        public GoodFeatureFlagService(IOptionsSnapshot<FeatureFlagOptions> options)
        {
            _options = options;
        }

        public void UseFeature()
        {
            // ✅ Wartość odczytana z aktualnej konfiguracji
            if (_options.Value.NewFeatureEnabled)
                Console.WriteLine("Using new feature");
        }
    }

    // ✅ Named options dla multiple configurations
    public class TenantOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public int RateLimit { get; set; }
    }

    public class GoodMultiTenantService
    {
        private readonly IOptionsSnapshot<TenantOptions> _options;

        public GoodMultiTenantService(IOptionsSnapshot<TenantOptions> options)
        {
            _options = options;
        }

        public void ProcessForTenant(string tenantId)
        {
            // ✅ Named options - pobieramy konfigurację dla konkretnego tenanta
            var tenantOptions = _options.Get(tenantId);
            
            Console.WriteLine($"Processing for {tenantId}");
            Console.WriteLine($"API Key: {tenantOptions.ApiKey}");
            Console.WriteLine($"Rate limit: {tenantOptions.RateLimit}");
        }
    }

    // ✅ IOptionsMonitor dla real-time changes
    public class ApiOptions
    {
        public const string SectionName = "ApiSettings";

        public string BaseUrl { get; set; } = string.Empty;
        public int Timeout { get; set; }
        public string ApiKey { get; set; } = string.Empty;
    }

    public class GoodApiClient
    {
        private readonly IOptionsMonitor<ApiOptions> _options;

        // ✅ IOptionsMonitor - notyfikacja o zmianach
        public GoodApiClient(IOptionsMonitor<ApiOptions> options)
        {
            _options = options;

            // ✅ Subscribe to changes
            _options.OnChange(newOptions =>
            {
                Console.WriteLine($"Configuration changed: {newOptions.BaseUrl}");
            });
        }

        public void MakeRequest()
        {
            // ✅ CurrentValue zawsze aktualne
            var options = _options.CurrentValue;
            Console.WriteLine($"Request to {options.BaseUrl} with timeout {options.Timeout}");
        }
    }

    // ✅ Post-configure dla dodatkowej konfiguracji
    public class PaymentOptions
    {
        public const string SectionName = "Payment";

        public decimal CommissionRate { get; set; }
        public decimal MinAmount { get; set; }
        public decimal MaxAmount { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public class GoodPaymentProcessor
    {
        private readonly PaymentOptions _options;

        public GoodPaymentProcessor(IOptions<PaymentOptions> options)
        {
            _options = options.Value;
        }

        public decimal ProcessPayment(decimal amount)
        {
            // ✅ Logika biznesowa oddzielona od konfiguracji
            if (amount < _options.MinAmount)
                throw new InvalidOperationException($"Amount must be at least {_options.MinAmount} {_options.Currency}");

            if (amount > _options.MaxAmount)
                throw new InvalidOperationException($"Amount cannot exceed {_options.MaxAmount} {_options.Currency}");

            return amount * (1 + _options.CommissionRate);
        }
    }

    // ✅ Dependency Injection setup
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ✅ Basic options
            services.Configure<EmailOptions>(
                configuration.GetSection(EmailOptions.SectionName));

            // ✅ Options z walidacją
            services.Configure<DatabaseOptions>(
                configuration.GetSection(DatabaseOptions.SectionName));
            services.AddSingleton<IValidateOptions<DatabaseOptions>, 
                DataAnnotationsValidateOptions<DatabaseOptions>>();

            // ✅ Options snapshot dla hot-reload
            services.Configure<FeatureFlagOptions>(
                configuration.GetSection(FeatureFlagOptions.SectionName));

            // ✅ Options monitor dla real-time changes
            services.Configure<ApiOptions>(
                configuration.GetSection(ApiOptions.SectionName));

            // ✅ Named options
            services.Configure<TenantOptions>("Tenant1",
                configuration.GetSection("Tenants:Tenant1"));
            services.Configure<TenantOptions>("Tenant2",
                configuration.GetSection("Tenants:Tenant2"));

            // ✅ Post-configure
            services.PostConfigure<PaymentOptions>(options =>
            {
                // Możemy modyfikować opcje po załadowaniu z konfiguracji
                if (options.MaxAmount == 0)
                    options.MaxAmount = 10000;
            });

            return services;
        }
    }

    // ✅ Eager validation przy starcie aplikacji
    public static class OptionsValidation
    {
        public static void ValidateOptionsOnStartup(IServiceProvider serviceProvider)
        {
            // ✅ Walidacja wszystkich options przy starcie
            var emailOptions = serviceProvider.GetRequiredService<IOptions<EmailOptions>>().Value;
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            
            // Jeśli validation attributes są nieprawidłowe, rzuci wyjątek
            Console.WriteLine("All options validated successfully");
        }
    }
}
