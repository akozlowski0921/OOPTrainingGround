# Factory & Abstract Factory

## 📌 Problem w Bad Example

Bezpośrednie tworzenie obiektów przez `new` w całej aplikacji prowadzi do:
- **Tight coupling** – kod bezpośrednio zależy od konkretnych klas
- **Naruszenia Open/Closed Principle** – dodanie nowego typu wymaga modyfikacji w wielu miejscach
- **Trudności w testowaniu** – niemożliwe mockowanie bez zmian w kodzie produkcyjnym
- **Duplikacji logiki tworzenia** – ta sama logika inicjalizacji rozrzucona po aplikacji
- **Braku centralizacji** – brak jednego miejsca zarządzającego tworzeniem obiektów

### Przykład problemu:
```csharp
// ❌ Bad: Rozrzucone 'new' po całej aplikacji
public class OrderProcessor
{
    public void ProcessPayment(string type)
    {
        if (type == "Credit")
        {
            var payment = new CreditCardPayment();
            payment.Initialize();
            payment.Process();
        }
        else if (type == "PayPal")
        {
            var payment = new PayPalPayment();
            payment.Initialize();
            payment.Process();
        }
        // Każdy nowy typ = kolejny if!
    }
}
```

## Factory Method Pattern

**Definicja:** Definiuje interfejs do tworzenia obiektów, ale pozwala podklasom decydować, którą klasę instancjonować.

### Kluczowe elementy:
1. **Interfejs produktu** (`IPayment`) – wspólny kontrakt dla wszystkich produktów
2. **Konkretne produkty** (`CreditCardPayment`, `PayPalPayment`) – różne implementacje
3. **Factory** – centralizuje logikę tworzenia obiektów

```csharp
// ✅ Good: Factory Method
public interface IPaymentFactory
{
    IPayment CreatePayment(PaymentType type);
}

public class PaymentFactory : IPaymentFactory
{
    public IPayment CreatePayment(PaymentType type)
    {
        return type switch
        {
            PaymentType.CreditCard => new CreditCardPayment(),
            PaymentType.PayPal => new PayPalPayment(),
            PaymentType.BankTransfer => new BankTransferPayment(),
            _ => throw new ArgumentException($"Unknown payment type: {type}")
        };
    }
}
```

## Abstract Factory Pattern

**Definicja:** Dostarcza interfejs do tworzenia **rodzin powiązanych obiektów** bez określania ich konkretnych klas.

### Kiedy używać Abstract Factory zamiast Factory Method?
- Potrzebujesz tworzyć **zestawy powiązanych obiektów** (np. GUI: Button + TextBox + Window)
- Chcesz zapewnić **spójność** między produktami (np. DarkTheme zawsze tworzy ciemne komponenty)
- Potrzebujesz **zamiennych rodzin** produktów (np. Windows UI vs macOS UI)

```csharp
// Abstract Factory example
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    IWindow CreateWindow();
}

public class DarkThemeFactory : IUIFactory
{
    public IButton CreateButton() => new DarkButton();
    public ITextBox CreateTextBox() => new DarkTextBox();
    public IWindow CreateWindow() => new DarkWindow();
}

public class LightThemeFactory : IUIFactory
{
    public IButton CreateButton() => new LightButton();
    public ITextBox CreateTextBox() => new LightTextBox();
    public IWindow CreateWindow() => new LightWindow();
}
```

## 🎯 Po co stosować Factory?

### 1. **Enkapsulacja logiki tworzenia**
Factory ukrywa złożoność inicjalizacji obiektów:
```csharp
// Zamiast:
var payment = new CreditCardPayment();
payment.SetValidator(new CreditCardValidator());
payment.SetGateway(new PaymentGateway());
payment.Configure();

// Mamy:
var payment = _factory.CreatePayment(PaymentType.CreditCard);
// Wszystko już skonfigurowane!
```

### 2. **Dependency Injection friendly**
```csharp
// Rejestracja w DI container:
services.AddTransient<IPaymentFactory, PaymentFactory>();

// Użycie:
public class OrderService
{
    private readonly IPaymentFactory _factory;
    
    public OrderService(IPaymentFactory factory)
    {
        _factory = factory;
    }
    
    public void ProcessOrder(Order order)
    {
        var payment = _factory.CreatePayment(order.PaymentType);
        payment.Process(order.Amount);
    }
}
```

### 3. **Łatwe testowanie**
```csharp
// Mock factory w testach:
var mockFactory = new Mock<IPaymentFactory>();
mockFactory
    .Setup(f => f.CreatePayment(It.IsAny<PaymentType>()))
    .Returns(new MockPayment());

var service = new OrderService(mockFactory.Object);
// Testowanie bez prawdziwych payment processors!
```

