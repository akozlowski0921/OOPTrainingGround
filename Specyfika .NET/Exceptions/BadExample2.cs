using System;
using System.Collections.Generic;
using System.Linq;

namespace SpecyfikaDotNet.ExceptionHandling.Bad2
{
    // ❌ BAD: Używanie wyjątków do kontroli flow
    public class UserService
    {
        private List<User> _users = new List<User>();

        public User FindUserByEmail(string email)
        {
            try
            {
                // BŁĄD: Używanie wyjątku zamiast normalnego return
                var user = _users.Single(u => u.Email == email);
                return user;
            }
            catch (InvalidOperationException)
            {
                // Wyjątek jako część normalnego flow - BARDZO ZŁE
                return null;
            }
        }

        public void ProcessUsers(List<string> emails)
        {
            foreach (var email in emails)
            {
                try
                {
                    var user = FindUserByEmail(email);
                    // BŁĄD: rzucanie wyjątku dla normalnego przypadku
                    if (user == null)
                        throw new Exception("User not found");

                    ProcessUser(user);
                }
                catch (Exception)
                {
                    // Ignorowanie - swallowing exception
                    continue;
                }
            }
        }

        private void ProcessUser(User user)
        {
            // Przetwarzanie użytkownika
        }

        public decimal CalculateTotalBalance()
        {
            decimal total = 0;

            foreach (var user in _users)
            {
                try
                {
                    total += user.Balance;
                }
                catch
                {
                    // BŁĄD: puste catch - ukrywanie wszystkich problemów
                    // Nie wiadomo, co poszło nie tak
                }
            }

            return total;
        }

        public void UpdateUser(int userId, string newName)
        {
            try
            {
                var user = _users[userId]; // BŁĄD: zakładamy że index jest ID
                user.Name = newName;
            }
            catch (ArgumentOutOfRangeException)
            {
                // BŁĄD: konwersja konkretnego wyjątku na ogólny
                throw new Exception("Błąd aktualizacji użytkownika");
                // Tracona informacja o tym, że to był problem z indexem
            }
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
