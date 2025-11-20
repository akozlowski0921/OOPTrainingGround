using System.Text.RegularExpressions;

namespace SOLID.SRP.GoodExample
{
    /// <summary>
    /// Odpowiedzialność: Walidacja danych użytkownika
    /// </summary>
    public class UserValidator
    {
        public ValidationResult Validate(string email, string password, string name)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                result.IsValid = false;
                result.ErrorMessage = "Nieprawidłowy email";
                return result;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                result.IsValid = false;
                result.ErrorMessage = "Hasło musi mieć minimum 8 znaków";
                return result;
            }

            if (string.IsNullOrEmpty(name) || name.Length < 2)
            {
                result.IsValid = false;
                result.ErrorMessage = "Imię musi mieć minimum 2 znaki";
                return result;
            }

            return result;
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
