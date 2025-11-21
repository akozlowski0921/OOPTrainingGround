// ❌ BAD: Testy z zależnościami zewnętrznymi, brak mockowania

using Xunit;

// ❌ Problem 1: Test zależy od prawdziwej bazy danych
public class OrderServiceTests
{
    [Fact]
    public void ProcessOrder_ValidOrder_SavesSuccessfully()
    {
        // ❌ Arrange - używa prawdziwej bazy danych
        var connectionString = "Server=localhost;Database=TestDb";
        var dbContext = new AppDbContext(connectionString);
        var repository = new OrderRepository(dbContext);
        var emailService = new SmtpEmailService("smtp.gmail.com"); // ❌ Prawdziwy SMTP
        var service = new OrderService(repository, emailService);
        
        var order = new Order 
        { 
            Quantity = 1, 
            CustomerEmail = "test@test.com" 
        };
        
        // ❌ Act - wykonuje prawdziwe operacje
        service.ProcessOrder(order);
        
        // ❌ Assert - sprawdza w prawdziwej bazie
        var savedOrder = dbContext.Orders.FirstOrDefault(o => o.CustomerEmail == order.CustomerEmail);
        Assert.NotNull(savedOrder);
        
        // ❌ Brak cleanup - dane zostają w bazie
    }
}

// ❌ Problem 2: Testy bez użycia mocków - trudne do kontroli
public class EmailServiceTests
{
    [Fact]
    public void SendEmail_ValidAddress_SendsEmail()
    {
        // ❌ Arrange - wysyła prawdziwy email!
        var emailService = new SmtpEmailService("smtp.gmail.com");
        
        // ❌ Act - faktycznie próbuje wysłać email
        emailService.SendConfirmation("test@test.com");
        
        // ❌ Assert - nie możemy sprawdzić czy email został wysłany
        // Musimy ręcznie sprawdzić skrzynkę pocztową
        Assert.True(true); // Bezsensowny assert
    }
}

// ❌ Problem 3: Test zależny od czasu systemowego
public class DiscountServiceTests
{
    [Fact]
    public void CalculateDiscount_Weekend_Returns20Percent()
    {
        // ❌ Arrange - zależy od dnia tygodnia
        var service = new DiscountService();
        
        // ❌ Act - wynik zależy od tego kiedy test jest uruchomiony
        var discount = service.GetWeekendDiscount();
        
        // ❌ Assert - test przechodzi tylko w weekend!
        Assert.Equal(0.2m, discount);
    }
}

public class DiscountService
{
    public decimal GetWeekendDiscount()
    {
        // ❌ Zależność od DateTime.Now
        return DateTime.Now.DayOfWeek == DayOfWeek.Saturday || 
               DateTime.Now.DayOfWeek == DayOfWeek.Sunday 
            ? 0.2m 
            : 0m;
    }
}

// ❌ Problem 4: Brak testowania wyjątków
public class ValidationTests
{
    [Fact]
    public void ValidateOrder_InvalidQuantity_NoAssert()
    {
        // ❌ Arrange
        var validator = new OrderValidator();
        var order = new Order { Quantity = -1 };
        
        // ❌ Act - powinien rzucić wyjątek, ale tego nie sprawdzamy
        try
        {
            validator.ValidateAndThrow(order);
            Assert.True(false, "Should have thrown exception"); // ❌ Słaby sposób
        }
        catch (Exception)
        {
            // ❌ Łapiemy dowolny wyjątek
            Assert.True(true);
        }
    }
}

// ❌ Problem 5: Test z wieloma assertami (testuje za dużo)
public class UserServiceTests
{
    [Fact]
    public void CreateUser_ValidData_CreatesUserCorrectly()
    {
        // ❌ Arrange
        var service = new UserService();
        
        // ❌ Act
        var user = service.CreateUser("John", "Doe", "john@test.com");
        
        // ❌ Assert - za dużo assertów w jednym teście
        Assert.Equal("John", user.FirstName);
        Assert.Equal("Doe", user.LastName);
        Assert.Equal("john@test.com", user.Email);
        Assert.NotEqual(Guid.Empty, user.Id); // ❌ Jeśli to failuje, nie wiemy co jest problem
        Assert.True(user.IsActive);
        Assert.NotNull(user.CreatedAt);
        Assert.True(user.CreatedAt <= DateTime.Now);
    }
}

// ❌ Problem 6: Brak testowania metod asynchronicznych
public class AsyncServiceTests
{
    [Fact]
    public void GetDataAsync_ReturnsData() // ❌ Brak async
    {
        // ❌ Arrange
        var service = new DataService();
        
        // ❌ Act - używamy .Result co może spowodować deadlock
        var result = service.GetDataAsync().Result;
        
        // ❌ Assert
        Assert.NotNull(result);
    }
}

// ❌ Problem 7: Testy bez izolacji - współdzielony state
public class CalculatorTests
{
    private static Calculator _calculator = new Calculator(); // ❌ Shared state
    
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        _calculator.Value = 5; // ❌ Modyfikuje współdzielony state
        _calculator.Add(3);
        Assert.Equal(8, _calculator.Value);
    }
    
    [Fact]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        // ❌ Test może failować w zależności od kolejności wykonania
        _calculator.Subtract(2);
        Assert.Equal(6, _calculator.Value); // Może być 6 lub inny wynik!
    }
}

// Supporting classes
public class AppDbContext
{
    public AppDbContext(string connectionString) { }
    public List<Order> Orders { get; set; } = new();
}

public class OrderRepository
{
    public OrderRepository(AppDbContext context) { }
    public void Save(Order order) { }
}

public class SmtpEmailService
{
    public SmtpEmailService(string server) { }
    public void SendConfirmation(string email) { }
}

public class OrderService
{
    public OrderService(OrderRepository repo, SmtpEmailService email) { }
    public void ProcessOrder(Order order) { }
}

public class OrderValidator
{
    public void ValidateAndThrow(Order order)
    {
        if (order.Quantity <= 0)
            throw new InvalidOperationException("Invalid quantity");
    }
}

public class UserService
{
    public User CreateUser(string firstName, string lastName, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
    }
}

public class DataService
{
    public async Task<string> GetDataAsync()
    {
        await Task.Delay(100);
        return "data";
    }
}

public class Calculator
{
    public int Value { get; set; }
    public void Add(int x) => Value += x;
    public void Subtract(int x) => Value -= x;
}

public class Order
{
    public int Quantity { get; set; }
    public string CustomerEmail { get; set; }
}

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
