# Dependency Injection w ASP.NET Core

## Problemy w BadExample

### 1. Ręczne tworzenie zależności (Tight Coupling)
```csharp
public OrderController()
{
    var dbConnection = new SqlConnection("Server=localhost;Database=Shop");
    var orderRepository = new OrderRepository(dbConnection);
    _orderService = new OrderService(orderRepository, emailService);
}
```
**Problemy:**
- Tight coupling do konkretnych implementacji
- Niemożliwe do przetestowania (nie można mockować)
- Trudne do zmiany implementacji
- Naruszenie SOLID (DIP - Dependency Inversion Principle)
- Brak kontroli nad czasem życia obiektów

### 2. Brak interfejsów
```csharp
public class OrderService
{
    private readonly OrderRepository _repository; // ❌ Konkretna klasa
}
```
**Problemy:**
- Niemożliwe podstawienie innej implementacji
- Testy wymagają prawdziwej bazy danych
- Naruszenie zasady programowania do interfejsów

### 3. Hardcoded konfiguracja
```csharp
public SmtpEmailService(string smtpServer)
{
    _smtpServer = smtpServer; // ❌ Hardcoded w kodzie
}
```
**Problemy:**
- Brak możliwości zmiany bez rekompilacji
- Różne środowiska (dev, staging, prod) wymagają zmian w kodzie

## Rozwiązania w GoodExample

### 1. Constructor Injection
```csharp
public OrderController(
    IOrderService orderService,
    ILogger<OrderController> logger)
{
    _orderService = orderService;
    _logger = logger;
}
```
**Korzyści:**
- DI Container automatycznie wstrzykuje zależności
- Explicit dependencies - widoczne w sygnaturze konstruktora
- Łatwe testowanie - można przekazać mocki
- Immutability - dependencies są readonly

### 2. Programowanie do interfejsów
```csharp
public interface IOrderRepository
{
    Task SaveAsync(Order order);
}

public class OrderRepository : IOrderRepository
{
    // Implementacja
}
```
**Korzyści:**
- Loose coupling
- Możliwość podstawienia różnych implementacji
- Łatwe mockowanie w testach
- Zgodność z SOLID (DIP)

### 3. Options Pattern
```csharp
public SmtpEmailService(
    IOptions<SmtpSettings> settings,
    ILogger<SmtpEmailService> logger)
{
    _settings = settings.Value;
}
```
**Korzyści:**
- Type-safe konfiguracja
- Łatwe testowanie
- Możliwość hot-reload (IOptionsSnapshot/IOptionsMonitor)
- Separacja konfiguracji od kodu

## Service Lifetimes

### Transient
```csharp
services.AddTransient<IOrderValidator, OrderValidator>();
```
- **Nowa instancja** przy każdym wstrzyknięciu
- **Użycie:** Lightweight, stateless services
- **Uwaga:** Może prowadzić do dużej liczby alokacji

### Scoped
```csharp
services.AddScoped<IOrderService, OrderService>();
```
- **Jedna instancja na scope** (HTTP request w ASP.NET Core)
- **Użycie:** DbContext, Unit of Work, services z state per request
- **Uwaga:** Prawidłowy disposal na końcu scope'a

### Singleton
```csharp
services.AddSingleton<IEmailService, SmtpEmailService>();
```
- **Jedna instancja przez cały czas działania** aplikacji
- **Użycie:** Cache, configuration, stateless services
- **Uwaga:** Musi być thread-safe

## Captive Dependency - Anti-pattern

```
Singleton → Scoped    ❌ BŁĄD!
Singleton → Transient ❌ MOŻE BYĆ PROBLEM
Scoped → Transient    ✅ OK
Scoped → Singleton    ✅ OK
```

### Problem:
```csharp
// ❌ ReportGenerator jest Singleton
public class ReportGenerator
{
    private readonly AppDbContext _dbContext; // ❌ DbContext jest Scoped
}
```

**Skutki:**
- DbContext żyje przez cały czas aplikacji (memory leak)
- Concurrency issues - DbContext nie jest thread-safe
- Stale dane w cache EF

### Rozwiązanie: IServiceScopeFactory
```csharp
public class ReportGenerator
{
    private readonly IServiceScopeFactory _scopeFactory;
    
    public async Task<Report> GenerateReportAsync()
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        // Używamy DbContext w scope
    }
}
```

## Circular Dependency

### Problem:
```
ServiceA → ServiceB → ServiceA
```
DI Container rzuci: `"A circular dependency was detected"`

### Rozwiązania:

#### 1. Mediator Pattern
```csharp
ServiceA → Coordinator ← ServiceB
```
Wprowadzamy trzeci serwis, który zarządza komunikacją.

#### 2. Event-based Communication
```csharp
OrderService → EventBus ← NotificationService
```
Services nie zależą bezpośrednio od siebie, komunikują się przez eventy.

