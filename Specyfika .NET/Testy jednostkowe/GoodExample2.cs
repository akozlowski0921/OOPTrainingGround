// ✅ GOOD: Zaawansowane mockowanie z Moq - precyzyjne testy, callback, sequence

using Moq;
using Xunit;
using FluentAssertions;

// ✅ Rozwiązanie 1: Prawidłowy setup mocka
public class PaymentServiceTests
{
    [Fact]
    public void ProcessPayment_ValidAmount_ReturnsSuccess()
    {
        // ✅ Arrange - mock z setup
        var mockGateway = new Mock<IPaymentGateway>();
        mockGateway
            .Setup(g => g.Process(It.IsAny<decimal>()))
            .Returns(true); // ✅ Zwraca konkretną wartość
        
        var service = new PaymentService(mockGateway.Object);
        
        // ✅ Act
        var result = service.ProcessPayment(100m);
        
        // ✅ Assert
        result.Should().BeTrue();
        mockGateway.Verify(g => g.Process(100m), Times.Once);
    }
    
    [Fact]
    public void ProcessPayment_NegativeAmount_ReturnsFalse()
    {
        // ✅ Arrange - setup z warunkiem
        var mockGateway = new Mock<IPaymentGateway>();
        mockGateway
            .Setup(g => g.Process(It.Is<decimal>(amount => amount < 0)))
            .Returns(false);
        
        var service = new PaymentService(mockGateway.Object);
        
        // ✅ Act
        var result = service.ProcessPayment(-50m);
        
        // ✅ Assert
        result.Should().BeFalse();
    }
}

// ✅ Rozwiązanie 2: Weryfikacja wywołań metod
public class NotificationServiceTests
{
    [Fact]
    public void SendNotification_ValidMessage_CallsSenderWithCorrectMessage()
    {
        // ✅ Arrange
        var mockSender = new Mock<IMessageSender>();
        var service = new NotificationService(mockSender.Object);
        var message = "Test message";
        
        // ✅ Act
        service.SendNotification(message);
        
        // ✅ Assert - weryfikujemy wywołanie z konkretnym parametrem
        mockSender.Verify(s => s.Send(message), Times.Once);
        mockSender.Verify(s => s.Send(It.IsAny<string>()), Times.Once); // Alternatywnie
    }
    
    [Fact]
    public void SendMultipleNotifications_ThreeMessages_CallsSenderThreeTimes()
    {
        // ✅ Arrange
        var mockSender = new Mock<IMessageSender>();
        var service = new NotificationService(mockSender.Object);
        
        // ✅ Act
        service.SendNotification("Message 1");
        service.SendNotification("Message 2");
        service.SendNotification("Message 3");
        
        // ✅ Assert - weryfikujemy ilość wywołań
        mockSender.Verify(s => s.Send(It.IsAny<string>()), Times.Exactly(3));
    }
}

// ✅ Rozwiązanie 3: Precyzyjna weryfikacja argumentów
public class OrderValidatorTests
{
    [Fact]
    public void ValidateOrder_SpecificOrder_CallsRepositoryWithCorrectId()
    {
        // ✅ Arrange
        var mockRepository = new Mock<IOrderRepository>();
        mockRepository
            .Setup(r => r.GetOrder(123))
            .Returns(new Order { Id = 123, Quantity = 5 });
        
        var validator = new OrderValidator(mockRepository.Object);
        var order = new Order { Id = 123, Quantity = 5 };
        
        // ✅ Act
        validator.ValidateOrder(order);
        
        // ✅ Assert - weryfikujemy z konkretnym ID
        mockRepository.Verify(r => r.GetOrder(123), Times.Once);
        
        // ✅ Alternatywnie - weryfikacja z warunkiem
        mockRepository.Verify(r => r.GetOrder(It.Is<int>(id => id == 123)), Times.Once);
    }
}

// ✅ Rozwiązanie 4: Testowanie null handling
public class UserServiceTests
{
    [Fact]
    public void GetUserDetails_ExistingUser_ReturnsDetails()
    {
        // ✅ Arrange - mock zwraca konkretny obiekt
        var mockRepository = new Mock<IUserRepository>();
        mockRepository
            .Setup(r => r.GetUser(1))
            .Returns(new User { Id = 1, Name = "John", Email = "john@test.com" });
        
        var service = new UserService(mockRepository.Object);
        
        // ✅ Act
        var details = service.GetUserDetails(1);
        
        // ✅ Assert
        details.Should().Be("John - john@test.com");
    }
    
