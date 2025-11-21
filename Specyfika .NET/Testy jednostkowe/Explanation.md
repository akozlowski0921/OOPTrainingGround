# Testy jednostkowe w .NET

## Problemy w BadExample

### 1. Testy z zależnościami zewnętrznymi
```csharp
var dbContext = new AppDbContext(connectionString); // ❌ Prawdziwa baza
var emailService = new SmtpEmailService("smtp.gmail.com"); // ❌ Prawdziwy SMTP
```
**Problemy:**
- Testy są wolne (I/O operations)
- Zależą od infrastruktury (DB musi działać)
- Mogą failować z powodów zewnętrznych
- Trudno testować edge cases
- Mogą modyfikować dane produkcyjne

### 2. Brak mockowania
```csharp
var mockGateway = new Mock<IPaymentGateway>();
// ❌ Brak setup - zwraca default values
var result = service.ProcessPayment(100m);
Assert.False(result); // ❌ Test nic nie testuje
```
**Problem:** Mock bez setup zwraca wartości domyślne (null, false, 0) co prowadzi do fałszywych pozytywów.

### 3. Brak weryfikacji wywołań
```csharp
service.SendNotification("Test message");
Assert.True(true); // ❌ Pusty assert
```
**Problem:** Test przechodzi ale nie sprawdzamy czy metoda została faktycznie wywołana.

### 4. Testy zależne od czasu
```csharp
var discount = service.GetWeekendDiscount();
Assert.Equal(0.2m, discount); // ❌ Przechodzi tylko w weekend
```
**Problem:** Test jest niestabilny - wynik zależy od dnia tygodnia.

### 5. Zbyt wiele assertów w jednym teście
```csharp
Assert.Equal("John", user.FirstName);
Assert.Equal("Doe", user.LastName);
Assert.Equal("john@test.com", user.Email);
Assert.NotEqual(Guid.Empty, user.Id);
Assert.True(user.IsActive);
// ❌ Jeśli pierwszy failuje, nie wiemy co z resztą
```
**Problem:** Pierwszy failujący assert przerywa test - nie wiemy które inne warunki są złamane.

### 6. Synchroniczne testowanie async kodu
```csharp
public void GetDataAsync_ReturnsData() // ❌ Brak async
{
    var result = service.GetDataAsync().Result; // ❌ Deadlock risk
}
```
**Problem:** `.Result` może powodować deadlocki, szczególnie w GUI i ASP.NET pre-Core.

### 7. Współdzielony state między testami
```csharp
private static Calculator _calculator = new Calculator(); // ❌ Shared
```
**Problem:** Testy mogą wpływać na siebie - kolejność wykonania ma znaczenie.

## Rozwiązania w GoodExample

### 1. Mockowanie z Moq
```csharp
var mockRepository = new Mock<IOrderRepository>();
mockRepository
    .Setup(r => r.SaveAsync(order))
    .ReturnsAsync(true);

// Weryfikacja
mockRepository.Verify(r => r.SaveAsync(order), Times.Once);
```
**Korzyści:**
- Pełna kontrola nad zachowaniem dependencies
- Szybkie testy (bez I/O)
- Możliwość testowania edge cases
- Weryfikacja wywołań metod

### 2. Abstrakcja czasu
```csharp
public interface IDateTimeProvider
{
    DateTime Now { get; }
}

// W teście
mockDateTimeProvider
    .Setup(d => d.Now)
    .Returns(new DateTime(2024, 1, 6)); // Sobota
```
**Korzyści:**
- Stabilne testy - nie zależą od aktualnej daty
- Możliwość testowania różnych scenariuszy czasowych

### 3. Theory - parametryzowane testy
```csharp
[Theory]
[InlineData(1, 2, 3)]
[InlineData(5, 10, 15)]
[InlineData(-1, 1, 0)]
public void Add_VariousInputs_ReturnsCorrectSum(int a, int b, int expected)
{
    var result = calculator.Add(a, b);
    result.Should().Be(expected);
}
```
**Korzyści:**
- Jeden test dla wielu przypadków
- Łatwe dodawanie nowych test cases
- Mniejsza duplikacja kodu

### 4. FluentAssertions
```csharp
user.Should().NotBeNull();
user.FirstName.Should().Be(firstName);
user.Email.Should().Be(email);
user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
```
**Korzyści:**
- Czytelniejsze asercje
- Lepsze komunikaty błędów
- Więcej assertion methods

