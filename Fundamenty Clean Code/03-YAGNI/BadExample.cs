using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCodeFundamentals.YAGNI.Bad
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Problem: Klasa pełna nieużywanych metod "na przyszłość"
    public class UserService
    {
        private readonly List<User> _users = new();

        // Aktualnie używane - to się faktycznie wykorzystuje
        public User GetUserById(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        // Aktualnie używane
        public void CreateUser(User user)
        {
            _users.Add(user);
        }

        // "Może kiedyś będzie potrzebne" - NIGDY NIE UŻYTE
        public User GetUserByEmail(string email)
        {
            return _users.FirstOrDefault(u => u.Email == email);
        }

        // "Na przyszłość jak będziemy robić archiwizację" - NIGDY NIE UŻYTE
        public List<User> GetUsersCreatedBetween(DateTime start, DateTime end)
        {
            return _users.Where(u => u.CreatedAt >= start && u.CreatedAt <= end).ToList();
        }

        // "Może będzie potrzebne do raportów" - NIGDY NIE UŻYTE
        public Dictionary<int, string> GetUserIdToNameMapping()
        {
            return _users.ToDictionary(u => u.Id, u => u.Name);
        }

        // "Może będzie bulk update" - NIGDY NIE UŻYTE
        public void UpdateUsers(List<User> users)
        {
            foreach (var user in users)
            {
                var existing = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existing != null)
                {
                    existing.Name = user.Name;
                    existing.Email = user.Email;
                }
            }
        }

        // "Może będziemy potrzebować statystyk" - NIGDY NIE UŻYTE
        public int GetTotalUserCount()
        {
            return _users.Count;
        }

        // "Może będzie paginacja" - NIGDY NIE UŻYTE
        public List<User> GetUsersPaginated(int page, int pageSize)
        {
            return _users.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }

        // "Może będziemy weryfikować emaile" - NIGDY NIE UŻYTE
        public bool IsEmailUnique(string email)
        {
            return !_users.Any(u => u.Email == email);
        }

        // "Może będzie sortowanie" - NIGDY NIE UŻYTE
        public List<User> GetUsersSortedByName(bool ascending = true)
        {
            return ascending 
                ? _users.OrderBy(u => u.Name).ToList() 
                : _users.OrderByDescending(u => u.Name).ToList();
        }

        // "Może będzie eksport do CSV" - NIGDY NIE UŻYTE
        public string ExportUsersToCsv()
        {
            var csv = "Id,Name,Email,CreatedAt\n";
            foreach (var user in _users)
            {
                csv += $"{user.Id},{user.Name},{user.Email},{user.CreatedAt:yyyy-MM-dd}\n";
            }
            return csv;
        }
    }
}
