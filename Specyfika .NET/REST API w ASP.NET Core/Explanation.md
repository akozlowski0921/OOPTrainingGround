# REST API w ASP.NET Core

## Problemy w BadExample

### 1. Brak [ApiController] attribute
```csharp
[Route("api/products")]
public class ProductController : ControllerBase // ❌ Brak [ApiController]
```
**Problemy:**
- Brak automatycznej walidacji ModelState
- Brak automatycznego binding source inference
- Trzeba ręcznie sprawdzać `ModelState.IsValid`

### 2. Nieprawidłowe HTTP verbs i status codes
```csharp
[HttpGet] // ❌ Powinien być POST
public IActionResult CreateProduct(Product product)
{
    Database.InsertProduct(product);
    return Ok(product); // ❌ Powinien być 201 Created
}
```
**Problemy:**
- GET nie powinien modyfikować stanu
- Tworzenie zasobu powinno zwracać 201 Created z Location header

### 3. Brak walidacji danych wejściowych
```csharp
public class Product
{
    public string Name { get; set; } // ❌ Może być null
    public decimal Price { get; set; } // ❌ Może być ujemny
}
```
**Problemy:**
- Brak Data Annotations
- Możliwość wprowadzenia nieprawidłowych danych
- Brak error handling

### 4. Logika biznesowa w kontrolerze
```csharp
[HttpPost]
public IActionResult CreateOrder(Order order)
{
    // ❌ Walidacja w kontrolerze
    if (order.Items == null || order.Items.Count == 0)
        return BadRequest("No items");
    
    // ❌ Obliczenia w kontrolerze
    var total = 0m;
    foreach (var item in order.Items) { }
    
    // ❌ Operacje na bazie w kontrolerze
    Database.InsertOrder(order);
}
```
**Problemy:**
- Naruszenie Single Responsibility Principle
- Trudne testowanie
- Duplikacja logiki

### 5. Zwracanie encji bazodanowych
```csharp
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var user = Database.GetUser(id);
    return Ok(user); // ❌ Zawiera PasswordHash!
}
```
**Problemy:**
- Wyciek wrażliwych danych (hasła, tokeny)
- Tight coupling do modelu domenowego
- Niemożliwe jest zmienienie struktury DB bez breaking changes w API

### 6. Brak paginacji
```csharp
[HttpGet]
public IActionResult GetAllCustomers()
{
    var customers = Database.GetAllCustomers(); // ❌ Miliony rekordów
    return Ok(customers);
}
```
**Problemy:**
- Performance issues
- High memory usage
- Timeout dla dużych datasetów

## Rozwiązania w GoodExample

### 1. [ApiController] attribute
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class ProductsController : ControllerBase
```
**Korzyści:**
- Automatyczna walidacja ModelState
- Automatyczne 400 BadRequest dla nieprawidłowych danych
- Binding source inference ([FromBody], [FromRoute], etc.)

### 2. Prawidłowe HTTP verbs i status codes
```csharp
[HttpPost]
public async Task<ActionResult<ProductResponse>> CreateProduct(
    [FromBody] CreateProductRequest request)
{
    var product = await _productService.CreateProductAsync(request);
    
    // ✅ 201 Created z Location header
    return CreatedAtAction(
        nameof(GetProduct),
        new { id = product.Id },
        product);
}
```

**Status codes:**
- `200 OK` - Sukces dla GET, PUT
- `201 Created` - Sukces dla POST (tworzenie zasobu)
- `204 No Content` - Sukces dla DELETE
- `400 Bad Request` - Nieprawidłowe dane wejściowe
- `401 Unauthorized` - Brak authentication
- `403 Forbidden` - Brak authorization
- `404 Not Found` - Zasób nie istnieje
- `409 Conflict` - Konflikt (np. duplikat)
- `422 Unprocessable Entity` - Walidacja biznesowa failuje
- `500 Internal Server Error` - Błąd serwera

### 3. Walidacja z Data Annotations
```csharp
public class CreateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }
}
```

**Dostępne atrybuty:**
- `[Required]` - pole wymagane
- `[StringLength]` - długość stringa
- `[Range]` - zakres wartości
- `[EmailAddress]` - walidacja emaila
- `[Phone]` - walidacja numeru telefonu
- `[RegularExpression]` - regex pattern
- `[Compare]` - porównanie dwóch pól
- `[CreditCard]` - walidacja karty kredytowej
- `[Url]` - walidacja URL

### 4. Separation of Concerns - Service Layer
```csharp
[ApiController]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService; // ✅ Service injected
    
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request)
    {
        // ✅ Logika biznesowa w serwisie
        var order = await _orderService.CreateOrderAsync(request);
        return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
    }
}
```

**Korzyści:**
- Testowalna logika biznesowa
- Reużywalne serwisy
- Czysty kod w kontrolerach

### 5. Response DTOs
```csharp
public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    // ❌ NIE zwracamy: PasswordHash, SecurityStamp, etc.
}
```

**Best practices:**
- Oddzielne DTOs dla request i response
- Maskowanie wrażliwych danych
- Versioning przez DTOs

### 6. Paginacja
```csharp
public class ProductQueryParameters
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, 100)]
    public int PageSize { get; set; } = 10;
    
    public string SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
