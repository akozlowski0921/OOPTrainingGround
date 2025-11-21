using System;
using System.Collections.Generic;
using System.Linq;

namespace CleanCodeFundamentals.YAGNI.Bad3
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    // Problem: Ogromna klasa z funkcjami "na przyszłość", których nikt nie używa
    public class UserService
    {
        private List<User> _users = new List<User>();
        private Dictionary<int, List<string>> _userTags = new Dictionary<int, List<string>>();
        private Dictionary<int, DateTime> _lastLogin = new Dictionary<int, DateTime>();
        private Dictionary<int, int> _loginCount = new Dictionary<int, int>();
        private Dictionary<string, List<int>> _userGroups = new Dictionary<string, List<int>>();

        // Rzeczywiście używana funkcjonalność
        public void AddUser(User user)
        {
            _users.Add(user);
        }

        public User GetUser(int id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        // Poniższe metody nigdy nie są używane, ale ktoś je "przewidział na przyszłość"
        public void AddTagToUser(int userId, string tag)
        {
            if (!_userTags.ContainsKey(userId))
                _userTags[userId] = new List<string>();
            _userTags[userId].Add(tag);
        }

        public List<string> GetUserTags(int userId)
        {
            return _userTags.ContainsKey(userId) ? _userTags[userId] : new List<string>();
        }

        public void RemoveTagFromUser(int userId, string tag)
        {
            if (_userTags.ContainsKey(userId))
                _userTags[userId].Remove(tag);
        }

        public void RecordLogin(int userId)
        {
            _lastLogin[userId] = DateTime.Now;
            if (!_loginCount.ContainsKey(userId))
                _loginCount[userId] = 0;
            _loginCount[userId]++;
        }

        public DateTime? GetLastLogin(int userId)
        {
            return _lastLogin.ContainsKey(userId) ? _lastLogin[userId] : (DateTime?)null;
        }

        public int GetLoginCount(int userId)
        {
            return _loginCount.ContainsKey(userId) ? _loginCount[userId] : 0;
        }

        public void AddUserToGroup(int userId, string groupName)
        {
            if (!_userGroups.ContainsKey(groupName))
                _userGroups[groupName] = new List<int>();
            _userGroups[groupName].Add(userId);
        }

        public void RemoveUserFromGroup(int userId, string groupName)
        {
            if (_userGroups.ContainsKey(groupName))
                _userGroups[groupName].Remove(userId);
        }

        public List<int> GetUsersInGroup(string groupName)
        {
            return _userGroups.ContainsKey(groupName) ? _userGroups[groupName] : new List<int>();
        }

        public List<string> GetUserGroups(int userId)
        {
            return _userGroups.Where(g => g.Value.Contains(userId)).Select(g => g.Key).ToList();
        }

        public void DeleteGroup(string groupName)
        {
            if (_userGroups.ContainsKey(groupName))
                _userGroups.Remove(groupName);
        }

        public List<User> SearchUsersByTag(string tag)
        {
            var userIds = _userTags.Where(t => t.Value.Contains(tag)).Select(t => t.Key).ToList();
            return _users.Where(u => userIds.Contains(u.Id)).ToList();
        }

        public Dictionary<string, int> GetTagStatistics()
        {
            var stats = new Dictionary<string, int>();
            foreach (var userTags in _userTags.Values)
            {
                foreach (var tag in userTags)
                {
                    if (!stats.ContainsKey(tag))
                        stats[tag] = 0;
                    stats[tag]++;
                }
            }
            return stats;
        }
    }
}
