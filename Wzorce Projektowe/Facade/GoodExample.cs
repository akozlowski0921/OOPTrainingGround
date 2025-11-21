using System;
using System.Threading.Tasks;

namespace DesignPatterns.Facade
{
    // ✅ GOOD: Facade Pattern

    // Subsystems (same as before)
    public class Database
    {
        public void Connect() => Console.WriteLine("DB Connected");
        public void Disconnect() => Console.WriteLine("DB Disconnected");
        public object Query(string sql) => new { Id = 1, Name = "User" };
    }

    public class Cache
    {
        public void Initialize() => Console.WriteLine("Cache init");
        public object Get(string key) => null;
        public void Set(string key, object value) => Console.WriteLine($"Cache: {key}");
    }

    public class Logger
    {
        public void Initialize() => Console.WriteLine("Logger init");
        public void Log(string message) => Console.WriteLine($"Log: {message}");
    }

    // ✅ Facade - upraszcza dostęp do subsystemów
    public class UserServiceFacade
    {
        private readonly Database _database;
        private readonly Cache _cache;
        private readonly Logger _logger;

        public UserServiceFacade()
        {
            _database = new Database();
            _cache = new Cache();
            _logger = new Logger();
            
            // ✅ Inicjalizacja w jednym miejscu
            _cache.Initialize();
            _logger.Initialize();
        }

        // ✅ Prosty interface dla skomplikowanej operacji
        public object GetUser(int userId)
        {
            _logger.Log($"Getting user {userId}");

            // Try cache
            var cacheKey = $"user_{userId}";
            var cachedData = _cache.Get(cacheKey);

            if (cachedData != null)
            {
                _logger.Log("Cache hit");
                return cachedData;
            }

            // Load from database
            _database.Connect();
            var data = _database.Query($"SELECT * FROM Users WHERE Id={userId}");
            _database.Disconnect();

            // Update cache
            _cache.Set(cacheKey, data);
            _logger.Log("Loaded from database");

            return data;
        }

        public void CreateUser(string name, string email)
        {
            _logger.Log($"Creating user {name}");
            
            _database.Connect();
            _database.Query($"INSERT INTO Users VALUES ('{name}', '{email}')");
            _database.Disconnect();
            
            _logger.Log("User created successfully");
        }
    }

    // ✅ Client - prosty kod
    public class GoodClient
    {
        private readonly UserServiceFacade _userService;

        public GoodClient()
        {
            _userService = new UserServiceFacade();
        }

        public void Run()
        {
            // ✅ Prosty interface - facade ukrywa complexity
            var user = _userService.GetUser(123);
            _userService.CreateUser("John", "john@example.com");
        }
    }

    // ✅ Advanced: Facade with external services
    public interface IPaymentGateway
    {
        Task<bool> ProcessPayment(decimal amount);
    }

    public interface IEmailService
    {
        Task SendEmail(string to, string subject, string body);
    }

    public interface ISmsService
    {
        Task SendSms(string phone, string message);
    }

    // ✅ Facade coordinates multiple external services
    public class OrderProcessingFacade
    {
        private readonly Database _database;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly Logger _logger;

        public OrderProcessingFacade(
            Database database,
            IPaymentGateway paymentGateway,
            IEmailService emailService,
            ISmsService smsService,
            Logger logger)
        {
            _database = database;
            _paymentGateway = paymentGateway;
            _emailService = emailService;
            _smsService = smsService;
            _logger = logger;
        }

        // ✅ Single method orchestrates complex workflow
        public async Task<bool> ProcessOrder(int orderId, string email, string phone)
        {
            _logger.Log($"Processing order {orderId}");

            // 1. Load order
            _database.Connect();
            var order = _database.Query($"SELECT * FROM Orders WHERE Id={orderId}");
            _database.Disconnect();

            // 2. Process payment
            var paymentSuccess = await _paymentGateway.ProcessPayment(100);
            if (!paymentSuccess)
            {
                _logger.Log("Payment failed");
                return false;
            }

            // 3. Update order status
            _database.Connect();
            _database.Query($"UPDATE Orders SET Status='Paid' WHERE Id={orderId}");
            _database.Disconnect();

            // 4. Send notifications
            await Task.WhenAll(
                _emailService.SendEmail(email, "Order Confirmed", "Thank you!"),
                _smsService.SendSms(phone, "Order confirmed")
            );

            _logger.Log("Order processed successfully");
            return true;
        }
    }

    // ✅ Facade with caching strategy
    public class CachedUserFacade
    {
        private readonly Database _database;
        private readonly Cache _cache;
        private readonly TimeSpan _cacheExpiry = TimeSpan.FromMinutes(5);

        public CachedUserFacade(Database database, Cache cache)
        {
            _database = database;
            _cache = cache;
        }

        public object GetUser(int userId)
        {
            var cacheKey = $"user_{userId}";
            var cached = _cache.Get(cacheKey);

            if (cached != null)
                return cached;

            _database.Connect();
            var data = _database.Query($"SELECT * FROM Users WHERE Id={userId}");
            _database.Disconnect();

            _cache.Set(cacheKey, data);
            return data;
        }

        public void InvalidateUserCache(int userId)
        {
            _cache.Set($"user_{userId}", null);
        }
    }
}
