// ✅ GOOD: API versioning, security, caching, best practices

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Cryptography;
using System.Text;

// ✅ Rozwiązanie 1: API Versioning
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiVersion("2.0")]
public class ProductsController : ControllerBase
{
    [HttpGet]
    [MapToApiVersion("1.0")]
    public IActionResult GetV1()
    {
        // ✅ Wersja 1 zwraca podstawowe dane
        return Ok(new { Id = 1, Name = "Product" });
    }
    
    [HttpGet]
    [MapToApiVersion("2.0")]
    public IActionResult GetV2()
    {
        // ✅ Wersja 2 zwraca rozszerzone dane
        return Ok(new { Id = 1, Name = "Product", Category = "Electronics", Tags = new[] { "sale" } });
    }
}

// ✅ Rozwiązanie 2: Authentication & Authorization
[ApiController]
[Route("api/v{version:apiVersion}/admin")]
[ApiVersion("1.0")]
[Authorize(Roles = "Admin")] // ✅ Wymaga roli Admin
public class AdminController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AdminController> _logger;
    
    public AdminController(IUserService userService, ILogger<AdminController> logger)
    {
        _userService = userService;
        _logger = logger;
    }
    
    [HttpDelete("users/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        // ✅ Logowanie akcji administracyjnych
        _logger.LogWarning("Admin {AdminId} attempting to delete user {UserId}", 
            User.Identity.Name, id);
        
        try
        {
            await _userService.DeleteUserAsync(id);
            
            _logger.LogInformation("User {UserId} deleted by admin {AdminId}", 
                id, User.Identity.Name);
            
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
}

// ✅ Rozwiązanie 3: Response Caching
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class CatalogController : ControllerBase
{
    private readonly ICatalogService _catalogService;
    private readonly IMemoryCache _cache;
    
    public CatalogController(ICatalogService catalogService, IMemoryCache cache)
    {
        _catalogService = catalogService;
        _cache = cache;
    }
    
    [HttpGet]
    [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any)] // ✅ Cache na 5 minut
    [ProducesResponseType(typeof(CatalogResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CatalogResponse>> GetCatalog()
    {
        // ✅ Cache w pamięci
        var cacheKey = "catalog";
        
        if (!_cache.TryGetValue(cacheKey, out CatalogResponse catalog))
        {
            catalog = await _catalogService.GetCatalogAsync();
            
            // ✅ Cache z sliding expiration
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                .SetAbsoluteExpiration(TimeSpan.FromHours(1));
            
            _cache.Set(cacheKey, catalog, cacheOptions);
        }
        
        return Ok(catalog);
    }
    
    [HttpPost("refresh")]
    [Authorize(Roles = "Admin")]
    public IActionResult RefreshCache()
    {
        // ✅ Endpoint do ręcznego odświeżania cache
        _cache.Remove("catalog");
        return NoContent();
    }
}

// ✅ Rozwiązanie 4: Response DTOs - tylko bezpieczne dane
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetUser(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        
        if (user == null)
            return NotFound();
        
        // ✅ Zwraca tylko bezpieczne dane
        return Ok(new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = MaskEmail(user.Email),
            // ❌ NIE zwracamy: PasswordHash, SSN, CreditCard
        });
    }
    
    private string MaskEmail(string email)
    {
        // ✅ Maskowanie wrażliwych danych
        var parts = email.Split('@');
        if (parts.Length != 2) return email;
        
        var name = parts[0];
        var masked = name.Length > 2 
            ? $"{name[0]}***{name[^1]}" 
            : "***";
        
        return $"{masked}@{parts[1]}";
    }
}

// ✅ Rozwiązanie 5: Parametryzowane queries - SQL Injection prevention
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class SearchController : ControllerBase
{
    private readonly IProductRepository _repository;
    
    public SearchController(IProductRepository repository)
    {
        _repository = repository;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductResponse>>> Search(
        [FromQuery] SearchParameters parameters)
    {
        // ✅ Walidacja input
        if (string.IsNullOrWhiteSpace(parameters.Query))
            return BadRequest(new { Error = "Query parameter is required" });
        
        // ✅ Parametryzowane query zamiast string concatenation
        var results = await _repository.SearchAsync(parameters);
        
        return Ok(results);
    }
}

// ✅ Rozwiązanie 6: Request size limits
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class UploadController : ControllerBase
{
    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB
    private readonly IStorageService _storageService;
    
    public UploadController(IStorageService storageService)
    {
        _storageService = storageService;
    }
    
    [HttpPost]
    [RequestSizeLimit(MaxFileSize)] // ✅ Limit na rozmiar requesta
    [ProducesResponseType(typeof(UploadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UploadResponse>> Upload(IFormFile file)
    {
        // ✅ Walidacja pliku
        if (file == null || file.Length == 0)
            return BadRequest(new { Error = "No file uploaded" });
        
        if (file.Length > MaxFileSize)
            return BadRequest(new { Error = $"File size exceeds {MaxFileSize / 1024 / 1024}MB limit" });
        
        // ✅ Walidacja typu pliku
        var allowedTypes = new[] { "image/jpeg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(file.ContentType))
            return BadRequest(new { Error = "Invalid file type" });
        
        var fileUrl = await _storageService.UploadAsync(file);
        
        return Ok(new UploadResponse { Url = fileUrl });
    }
}

// ✅ Rozwiązanie 7: Timeout dla external calls
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ExternalApiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public ExternalApiController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ExternalDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status408RequestTimeout)]
    public async Task<ActionResult<ExternalDataResponse>> GetExternalData()
    {
        // ✅ HttpClient z timeout
        var client = _httpClientFactory.CreateClient("ExternalApi");
        client.Timeout = TimeSpan.FromSeconds(30);
        
        try
        {
            var response = await client.GetStringAsync("https://external-api.com/data");
            return Ok(new ExternalDataResponse { Data = response });
        }
        catch (TaskCanceledException)
        {
            // ✅ Obsługa timeout
            return StatusCode(StatusCodes.Status408RequestTimeout, 
                new { Error = "Request to external API timed out" });
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, 
                new { Error = "External API is unavailable", Details = ex.Message });
        }
    }
}

// ✅ Rozwiązanie 8: Idempotency dla POST
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    
    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PaymentResponse>> ProcessPayment(
        [FromBody] PaymentRequest request,
        [FromHeader(Name = "Idempotency-Key")] string idempotencyKey)
    {
        // ✅ Wymaganie idempotency key
        if (string.IsNullOrWhiteSpace(idempotencyKey))
            return BadRequest(new { Error = "Idempotency-Key header is required" });
        
        try
        {
            var payment = await _paymentService.ProcessPaymentAsync(request, idempotencyKey);
            return Ok(payment);
        }
        catch (DuplicateRequestException)
        {
            // ✅ 409 Conflict jeśli request już został przetworzony
            var existingPayment = await _paymentService.GetPaymentByIdempotencyKeyAsync(idempotencyKey);
            return Conflict(new { Error = "Payment already processed", Payment = existingPayment });
        }
    }
}

// ✅ Rozwiązanie 9: Projection - zwracanie tylko potrzebnych danych
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    
    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReportSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ReportSummary>>> GetReport(
        [FromQuery] ReportQueryParameters parameters)
    {
        // ✅ Paginacja + projection - tylko potrzebne pola
        var data = await _reportService.GetReportDataAsync(parameters);
        return Ok(data);
    }
}

// ✅ Rozwiązanie 10: Structured logging z ILogger
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;
    
    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<OrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        // ✅ Structured logging z placeholders
        _logger.LogInformation("Creating order for customer {CustomerEmail}", request.CustomerEmail);
        
        try
        {
            var order = await _orderService.CreateOrderAsync(request);
            
            // ✅ Logowanie sukcesu z dodatkowymi informacjami
            _logger.LogInformation(
                "Order {OrderId} created successfully for customer {CustomerEmail} with total {TotalAmount}",
                order.Id, request.CustomerEmail, order.TotalAmount);
            
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }
        catch (Exception ex)
        {
            // ✅ Logowanie błędu z exception
            _logger.LogError(ex, "Failed to create order for customer {CustomerEmail}", request.CustomerEmail);
            throw;
        }
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        return order == null ? NotFound() : Ok(order);
    }
}

