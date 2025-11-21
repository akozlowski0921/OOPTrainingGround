using System;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace SpecyfikaDotNet.ResourceManagement.Bad2
{
    // ❌ BAD: Brak zarządzania połączeniami do bazy danych
    public class UserRepository
    {
        private readonly string _connectionString;
        private SqlConnection _connection;
        private List<SqlCommand> _activeCommands;

        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
            _connection = new SqlConnection(_connectionString);
            _connection.Open();
            _activeCommands = new List<SqlCommand>();
        }

        public User GetUser(int id)
        {
            // BŁĄD: SqlCommand nie jest dispose'owany
            var command = new SqlCommand("SELECT * FROM Users WHERE Id = @Id", _connection);
            command.Parameters.AddWithValue("@Id", id);
            _activeCommands.Add(command);

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

            return null;
        }

        public void UpdateUser(User user)
        {
            // BŁĄD: Kolejny SqlCommand bez dispose
            var command = new SqlCommand(
                "UPDATE Users SET Name = @Name, Email = @Email WHERE Id = @Id",
                _connection
            );
            command.Parameters.AddWithValue("@Name", user.Name);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@Id", user.Id);
            _activeCommands.Add(command);

            command.ExecuteNonQuery();
        }

        public List<User> GetAllUsers()
        {
            var users = new List<User>();

            // BŁĄD: Ani command, ani reader nie są dispose'owane
            var command = new SqlCommand("SELECT * FROM Users", _connection);
            _activeCommands.Add(command);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add(new User
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Email = reader.GetString(2)
                });
            }

            return users;
        }

        // BŁĄD: Brak Dispose() - connection i wszystkie commands pozostają otwarte
        // Prowadzi to do wyczerpania puli połączeń do bazy danych
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
