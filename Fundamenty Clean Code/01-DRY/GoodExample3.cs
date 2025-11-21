using System;
using System.Text.RegularExpressions;

namespace CleanCodeFundamentals.DRY.Good3
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }

    // Rozwiązanie: Wydzielona klasa do walidacji email
    public class EmailValidator
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return EmailRegex.IsMatch(email);
        }
    }

    public class UserRegistrationService
    {
        private readonly EmailValidator _emailValidator;

        public UserRegistrationService(EmailValidator emailValidator)
        {
            _emailValidator = emailValidator;
        }

        public bool RegisterUser(User user)
        {
            if (!_emailValidator.IsValidEmail(user.Email))
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
        private readonly EmailValidator _emailValidator;

        public UserProfileUpdateService(EmailValidator emailValidator)
        {
            _emailValidator = emailValidator;
        }

        public bool UpdateUserEmail(User user, string newEmail)
        {
            if (!_emailValidator.IsValidEmail(newEmail))
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
        private readonly EmailValidator _emailValidator;

        public NewsletterSubscriptionService(EmailValidator emailValidator)
        {
            _emailValidator = emailValidator;
        }

        public bool SubscribeToNewsletter(string email)
        {
            if (!_emailValidator.IsValidEmail(email))
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
        private readonly EmailValidator _emailValidator;

        public ContactFormService(EmailValidator emailValidator)
        {
            _emailValidator = emailValidator;
        }

        public bool SendContactMessage(string email, string message)
        {
            if (!_emailValidator.IsValidEmail(email))
            {
                Console.WriteLine("Invalid email in contact form");
                return false;
            }

            // Logika wysyłania wiadomości...
            return true;
        }
    }
}
