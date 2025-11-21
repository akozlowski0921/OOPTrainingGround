// ❌ BAD: Brak versioning, security, caching

using Microsoft.AspNetCore.Mvc;

// ❌ Problem 1: Brak API versioning
[ApiController]
[Route("api/products")] // ❌ Brak wersji w route
public class ProductsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Jeśli zmienimy format response, zepsujemy wszystkie klienty
        return Ok(new { Message = "Data" });
    }
}

// ❌ Problem 2: Brak authentication/authorization
[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    [HttpDelete("users/{id}")]
    public IActionResult DeleteUser(int id)
    {
        // ❌ Każdy może usunąć użytkownika!
        DeleteUserFromDatabase(id);
        return NoContent();
    }
    
    private void DeleteUserFromDatabase(int id) { }
}

// ❌ Problem 3: Brak caching
[ApiController]
[Route("api/catalog")]
public class CatalogController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCatalog()
    {
        // ❌ Każde wywołanie pobiera z bazy, nawet jeśli dane się nie zmieniły
        var catalog = await GetFromDatabase();
        return Ok(catalog);
    }
    
    private async Task<object> GetFromDatabase()
    {
        await Task.Delay(1000); // Symulacja query
        return new { };
    }
}

// ❌ Problem 4: Zwracanie wrażliwych danych
[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = GetUserFromDatabase(id);
        
        // ❌ Zwraca wszystko włącznie z hasłem, SSN, etc.
        return Ok(user);
    }
    
    private User GetUserFromDatabase(int id)
    {
        return new User
        {
            Id = id,
            Username = "john",
            PasswordHash = "hash123", // ❌ Wrażliwe!
            SSN = "123-45-6789", // ❌ Bardzo wrażliwe!
            CreditCard = "4111-1111-1111-1111" // ❌ Bardzo wrażliwe!
        };
    }
}

// ❌ Problem 5: Brak input sanitization
[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    [HttpGet]
    public IActionResult Search(string query)
    {
        // ❌ SQL Injection risk
        var sql = $"SELECT * FROM Products WHERE Name LIKE '%{query}%'";
        var results = ExecuteRawSql(sql);
        
        return Ok(results);
    }
    
    private object ExecuteRawSql(string sql) => new { };
}

// ❌ Problem 6: Brak request size limits
[ApiController]
[Route("api/upload")]
public class UploadController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        // ❌ Brak limitu rozmiaru - podatny na DoS
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        
        return Ok();
    }
}

// ❌ Problem 7: Brak timeout dla external calls
[ApiController]
[Route("api/external")]
public class ExternalApiController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetExternalData()
    {
        using var client = new HttpClient();
        
        // ❌ Brak timeout - może wisieć w nieskończoność
        var response = await client.GetStringAsync("https://slow-api.com/data");
        
        return Ok(response);
    }
}

// ❌ Problem 8: Brak idempotency dla POST/PUT
[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    [HttpPost]
    public IActionResult ProcessPayment(PaymentRequest request)
    {
        // ❌ Wielokrotne wywołania tworzą multiple payments
        // Brak idempotency key
        var payment = ProcessPaymentInDatabase(request);
        return Ok(payment);
    }
    
    private object ProcessPaymentInDatabase(PaymentRequest request) => new { };
}

// ❌ Problem 9: Returning too much data
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetReport()
    {
        // ❌ Zwraca całą kolekcję z wszystkimi polami
        var data = GetHugeDataset();
        return Ok(data); // Może być 100MB+
    }
    
    private object GetHugeDataset()
    {
        return Enumerable.Range(1, 1000000).Select(i => new
        {
            Id = i,
            Name = $"Item {i}",
            Description = new string('x', 1000),
            // ... więcej pól
        });
    }
}

// ❌ Problem 10: Brak proper logging
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder(Order order)
    {
        try
        {
            ProcessOrder(order);
            // ❌ Brak logowania sukcesu
            return Ok();
        }
        catch (Exception ex)
        {
            // ❌ Console.WriteLine zamiast ILogger
            Console.WriteLine(ex.Message);
            return BadRequest();
        }
    }
    
    private void ProcessOrder(Order order) { }
}

// ❌ Problem 11: Brak ETag dla conditional requests
[ApiController]
[Route("api/documents")]
public class DocumentsController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetDocument(int id)
    {
        var document = GetDocumentFromDatabase(id);
        
        // ❌ Zawsze zwraca pełny dokument
        // Brak wsparcia dla If-None-Match
        return Ok(document);
    }
    
    [HttpPut("{id}")]
    public IActionResult UpdateDocument(int id, Document document)
    {
        // ❌ Brak sprawdzania If-Match
        // Podatny na lost update problem
        UpdateDocumentInDatabase(id, document);
        return Ok();
    }
    
    private object GetDocumentFromDatabase(int id) => new { };
    private void UpdateDocumentInDatabase(int id, Document document) { }
}

// ❌ Problem 12: Brak proper error codes
[ApiController]
[Route("api/resources")]
public class ResourcesController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetResource(int id)
    {
        if (!UserIsAuthenticated())
        {
            // ❌ 404 zamiast 401 - security through obscurity
            return NotFound();
        }
        
        if (!UserHasAccess(id))
        {
            // ❌ 404 zamiast 403
            return NotFound();
        }
        
        return Ok(GetResourceData(id));
    }
    
    private bool UserIsAuthenticated() => false;
    private bool UserHasAccess(int id) => false;
    private object GetResourceData(int id) => new { };
}

// Supporting classes
public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string SSN { get; set; }
    public string CreditCard { get; set; }
}

public class PaymentRequest
{
    public decimal Amount { get; set; }
}

public class Order
{
    public int Id { get; set; }
}

public class Document
{
    public int Id { get; set; }
    public string Content { get; set; }
}