## W czym pomaga?

✅ **Centralizacja logiki tworzenia** – jedna lokalizacja, łatwe zmiany  
✅ **Dependency Injection** – factory jako zależność, łatwe mockowanie  
✅ **Open/Closed Principle** – nowe typy bez modyfikacji klientów  
✅ **Separation of Concerns** – logika tworzenia oddzielona od logiki biznesowej  
✅ **Testowalność** – łatwe mockowanie całej factory  
✅ **Czytelność** – jasna intencja tworzenia obiektów  

## ⚖️ Zalety i wady

### Zalety
✅ **Loose coupling** – kod nie zależy od konkretnych klas  
✅ **Single Responsibility** – tworzenie obiektów to osobna odpowiedzialność  
✅ **Łatwa rozbudowa** – dodanie nowego typu bez zmian w istniejącym kodzie  
✅ **DI Integration** – naturalna integracja z kontenerami IoC  
✅ **Testowanie** – mockowanie factory zamiast konkretnych implementacji  

### Wady
❌ **Wzrost liczby klas** – factory + interface + implementacje  
❌ **Over-engineering** – dla prostych przypadków może być przesadą  
❌ **Większa złożoność** – dodatkowa warstwa abstrakcji  

## ⚠️ Na co uważać?

### 1. **Nie nadużywaj Factory dla prostych przypadków**
```csharp
// ❌ Przesada:
public interface IUserFactory
{
    User CreateUser(string name);
}

// ✅ Wystarczy:
var user = new User(name);
```

### 2. **Factory vs Constructor Injection**
```csharp
// ❌ Factory niepotrzebna gdy DI wystarczy:
public class OrderService
{
    private readonly IPaymentFactory _factory;
    
    public void Process(Order order)
    {
        var payment = _factory.CreatePayment(PaymentType.CreditCard);
        // Zawsze CreditCard? Po co factory?
    }
}

// ✅ Lepiej:
public class OrderService
{
    private readonly ICreditCardPayment _payment;
    
    public OrderService(ICreditCardPayment payment)
    {
        _payment = payment; // Direct injection
    }
}
```

### 3. **Unikaj logiki biznesowej w Factory**
```csharp
// ❌ Factory nie powinna zawierać logiki biznesowej:
public class PaymentFactory
{
    public IPayment CreatePayment(Order order)
    {
        if (order.Amount > 10000 && order.Customer.IsVIP)
            return new PremiumPayment(); // Logika biznesowa!
        
        return new StandardPayment();
    }
}

// ✅ Logika biznesowa w serwisie:
public class PaymentService
{
    private readonly IPaymentFactory _factory;
    
    public IPayment GetPaymentProcessor(Order order)
    {
        var type = DeterminePaymentType(order); // Logika tutaj
        return _factory.CreatePayment(type); // Factory tylko tworzy
    }
}
```

### 4. **Lifetime management w DI**
```csharp
// Uwaga na lifecycle:
services.AddSingleton<IPaymentFactory, PaymentFactory>();

// Factory jest singleton, ale tworzone obiekty mogą być transient:
public class PaymentFactory : IPaymentFactory
{
    private readonly IServiceProvider _serviceProvider;
    
    public IPayment CreatePayment(PaymentType type)
    {
        // Używaj ServiceProvider do tworzenia z DI:
        return type switch
        {
            PaymentType.CreditCard => 
                _serviceProvider.GetRequiredService<ICreditCardPayment>(),
            // ...
        };
    }
}
```

## 🔄 Kiedy stosować?

### Użyj Factory Method gdy:
✅ **Runtime decision** – typ obiektu określany w czasie wykonania  
✅ **Wiele podobnych klas** – wspólny interfejs, różne implementacje  
✅ **Złożona inicjalizacja** – obiekty wymagają skomplikowanej konfiguracji  
✅ **Potrzebujesz testować** – mockowanie konkretnych typów  

### Użyj Abstract Factory gdy:
✅ **Rodziny produktów** – tworzysz zestawy powiązanych obiektów  
✅ **Spójność** – produkty muszą być ze sobą kompatybilne  
✅ **Wymienne implementacje** – różne "smaki" tej samej funkcjonalności  

### NIE używaj Factory gdy:
❌ **Prosta konstrukcja** – `new` jest wystarczające  
❌ **Jeden typ** – nie ma wymiennych implementacji  
❌ **DI wystarczy** – typ znany w compile time, inject bezpośrednio  

## 🚨 Najczęstsze pomyłki

