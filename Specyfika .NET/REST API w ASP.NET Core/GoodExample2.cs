// ✅ GOOD: Dokumentacja API, middleware, error handling, best practices

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Annotations;

// ✅ Program.cs z pełną konfiguracją
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // ✅ Konfiguracja serwisów
        builder.Services.AddControllers(options =>
        {
            // ✅ Custom filters
            options.Filters.Add<ValidationFilter>();
        });
        
        // ✅ Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Products API",
                Version = "v1",
                Description = "API for managing products",
                Contact = new OpenApiContact
                {
                    Name = "Support Team",
                    Email = "support@example.com"
                }
            });
            
            // ✅ Włączenie XML komentarzy
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            
            // ✅ JWT authentication w Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
        
        // ✅ CORS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigins", policy =>
            {
                policy.WithOrigins("https://example.com", "https://app.example.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
        
        // ✅ Health checks
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>()
            .AddUrlGroup(new Uri("https://external-api.com/health"), "External API");
        
        // ✅ Response compression
        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        });
        
        // ✅ API versioning
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });
        
        // ✅ Rate limiting (wymaga Microsoft.AspNetCore.RateLimiting)
        builder.Services.AddRateLimiter(options =>
        {
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.User.Identity?.Name ?? context.Request.Headers.Host.ToString(),
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1)
                    }));
        });
        
        // ✅ Dependency Injection
        builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<IOrderService, OrderService>();
        
        var app = builder.Build();
        
        // ✅ Middleware pipeline
        
        // ✅ Global exception handler
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                
                var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (exceptionFeature != null)
                {
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exceptionFeature.Error, "Unhandled exception");
                    
                    // ✅ Nie zwracamy stack trace w production
                    var error = new
                    {
                        Error = "An error occurred while processing your request",
                        RequestId = context.TraceIdentifier
                    };
                    
                    await context.Response.WriteAsJsonAsync(error);
                }
            });
        });
        
        // ✅ HTTPS redirection
        app.UseHttpsRedirection();
        
        // ✅ Response compression
        app.UseResponseCompression();
        
        // ✅ Swagger UI
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products API V1");
                c.RoutePrefix = string.Empty; // Swagger UI na root URL
            });
        }
        
        // ✅ CORS
        app.UseCors("AllowSpecificOrigins");
        
        // ✅ Rate limiting
        app.UseRateLimiter();
        
        // ✅ Authentication & Authorization
        app.UseAuthentication();
        app.UseAuthorization();
        
        // ✅ Custom middleware
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        
        app.MapControllers();
        
        // ✅ Health check endpoint
        app.MapHealthChecks("/health");
        
        app.Run();
    }
}

// ✅ Kontroler z pełną dokumentacją Swagger
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductsController> _logger;
    
    public ProductsController(
        IProductService productService,
        ILogger<ProductsController> logger)
    {
        _productService = productService;
        _logger = logger;
    }
    
    /// <summary>
    /// Pobiera produkt po ID
    /// </summary>
    /// <param name="id">ID produktu</param>
    /// <returns>Szczegóły produktu</returns>
    /// <remarks>
    /// Przykładowe wywołanie:
    /// 
    ///     GET /api/v1/products/123
    /// 
    /// </remarks>
    /// <response code="200">Zwraca produkt</response>
    /// <response code="404">Produkt nie został znaleziony</response>
    /// <response code="400">Nieprawidłowe ID</response>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get product by ID", Description = "Returns a single product")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> GetProduct(
        [FromRoute] [SwaggerParameter("Product identifier", Required = true)] int id)
    {
        if (id <= 0)
        {
            return BadRequest(new ErrorResponse
            {
                Error = "Invalid product ID",
                Details = "Product ID must be greater than 0"
            });
        }
        
        try
        {
            var product = await _productService.GetProductByIdAsync(id);
            return Ok(product);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Product {ProductId} not found", id);
            return NotFound(new ErrorResponse
            {
                Error = "Product not found",
                Details = ex.Message,
                RequestId = HttpContext.TraceIdentifier
            });
        }
    }
}

// ✅ Custom middleware - Request logging
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    
    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ Logowanie requesta
        _logger.LogInformation(
            "Request {Method} {Path} started",
            context.Request.Method,
            context.Request.Path);
        
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            
            // ✅ Logowanie response z czasem wykonania
            _logger.LogInformation(
                "Request {Method} {Path} completed in {ElapsedMs}ms with status {StatusCode}",
                context.Request.Method,
                context.Request.Path,
                sw.ElapsedMilliseconds,
                context.Response.StatusCode);
        }
    }
}

// ✅ Custom middleware - Correlation ID
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";
    
    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // ✅ Generuj lub użyj istniejącego correlation ID
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault()
            ?? Guid.NewGuid().ToString();
        
        context.Items[CorrelationIdHeader] = correlationId;
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);
        
        await _next(context);
    }
}

// ✅ Custom validation filter
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            // ✅ Custom error response format
            var errors = context.ModelState
                .Where(x => x.Value.Errors.Any())
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            
            context.Result = new BadRequestObjectResult(new ValidationErrorResponse
            {
                Error = "Validation failed",
                ValidationErrors = errors
            });
        }
    }
    
    public void OnActionExecuted(ActionExecutedContext context) { }
}

// ✅ Service z separation of concerns
public interface IProductService
{
    Task<ProductResponse> GetProductByIdAsync(int id);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;
    
    public ProductService(
        IProductRepository repository,
        ILogger<ProductService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public async Task<ProductResponse> GetProductByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        
        if (product == null)
        {
            throw new NotFoundException($"Product with ID {id} not found");
        }
        
        return MapToResponse(product);
    }
    
    private ProductResponse MapToResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price
        };
    }
}

public interface IProductRepository
{
    Task<Product> GetByIdAsync(int id);
}

public interface IOrderService { }

// ✅ Response DTOs
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ErrorResponse
{
    public string Error { get; set; }
    public string Details { get; set; }
    public string RequestId { get; set; }
}

public class ValidationErrorResponse
{
    public string Error { get; set; }
    public Dictionary<string, string[]> ValidationErrors { get; set; }
}

// ✅ Custom exceptions
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Supporting classes
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