    [Fact]
    public void GetUserDetails_UserNotFound_ThrowsException()
    {
        // ✅ Arrange - mock zwraca null
        var mockRepository = new Mock<IUserRepository>();
        mockRepository
            .Setup(r => r.GetUser(999))
            .Returns((User)null);
        
        var service = new UserService(mockRepository.Object);
        
        // ✅ Act & Assert - testujemy NullReferenceException
        Assert.Throws<NullReferenceException>(() => service.GetUserDetails(999));
    }
}

// ✅ Rozwiązanie 5: Testowanie edge cases z Theory
public class DiscountCalculatorTests
{
    [Theory]
    [InlineData(100, 0.1, 10)]    // Normal case
    [InlineData(0, 0.1, 0)]       // Zero amount
    [InlineData(100, 0, 0)]       // Zero discount
    [InlineData(100, 1.0, 100)]   // 100% discount
    [InlineData(50, 0.5, 25)]     // 50% discount
    public void CalculateDiscount_VariousInputs_ReturnsCorrectDiscount(
        decimal amount, 
        decimal rate, 
        decimal expected)
    {
        // ✅ Arrange
        var calculator = new DiscountCalculator();
        
        // ✅ Act
        var result = calculator.Calculate(amount, rate);
        
        // ✅ Assert
        result.Should().Be(expected);
    }
    
    [Theory]
    [InlineData(-100, 0.1)]  // Negative amount
    [InlineData(100, -0.1)]  // Negative rate
    [InlineData(100, 1.5)]   // > 100% discount
    public void CalculateDiscount_InvalidInputs_ThrowsException(decimal amount, decimal rate)
    {
        // ✅ Arrange
        var calculator = new DiscountCalculator();
        
        // ✅ Act & Assert
        Assert.Throws<ArgumentException>(() => calculator.Calculate(amount, rate));
    }
}

// ✅ Rozwiązanie 6: Callback - różne wartości dla różnych parametrów
public class WeatherServiceTests
{
    [Fact]
    public async Task GetTemperature_DifferentCities_ReturnsDifferentTemperatures()
    {
        // ✅ Arrange - callback zwraca różne wartości
        var mockClient = new Mock<IWeatherClient>();
        mockClient
            .Setup(c => c.GetTemperatureAsync(It.IsAny<string>()))
            .ReturnsAsync((string city) => city switch
            {
                "London" => 15.0,
                "Tokyo" => 25.0,
                "Moscow" => -5.0,
                _ => 20.0
            });
        
        var service = new WeatherService(mockClient.Object);
        
        // ✅ Act
        var tempLondon = await service.GetTemperature("London");
        var tempTokyo = await service.GetTemperature("Tokyo");
        var tempMoscow = await service.GetTemperature("Moscow");
        
        // ✅ Assert - różne temperatury dla różnych miast
        tempLondon.Should().Be(15.0);
        tempTokyo.Should().Be(25.0);
        tempMoscow.Should().Be(-5.0);
    }
    
    [Fact]
    public async Task GetTemperature_ApiThrowsException_PropagatesException()
    {
        // ✅ Arrange - mock rzuca wyjątkiem
        var mockClient = new Mock<IWeatherClient>();
        mockClient
            .Setup(c => c.GetTemperatureAsync(It.IsAny<string>()))
            .ThrowsAsync(new HttpRequestException("API unavailable"));
        
        var service = new WeatherService(mockClient.Object);
        
        // ✅ Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetTemperature("London"));
    }
}

// ✅ Rozwiązanie 7: MockSequence - testowanie kolejności wywołań
public class TransactionServiceTests
{
    [Fact]
    public void ExecuteTransaction_ValidData_CallsMethodsInCorrectOrder()
    {
        // ✅ Arrange - MockSequence wymusza kolejność
        var mockRepository = new Mock<ITransactionRepository>();
        var mockLogger = new Mock<ILogger>();
        
        var sequence = new MockSequence();
        
        mockLogger.InSequence(sequence)
            .Setup(l => l.Log("Starting transaction"));
        
        mockRepository.InSequence(sequence)
            .Setup(r => r.Save(It.IsAny<Transaction>()));
        
        mockLogger.InSequence(sequence)
            .Setup(l => l.Log("Transaction completed"));
        
        var service = new TransactionService(mockRepository.Object, mockLogger.Object);
        
        // ✅ Act
        service.ExecuteTransaction(new Transaction());
        
        // ✅ Assert - wszystkie wywołania muszą być w poprawnej kolejności
        mockLogger.Verify(l => l.Log("Starting transaction"), Times.Once);
        mockRepository.Verify(r => r.Save(It.IsAny<Transaction>()), Times.Once);
        mockLogger.Verify(l => l.Log("Transaction completed"), Times.Once);
    }
}