### 5. Prawidłowe testowanie async
```csharp
[Fact]
public async Task GetDataAsync_ReturnsData() // ✅ async Task
{
    var result = await service.GetDataAsync(); // ✅ await
    result.Should().Be("test data");
}
```
**Korzyści:**
- Brak deadlocków
- Prawidłowa obsługa async flow
- Lepsze performance

### 6. Izolacja testów
```csharp
public class CalculatorTests
{
    [Fact]
    public void Add_TwoNumbers_ReturnsSum()
    {
        var calculator = new Calculator(); // ✅ Nowa instancja
        // Test logic
    }
}
```
**Korzyści:**
- Testy są niezależne
- Kolejność wykonania nie ma znaczenia
- Łatwiejsze debugowanie

## Frameworki testowe

### xUnit (Zalecany)
- Nowoczesny design
- Brak static state
- Używa konstruktora dla setup, IDisposable dla cleanup
- Parallel execution domyślnie
- `[Fact]` dla pojedynczych testów, `[Theory]` dla parametryzowanych

**Setup/Teardown:**
```csharp
public class DatabaseTests : IDisposable
{
    private readonly Database _database;
    
    public DatabaseTests() // Setup
    {
        _database = new Database();
    }
    
    public void Dispose() // Teardown
    {
        _database?.Dispose();
    }
}
```

### NUnit
- Bogaty API
- `[TestFixture]` dla klas testowych
- `[SetUp]`/`[TearDown]` dla przed/po każdym teście
- `[OneTimeSetUp]`/`[OneTimeTearDown]` dla przed/po wszystkich testach
- `[TestCase]` dla parametryzowanych testów
- CollectionAssert dla kolekcji

### MSTest
- Framework od Microsoft
- `[TestClass]` dla klas testowych
- `[TestInitialize]`/`[TestCleanup]` dla przed/po każdym teście
- `[ClassInitialize]`/`[ClassCleanup]` dla przed/po wszystkich testach
- `[DataTestMethod]` z `[DataRow]` dla parametryzowanych testów

## Moq - Zaawansowane funkcje

### Setup z warunkami
```csharp
mockGateway
    .Setup(g => g.Process(It.Is<decimal>(amount => amount > 0)))
    .Returns(true);
```

### Callback
```csharp
mockClient
    .Setup(c => c.GetTemperatureAsync(It.IsAny<string>()))
    .ReturnsAsync((string city) => city switch
    {
        "London" => 15.0,
        "Tokyo" => 25.0,
        _ => 20.0
    });
```

### Przechwytywanie argumentów
```csharp
string capturedEmail = null;
mockClient
    .Setup(c => c.SendAsync(It.IsAny<string>(), It.IsAny<string>()))
    .Callback<string, string>((email, message) =>
    {
        capturedEmail = email;
    })
    .ReturnsAsync(true);
```

### MockSequence - kolejność wywołań
```csharp
var sequence = new MockSequence();

mockLogger.InSequence(sequence)
    .Setup(l => l.Log("Starting"));

mockRepository.InSequence(sequence)
    .Setup(r => r.Save(It.IsAny<Order>()));

mockLogger.InSequence(sequence)
    .Setup(l => l.Log("Completed"));
```

### Weryfikacja
```csharp
// Podstawowa
mockRepository.Verify(r => r.Save(order), Times.Once);

// Z warunkiem
mockRepository.Verify(r => r.Save(
    It.Is<Order>(o => o.Quantity > 0)), Times.Once);

// Nigdy nie wywołana
mockRepository.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);

// Co najmniej raz
mockRepository.Verify(r => r.Save(It.IsAny<Order>()), Times.AtLeastOnce);
```

## Kategoryzacja testów

### xUnit Traits
```csharp
[Fact]
[Trait("Category", "Unit")]
[Trait("Speed", "Fast")]
public void FastUnitTest() { }
```
Uruchomienie: `dotnet test --filter "Category=Unit"`

### NUnit Category
```csharp
[Test]
[Category("Unit")]
[Category("Fast")]
public void FastUnitTest() { }
```

### MSTest TestCategory
```csharp
[TestMethod]
[TestCategory("Unit")]
[TestCategory("Fast")]
public void FastUnitTest() { }
```

## Best Practices

### Nazewnictwo
**Pattern:** `MethodName_Scenario_ExpectedResult`

**Przykłady:**
- `Add_TwoPositiveNumbers_ReturnsPositiveSum`
- `GetUser_NonExistentId_ThrowsNotFoundException`
- `ValidateEmail_InvalidFormat_ReturnsFalse`

