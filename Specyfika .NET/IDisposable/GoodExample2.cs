using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SpecyfikaDotNet.ResourceManagement.Good2
{
    // ✅ GOOD: Prawidłowe zarządzanie połączeniami i komendami
    public class UserRepository : IDisposable
    {
        private readonly string _connectionString;
        private bool _disposed = false;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public User GetUser(int id)
        {
            // Using zapewnia, że connection i command zostaną dispose'owane
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2)
                        };
                    }
                }
            }

            return null;
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id",
                connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Id", user.Id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM Users", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Email = reader.GetString(2)
                        });
                    }
                }
            }

            return users;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Repository samo w sobie nie trzyma zasobów niezarządzanych,
                    // ponieważ używamy using dla każdej operacji
                }

                _disposed = true;
            }
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
