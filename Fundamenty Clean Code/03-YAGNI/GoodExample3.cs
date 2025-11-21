using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCodeFundamentals.YAGNI.Good3
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // Rozwiązanie: Implementujemy tylko to, co jest rzeczywiście potrzebne
    public class UserService
    {
        private List<User> _users = new List<User>();

        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public User GetUser(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        // Uwaga: Gdy będziemy potrzebować tagów, grup, statystyk logowania,
        // wtedy dodamy te funkcje. Teraz są zbędne i tylko zwiększają złożoność.
    }
}
