// ✅ GOOD: Prawidłowe testy jednostkowe z mockowaniem i izolacją

using Xunit;
using Moq;
using FluentAssertions;

// ✅ Rozwiązanie 1: Mockowanie zależności z użyciem Moq
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IEmailService> _mockEmailService;
    private readonly Mock<IOrderValidator> _mockValidator;
    private readonly OrderService _sut; // System Under Test
    
    public OrderServiceTests()
    {
        // ✅ Setup - tworzymy mocki dla każdego testu
        _mockRepository = new Mock<IOrderRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockValidator = new Mock<IOrderValidator>();
        
        _sut = new OrderService(
            _mockRepository.Object,
            _mockEmailService.Object,
            _mockValidator.Object);
    }
    
    [Fact]
    public async Task ProcessOrder_ValidOrder_SavesAndSendsEmail()
    {
        // ✅ Arrange - kontrolujemy zachowanie mocków
        var order = new Order 
        { 
            Quantity = 1, 
            CustomerEmail = "test@test.com" 
        };
        
        _mockValidator
            .Setup(v => v.Validate(order))
            .Returns(true);
        
        _mockRepository
            .Setup(r => r.SaveAsync(order))
            .ReturnsAsync(true);
        
        _mockEmailService
            .Setup(e => e.SendConfirmationAsync(order.CustomerEmail, order))
            .ReturnsAsync(true);
        
        // ✅ Act
        await _sut.ProcessOrderAsync(order);
        
        // ✅ Assert - weryfikujemy że metody zostały wywołane
        _mockRepository.Verify(r => r.SaveAsync(order), Times.Once);
        _mockEmailService.Verify(
            e => e.SendConfirmationAsync(order.CustomerEmail, order), 
            Times.Once);
    }
    
    [Fact]
    public async Task ProcessOrder_InvalidOrder_ThrowsException()
    {
        // ✅ Arrange
        var order = new Order { Quantity = -1 };
        
        _mockValidator
            .Setup(v => v.Validate(order))
            .Returns(false);
        
        // ✅ Act & Assert - testowanie wyjątków
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ProcessOrderAsync(order));
        
        // ✅ Verify że Save nie został wywołany
        _mockRepository.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never);
    }
    
    [Theory] // ✅ Testowanie z różnymi danymi wejściowymi
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task ProcessOrder_InvalidQuantity_ThrowsException(int quantity)
    {
        // ✅ Arrange
        var order = new Order { Quantity = quantity };
        
        _mockValidator
            .Setup(v => v.Validate(order))
            .Returns(false);
        
        // ✅ Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.ProcessOrderAsync(order));
    }
}

// ✅ Rozwiązanie 2: Testowanie z dependency injection i abstrakcją czasu
public interface IDateTimeProvider
{
    DateTime Now { get; }
}

public class DiscountService
{
    private readonly IDateTimeProvider _dateTimeProvider;
    
    public DiscountService(IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
    }
    
    public decimal GetWeekendDiscount()
    {
        // ✅ Używa wstrzykniętego provider zamiast DateTime.Now
        var dayOfWeek = _dateTimeProvider.Now.DayOfWeek;
        return dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday 
            ? 0.2m 
            : 0m;
    }
}

public class DiscountServiceTests
{
    [Fact]
    public void GetWeekendDiscount_Saturday_Returns20Percent()
    {
        // ✅ Arrange - kontrolujemy czas
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider
            .Setup(d => d.Now)
            .Returns(new DateTime(2024, 1, 6)); // Sobota
        
        var service = new DiscountService(mockDateTimeProvider.Object);
        
        // ✅ Act
        var discount = service.GetWeekendDiscount();
        
        // ✅ Assert
        Assert.Equal(0.2m, discount);
    }
    