```

## Swagger/OpenAPI Documentation

### Konfiguracja
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Products API",
        Version = "v1",
        Description = "API for managing products"
    });
    
    // XML komentarze
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(xmlFile);
});
```

### XML Documentation
```csharp
/// <summary>
/// Pobiera produkt po ID
/// </summary>
/// <param name="id">ID produktu</param>
/// <returns>Szczegóły produktu</returns>
/// <response code="200">Zwraca produkt</response>
/// <response code="404">Produkt nie został znaleziony</response>
[HttpGet("{id:int}")]
[ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
public async Task<ActionResult<ProductResponse>> GetProduct(int id)
```

### SwaggerOperation attribute
```csharp
[SwaggerOperation(
    Summary = "Get product by ID",
    Description = "Returns a single product")]
```

## API Versioning

### URL-based versioning
```csharp
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1() { }
    
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2() { }
}
```

### Header-based versioning
```csharp
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
});
```

## Security

### Authentication & Authorization
```csharp
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteUser(int id)
}
```

### Rate Limiting
```csharp
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

### Request Size Limits
```csharp
[HttpPost]
[RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
public async Task<IActionResult> Upload(IFormFile file)
```

### Input Validation
```csharp
// ✅ Parametryzowane queries
var results = await _repository.SearchAsync(parameters);

// ❌ NIE używaj string concatenation
var sql = $"SELECT * FROM Products WHERE Name = '{name}'"; // SQL Injection!
```

## Caching

### Response Caching
```csharp
[HttpGet]
[ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)]
public async Task<ActionResult<CatalogResponse>> GetCatalog()
```

### In-Memory Cache
```csharp
private readonly IMemoryCache _cache;

public async Task<CatalogResponse> GetCatalog()
{
    if (!_cache.TryGetValue("catalog", out CatalogResponse catalog))
    {
        catalog = await _catalogService.GetCatalogAsync();
        
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(5))
            .SetAbsoluteExpiration(TimeSpan.FromHours(1));
        
        _cache.Set("catalog", catalog, cacheOptions);
    }
    
    return catalog;
}
```

### Distributed Cache (Redis)
```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
});
```

## Middleware

### Global Exception Handler
```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionFeature.Error, "Unhandled exception");
        
        await context.Response.WriteAsJsonAsync(new
        {
            Error = "An error occurred",
            RequestId = context.TraceIdentifier
        });
    });
});
```

### Custom Middleware
```csharp
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request {Method} {Path}", 
            context.Request.Method, context.Request.Path);
        
        await _next(context);
        
        _logger.LogInformation("Response {StatusCode}", 
            context.Response.StatusCode);
    }
}

// Rejestracja
app.UseMiddleware<RequestLoggingMiddleware>();
```

## CORS

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("https://example.com")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

app.UseCors("AllowSpecificOrigins");
```

## Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>()
    .AddUrlGroup(new Uri("https://api.com/health"), "External API");