#### 3. Lazy<T>
```csharp
public ServiceA(Lazy<IServiceB> lazyServiceB)
{
    _lazyServiceB = lazyServiceB;
}
```
Opóźnione tworzenie zależności.

#### 4. Property Injection (ostateczność)
```csharp
public class ServiceA
{
    public IServiceB ServiceB { get; set; }
}
```
**Uwaga:** Używaj tylko w ostateczności, preferuj constructor injection.

## HttpClient i IHttpClientFactory

### ❌ Problem:
```csharp
using var client = new HttpClient(); // Socket exhaustion!
```

### ✅ Rozwiązanie:
```csharp
services.AddHttpClient<IApiService, ApiService>(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

**Korzyści:**
- Prawidłowe zarządzanie connection pool
- Brak socket exhaustion
- Built-in retry policies (Polly)

## Service Locator Anti-pattern

### ❌ Problem:
```csharp
public class OrderProcessor
{
    private readonly IServiceProvider _serviceProvider;
    
    public void ProcessOrder()
    {
        var validator = _serviceProvider.GetService<IOrderValidator>();
    }
}
```

### ✅ Rozwiązanie:
```csharp
public class OrderProcessor
{
    private readonly IOrderValidator _validator;
    
    public OrderProcessor(IOrderValidator validator)
    {
        _validator = validator;
    }
}
```

**Dlaczego Service Locator to anti-pattern?**
- Ukrywa dependencies
- Trudniejsze testowanie
- Runtime errors zamiast compile-time
- Naruszenie Explicit Dependencies Principle

## Lifetime Guidelines

| Service Type | Recommended Lifetime | Reason |
|-------------|---------------------|---------|
| DbContext | Scoped | Per-request unit of work |
| HttpClient (via Factory) | Transient/Scoped | Managed connection pooling |
| Configuration/Cache | Singleton | Shared, immutable state |
| Stateless services | Transient | No state, lightweight |
| Per-request state | Scoped | User-specific data |
| IDisposable services | Scoped | Proper disposal |
| Logger | Singleton | Thread-safe, stateless |

## Best Practices

### DO:
✅ Używaj Constructor Injection  
✅ Programuj do interfejsów  
✅ Używaj Options Pattern dla konfiguracji  
✅ Wybieraj odpowiedni lifetime dla service  
✅ Używaj IHttpClientFactory dla HttpClient  
✅ Waliduj null w konstruktorach (`?? throw new ArgumentNullException`)  
✅ Używaj IServiceScopeFactory w Singleton dla Scoped dependencies  

### DON'T:
❌ Nie twórz obiektów ręcznie (new) dla serwisów  
❌ Nie wstrzykuj Scoped do Singleton (captive dependency)  
❌ Nie używaj Service Locator pattern  
❌ Nie hardcoduj konfiguracji  
❌ Nie twórz nowych HttpClient instances  
❌ Nie ignoruj circular dependencies - refaktoruj design  

## Testowanie z DI

```csharp
[Fact]
public async Task ProcessOrder_ValidOrder_SavesSuccessfully()
{
    // Arrange
    var mockRepository = new Mock<IOrderRepository>();
    var mockEmailService = new Mock<IEmailService>();
    var validator = new OrderValidator();
    
    var service = new OrderService(
        mockRepository.Object,
        mockEmailService.Object,
        validator);
    
    var order = new Order { Quantity = 1, CustomerEmail = "test@test.com" };
    
    // Act
    await service.ProcessOrderAsync(order);
    
    // Assert
    mockRepository.Verify(r => r.SaveAsync(order), Times.Once);
    mockEmailService.Verify(e => 
        e.SendConfirmationAsync(order.CustomerEmail, order), Times.Once);
}
```

**Korzyści DI w testach:**
- Łatwe mockowanie dependencies
- Testy jednostkowe bez infrastruktury (DB, email)
- Szybkie wykonanie testów
- Testowanie edge cases

## Diagnostyka problemów

### Symptomy Captive Dependency:
- `InvalidOperationException: Cannot access a disposed object`
- Memory leaks
- Stale dane w cache
- Concurrency issues

### Narzędzia:
- `ValidateScopes()` w Development environment
- dotMemory - wykrywanie memory leaks
- Application Insights - monitoring

### Włączenie walidacji:
```csharp
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});
```

## Podsumowanie

Dependency Injection to fundamentalny wzorzec w ASP.NET Core, który:
- Redukuje coupling między komponentami
- Ułatwia testowanie
- Zwiększa reużywalność kodu
- Pozwala na łatwą wymianę implementacji
- Centralizuje zarządzanie czasem życia obiektów

Kluczem do sukcesu jest:
1. Programowanie do interfejsów
2. Wybór odpowiedniego lifetime
3. Unikanie anti-patterns (Service Locator, Captive Dependency)
4. Prawidłowa obsługa circular dependencies
