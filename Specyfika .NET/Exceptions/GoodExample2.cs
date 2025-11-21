using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.ExceptionHandling.Good2
{
    // ✅ GOOD: Prawidłowe obsługiwanie wyjątków i kontrola flow
    public class UserService
    {
        private Dictionary<string, User> _usersByEmail = new Dictionary<string, User>();
        private Dictionary<int, User> _usersById = new Dictionary<int, User>();

        public User FindUserByEmail(string email)
        {
            // Używamy TryGetValue zamiast wyjątku dla normalnego flow
            _usersByEmail.TryGetValue(email, out var user);
            return user; // null jeśli nie znaleziono - to nie jest wyjątkowa sytuacja
        }

        public void ProcessUsers(List<string> emails)
        {
            var errors = new List<string>();

            foreach (var email in emails)
            {
                var user = FindUserByEmail(email);

                // Normalne sprawdzenie, bez wyjątków
                if (user == null)
                {
                    errors.Add($"User with email {email} not found");
                    continue;
                }

                try
                {
                    ProcessUser(user);
                }
                catch (Exception ex)
                {
                    // Logujemy konkretny błąd
                    errors.Add($"Failed to process user {email}: {ex.Message}");
                }
            }

            // Jeśli były błędy, możemy je zgłosić na koniec
            if (errors.Any())
            {
                throw new AggregateException(
                    "Some users could not be processed",
                    errors.Select(e => new InvalidOperationException(e))
                );
            }
        }

        private void ProcessUser(User user)
        {
            // Przetwarzanie użytkownika
            // Może rzucić wyjątek w przypadku prawdziwego błędu
        }

        public decimal CalculateTotalBalance()
        {
            decimal total = 0;

            foreach (var user in _usersById.Values)
            {
                try
                {
                    total += user.Balance;
                }
                catch (Exception ex)
                {
                    // Logujemy szczegóły błędu zamiast go ukrywać
                    Console.Error.WriteLine($"Error calculating balance for user {user.Id}: {ex.Message}");
                    // W zależności od wymagań biznesowych:
                    // - możemy przerwać (re-throw)
                    // - możemy kontynuować z logowaniem
                    // Tutaj kontynuujemy, ale błąd jest widoczny
                }
            }

            return total;
        }

        public void UpdateUser(int userId, string newName)
        {
            if (!_usersById.TryGetValue(userId, out var user))
            {
                // Rzucamy konkretny, znaczący wyjątek
                throw new UserNotFoundException($"User with ID {userId} not found");
            }

            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException("Name cannot be empty", nameof(newName));
            }

            user.Name = newName;
        }
    }

    // Własny wyjątek dla lepszej komunikacji błędów
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string message) : base(message)
        {
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }
    }
}