app.MapHealthChecks("/health");
```

## Idempotency

```csharp
[HttpPost]
public async Task<ActionResult<PaymentResponse>> ProcessPayment(
    [FromBody] PaymentRequest request,
    [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
{
    if (string.IsNullOrWhiteSpace(idempotencyKey))
        return BadRequest(new { Error = "Idempotency-Key required" });
    
    // Sprawdź czy request z tym kluczem już został przetworzony
    var existing = await _paymentService.GetByIdempotencyKeyAsync(idempotencyKey);
    if (existing != null)
        return Conflict(new { Error = "Already processed", Payment = existing });
    
    var payment = await _paymentService.ProcessAsync(request, idempotencyKey);
    return Ok(payment);
}
```

## ETag - Conditional Requests

```csharp
[HttpGet("{id}")]
public async Task<ActionResult<DocumentResponse>> GetDocument(int id)
{
    var document = await _service.GetDocumentAsync(id);
    var etag = GenerateETag(document);
    
    // If-None-Match - zwróć 304 jeśli nie zmieniony
    if (Request.Headers["If-None-Match"] == etag)
        return StatusCode(StatusCodes.Status304NotModified);
    
    Response.Headers.Add("ETag", etag);
    return Ok(document);
}

[HttpPut("{id}")]
public async Task<ActionResult> UpdateDocument(int id, DocumentRequest request)
{
    var current = await _service.GetDocumentAsync(id);
    var currentETag = GenerateETag(current);
    
    // If-Match - update tylko jeśli ETag się zgadza (optimistic concurrency)
    if (Request.Headers["If-Match"] != currentETag)
        return StatusCode(StatusCodes.Status412PreconditionFailed);
    
    await _service.UpdateAsync(id, request);
    return NoContent();
}
```

## Best Practices

### DO:
✅ Używaj [ApiController] attribute  
✅ Waliduj dane wejściowe z Data Annotations  
✅ Zwracaj odpowiednie status codes  
✅ Używaj DTOs dla request/response  
✅ Implementuj paginację  
✅ Dokumentuj API z Swagger/OpenAPI  
✅ Używaj API versioning  
✅ Implementuj authentication & authorization  
✅ Używaj rate limiting  
✅ Loguj wszystkie requesty i errory  
✅ Implementuj global exception handler  
✅ Używaj async/await  
✅ Implementuj caching  
✅ Używaj CORS  
✅ Implementuj health checks  
✅ Używaj idempotency dla mutating operations  

### DON'T:
❌ Nie wrzucaj logiki biznesowej do kontrolerów  
❌ Nie zwracaj encji bazodanowych  
❌ Nie używaj synchronicznych operacji I/O  
❌ Nie ignoruj walidacji  
❌ Nie zwracaj wrażliwych danych  
❌ Nie używaj string concatenation w SQL queries  
❌ Nie zwracaj stack trace w production  
❌ Nie ignoruj status codes  
❌ Nie zwracaj wszystkich danych bez paginacji  
❌ Nie hardcoduj configuration  

## Performance Tips

1. **Async/await** - używaj dla wszystkich I/O operations
2. **Response compression** - kompresuj response dla lepszej performance
3. **Caching** - cache expensive operations
4. **Projection** - zwracaj tylko potrzebne pola
5. **Paginacja** - zawsze implementuj dla kolekcji
6. **Database indexes** - optymalizuj queries
7. **Connection pooling** - reużywaj połączenia DB
8. **HTTP compression** - włącz gzip/brotli

## Testing API

```csharp
public class ProductsControllerTests
{
    [Fact]
    public async Task GetProduct_ExistingId_ReturnsProduct()
    {
        // Arrange
        var mockService = new Mock<IProductService>();
        mockService.Setup(s => s.GetProductByIdAsync(1))
            .ReturnsAsync(new ProductResponse { Id = 1, Name = "Test" });
        
        var controller = new ProductsController(mockService.Object, Mock.Of<ILogger>());
        
        // Act
        var result = await controller.GetProduct(1);
        
        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var product = Assert.IsType<ProductResponse>(okResult.Value);
        Assert.Equal("Test", product.Name);
    }
}
```

## Podsumowanie

REST API w ASP.NET Core powinno:
- Używać prawidłowych HTTP verbs i status codes
- Walidować dane wejściowe
- Dokumentować API
- Implementować security (auth, rate limiting)
- Używać DTOs dla separacji concerns
- Implementować paginację i caching
- Logować wszystkie operacje
- Obsługiwać błędy globalnie
- Być wydajne i skalowalne
