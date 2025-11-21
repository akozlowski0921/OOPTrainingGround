using System;
using System.Collections.Generic;

namespace SpecyfikaDotNet.Attributes
{
    // ❌ BAD: Walidacja bez atrybutów - duplikacja kodu, trudna w utrzymaniu
    public class BadUserRegistration
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public int Age { get; set; }
        public string Password { get; set; }

        // Walidacja w osobnej metodzie - trzeba pamiętać o jej wywołaniu
        public List<string> Validate()
        {
            var errors = new List<string>();

            // Ręczna walidacja każdego pola
            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("Email is required");
            else if (!Email.Contains("@"))
                errors.Add("Email must contain @");

            if (string.IsNullOrWhiteSpace(Username))
                errors.Add("Username is required");
            else if (Username.Length < 3)
                errors.Add("Username must be at least 3 characters");

            if (Age < 18)
                errors.Add("Age must be at least 18");
            else if (Age > 120)
                errors.Add("Age must be less than 120");

            if (string.IsNullOrWhiteSpace(Password))
                errors.Add("Password is required");
            else if (Password.Length < 8)
                errors.Add("Password must be at least 8 characters");

            return errors;
        }
    }

    public class BadProductModel
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // Duplikacja logiki walidacji
        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required");

            if (Price <= 0)
                errors.Add("Price must be greater than 0");

            if (Stock < 0)
                errors.Add("Stock cannot be negative");

            return errors;
        }
    }

    public class BadValidationUsage
    {
        public static void Main()
        {
            var user = new BadUserRegistration
            {
                Email = "invalid-email",
                Username = "ab",
                Age = 15,
                Password = "short"
            };

            // Trzeba pamiętać o wywołaniu Validate
            var errors = user.Validate();

            Console.WriteLine("Validation errors:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }

            // Problemy:
            // - Duplikacja kodu walidacji w każdej klasie
            // - Brak reużywalności reguł walidacji
            // - Trudne testowanie poszczególnych reguł
            // - Łatwo zapomnieć o wywołaniu Validate
            // - Nie ma automatycznej walidacji
            // - Brak metadanych o regułach walidacji

            var product = new BadProductModel
            {
                Name = "",
                Price = -10,
                Stock = -5
            };

            // Znowu trzeba pamiętać o wywołaniu
            var productErrors = product.Validate();
            Console.WriteLine("\nProduct validation errors:");
            foreach (var error in productErrors)
            {
                Console.WriteLine($"- {error}");
            }
        }
    }
}
