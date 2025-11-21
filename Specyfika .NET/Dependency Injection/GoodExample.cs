// ✅ GOOD: Dependency Injection z ASP.NET Core - loose coupling, testowalność, SOLID

using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

// ✅ Interfejsy - pozwalają na łatwą wymianę implementacji i mockowanie
public interface IOrderRepository
{
    Task SaveAsync(Order order);
}

public interface IEmailService
{
    Task SendConfirmationAsync(string email, Order order);
}

public interface IOrderValidator
{
    bool Validate(Order order);
}

// ✅ Controller przyjmuje zależności przez konstruktor (Constructor Injection)
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrderController> _logger;
    
    // ✅ DI Container automatycznie wstrzykuje zależności
    public OrderController(
        IOrderService orderService,
        ILogger<OrderController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] Order order)
    {
        try
        {
            await _orderService.ProcessOrderAsync(order);
            _logger.LogInformation("Order created successfully for {Email}", order.CustomerEmail);
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid order attempt");
            return BadRequest(ex.Message);
        }
    }
}

// ✅ Service layer z zależnościami przez interfejsy
public interface IOrderService
{
    Task ProcessOrderAsync(Order order);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IOrderValidator _validator;
    
    // ✅ Wszystkie zależności przez konstruktor - łatwe do przetestowania
    public OrderService(
        IOrderRepository repository,
        IEmailService emailService,
        IOrderValidator validator)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }
    
    public async Task ProcessOrderAsync(Order order)
    {
        // ✅ Validator jest wstrzyknięty - nie tworzymy nowej instancji
        if (!_validator.Validate(order))
            throw new InvalidOperationException("Invalid order");
            
        await _repository.SaveAsync(order);
        await _emailService.SendConfirmationAsync(order.CustomerEmail, order);
    }
}

// ✅ Repository implementation z DbContext (Scoped lifetime)
public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    
    public OrderRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task SaveAsync(Order order)
    {
        // ✅ DbContext zarządzany przez DI - proper lifetime management
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }
}

// ✅ Email service z konfiguracją przez Options Pattern
public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;
    
    public SmtpEmailService(
        IOptions<SmtpSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task SendConfirmationAsync(string email, Order order)
    {
        _logger.LogInformation("Sending confirmation to {Email}", email);
        // ✅ Używa konfiguracji z appsettings.json
        // ... email logic using _settings.SmtpServer
        await Task.CompletedTask;
    }
}

public class OrderValidator : IOrderValidator
{
    public bool Validate(Order order)
    {
        return order != null 
            && order.Quantity > 0 
            && !string.IsNullOrEmpty(order.CustomerEmail);
    }
}

// ✅ Configuration classes dla Options Pattern
public class SmtpSettings
{
    public string SmtpServer { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}

// ✅ DbContext - będzie Scoped (jedna instancja na request)
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Order> Orders { get; set; }
}

// ✅ Program.cs - Konfiguracja DI Container
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // ✅ Rejestracja serwisów z odpowiednimi lifetime'ami
        
        // Transient - nowa instancja przy każdym wstrzyknięciu
        // Używaj dla lightweight, stateless services
        builder.Services.AddTransient<IOrderValidator, OrderValidator>();
        
        // Scoped - jedna instancja na HTTP request
        // Używaj dla DbContext, Unit of Work, services z state per request
        builder.Services.AddScoped<IOrderService, OrderService>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        
        // Singleton - jedna instancja przez cały czas działania aplikacji
        // Używaj dla cache, configuration, services bez state
        builder.Services.AddSingleton<IEmailService, SmtpEmailService>();
        
        // ✅ DbContext zawsze Scoped
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // ✅ Options Pattern dla konfiguracji
        builder.Services.Configure<SmtpSettings>(
            builder.Configuration.GetSection("SmtpSettings"));
        
        builder.Services.AddControllers();
        
        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; }
}
