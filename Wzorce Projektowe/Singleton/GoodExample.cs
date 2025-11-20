// ✅ GOOD EXAMPLE: Thread-safe Singleton

// Podejście 1: Lazy<T> (zalecane w .NET)
public class DatabaseConnection
{
    private static readonly Lazy<DatabaseConnection> _lazyInstance = 
        new Lazy<DatabaseConnection>(() => new DatabaseConnection());

    private string _connectionString;

    private DatabaseConnection()
    {
        _connectionString = "Server=localhost;Database=MyDb;";
        Console.WriteLine("Database connection initialized (Lazy)");
        Thread.Sleep(100); // Symulacja kosztownej operacji
    }

    public static DatabaseConnection Instance => _lazyInstance.Value;

    public void ExecuteQuery(string query)
    {
        Console.WriteLine($"Executing: {query} on {_connectionString}");
    }
}

// Podejście 2: Static constructor (eager initialization)
public class ConfigurationManager
{
    private static readonly ConfigurationManager _instance = new ConfigurationManager();

    private Dictionary<string, string> _settings;

    static ConfigurationManager() { }

    private ConfigurationManager()
    {
        _settings = new Dictionary<string, string>
        {
            { "ApiUrl", "https://api.example.com" },
            { "Timeout", "30" }
        };
        Console.WriteLine("Configuration loaded (Static)");
    }

    public static ConfigurationManager Instance => _instance;

    public string GetSetting(string key)
    {
        return _settings.TryGetValue(key, out var value) ? value : null;
    }
}

// Podejście 3: Double-checked locking (dla starszych wersji .NET)
public class LegacyService
{
    private static LegacyService _instance;
    private static readonly object _lock = new object();

    private LegacyService()
    {
        Console.WriteLine("Legacy service initialized (Double-checked)");
    }

    public static LegacyService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new LegacyService();
                    }
                }
            }
            return _instance;
        }
    }
}

// Użycie - bezpieczne w środowisku wielowątkowym:
var tasks = new Task[10];
for (int i = 0; i < 10; i++)
{
    int taskId = i;
    tasks[i] = Task.Run(() =>
    {
        var db = DatabaseConnection.Instance;
        var config = ConfigurationManager.Instance;
        Console.WriteLine($"Task {taskId}: DB={db.GetHashCode()}, Config={config.GetHashCode()}");
    });
}

Task.WaitAll(tasks);
// Zawsze te same instancje! ✅

// ⚠️ NAJLEPSZE ROZWIĄZANIE: Dependency Injection w ASP.NET Core
// public class Startup
// {
//     public void ConfigureServices(IServiceCollection services)
//     {
//         // DI Container zarządza życiem singletona:
//         services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
//         services.AddSingleton<IConfigurationManager, ConfigurationManager>();
//     }
// }
//
// public class MyController : ControllerBase
// {
//     private readonly IDatabaseConnection _db;
//
//     public MyController(IDatabaseConnection db)
//     {
//         _db = db; // Injected przez DI, thread-safe, testable!
//     }
// }

// Korzyści:
// 1. Pełna thread-safety - gwarantowana jedna instancja
// 2. Lazy<T> - inicjalizacja dopiero przy pierwszym użyciu
// 3. Brak race conditions
// 4. Performance - Lazy<T> jest zoptymalizowany przez CLR
// 5. W .NET: preferuj DI zamiast ręcznych singletonów!
