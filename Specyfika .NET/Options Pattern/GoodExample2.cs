using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace SpecyfikaDotNet.OptionsPattern.Good2
{
    // ✅ GOOD: Advanced Options Pattern

    // ✅ Options z default values
    public class EmailOptions
    {
        public const string SectionName = "Email";

        [Required]
        public string SmtpHost { get; set; } = "smtp.gmail.com";
        
        [Range(1, 65535)]
        public int SmtpPort { get; set; } = 587;
        
        public bool EnableSsl { get; set; } = true;
    }

    // ✅ Strongly-typed service
    public class GoodEmailService
    {
        private readonly EmailOptions _options;

        public GoodEmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public void SendEmail()
        {
            Console.WriteLine($"Sending via {_options.SmtpHost}:{_options.SmtpPort}");
        }
    }

    // ✅ Named options dla multiple configurations
    public class GoodMultiTenantService
    {
        private readonly IOptionsSnapshot<TenantOptions> _options;

        public GoodMultiTenantService(IOptionsSnapshot<TenantOptions> options)
        {
            _options = options;
        }

        public void ProcessForTenant(string tenantId)
        {
            var tenantOptions = _options.Get(tenantId);
            Console.WriteLine($"Processing for {tenantId}: {tenantOptions.ApiKey}");
        }
    }

    public class TenantOptions
    {
        public string ApiKey { get; set; }
        public int RateLimit { get; set; }
    }

    // ✅ Validation z IValidateOptions
    public class EmailOptionsValidator : IValidateOptions<EmailOptions>
    {
        public ValidateOptionsResult Validate(string name, EmailOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.SmtpHost))
                return ValidateOptionsResult.Fail("SmtpHost is required");

            if (options.SmtpPort < 1 || options.SmtpPort > 65535)
                return ValidateOptionsResult.Fail("Invalid port");

            return ValidateOptionsResult.Success;
        }
    }
}
