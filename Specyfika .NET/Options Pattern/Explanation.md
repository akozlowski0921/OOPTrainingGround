# Options Pattern - Zarządzanie konfiguracją w ASP.NET Core

## Czym jest Options Pattern?

Options Pattern to zalecany sposób dostępu do konfiguracji w ASP.NET Core przez strongly-typed classes zamiast bezpośredniego dostępu do `IConfiguration`.

## Problemy w BadExample

### 1. Bezpośredni dostęp do IConfiguration
```csharp
var smtpHost = _configuration["Email:SmtpHost"]; // ❌ Magic strings
var smtpPort = int.Parse(_configuration["Email:SmtpPort"]); // ❌ Manual parsing
```

**Problemy:**
- Brak type safety
- Magic strings - łatwe literówki
- Manual parsing
- Brak IntelliSense
- Trudne w testowaniu

### 2. Statyczne ustawienia
```csharp
public static string SmtpHost { get; set; } // ❌ Global state
```

**Problemy:**
- Global mutable state
- Niemożliwe do przetestowania
- Tight coupling
- Thread safety issues

### 3. Brak walidacji
```csharp
_settings.ConnectionString // ❌ Może być null lub nieprawidłowy
```

**Problem:** Błędy wykrywane w runtime zamiast w startup

## Rozwiązania w GoodExample

### 1. IOptions<T> - Singleton
```csharp
public class EmailService {
    private readonly EmailOptions _options;
    
    public EmailService(IOptions<EmailOptions> options) {
        _options = options.Value; // Cached value
    }
}
```

**Użycie:**
- Wartość pobrana raz przy starcie
- Cached dla całego lifetime
- Najszybsza opcja

**Kiedy:** Configuration nie zmienia się

### 2. IOptionsSnapshot<T> - Scoped
```csharp
public class FeatureFlagService {
    private readonly IOptionsSnapshot<FeatureFlagOptions> _options;
    
    public FeatureFlagService(IOptionsSnapshot<FeatureFlagOptions> options) {
        _options = options;
    }
    
    public void Use() {
        // Wartość odczytana na początku request scope
        if (_options.Value.NewFeatureEnabled) { }
    }
}
```

**Użycie:**
- Reloaded per request
- Reaguje na zmiany w appsettings.json (z reload)

**Kiedy:** Configuration może się zmieniać między requestami

### 3. IOptionsMonitor<T> - Singleton with notifications
```csharp
public class ApiClient {
    private readonly IOptionsMonitor<ApiOptions> _options;
    
    public ApiClient(IOptionsMonitor<ApiOptions> options) {
        _options = options;
        
        // Subscribe to changes
        _options.OnChange(newOptions => {
            Console.WriteLine("Config changed!");
        });
    }
    
    public void MakeRequest() {
        var options = _options.CurrentValue; // Always fresh
    }
}
```

**Użycie:**
- Real-time updates
- Notification callbacks
- CurrentValue zawsze aktualne

**Kiedy:** Potrzebujesz reagować na zmiany w czasie rzeczywistym

## Walidacja

### Data Annotations
```csharp
public class EmailOptions {
    [Required]
    [EmailAddress]
    public string Username { get; set; }
    
    [Range(1, 65535)]
    public int Port { get; set; }
}
```

### IValidatableObject
```csharp
public class DatabaseOptions : IValidatableObject {
    public IEnumerable<ValidationResult> Validate(ValidationContext ctx) {
        if (string.IsNullOrWhiteSpace(ConnectionString)) {
            yield return new ValidationResult("ConnectionString required");
        }
    }
}
```

### Rejestracja walidacji
```csharp
services.AddOptions<EmailOptions>()
    .Bind(configuration.GetSection("Email"))
    .ValidateDataAnnotations()
    .ValidateOnStart(); // Walidacja przy starcie aplikacji
```

## Named Options

Dla wielu konfiguracji tego samego typu:

```csharp
// Rejestracja
services.Configure<TenantOptions>("Tenant1", 
    configuration.GetSection("Tenants:Tenant1"));
services.Configure<TenantOptions>("Tenant2", 
    configuration.GetSection("Tenants:Tenant2"));

// Użycie
public class MultiTenantService {
    private readonly IOptionsSnapshot<TenantOptions> _options;
    
    public void ProcessForTenant(string tenantId) {
        var options = _options.Get(tenantId); // Named instance
    }
}
```

## Post-Configure

Modyfikacja opcji po załadowaniu:

```csharp
services.PostConfigure<PaymentOptions>(options => {
    // Ustawienie wartości domyślnych
    if (options.MaxAmount == 0)
        options.MaxAmount = 10000;
        
    // Walidacja
    if (options.CommissionRate < 0)
        throw new InvalidOperationException("Rate must be >= 0");
});
```

## appsettings.json

```json
{
  "Email": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "user@example.com",
    "Password": "secret"
  },
  "Database": {
    "ConnectionString": "Server=localhost;Database=MyDb",
    "CommandTimeout": 30
  },
  "FeatureFlags": {
    "NewFeatureEnabled": true
  }
}
```

## Comparison

| Feature | IOptions | IOptionsSnapshot | IOptionsMonitor |
|---------|----------|------------------|-----------------|
| Lifetime | Singleton | Scoped | Singleton |
| Reload | ❌ No | ✅ Per request | ✅ Real-time |
| Performance | Fastest | Medium | Slower |
| Notifications | ❌ No | ❌ No | ✅ Yes |
| Use case | Static config | Per-request reload | Real-time changes |

## Best Practices

### DO:
✅ Używaj strongly-typed options classes  
✅ Dodaj walidację z Data Annotations  
✅ Używaj const dla section names  
✅ ValidateOnStart() dla early error detection  
✅ IOptions dla static configuration  
✅ IOptionsSnapshot dla per-request reload  
✅ IOptionsMonitor dla real-time changes  
✅ Named options dla multiple instances  

### DON'T:
❌ Bezpośredni dostęp do IConfiguration w services  
❌ Magic strings dla kluczy konfiguracji  
❌ Statyczne ustawienia (global state)  
❌ Ignorowanie walidacji  
❌ Tworzenie nowych obiektów options w każdym wywołaniu  

## Testing

```csharp
[Fact]
public void Test_EmailService() {
    // Arrange
    var options = Options.Create(new EmailOptions {
        SmtpHost = "test.smtp.com",
        SmtpPort = 587
    });
    
    var service = new EmailService(options);
    
    // Act & Assert
    // ...
}
```

## Migration Guide

**Z IConfiguration:**
```csharp
// ❌ Before
var host = _configuration["Email:SmtpHost"];

// ✅ After
var host = _options.Value.SmtpHost;
```

**Setup:**
1. Utwórz options class
2. Configure w DI: `services.Configure<EmailOptions>(config.GetSection("Email"))`
3. Inject `IOptions<EmailOptions>` w service
4. Użyj `_options.Value`