// ✅ Rozwiązanie 8: Nie mockuj prostych klas - testuj prawdziwą implementację
public class StringHelperTests
{
    [Fact]
    public void Concatenate_TwoStrings_ReturnsConcatenated()
    {
        // ✅ Testuj prawdziwą implementację
        var helper = new StringHelper(); // Nie mock!
        
        var result = helper.Concatenate("Hello", "World");
        
        result.Should().Be("HelloWorld");
    }
    
    [Theory]
    [InlineData("", "", "")]
    [InlineData("Hello", "", "Hello")]
    [InlineData("", "World", "World")]
    [InlineData("Test", "123", "Test123")]
    public void Concatenate_VariousInputs_ReturnsCorrectResult(
        string first, 
        string second, 
        string expected)
    {
        // ✅ Arrange
        var helper = new StringHelper();
        
        // ✅ Act
        var result = helper.Concatenate(first, second);
        
        // ✅ Assert
        result.Should().Be(expected);
    }
}

// ✅ Rozwiązanie 9: Callback z weryfikacją argumentów
public class EmailServiceTests
{
    [Fact]
    public async Task SendEmail_ValidRecipient_PassesCorrectDataToClient()
    {
        // ✅ Arrange
        var mockClient = new Mock<IEmailClient>();
        string capturedRecipient = null;
        string capturedSubject = null;
        
        mockClient
            .Setup(c => c.SendAsync(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()))
            .Callback<string, string, string>((recipient, subject, body) =>
            {
                // ✅ Callback przechwytuje argumenty
                capturedRecipient = recipient;
                capturedSubject = subject;
            })
            .ReturnsAsync(true);
        
        var service = new EmailService(mockClient.Object);
        
        // ✅ Act
        await service.SendWelcomeEmail("test@test.com", "John");
        
        // ✅ Assert - sprawdzamy przechwycone wartości
        capturedRecipient.Should().Be("test@test.com");
        capturedSubject.Should().Contain("Welcome");
    }
}

// ✅ Rozwiązanie 10: Setup z Returns vs ReturnsAsync
public class DataServiceTests
{
    [Fact]
    public async Task GetDataAsync_ReturnsData()
    {
        // ✅ Arrange - ReturnsAsync dla async metod
        var mockRepository = new Mock<IDataRepository>();
        mockRepository
            .Setup(r => r.FetchAsync())
            .ReturnsAsync("test data"); // ✅ ReturnsAsync, nie Returns(Task.FromResult(...))
        
        var service = new DataService(mockRepository.Object);
        
        // ✅ Act
        var result = await service.GetDataAsync();
        
        // ✅ Assert
        result.Should().Be("test data");
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
        return $"{user.Name} - {user.Email}";
    }
}

public class DiscountCalculator
{
    public decimal Calculate(decimal amount, decimal rate)
    {
        if (amount < 0 || rate < 0 || rate > 1.0m)
            throw new ArgumentException("Invalid arguments");
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

public class StringHelper
{
    public string Concatenate(string a, string b) => a + b;
}

public interface IEmailClient
{
    Task<bool> SendAsync(string recipient, string subject, string body);
}

public class EmailService
{
    private readonly IEmailClient _client;
    public EmailService(IEmailClient client) => _client = client;
    
    public async Task SendWelcomeEmail(string email, string name)
    {
        await _client.SendAsync(email, $"Welcome {name}!", $"Hello {name}, welcome to our service!");
    }
}

public interface IDataRepository
{
    Task<string> FetchAsync();
}

public class DataService
{
    private readonly IDataRepository _repository;
    public DataService(IDataRepository repository) => _repository = repository;
    public Task<string> GetDataAsync() => _repository.FetchAsync();
}

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}

public class Transaction { }
