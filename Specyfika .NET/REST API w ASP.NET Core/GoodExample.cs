// ✅ GOOD: Prawidłowo zaprojektowane REST API - walidacja, routing, error handling, best practices

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

// ✅ [ApiController] - automatyczna walidacja, binding source inference
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[Produces("application/json")] // ✅ Content negotiation
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
    /// <response code="200">Zwraca produkt</response>
    /// <response code="404">Produkt nie został znaleziony</response>
    [HttpGet("{id:int}")] // ✅ Route constraint
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponse>> GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);
        
        // ✅ Walidacja parametru
        if (id <= 0)
            return BadRequest(new { Error = "Invalid product ID" });
        
        var product = await _productService.GetProductByIdAsync(id);
        
        // ✅ Zwracanie 404 gdy zasób nie istnieje
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound(new { Error = $"Product with ID {id} not found" });
        }
        
        // ✅ Zwracanie DTO zamiast encji
        return Ok(product);
    }
    
    /// <summary>
    /// Tworzy nowy produkt
    /// </summary>
    /// <param name="request">Dane nowego produktu</param>
    /// <returns>Utworzony produkt</returns>
    /// <response code="201">Produkt został utworzony</response>
    /// <response code="400">Nieprawidłowe dane wejściowe</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        [FromBody] CreateProductRequest request)
    {
        // ✅ [ApiController] automatycznie sprawdza ModelState
        // Jeśli walidacja failuje, zwraca 400 BadRequest
        
        try
        {
            var product = await _productService.CreateProductAsync(request);
            
            _logger.LogInformation("Product created with ID: {ProductId}", product.Id);
            
            // ✅ 201 Created z Location header
            return CreatedAtAction(
                nameof(GetProduct),
                new { id = product.Id },
                product);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create product");
            return BadRequest(new { Error = ex.Message });
        }
    }
    
    /// <summary>
    /// Aktualizuje istniejący produkt
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        int id,
        [FromBody] UpdateProductRequest request)
    {
        if (id <= 0)
            return BadRequest(new { Error = "Invalid product ID" });
        
        try
        {
            var product = await _productService.UpdateProductAsync(id, request);
            return Ok(product);
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    
    /// <summary>
    /// Usuwa produkt
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        if (id <= 0)
            return BadRequest(new { Error = "Invalid product ID" });
        
        try
        {
            await _productService.DeleteProductAsync(id);
            
            // ✅ 204 No Content dla successful delete
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { Error = ex.Message });
        }
    }
    
    /// <summary>
    /// Pobiera listę produktów z paginacją
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetProducts(
        [FromQuery] ProductQueryParameters parameters)
    {
        // ✅ Paginacja + filtrowanie + sortowanie
        var result = await _productService.GetProductsAsync(parameters);
        return Ok(result);
    }
}

// ✅ Request DTOs z walidacją
public class CreateProductRequest
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
    public string Name { get; set; }
    
    [Required]
    [StringLength(500)]
    public string Description { get; set; }
    
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
    public int Quantity { get; set; }
    
    [Required]
    public string Category { get; set; }
}

public class UpdateProductRequest
{
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [StringLength(500)]
    public string Description { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal? Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int? Quantity { get; set; }
}

// ✅ Response DTOs - tylko potrzebne dane
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

// ✅ Query parameters dla paginacji i filtrowania
public class ProductQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;
    
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;
    
    [Range(1, MaxPageSize)]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    
    public string SearchTerm { get; set; }
    public string Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string SortBy { get; set; } = "Name";
    public bool SortDescending { get; set; } = false;
}

// ✅ Paginowany rezultat
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

// ✅ Kontroler z logiką biznesową w serwisie
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;
    
    public OrdersController(
        IOrderService orderService,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request)
    {
        try
        {
            // ✅ Logika biznesowa w serwisie, nie w kontrolerze
            var order = await _orderService.CreateOrderAsync(request);
            
            _logger.LogInformation("Order created with ID: {OrderId}", order.Id);
            
            return CreatedAtAction(
                nameof(GetOrder),
                new { id = order.Id },
                order);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
    
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrder(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
            return NotFound();
        
        return Ok(order);
    }
}

// ✅ Custom Exception dla 404
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}

// Service interfaces
public interface IProductService
{
    Task<ProductResponse> GetProductByIdAsync(int id);
    Task<ProductResponse> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse> UpdateProductAsync(int id, UpdateProductRequest request);
    Task DeleteProductAsync(int id);
    Task<PagedResult<ProductResponse>> GetProductsAsync(ProductQueryParameters parameters);
}

public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    Task<OrderResponse> GetOrderByIdAsync(int id);
}

// Order DTOs
public class CreateOrderRequest
{
    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; }
    
    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required")]
    public List<OrderItemRequest> Items { get; set; }
}

public class OrderItemRequest
{
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}

public class OrderResponse
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItemResponse> Items { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderItemResponse
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}

// ✅ Authentication controller z poprawnymi status codes
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request.Email, request.Password);
        
        if (result == null)
        {
            // ✅ 401 Unauthorized dla nieprawidłowych credentials
            return Unauthorized(new { Error = "Invalid credentials" });
        }
        
        return Ok(result);
    }
}

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
}

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    [MinLength(8)]
    public string Password { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public UserInfo User { get; set; }
}

// ✅ Response DTO bez wrażliwych danych
public class UserInfo
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    // ❌ Brak PasswordHash, SecurityStamp, itp.
}
