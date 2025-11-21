// ❌ BAD: Brak dokumentacji API, error handling, middleware

using Microsoft.AspNetCore.Mvc;

// ❌ Problem 1: Brak dokumentacji Swagger/OpenAPI
[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    // ❌ Brak XML komentarzy
    // ❌ Brak ProducesResponseType
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return Ok(new { Id = id });
    }
}

// ❌ Problem 2: Brak globalnego error handlera
public class Startup
{
    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
        // ❌ Brak app.UseExceptionHandler()
        // ❌ Nieobsłużone wyjątki zwracają 500 z stack trace w production
    }
}

// ❌ Problem 3: Kontroler rzuca exception zamiast zwracać proper response
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = GetUserFromDatabase(id);
        
        if (user == null)
        {
            // ❌ Rzuca exception zamiast zwrócić 404
            throw new Exception("User not found");
        }
        
        return Ok(user);
    }
    
    private User GetUserFromDatabase(int id) => null;
}

// ❌ Problem 4: Brak rate limiting
[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    [HttpGet]
    public IActionResult GetData()
    {
        // ❌ Brak rate limiting - podatny na abuse
        return Ok("data");
    }
}

// ❌ Problem 5: Brak CORS configuration
public class Startup2
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // ❌ Brak services.AddCors()
    }
    
    public void Configure(IApplicationBuilder app)
    {
        // ❌ Brak app.UseCors()
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}

// ❌ Problem 6: Nieczytelne error responses
[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(Order order)
    {
        try
        {
            ProcessOrder(order);
            return Ok();
        }
        catch (Exception ex)
        {
            // ❌ Zwraca pełny stack trace do klienta
            return BadRequest(ex.ToString());
        }
    }
    
    private void ProcessOrder(Order order) { }
}

// ❌ Problem 7: Brak health checks
public class Startup3
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        // ❌ Brak services.AddHealthChecks()
    }
}

// ❌ Problem 8: Brak request/response logging
[ApiController]
[Route("api/logs")]
public class LogController : ControllerBase
{
    [HttpPost]
    public IActionResult Log(LogEntry entry)
    {
        // ❌ Brak logowania requesta
        // ❌ Brak correlation ID
        Console.WriteLine(entry.Message); // ❌ Console.WriteLine zamiast ILogger
        return Ok();
    }
}

// ❌ Problem 9: Brak throttling dla expensive operacji
[ApiController]
[Route("api/reports")]
public class ReportController : ControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateReport(ReportRequest request)
    {
        // ❌ Expensive operacja bez throttling/queue
        var report = await GenerateHugeReport(request);
        return Ok(report);
    }
    
    private async Task<object> GenerateHugeReport(ReportRequest request)
    {
        await Task.Delay(10000); // Symulacja długiej operacji
        return new { };
    }
}

// ❌ Problem 10: Brak proper content negotiation
[ApiController]
[Route("api/content")]
public class ContentController : ControllerBase
{
    [HttpGet]
    public string GetData()
    {
        // ❌ Zawsze zwraca string, ignoruje Accept header
        return "data";
    }
}

// ❌ Problem 11: Brak validation middleware
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddControllers();
        
        // ❌ Brak konfiguracji walidacji
        // ❌ Brak custom error responses
        
        var app = builder.Build();
        
        app.MapControllers();
        app.Run();
    }
}

// ❌ Problem 12: Mieszanie concerns - kontroler robi za dużo
[ApiController]
[Route("api/payments")]
public class PaymentController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PaymentRequest request)
    {
        // ❌ Walidacja w kontrolerze
        if (request.Amount <= 0)
            return BadRequest("Invalid amount");
        
        // ❌ Logika biznesowa w kontrolerze
        var user = await GetUser(request.UserId);
        if (user.Balance < request.Amount)
            return BadRequest("Insufficient funds");
        
        // ❌ Wywołanie external API z kontrolera
        var paymentGateway = new PaymentGateway();
        var result = await paymentGateway.Process(request);
        
        // ❌ Aktualizacja bazy bezpośrednio z kontrolera
        await UpdateDatabase(user, request);
        
        // ❌ Wysyłanie emaila z kontrolera
        await SendEmail(user.Email);
        
        return Ok(result);
    }
    
    private Task<User> GetUser(int id) => Task.FromResult(new User());
    private Task UpdateDatabase(User user, PaymentRequest request) => Task.CompletedTask;
    private Task SendEmail(string email) => Task.CompletedTask;
}

public class PaymentGateway
{
    public Task<object> Process(PaymentRequest request) => Task.FromResult(new object());
}

// Supporting classes
public class User
{
    public int Id { get; set; }
    public decimal Balance { get; set; }
    public string Email { get; set; }
}

public class Order
{
    public int Id { get; set; }
}

public class LogEntry
{
    public string Message { get; set; }
}

public class ReportRequest
{
    public string Type { get; set; }
}

public class PaymentRequest
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
}