// ✅ Rozwiązanie 11: ETag dla conditional requests
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    
    public DocumentsController(IDocumentService documentService)
    {
        _documentService = documentService;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status304NotModified)]
    public async Task<ActionResult<DocumentResponse>> GetDocument(int id)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        
        if (document == null)
            return NotFound();
        
        // ✅ Generowanie ETag
        var etag = GenerateETag(document);
        
        // ✅ Sprawdzanie If-None-Match header
        if (Request.Headers.TryGetValue("If-None-Match", out var requestETag) 
            && requestETag == etag)
        {
            return StatusCode(StatusCodes.Status304NotModified);
        }
        
        // ✅ Dodawanie ETag do response
        Response.Headers.Add("ETag", etag);
        
        return Ok(document);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(DocumentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    public async Task<ActionResult<DocumentResponse>> UpdateDocument(
        int id, 
        [FromBody] UpdateDocumentRequest request)
    {
        var document = await _documentService.GetDocumentByIdAsync(id);
        
        if (document == null)
            return NotFound();
        
        // ✅ Sprawdzanie If-Match header dla optimistic concurrency
        if (Request.Headers.TryGetValue("If-Match", out var requestETag))
        {
            var currentETag = GenerateETag(document);
            
            if (requestETag != currentETag)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed, 
                    new { Error = "Document has been modified by another user" });
            }
        }
        
        var updated = await _documentService.UpdateDocumentAsync(id, request);
        
        return Ok(updated);
    }
    
    private string GenerateETag(object obj)
    {
        // ✅ Generate ETag based on content
        var json = System.Text.Json.JsonSerializer.Serialize(obj);
        var bytes = Encoding.UTF8.GetBytes(json);
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }
}