    [Fact]
    public void GetWeekendDiscount_Monday_ReturnsZero()
    {
        // ✅ Arrange
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        mockDateTimeProvider
            .Setup(d => d.Now)
            .Returns(new DateTime(2024, 1, 1)); // Poniedziałek
        
        var service = new DiscountService(mockDateTimeProvider.Object);
        
        // ✅ Act
        var discount = service.GetWeekendDiscount();
        
        // ✅ Assert
        Assert.Equal(0m, discount);
    }
}

// ✅ Rozwiązanie 3: FluentAssertions dla lepszej czytelności
public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockRepository;
    private readonly UserService _sut;
    
    public UserServiceTests()
    {
        _mockRepository = new Mock<IUserRepository>();
        _sut = new UserService(_mockRepository.Object);
    }
    
    [Fact]
    public async Task CreateUser_ValidData_CreatesUserWithCorrectProperties()
    {
        // ✅ Arrange
        var firstName = "John";
        var lastName = "Doe";
        var email = "john@test.com";
        
        // ✅ Act
        var user = await _sut.CreateUserAsync(firstName, lastName, email);
        
        // ✅ Assert - FluentAssertions dla lepszej czytelności
        user.Should().NotBeNull();
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.Email.Should().Be(email);
        user.Id.Should().NotBe(Guid.Empty);
        user.IsActive.Should().BeTrue();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
    
    [Fact]
    public async Task CreateUser_ValidData_SavesUser()
    {
        // ✅ Arrange
        _mockRepository
            .Setup(r => r.SaveAsync(It.IsAny<User>()))
            .ReturnsAsync(true);
        
        // ✅ Act
        await _sut.CreateUserAsync("John", "Doe", "john@test.com");
        
        // ✅ Assert - verify z argument matching
        _mockRepository.Verify(r => r.SaveAsync(
            It.Is<User>(u => 
                u.FirstName == "John" && 
                u.LastName == "Doe" && 
                u.Email == "john@test.com")),
            Times.Once);
    }
}

// ✅ Rozwiązanie 4: Prawidłowe testowanie metod asynchronicznych
public class AsyncServiceTests
{
    [Fact]
    public async Task GetDataAsync_ReturnsData() // ✅ async Task
    {
        // ✅ Arrange
        var mockRepository = new Mock<IDataRepository>();
        mockRepository
            .Setup(r => r.FetchDataAsync())
            .ReturnsAsync("test data");
        
        var service = new DataService(mockRepository.Object);
        
        // ✅ Act - używamy await
        var result = await service.GetDataAsync();
        
        // ✅ Assert
        result.Should().Be("test data");
    }
    
    [Fact]
    public async Task GetDataAsync_ThrowsException_HandlesGracefully()
    {
        // ✅ Arrange
        var mockRepository = new Mock<IDataRepository>();
        mockRepository
            .Setup(r => r.FetchDataAsync())
            .ThrowsAsync(new HttpRequestException("Network error"));
        
        var service = new DataService(mockRepository.Object);
        
        // ✅ Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(
            () => service.GetDataAsync());
    }
}

// ✅ Rozwiązanie 5: Testy z pełną izolacją
public class CalculatorTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        // ✅ Arrange - każdy test ma własną instancję
        var calculator = new Calculator();
        calculator.Value = 5;
        
        // ✅ Act
        calculator.Add(3);
        
        // ✅ Assert
        calculator.Value.Should().Be(8);
    }
    
    [Fact]
    public void Subtract_TwoNumbers_ReturnsDifference()
    {
        // ✅ Arrange - nowa instancja, nie współdzieli state
        var calculator = new Calculator();
        calculator.Value = 10;
        
        // ✅ Act
        calculator.Subtract(2);
        
        // ✅ Assert
        calculator.Value.Should().Be(8);
    }
    
    [Theory]
    [InlineData(5, 3, 8)]
    [InlineData(10, 20, 30)]
    [InlineData(-5, 5, 0)]
    public void Add_VariousInputs_ReturnsCorrectSum(int initial, int toAdd, int expected)
    {
        // ✅ Arrange
        var calculator = new Calculator { Value = initial };
        
        // ✅ Act
        calculator.Add(toAdd);
        
        // ✅ Assert
        calculator.Value.Should().Be(expected);
    }
}

