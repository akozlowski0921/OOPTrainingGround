// ❌ BAD: Źle zaprojektowane REST API - brak walidacji, routing, error handling

using Microsoft.AspNetCore.Mvc;

// ❌ Problem 1: Brak [ApiController] - brak automatycznej walidacji
[Route("api/products")]
public class ProductController : ControllerBase
{
    // ❌ Problem 2: Brak HTTP verb attributes
    public IActionResult GetProduct(int id)
    {
        // ❌ Problem 3: Brak walidacji parametrów
        var product = Database.GetProduct(id); // Może być null
        
        // ❌ Problem 4: Zwracanie null zamiast 404
        return Ok(product); // 200 OK z null!
    }
    
    // ❌ Problem 5: Nieprawidłowy HTTP verb dla tworzenia zasobu
    [HttpGet] // ❌ Powinien być POST
    public IActionResult CreateProduct(Product product)
    {
        // ❌ Problem 6: Brak walidacji modelu
        // ModelState.IsValid nie jest sprawdzany
        
        // ❌ Problem 7: Brak error handlingu
        Database.InsertProduct(product); // Może rzucić exception
        
        // ❌ Problem 8: Zwracanie 200 OK zamiast 201 Created
        return Ok(product);
    }
    
    // ❌ Problem 9: Akcja zwraca różne typy (int, string, object)
    public object UpdateProduct(int id, Product product)
    {
        if (id <= 0)
            return "Invalid ID"; // ❌ Zwraca string
        
        var existing = Database.GetProduct(id);
        if (existing == null)
            return 404; // ❌ Zwraca int zamiast proper HTTP response
        
        Database.UpdateProduct(product);
        return product; // ❌ Zwraca object
    }
}

// ❌ Problem 10: Model bez walidacji
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } // ❌ Może być null lub pusty
    public decimal Price { get; set; } // ❌ Może być ujemny
    public int Quantity { get; set; } // ❌ Może być ujemny
}

// ❌ Problem 11: Kontroler z logiką biznesową
[Route("api/orders")]
public class OrderController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateOrder(Order order)
    {
        // ❌ Logika biznesowa w kontrolerze zamiast w serwisie
        if (order.Items == null || order.Items.Count == 0)
            return BadRequest("No items");
        
        var total = 0m;
        foreach (var item in order.Items)
        {
            var product = Database.GetProduct(item.ProductId);
            if (product == null)
                return BadRequest($"Product {item.ProductId} not found");
            
            total += product.Price * item.Quantity;
        }
        
        order.TotalAmount = total;
        
        // ❌ Bezpośrednie operacje na bazie w kontrolerze
        Database.InsertOrder(order);
        
        // ❌ Wysyłanie emaila z kontrolera
        EmailService.SendOrderConfirmation(order.CustomerEmail);
        
        return Ok(order);
    }
}

// ❌ Problem 12: Brak response DTOs - zwracanie encji bazodanowych
[Route("api/users")]
public class UserController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = Database.GetUser(id);
        
        // ❌ Zwracanie encji z wrażliwymi danymi
        return Ok(user); // Zawiera PasswordHash, SecurityStamp, etc.
    }
}

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; } // ❌ Wrażliwe dane w response!
    public string SecurityStamp { get; set; } // ❌ Nie powinno być w API
}

// ❌ Problem 13: Brak paginacji
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllCustomers()
    {
        // ❌ Zwraca wszystkie rekordy bez paginacji
        var customers = Database.GetAllCustomers(); // Może być miliony rekordów!
        return Ok(customers);
    }
}

// ❌ Problem 14: Nieprawidłowe status codes
[Route("api/auth")]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        var user = Database.GetUserByEmail(request.Email);
        
        if (user == null || !VerifyPassword(user, request.Password))
        {
            // ❌ 404 Not Found zamiast 401 Unauthorized
            return NotFound("Invalid credentials");
        }
        
        var token = GenerateToken(user);
        
        // ❌ 200 OK zamiast odpowiedniego kodu dla operacji
        return Ok(token);
    }
    
    private bool VerifyPassword(User user, string password) => true;
    private string GenerateToken(User user) => "token";
}

// ❌ Problem 15: Brak wersjonowania API
[Route("api/products")] // ❌ Brak wersji w route
public class ProductV1Controller : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { Message = "v1" });
    }
}

// ❌ Problem 16: Synchroniczne operacje I/O
[Route("api/reports")]
public class ReportController : ControllerBase
{
    [HttpGet("{id}")]
    public IActionResult GetReport(int id)
    {
        // ❌ Synchroniczne wywołanie do bazy danych
        var report = Database.GenerateReport(id); // Może trwać sekundy
        
        return Ok(report);
    }
}

// ❌ Problem 17: Brak content negotiation
[Route("api/data")]
public class DataController : ControllerBase
{
    [HttpGet]
    public string GetData()
    {
        // ❌ Zawsze zwraca string, ignoruje Accept header
        return "{\"data\":\"value\"}";
    }
}

// Supporting classes
public static class Database
{
    public static Product GetProduct(int id) => null;
    public static void InsertProduct(Product product) { }
    public static void UpdateProduct(Product product) { }
    public static void InsertOrder(Order order) { }
    public static User GetUser(int id) => new User 
    { 
        Id = id, 
        PasswordHash = "hash123", 
        SecurityStamp = "stamp" 
    };
    public static List<Customer> GetAllCustomers() => new List<Customer>();
    public static User GetUserByEmail(string email) => null;
    public static object GenerateReport(int id) => new { };
}

public static class EmailService
{
    public static void SendOrderConfirmation(string email) { }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
    public List<OrderItem> Items { get; set; }
    public decimal TotalAmount { get; set; }
}

public class OrderItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
