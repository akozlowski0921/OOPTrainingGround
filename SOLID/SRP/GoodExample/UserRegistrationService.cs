using System;

namespace SOLID.SRP.GoodExample
{
    /// <summary>
    /// Odpowiedzialność: Koordynacja procesu rejestracji użytkownika
    /// Ta klasa deleguje poszczególne zadania do wyspecjalizowanych klas
    /// </summary>
    public class UserRegistrationService
    {
        private readonly UserValidator _validator;
        private readonly PasswordHasher _passwordHasher;
        private readonly UserRepository _userRepository;
        private readonly Logger _logger;

        public UserRegistrationService(
            UserValidator validator,
            PasswordHasher passwordHasher,
            UserRepository userRepository,
            Logger logger)
        {
            _validator = validator;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _logger = logger;
        }

        public bool RegisterUser(string email, string password, string name)
        {
            // Walidacja
            var validationResult = _validator.Validate(email, password, name);
            if (!validationResult.IsValid)
            {
                _logger.LogError($"Błąd walidacji: {validationResult.ErrorMessage}");
                return false;
            }

            // Hashowanie
            string hashedPassword = _passwordHasher.HashPassword(password);

            // Zapis
            try
            {
                var user = new User
                {
                    Email = email,
                    HashedPassword = hashedPassword,
                    Name = name
                };
                _userRepository.Save(user);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Błąd zapisu do bazy: {ex.Message}");
                return false;
            }

            // Logowanie sukcesu
            _logger.LogInfo($"Użytkownik {email} został zarejestrowany pomyślnie");
            return true;
        }
    }

    // Program demonstracyjny
    public class Program
    {
        public static void Main()
        {
            var validator = new UserValidator();
            var passwordHasher = new PasswordHasher();
            var userRepository = new UserRepository();
            var logger = new Logger();

            var registrationService = new UserRegistrationService(
                validator,
                passwordHasher,
                userRepository,
                logger
            );

            Console.WriteLine("=== Próba rejestracji użytkownika ===\n");
            
            // Prawidłowa rejestracja
            registrationService.RegisterUser("jan.kowalski@example.com", "SecurePass123", "Jan Kowalski");
            
            Console.WriteLine("\n=== Próba z błędami ===\n");
            
            // Błędna walidacja
            registrationService.RegisterUser("invalid-email", "short", "J");
        }
    }
}
