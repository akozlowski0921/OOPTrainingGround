using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SpecyfikaDotNet.Attributes
{
    // ✅ GOOD: Custom validation attributes - reużywalne, deklaratywne
    
    // Custom attribute dla zakresu wiekowego
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class AgeRangeAttribute : ValidationAttribute
    {
        public int MinAge { get; }
        public int MaxAge { get; }

        public AgeRangeAttribute(int minAge, int maxAge)
        {
            MinAge = minAge;
            MaxAge = maxAge;
            ErrorMessage = $"Age must be between {minAge} and {maxAge}";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is int age)
            {
                if (age >= MinAge && age <= MaxAge)
                {
                    return ValidationResult.Success;
                }
            }

            return new ValidationResult(ErrorMessage);
        }
    }

    // Custom attribute dla siły hasła
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public int MinLength { get; set; } = 8;
        public bool RequireDigit { get; set; } = true;
        public bool RequireUppercase { get; set; } = true;

        public StrongPasswordAttribute()
        {
            ErrorMessage = "Password does not meet strength requirements";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not string password)
            {
                return new ValidationResult("Password must be a string");
            }

            var errors = new List<string>();

            if (password.Length < MinLength)
                errors.Add($"at least {MinLength} characters");

            if (RequireDigit && !password.Any(char.IsDigit))
                errors.Add("at least one digit");

            if (RequireUppercase && !password.Any(char.IsUpper))
                errors.Add("at least one uppercase letter");

            if (errors.Any())
            {
                return new ValidationResult($"Password must contain {string.Join(", ", errors)}");
            }

            return ValidationResult.Success;
        }
    }

    // Custom attribute dla email
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EmailAddressExtendedAttribute : ValidationAttribute
    {
        public string[] AllowedDomains { get; set; }

        public EmailAddressExtendedAttribute()
        {
            ErrorMessage = "Invalid email address";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is not string email)
            {
                return new ValidationResult("Email must be a string");
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            {
                return new ValidationResult(ErrorMessage);
            }

            if (AllowedDomains != null && AllowedDomains.Length > 0)
            {
                string domain = email.Split('@')[1];
                if (!AllowedDomains.Contains(domain))
                {
                    return new ValidationResult($"Email domain must be one of: {string.Join(", ", AllowedDomains)}");
                }
            }

            return ValidationResult.Success;
        }
    }

    // ✅ Model z deklaratywnymi atrybutami walidacji
    public class GoodUserRegistration
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddressExtended(AllowedDomains = new[] { "example.com", "test.com" })]
        public string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string Username { get; set; }

        [Required]
        [AgeRange(18, 120)]
        public int Age { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StrongPassword(MinLength = 8, RequireDigit = true, RequireUppercase = true)]
        public string Password { get; set; }
    }

    public class GoodProductModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }
    }

    // Helper do walidacji
    public static class ValidationHelper
    {
        public static (bool IsValid, List<string> Errors) Validate<T>(T model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            bool isValid = Validator.TryValidateObject(
                model, 
                validationContext, 
                validationResults, 
                validateAllProperties: true);

            var errors = validationResults.Select(r => r.ErrorMessage).ToList();

            return (isValid, errors);
        }
    }

    public class GoodValidationUsage
    {
        public static void Main()
        {
            Console.WriteLine("=== Good Example: Attribute-based validation ===\n");

            var user = new GoodUserRegistration
            {
                Email = "user@invalid.com",
                Username = "ab",
                Age = 15,
                Password = "short"
            };

            // Automatyczna walidacja używając atrybutów
            var (isValid, errors) = ValidationHelper.Validate(user);

            Console.WriteLine($"User validation result: {(isValid ? "Valid" : "Invalid")}");
            if (!isValid)
            {
                Console.WriteLine("Errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"- {error}");
                }
            }

            Console.WriteLine("\n=== Valid user ===");
            var validUser = new GoodUserRegistration
            {
                Email = "john@example.com",
                Username = "john_doe",
                Age = 25,
                Password = "SecurePass123"
            };

            var (isValidUser, userErrors) = ValidationHelper.Validate(validUser);
            Console.WriteLine($"Valid user validation: {(isValidUser ? "Valid" : "Invalid")}");

            Console.WriteLine("\n=== Product validation ===");
            var product = new GoodProductModel
            {
                Name = "",
                Price = -10,
                Stock = -5
            };

            var (isValidProduct, productErrors) = ValidationHelper.Validate(product);
            Console.WriteLine($"Product validation result: {(isValidProduct ? "Valid" : "Invalid")}");
            if (!isValidProduct)
            {
                Console.WriteLine("Errors:");
                foreach (var error in productErrors)
                {
                    Console.WriteLine($"- {error}");
                }
            }

            // Korzyści:
            // ✅ Reużywalne atrybuty (AgeRange, StrongPassword)
            // ✅ Deklaratywne - walidacja widoczna przy property
            // ✅ Łatwe testowanie poszczególnych atrybutów
            // ✅ Automatyczna walidacja przez framework (ASP.NET, EF)
            // ✅ Metadane dostępne przez Reflection
            // ✅ Brak duplikacji kodu
        }
    }
}
