using System;
using System.Collections.Generic;
using System.Linq;

namespace DesignPatterns.CQRS.Bad3
{
    // ❌ BAD: Poor performance - no optimization for scalability

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public List<Order> Orders { get; set; } = new();
        public List<Address> Addresses { get; set; } = new();
        public UserProfile Profile { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Total { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
    }

    public class UserProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    // ❌ Single service without optimization
    public class UserService
    {
        private readonly List<User> _users = new();

        // ❌ Write operation - creates entire graph
        public void CreateUser(string username, string email, string passwordHash)
        {
            var user = new User
            {
                Id = _users.Count + 1,
                Username = username,
                Email = email,
                PasswordHash = passwordHash,
                Orders = new List<Order>(),
                Addresses = new List<Address>(),
                Profile = new UserProfile()
            };
            _users.Add(user);
            // ❌ Loading entire object graph even when not needed
        }

        // ❌ Query - loads entire user with all relationships
        public User GetUserByUsername(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
            // ❌ Loads all orders, addresses, profile even if we just need username
            // ❌ N+1 query problem if orders have items
            // ❌ No caching
        }

        // ❌ Complex query without optimization
        public List<User> SearchUsers(string searchTerm)
        {
            return _users.Where(u => 
                u.Username.Contains(searchTerm) || 
                u.Email.Contains(searchTerm)).ToList();
            // ❌ Full table scan
            // ❌ No indexing
            // ❌ Returns full user objects with all relations
        }

        // ❌ Query for reporting - very expensive
        public Dictionary<string, int> GetOrderCountByUser()
        {
            return _users.ToDictionary(
                u => u.Username,
                u => u.Orders.Count);
            // ❌ Loads all users and all orders into memory
            // ❌ No pagination
            // ❌ Could be a simple SQL aggregate but loads everything
        }

        // ❌ Update operation
        public void UpdateUserEmail(int userId, string newEmail)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Email = newEmail;
            }
            // ❌ Loads entire user object to update one field
        }

        // ❌ Check if email exists - loads entire user
        public bool EmailExists(string email)
        {
            return _users.Any(u => u.Email == email);
            // ❌ Could be a simple indexed query
            // ❌ Might load entire users to check existence
        }
    }

    // ❌ PROBLEMS:
    // - Single database for reads and writes (cannot scale independently)
    // - No caching strategy for frequently accessed data
    // - Loading entire object graphs when only few fields needed
    // - No indexing strategy for queries
    // - Write operations locked with read operations
    // - No query optimization (projections, pagination)
    // - Cannot use different storage technologies for different access patterns
}