### 1. **Factory z if/else zamiast strategii**
```csharp
// ❌ Bad:
public IPayment CreatePayment(Order order)
{
    if (order.Type == "Credit")
        return new CreditCard();
    else if (order.Type == "PayPal")
        return new PayPal();
    // Naruszenie OCP - każdy nowy typ = modyfikacja factory
}

// ✅ Good: Dictionary/Map strategy
private readonly Dictionary<PaymentType, Func<IPayment>> _creators = new()
{
    { PaymentType.CreditCard, () => new CreditCardPayment() },
    { PaymentType.PayPal, () => new PayPalPayment() }
};

public IPayment CreatePayment(PaymentType type)
{
    return _creators[type]();
}
```

### 2. **Zapominanie o disposal**
```csharp
// ❌ Memory leak jeśli IPayment : IDisposable
public void ProcessOrder(Order order)
{
    var payment = _factory.CreatePayment(order.Type);
    payment.Process();
    // Payment nie został zutylizowany!
}

// ✅ Proper disposal
public void ProcessOrder(Order order)
{
    using var payment = _factory.CreatePayment(order.Type);
    payment.Process();
}
```

### 3. **Factory jako Service Locator (anti-pattern)**
```csharp
// ❌ Service Locator anti-pattern:
public class OrderService
{
    public void Process()
    {
        var factory = ServiceLocator.Get<IPaymentFactory>(); // ZŁE!
        // Ukryte zależności
    }
}

// ✅ Explicit dependency:
public class OrderService
{
    private readonly IPaymentFactory _factory;
    
    public OrderService(IPaymentFactory factory) // DOBRE!
    {
        _factory = factory;
    }
}
```

### 4. **Statyczna Factory (trudne testowanie)**
```csharp
// ❌ Static factory:
public static class PaymentFactory
{
    public static IPayment Create(PaymentType type)
    {
        // Nie da się mockować!
    }
}

// ✅ Instance factory (injectable):
public class PaymentFactory : IPaymentFactory
{
    public IPayment CreatePayment(PaymentType type)
    {
        // Można mockować przez interface!
    }
}
```

## 💼 Kontekst biznesowy

### Scenariusz: E-commerce z wieloma metodami płatności

**Bez Factory:**
- Każdy nowy payment provider wymaga zmian w 10+ miejscach
- Trudne testowanie różnych scenariuszy płatności
- Ryzyko błędów przy inicjalizacji payment processors
- Kod ściśle powiązany z konkretnymi implementacjami

**Z Factory:**
- Nowy provider = nowa klasa + jedna linia w factory
- Łatwe A/B testing różnych providerów
- Centralna konfiguracja wszystkich payment methods
- Możliwość dynamicznej zmiany providerów bez redeployu

## DI Integration w ASP.NET Core

```csharp
// Startup.cs / Program.cs
public void ConfigureServices(IServiceCollection services)
{
    // Rejestracja factory
    services.AddTransient<IPaymentFactory, PaymentFactory>();
    
    // Alternatywnie: Factory jako Func<>
    services.AddTransient<Func<PaymentType, IPayment>>(provider => type =>
    {
        return type switch
        {
            PaymentType.CreditCard => provider.GetRequiredService<ICreditCardPayment>(),
            PaymentType.PayPal => provider.GetRequiredService<IPayPalPayment>(),
            _ => throw new ArgumentException($"Unknown payment type: {type}")
        };
    });
    
    // Rejestracja konkretnych implementacji
    services.AddTransient<ICreditCardPayment, CreditCardPayment>();
    services.AddTransient<IPayPalPayment, PayPalPayment>();
}

// Użycie w kontrolerze
public class PaymentController : ControllerBase
{
    private readonly IPaymentFactory _factory;
    
    public PaymentController(IPaymentFactory factory)
    {
        _factory = factory;
    }
    
    [HttpPost]
    public async Task<IActionResult> ProcessPayment(PaymentRequest request)
    {
        var payment = _factory.CreatePayment(request.Type);
        var result = await payment.ProcessAsync(request.Amount);
        return Ok(result);
    }
}
```

## 📝 Podsumowanie

- **Factory Method** – centralizuje tworzenie obiektów, eliminuje tight coupling
- **Abstract Factory** – tworzy rodziny powiązanych obiektów zapewniając spójność
- **Stosuj dla:** złożonej inicjalizacji, runtime decisions, testowania
- **Unikaj dla:** prostych przypadków gdzie `new` lub DI wystarczą
- **Integruj z DI** – factory jako dependency, nie service locator
- **Uwaga na:** over-engineering, lifetime management, logikę biznesową w factory