### AAA Pattern
```csharp
[Fact]
public void Test()
{
    // Arrange - przygotowanie danych i mocków
    var calculator = new Calculator();
    
    // Act - wywołanie testowanej metody
    var result = calculator.Add(5, 3);
    
    // Assert - sprawdzenie wyniku
    result.Should().Be(8);
}
```

### Jeden concept na test
❌ **Źle:**
```csharp
[Fact]
public void UserService_AllOperations_Work()
{
    // Testuje create, update, delete w jednym teście
}
```

✅ **Dobrze:**
```csharp
[Fact]
public void CreateUser_ValidData_CreatesUser() { }

[Fact]
public void UpdateUser_ExistingUser_UpdatesSuccessfully() { }

[Fact]
public void DeleteUser_ExistingUser_DeletesSuccessfully() { }
```

### DRY w testach
Używaj helper methods, fixtures, ale zachowuj czytelność:
```csharp
private OrderService CreateSystemUnderTest()
{
    var mockRepository = new Mock<IOrderRepository>();
    var mockEmailService = new Mock<IEmailService>();
    return new OrderService(mockRepository.Object, mockEmailService.Object);
}
```

### Co mockować, a czego nie

**✅ Mockuj:**
- External dependencies (DB, HTTP, File System)
- Services z własną logiką
- Dependencies z side effects

**❌ Nie mockuj:**
- DTOs, value objects
- Prostych klas bez dependencies
- .NET framework classes (String, DateTime - użyj abstrakcji)

## Testowanie edge cases

Zawsze testuj:
1. **Happy path** - normalny scenariusz
2. **Boundary values** - wartości graniczne (0, -1, max, min)
3. **Null values** - null parameters
4. **Empty collections** - puste listy/arrays
5. **Exceptions** - spodziewane wyjątki
6. **Invalid input** - nieprawidłowe dane

**Przykład:**
```csharp
[Theory]
[InlineData(100, 0.1, 10)]    // Normal
[InlineData(0, 0.1, 0)]       // Zero amount
[InlineData(100, 0, 0)]       // Zero discount
[InlineData(100, 1.0, 100)]   // 100% discount
public void CalculateDiscount_VariousInputs_ReturnsCorrectDiscount(
    decimal amount, decimal rate, decimal expected)
```

## Testowanie wyjątków

### xUnit
```csharp
var exception = Assert.Throws<ArgumentException>(
    () => calculator.Divide(10, 0));

exception.Message.Should().Contain("divide by zero");
```

### NUnit
```csharp
var exception = Assert.Throws<ArgumentException>(
    () => calculator.Divide(10, 0));

Assert.That(exception.Message, Does.Contain("divide by zero"));
```

### MSTest
```csharp
var exception = Assert.ThrowsException<ArgumentException>(
    () => calculator.Divide(10, 0));

StringAssert.Contains(exception.Message, "divide by zero");
```

## Code Coverage

**Target:** 70-80% code coverage, ale jakość > ilość

**Tools:**
- Coverlet - dla .NET Core
- dotCover - JetBrains
- Visual Studio Code Coverage

**Command:**
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## Anti-patterns

### 1. Test tylko happy path
❌ Testuj tylko pozytywne scenariusze

### 2. Ignorowanie test failures
❌ Komentowanie failujących testów

### 3. Flaky tests
❌ Testy które czasem przechodzą, czasem nie

### 4. Slow tests
❌ Testy które trwają minuty

### 5. Integration tests jako unit tests
❌ Testy używające prawdziwej DB/HTTP w unit tests

### 6. Testing implementation details
❌ Testowanie private methods zamiast public API

## Podsumowanie

Dobre testy jednostkowe:
- Są **szybkie** (< 100ms per test)
- Są **niezależne** (mogą być uruchomione w dowolnej kolejności)
- Są **powtarzalne** (ten sam wynik przy każdym uruchomieniu)
- Są **samowalidujące** (pass/fail bez manualnej weryfikacji)
- Są **terminowe** (pisane przed lub wraz z kodem produkcyjnym)

Używaj:
- ✅ Moq dla mockowania
- ✅ FluentAssertions dla czytelnych assertów
- ✅ Theory/TestCase dla parametryzowanych testów
- ✅ AAA pattern dla struktury
- ✅ Dobrego nazewnictwa
- ✅ Kategoryzacji dla różnych typów testów
