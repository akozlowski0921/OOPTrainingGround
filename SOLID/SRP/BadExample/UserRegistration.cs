using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SOLID.SRP.BadExample
{
    /// <summary>
    /// God Class - naruszenie SRP: jedna klasa odpowiada za wiele różnych zadań
    /// </summary>
    public class UserRegistration
    {
        public bool RegisterUser(string email, string password, string name)
        {
            // Walidacja
            if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
            {
                Console.WriteLine("Błąd: Nieprawidłowy email");
                return false;
            }

            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                Console.WriteLine("Błąd: Hasło musi mieć minimum 8 znaków");
                return false;
            }

            if (string.IsNullOrEmpty(name) || name.Length < 2)
            {
                Console.WriteLine("Błąd: Imię musi mieć minimum 2 znaki");
                return false;
            }

            // Hashowanie hasła
            string hashedPassword = HashPassword(password);

            // Zapis do bazy danych
            try
            {
                SaveToDatabase(email, hashedPassword, name);
            }
            catch (Exception ex)
            {
                LogError($"Błąd zapisu do bazy: {ex.Message}");
                return false;
            }

            // Logowanie sukcesu
            LogInfo($"Użytkownik {email} został zarejestrowany pomyślnie");
            return true;
        }

        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void SaveToDatabase(string email, string hashedPassword, string name)
        {
            // Symulacja zapisu do bazy danych
            Console.WriteLine($"[DB] Zapisywanie użytkownika: {email}, {name}");
            
            // W prawdziwej aplikacji: połączenie z DB, SQL INSERT, etc.
            if (email.Contains("test"))
            {
                throw new Exception("Testowy email nie może być zapisany");
            }
        }

        private void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }

        private void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}");
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            var registration = new UserRegistration();
            
            Console.WriteLine("=== Próba rejestracji użytkownika ===\n");
            
            // Prawidłowa rejestracja
            registration.RegisterUser("jan.kowalski@example.com", "SecurePass123", "Jan Kowalski");
            
            Console.WriteLine("\n=== Próba z błędami ===\n");
            
            // Błędna walidacja
            registration.RegisterUser("invalid-email", "short", "J");
        }
    }
}
