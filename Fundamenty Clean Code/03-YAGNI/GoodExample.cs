using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCodeFundamentals.YAGNI.Good
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Rozwiązanie: Tylko metody faktycznie potrzebne biznesowi
    public class UserService
    {
        private readonly List<User> _users = new();

        public User GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public void CreateUser(User user)
        {
            _users.Add(user);
        }

        // Gdy pojawi się nowe wymaganie biznesowe, WTEDY dodamy nową metodę
        // Nie wcześniej!
    }
}
