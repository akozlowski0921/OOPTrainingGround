using System;
using System.Collections.Generic;

namespace CleanCodeFundamentals.DRY.Bad3
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }

    // Problem: Ta sama logika walidacji email jest powielona w różnych miejscach
    public class UserRegistrationService
    {
        public bool RegisterUser(User user)
        {
            // Kopia #1 walidacji email
            bool isValidEmail = false;
            if (user.Email != null && user.Email.Contains("@"))
            {
                var parts = user.Email.Split('@');
                if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Contains("."))
                {
                    var domainParts = parts[1].Split('.');
                    if (domainParts.Length >= 2 && domainParts[1].Length >= 2)
                    {
                        isValidEmail = true;
                    }
                }
            }

            if (!isValidEmail)
            {
                Console.WriteLine("Invalid email during registration");
                return false;
            }

            // Logika rejestracji...
            return true;
        }
    }

    public class UserProfileUpdateService
    {
        public bool UpdateUserEmail(User user, string newEmail)
        {
            // Kopia #2 tej samej walidacji
            bool isValidEmail = false;
            if (newEmail != null && newEmail.Contains("@"))
            {
                var parts = newEmail.Split('@');
                if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Contains("."))
                {
                    var domainParts = parts[1].Split('.');
                    if (domainParts.Length >= 2 && domainParts[1].Length >= 2)
                    {
                        isValidEmail = true;
                    }
                }
            }

            if (!isValidEmail)
            {
                Console.WriteLine("Invalid email during update");
                return false;
            }

            user.Email = newEmail;
            return true;
        }
    }

    public class NewsletterSubscriptionService
    {
        public bool SubscribeToNewsletter(string email)
        {
            // Kopia #3 tej samej walidacji - ryzyko desynchronizacji!
            bool isValidEmail = false;
            if (email != null && email.Contains("@"))
            {
                var parts = email.Split('@');
                if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Contains("."))
                {
                    var domainParts = parts[1].Split('.');
                    if (domainParts.Length >= 2 && domainParts[1].Length >= 2)
                    {
                        isValidEmail = true;
                    }
                }
            }

            if (!isValidEmail)
            {
                Console.WriteLine("Invalid email for newsletter");
                return false;
            }

            // Logika subskrypcji...
            return true;
        }
    }

    public class ContactFormService
    {
        public bool SendContactMessage(string email, string message)
        {
            // Kopia #4 tej samej walidacji!
            bool isValidEmail = false;
            if (email != null && email.Contains("@"))
            {
                var parts = email.Split('@');
                if (parts.Length == 2 && parts[0].Length > 0 && parts[1].Contains("."))
                {
                    var domainParts = parts[1].Split('.');
                    if (domainParts.Length >= 2 && domainParts[1].Length >= 2)
                    {
                        isValidEmail = true;
                    }
                }
            }

            if (!isValidEmail)
            {
                Console.WriteLine("Invalid email in contact form");
                return false;
            }

            // Logika wysyłania wiadomości...
            return true;
        }
    }
}