// ✅ Rozwiązanie 12: Proper status codes
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class ResourcesController : ControllerBase
{
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResourceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ResourceResponse> GetResource(int id)
    {
        if (!UserIsAuthenticated())
        {
            // ✅ 401 Unauthorized gdy nie ma authentication
            return Unauthorized(new { Error = "Authentication required" });
        }
        
        if (!UserHasAccess(id))
        {
            // ✅ 403 Forbidden gdy user nie ma dostępu
            return Forbid();
        }
        
        var resource = GetResourceData(id);
        
        if (resource == null)
        {
            // ✅ 404 Not Found gdy zasób nie istnieje
            return NotFound();
        }
        
        return Ok(resource);
    }
    
    private bool UserIsAuthenticated() => true;
    private bool UserHasAccess(int id) => true;
    private ResourceResponse GetResourceData(int id) => new ResourceResponse();
}

// Supporting classes and interfaces
public interface IUserService
{
    Task DeleteUserAsync(int id);
    Task<User> GetUserByIdAsync(int id);
}

public interface ICatalogService
{
    Task<CatalogResponse> GetCatalogAsync();
}

public interface IProductRepository
{
    Task<PagedResult<ProductResponse>> SearchAsync(SearchParameters parameters);
}

public interface IStorageService
{
    Task<string> UploadAsync(IFormFile file);
}

public interface IPaymentService
{
    Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest request, string idempotencyKey);
    Task<PaymentResponse> GetPaymentByIdempotencyKeyAsync(string key);
}

public interface IReportService
{
    Task<PagedResult<ReportSummary>> GetReportDataAsync(ReportQueryParameters parameters);
}

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse> GetOrderByIdAsync(int id);
}

public interface IDocumentService
{
    Task<DocumentResponse> GetDocumentByIdAsync(int id);
    Task<DocumentResponse> UpdateDocumentAsync(int id, UpdateDocumentRequest request);
}

// DTOs and exceptions
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
}

public class UserResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}

public class CatalogResponse { }
public class SearchParameters 
{ 
    public string Query { get; set; } 
}
public class ProductResponse { }
public class PagedResult<T> 
{ 
    public List<T> Items { get; set; } 
}
public class UploadResponse 
{ 
    public string Url { get; set; } 
}
public class ExternalDataResponse 
{ 
    public string Data { get; set; } 
}
public class PaymentRequest 
{ 
    public decimal Amount { get; set; } 
}
public class PaymentResponse 
{ 
    public int Id { get; set; } 
}
public class DuplicateRequestException : Exception { }
public class ReportSummary { }
public class ReportQueryParameters { }
public class CreateOrderRequest 
{ 
    public string CustomerEmail { get; set; } 
}
public class OrderResponse 
{ 
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
}
public class DocumentResponse { }
public class UpdateDocumentRequest { }
public class ResourceResponse { }
public class NotFoundException : Exception 
{ 
    public NotFoundException(string message) : base(message) { } 
}
