using System;

namespace SOLID.SRP.GoodExample
{
    /// <summary>
    /// Odpowiedzialność: Zapis i odczyt użytkowników z bazy danych
    /// </summary>
    public class UserRepository
    {
        public void Save(User user)
        {
            // Symulacja zapisu do bazy danych
            Console.WriteLine($"[DB] Zapisywanie użytkownika: {user.Email}, {user.Name}");
            
            // W prawdziwej aplikacji: połączenie z DB, SQL INSERT, etc.
            if (user.Email.Contains("test"))
            {
                throw new Exception("Testowy email nie może być zapisany");
            }
        }
    }

    public class User
    {
        public string Email { get; set; }
        public string HashedPassword { get; set; }
        public string Name { get; set; }
    }
}