// ✅ Rozwiązanie 6: Używanie CollectionData dla złożonych test cases
public class PriceCalculatorTests
{
    public static IEnumerable<object[]> PriceTestData =>
        new List<object[]>
        {
            new object[] { 100m, 0.1m, 90m },
            new object[] { 50m, 0.2m, 40m },
            new object[] { 75m, 0m, 75m },
        };
    
    [Theory]
    [MemberData(nameof(PriceTestData))]
    public void CalculateDiscountedPrice_VariousInputs_ReturnsCorrectPrice(
        decimal originalPrice, 
        decimal discountRate, 
        decimal expectedPrice)
    {
        // ✅ Arrange
        var calculator = new PriceCalculator();
        
        // ✅ Act
        var result = calculator.CalculateDiscountedPrice(originalPrice, discountRate);
        
        // ✅ Assert
        result.Should().Be(expectedPrice);
    }
}

// ✅ Rozwiązanie 7: Fixture dla setup współdzielony między testami
public class OrderProcessorTestFixture : IDisposable
{
    public Mock<IOrderRepository> MockRepository { get; }
    public Mock<IEmailService> MockEmailService { get; }
    
    public OrderProcessorTestFixture()
    {
        MockRepository = new Mock<IOrderRepository>();
        MockEmailService = new Mock<IEmailService>();
    }
    
    public void Dispose()
    {
        // Cleanup jeśli potrzebny
    }
}

public class OrderProcessorTests : IClassFixture<OrderProcessorTestFixture>
{
    private readonly OrderProcessorTestFixture _fixture;
    
    public OrderProcessorTests(OrderProcessorTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task Process_ValidOrder_Succeeds()
    {
        // ✅ Używamy fixture dla shared setup
        var processor = new OrderProcessor(
            _fixture.MockRepository.Object,
            _fixture.MockEmailService.Object);
        
        // Test logic
    }
}

// Supporting interfaces and classes
public interface IOrderRepository
{
    Task<bool> SaveAsync(Order order);
}

public interface IEmailService
{
    Task<bool> SendConfirmationAsync(string email, Order order);
}

public interface IOrderValidator
{
    bool Validate(Order order);
}

public interface IUserRepository
{
    Task<bool> SaveAsync(User user);
}

public interface IDataRepository
{
    Task<string> FetchDataAsync();
}

public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly IEmailService _emailService;
    private readonly IOrderValidator _validator;
    
    public OrderService(
        IOrderRepository repository,
        IEmailService emailService,
        IOrderValidator validator)
    {
        _repository = repository;
        _emailService = emailService;
        _validator = validator;
    }
    
    public async Task ProcessOrderAsync(Order order)
    {
        if (!_validator.Validate(order))
            throw new InvalidOperationException("Invalid order");
        
        await _repository.SaveAsync(order);
        await _emailService.SendConfirmationAsync(order.CustomerEmail, order);
    }
}

public class UserService
{
    private readonly IUserRepository _repository;
    
    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<User> CreateUserAsync(string firstName, string lastName, string email)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        await _repository.SaveAsync(user);
        return user;
    }
}

public class DataService
{
    private readonly IDataRepository _repository;
    
    public DataService(IDataRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<string> GetDataAsync()
    {
        return await _repository.FetchDataAsync();
    }
}

public class Calculator
{
    public int Value { get; set; }
    public void Add(int x) => Value += x;
    public void Subtract(int x) => Value -= x;
}

public class PriceCalculator
{
    public decimal CalculateDiscountedPrice(decimal price, decimal discountRate)
    {
        return price * (1 - discountRate);
    }
}

public class OrderProcessor
{
    public OrderProcessor(IOrderRepository repo, IEmailService email) { }
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
