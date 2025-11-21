// ✅ GOOD: Prawidłowe użycie Service Lifetimes

// ✅ Rozwiązanie 1: Scoped dla state per request
public interface IShoppingCartService
{
    void AddItem(CartItem item);
    List<CartItem> GetItems();
}

public class ShoppingCartService : IShoppingCartService
{
    // ✅ Scoped - każdy request ma własną instancję z własnym state
    private readonly List<CartItem> _items = new();
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ShoppingCartService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void AddItem(CartItem item)
    {
        // ✅ State jest per-request, nie współdzielony między użytkownikami
        _items.Add(item);
    }
    
    public List<CartItem> GetItems()
    {
        return _items;
    }
}

// ✅ Rozwiązanie 2: Singleton dla expensive initialization
public interface IConfigurationCache
{
    string GetSetting(string key);
}

public class ConfigurationCache : IConfigurationCache
{
    private readonly Dictionary<string, string> _cache;
    
    // ✅ Singleton - inicjalizacja wykonana raz przy starcie
    public ConfigurationCache()
    {
        _cache = new Dictionary<string, string>();
        LoadConfiguration(); // Wykonane tylko raz
    }
    
    private void LoadConfiguration()
    {
        // ✅ Expensive operation, ale tylko raz przy starcie aplikacji
        // Załaduj konfigurację z pliku, bazy danych itp.
    }
    
    public string GetSetting(string key)
    {
        // ✅ Thread-safe read operations
        return _cache.TryGetValue(key, out var value) ? value : null;
    }
}

// ✅ Rozwiązanie 3: IServiceScopeFactory dla Singleton + Scoped
public class ReportGenerator
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ReportGenerator> _logger;
    
    // ✅ Singleton używa IServiceScopeFactory do tworzenia scope'ów
    public ReportGenerator(
        IServiceScopeFactory scopeFactory,
        ILogger<ReportGenerator> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }
    
    public async Task<Report> GenerateReportAsync()
    {
        // ✅ Tworzymy nowy scope dla DbContext
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // ✅ DbContext ma prawidłowy lifetime - tylko dla tego scope'a
        var orders = await dbContext.Orders.ToListAsync();
        
        _logger.LogInformation("Generated report with {Count} orders", orders.Count);
        
        return new Report { OrderCount = orders.Count };
    }
}

// ✅ Rozwiązanie 4: IHttpClientFactory dla HttpClient
public interface IApiService
{
    Task<string> GetDataAsync();
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    
    // ✅ HttpClient wstrzyknięty przez IHttpClientFactory
    // Prawidłowe zarządzanie connection pool
    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    
    public async Task<string> GetDataAsync()
    {
        // ✅ Używamy wstrzykniętego HttpClient - brak socket exhaustion
        return await _httpClient.GetStringAsync("https://api.example.com/data");
    }
}

// ✅ Rozwiązanie 5: Scoped dla IDisposable services
public interface IFileProcessor : IDisposable
{
    void Process();
}

public class FileProcessor : IFileProcessor
{
    private FileStream _fileStream;
    private bool _disposed;
    
    public FileProcessor()
    {
        // ✅ Registered as Scoped - prawidłowy disposal na końcu scope'a
        _fileStream = new FileStream("data.txt", FileMode.OpenOrCreate);
    }
    
    public void Process()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(FileProcessor));
            
        // Processing logic
    }
    
    public void Dispose()
    {
        if (_disposed)
            return;
            
        _fileStream?.Dispose();
        _disposed = true;
    }
}

// ✅ Prawidłowa konfiguracja
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // ✅ Scoped dla state per-request
        services.AddScoped<IShoppingCartService, ShoppingCartService>();
        services.AddHttpContextAccessor(); // Potrzebne dla Scoped services w HTTP context
        
        // ✅ Singleton dla expensive initialization i immutable/thread-safe state
        services.AddSingleton<IConfigurationCache, ConfigurationCache>();
        
        // ✅ Singleton używa IServiceScopeFactory dla Scoped dependencies
        // IServiceScopeFactory is automatically registered by framework
        services.AddSingleton<ReportGenerator>();
        
        // ✅ DbContext zawsze Scoped
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer("connection string"));
        
        // ✅ IHttpClientFactory dla prawidłowego zarządzania HttpClient
        services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://api.example.com");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        
        // ✅ Scoped dla IDisposable - prawidłowy disposal
        services.AddScoped<IFileProcessor, FileProcessor>();
        
        // ✅ Transient dla lightweight, stateless services
        services.AddTransient<IOrderValidator, OrderValidator>();
    }
}

// ✅ Constructor Injection zamiast Service Locator
public class OrderProcessor
{
    private readonly IOrderValidator _validator;
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderProcessor> _logger;
    
    // ✅ Explicit dependencies w konstruktorze
    public OrderProcessor(
        IOrderValidator validator,
        IOrderRepository repository,
        ILogger<OrderProcessor> logger)
    {
        _validator = validator;
        _repository = repository;
        _logger = logger;
    }
    
    public async Task ProcessOrderAsync(Order order)
    {
        // ✅ Używamy wstrzykniętych dependencies
        if (_validator.Validate(order))
        {
            await _repository.SaveAsync(order);
            _logger.LogInformation("Order {OrderId} processed", order.Id);
        }
        else
        {
            _logger.LogWarning("Invalid order {OrderId}", order.Id);
            throw new InvalidOperationException("Invalid order");
        }
    }
}

// ✅ Lifetime Guidelines Table
/*
| Service Type                  | Recommended Lifetime | Reason |
|-------------------------------|---------------------|--------|
| DbContext                     | Scoped              | Per-request unit of work |
| HttpClient (via Factory)      | Transient/Scoped    | Managed connection pooling |
| Configuration/Cache           | Singleton           | Shared, immutable state |
| Stateless services            | Transient           | No state, lightweight |
| Per-request state             | Scoped              | User-specific data |
| IDisposable services          | Scoped/Transient    | Proper disposal |
| Logger                        | Singleton           | Thread-safe, stateless |
*/

// Supporting classes
public class CartItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Report
{
    public int OrderCount { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Order> Orders { get; set; }
}

public interface IOrderValidator
{
    bool Validate(Order order);
}

public class OrderValidator : IOrderValidator
{
    public bool Validate(Order order)
    {
        return order != null && order.Id > 0;
    }
}

public interface IOrderRepository
{
    Task SaveAsync(Order order);
}

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
}
