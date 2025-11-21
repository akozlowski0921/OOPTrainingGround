// ❌ BAD: Niewłaściwe użycie Service Lifetimes

// ❌ Problem 1: Singleton z mutable state
public class ShoppingCartService
{
    // ❌ Singleton przechowuje state - będzie współdzielony między wszystkimi użytkownikami!
    private List<CartItem> _items = new();
    
    public void AddItem(CartItem item)
    {
        // ❌ Wszyscy użytkownicy będą widzieli te same przedmioty!
        _items.Add(item);
    }
    
    public List<CartItem> GetItems()
    {
        return _items;
    }
}

// ❌ Problem 2: Transient z heavy initialization
public class DatabaseConnection
{
    private SqlConnection _connection;
    
    // ❌ Transient tworzy nową instancję przy każdym wstrzyknięciu
    // Heavy initialization będzie wykonywane zbyt często!
    public DatabaseConnection()
    {
        // ❌ Expensive operation wykonywana za każdym razem
        _connection = new SqlConnection("Server=localhost;Database=Shop");
        _connection.Open();
        
        // ❌ Inicjalizacja cache, ładowanie konfiguracji itp.
        LoadConfiguration();
    }
    
    private void LoadConfiguration()
    {
        // Expensive operation
        Thread.Sleep(1000);
    }
}

// ❌ Problem 3: Scoped w Singleton - Captive Dependency
public class ReportGenerator
{
    private readonly AppDbContext _dbContext;
    
    // ❌ ReportGenerator jest Singleton, ale DbContext jest Scoped
    // DbContext będzie żył przez cały czas aplikacji zamiast per-request
    public ReportGenerator(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<Report> GenerateReportAsync()
    {
        // ❌ DbContext może być w stale state lub disposed
        return await _dbContext.Orders.Select(o => new Report()).FirstOrDefaultAsync();
    }
}

// ❌ Problem 4: Niepoprawne lifetime dla HttpClient
public class ApiService
{
    // ❌ Tworzenie nowego HttpClient przy każdym wywołaniu
    public async Task<string> GetDataAsync()
    {
        using var client = new HttpClient(); // ❌ Socket exhaustion!
        return await client.GetStringAsync("https://api.example.com/data");
    }
}

// ❌ Problem 5: Transient dla service z IDisposable
public class FileProcessor : IDisposable
{
    private FileStream _fileStream;
    
    public FileProcessor()
    {
        // ❌ Registered as Transient - może nie być disposed prawidłowo
        _fileStream = new FileStream("data.txt", FileMode.Open);
    }
    
    public void Process()
    {
        // Processing logic
    }
    
    public void Dispose()
    {
        _fileStream?.Dispose();
    }
}

// ❌ Niewłaściwa konfiguracja
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ❌ Problem 1: Singleton z mutable state
        services.AddSingleton<ShoppingCartService>();
        
        // ❌ Problem 2: Transient dla expensive initialization
        services.AddTransient<DatabaseConnection>();
        
        // ❌ Problem 3: Captive dependency - Singleton zawiera Scoped
        services.AddSingleton<ReportGenerator>();
        services.AddScoped<AppDbContext>();
        
        // ❌ Problem 4: Brak użycia IHttpClientFactory
        services.AddTransient<ApiService>();
        
        // ❌ Problem 5: Transient dla IDisposable może prowadzić do memory leaks
        services.AddTransient<FileProcessor>();
    }
}

// ❌ Problem 6: Wielokrotne rozwiązywanie tego samego serwisu
public class OrderProcessor
{
    private readonly IServiceProvider _serviceProvider;
    
    public OrderProcessor(IServiceProvider serviceProvider)
    {
        // ❌ Service Locator anti-pattern
        _serviceProvider = serviceProvider;
    }
    
    public void ProcessOrder(Order order)
    {
        // ❌ Każde wywołanie tworzy nowe instancje
        var validator = _serviceProvider.GetService<IOrderValidator>();
        var repository = _serviceProvider.GetService<IOrderRepository>();
        
        if (validator.Validate(order))
        {
            repository.Save(order);
        }
    }
}

public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Report { }

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
}

public interface IOrderValidator
{
    bool Validate(Order order);
}

public interface IOrderRepository
{
    void Save(Order order);
}

public class Order
{
    public int Id { get; set; }
}
