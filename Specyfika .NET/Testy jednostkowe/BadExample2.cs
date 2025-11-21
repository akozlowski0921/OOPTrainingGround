// ❌ BAD: Słabe mockowanie, brak weryfikacji, nieprecyzyjne testy

using Moq;
using Xunit;

// ❌ Problem 1: Mock bez setup - zwraca default values
public class PaymentServiceTests
{
    [Fact]
    public void ProcessPayment_ValidAmount_ReturnsSuccess()
    {
        // ❌ Arrange - mock bez setup
        var mockGateway = new Mock<IPaymentGateway>();
        // mockGateway nie ma setup - zwróci domyślną wartość (null/false)
        
        var service = new PaymentService(mockGateway.Object);
        
        // ❌ Act
        var result = service.ProcessPayment(100m);
        
        // ❌ Assert - test przechodzi ale nie testuje prawdziwej logiki
        Assert.False(result); // Zwraca false bo mock nie został skonfigurowany
    }
}

// ❌ Problem 2: Brak weryfikacji wywołań metod
public class NotificationServiceTests
{
    [Fact]
    public void SendNotification_ValidMessage_Succeeds()
    {
        // ❌ Arrange
        var mockSender = new Mock<IMessageSender>();
        var service = new NotificationService(mockSender.Object);
        
        // ❌ Act
        service.SendNotification("Test message");
        
        // ❌ Assert - brak weryfikacji czy metoda została wywołana
        Assert.True(true); // Pusty assert - nic nie sprawdzamy
    }
}

// ❌ Problem 3: Zbyt luźna weryfikacja z It.IsAny<>
public class OrderValidatorTests
{
    [Fact]
    public void ValidateOrder_SpecificOrder_CallsRepository()
    {
        // ❌ Arrange
        var mockRepository = new Mock<IOrderRepository>();
        var validator = new OrderValidator(mockRepository.Object);
        
        var order = new Order { Id = 123, Quantity = 5 };
        
        // ❌ Act
        validator.ValidateOrder(order);
        
        // ❌ Assert - weryfikuje że metoda została wywołana, ale z dowolnym argumentem
        mockRepository.Verify(r => r.GetOrder(It.IsAny<int>()), Times.Once);
        // Nie sprawdzamy czy została wywołana z WŁAŚCIWYM ID
    }
}

// ❌ Problem 4: Mock zwraca null - test nie wykrywa NullReferenceException
public class UserServiceTests
{
    [Fact]
    public void GetUserDetails_ExistingUser_ReturnsDetails()
    {
        // ❌ Arrange
        var mockRepository = new Mock<IUserRepository>();
        // Brak setup - GetUser zwróci null
        
        var service = new UserService(mockRepository.Object);
        
        // ❌ Act - NullReferenceException!
        var details = service.GetUserDetails(1);
        
        // ❌ Assert - nigdy się nie wykona
        Assert.NotNull(details);
    }
}

// ❌ Problem 5: Brak testowania edge cases
public class DiscountCalculatorTests
{
    [Fact]
    public void CalculateDiscount_NormalCase_ReturnsDiscount()
    {
        // ❌ Testuje tylko jeden scenariusz
        var calculator = new DiscountCalculator();
        
        var result = calculator.Calculate(100m, 0.1m);
        
        Assert.Equal(10m, result);
        // ❌ Nie testuje: negative amounts, 0, 100% discount, > 100% discount
    }
}

// ❌ Problem 6: Setup zwraca stałe wartości zamiast callback
public class WeatherServiceTests
{
    [Fact]
    public void GetTemperature_DifferentCities_ReturnsDifferentTemperatures()
    {
        // ❌ Arrange
        var mockClient = new Mock<IWeatherClient>();
        
        // ❌ Setup zwraca zawsze tę samą wartość
        mockClient
            .Setup(c => c.GetTemperatureAsync(It.IsAny<string>()))
            .ReturnsAsync(20.0);
        
        var service = new WeatherService(mockClient.Object);
        
        // ❌ Act
        var temp1 = service.GetTemperature("London").Result;
        var temp2 = service.GetTemperature("Tokyo").Result;
        
        // ❌ Assert - obie temperatury są takie same!
        Assert.Equal(20.0, temp1);
        Assert.Equal(20.0, temp2); // Nie realistyczne
    }
}

// ❌ Problem 7: Brak testowania kolejności wywołań
public class TransactionServiceTests
{
    [Fact]
    public void ExecuteTransaction_ValidData_CompletesSuccessfully()
    {
        // ❌ Arrange
        var mockRepository = new Mock<ITransactionRepository>();
        var mockLogger = new Mock<ILogger>();
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        // ❌ Act
        service.ExecuteTransaction(new Transaction());
        
        // ❌ Assert - nie sprawdzamy kolejności
        mockLogger.Verify(l => l.Log("Starting transaction"), Times.Once);
        mockRepository.Verify(r => r.Save(It.IsAny<Transaction>()), Times.Once);
        mockLogger.Verify(l => l.Log("Transaction completed"), Times.Once);
        // Kolejność może być zła (completed przed save) i test przejdzie!
    }
}

// ❌ Problem 8: Over-mocking - mockowanie prostych klas
public class StringHelperTests
{
    [Fact]
    public void Concatenate_TwoStrings_ReturnsConcatenated()
    {
        // ❌ Mockowanie prostej klasy bez dependencies
        var mockHelper = new Mock<IStringHelper>();
        mockHelper
            .Setup(h => h.Concatenate("Hello", "World"))
            .Returns("HelloWorld");
        
        var result = mockHelper.Object.Concatenate("Hello", "World");
        
        Assert.Equal("HelloWorld", result);
        // ❌ To nie testuje niczego - tylko sprawdza setup mocka!
    }
}

// Supporting code
public interface IPaymentGateway
{
    bool Process(decimal amount);
}

public class PaymentService
{
    private readonly IPaymentGateway _gateway;
    public PaymentService(IPaymentGateway gateway) => _gateway = gateway;
    public bool ProcessPayment(decimal amount) => _gateway.Process(amount);
}

public interface IMessageSender
{
    void Send(string message);
}

public class NotificationService
{
    private readonly IMessageSender _sender;
    public NotificationService(IMessageSender sender) => _sender = sender;
    public void SendNotification(string message) => _sender.Send(message);
}

public interface IOrderRepository
{
    Order GetOrder(int id);
}

public class OrderValidator
{
    private readonly IOrderRepository _repository;
    public OrderValidator(IOrderRepository repository) => _repository = repository;
    public bool ValidateOrder(Order order)
    {
        var existing = _repository.GetOrder(order.Id);
        return existing != null;
    }
}

public interface IUserRepository
{
    User GetUser(int id);
}

public class UserService
{
    private readonly IUserRepository _repository;
    public UserService(IUserRepository repository) => _repository = repository;
    public string GetUserDetails(int id)
    {
        var user = _repository.GetUser(id);
        return $"{user.Name} - {user.Email}"; // NullReferenceException jeśli user == null
    }
}

public class DiscountCalculator
{
    public decimal Calculate(decimal amount, decimal rate)
    {
        return amount * rate;
    }
}

public interface IWeatherClient
{
    Task<double> GetTemperatureAsync(string city);
}

public class WeatherService
{
    private readonly IWeatherClient _client;
    public WeatherService(IWeatherClient client) => _client = client;
    public Task<double> GetTemperature(string city) => _client.GetTemperatureAsync(city);
}

public interface ITransactionRepository
{
    void Save(Transaction transaction);
}

public interface ILogger
{
    void Log(string message);
}

public class TransactionService
{
    private readonly ITransactionRepository _repository;
    private readonly ILogger _logger;
    
    public TransactionService(ITransactionRepository repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public void ExecuteTransaction(Transaction transaction)
    {
        _logger.Log("Starting transaction");
        _repository.Save(transaction);
        _logger.Log("Transaction completed");
    }
}

public interface IStringHelper
{
    string Concatenate(string a, string b);
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}

public class User
{
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Transaction { }
