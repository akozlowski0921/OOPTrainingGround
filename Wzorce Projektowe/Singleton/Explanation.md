# Singleton Pattern

## 📌 Problem w Bad Example
Naiwna implementacja singletona bez thread-safety prowadzi do:
- **Race condition** – wiele wątków może jednocześnie sprawdzić `if (_instance == null)`
- **Wiele instancji** – możliwe utworzenie kilku "singletonów" zamiast jednego
- **Niezdefiniowane zachowanie** – błędy występują sporadycznie, trudne do debugowania
- **Brak gwarancji** – żadna ochrona przed wielowątkowością

### Scenariusz problemu:
```
Thread 1: sprawdza _instance == null → TRUE
Thread 2: sprawdza _instance == null → TRUE (jeszcze nie ustawione!)
Thread 1: tworzy nową instancję
Thread 2: tworzy KOLEJNĄ nową instancję
Rezultat: DWA SINGLETONY! 😱
```

## ✅ Rozwiązanie: Thread-safe Singleton
Singleton to wzorzec kreacyjny zapewniający, że klasa ma **tylko jedną instancję** i **globalny punkt dostępu** do niej.

### Podejścia thread-safe w C#:

#### 1. Lazy\<T\> (✅ ZALECANE)
```csharp
private static readonly Lazy<DatabaseConnection> _lazyInstance = 
    new Lazy<DatabaseConnection>(() => new DatabaseConnection());

public static DatabaseConnection Instance => _lazyInstance.Value;
```
**Korzyści:**
- Thread-safe z natury (gwarantowane przez .NET)
- Lazy initialization (tworzenie przy pierwszym użyciu)
- Prosty i czytelny kod
- Zoptymalizowany przez CLR

#### 2. Static Constructor (Eager)
```csharp
private static readonly ConfigurationManager _instance = new ConfigurationManager();
static ConfigurationManager() { }
public static ConfigurationManager Instance => _instance;
```
**Korzyści:**
- Thread-safe (gwarantowane przez C#)
- Eager initialization (tworzenie przy ładowaniu typu)
- Najprostszy kod

**Wady:**
- Brak lazy loading (tworzenie nawet jeśli nie używany)

#### 3. Double-Checked Locking
```csharp
private static LegacyService _instance;
private static readonly object _lock = new object();

public static LegacyService Instance
{
    get
    {
        if (_instance == null)  // First check
        {
            lock (_lock)
            {
                if (_instance == null)  // Second check
                {
                    _instance = new LegacyService();
                }
            }
        }
        return _instance;
    }
}
```
**Uwagi:**
- Thread-safe ale bardziej złożony
- Performance optimization (lock tylko przy pierwszym wywołaniu)
- Historycznie używany przed wprowadzeniem `Lazy<T>`

## 🎯 Najlepsze praktyki w .NET

### ⚠️ UNIKAJ ręcznych singletonów w aplikacjach ASP.NET Core!

Zamiast tego użyj **Dependency Injection**:

```csharp
// Startup.cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddSingleton<IDatabaseConnection, DatabaseConnection>();
}

// Controller
public class MyController : ControllerBase
{
    private readonly IDatabaseConnection _db;

    public MyController(IDatabaseConnection db)
    {
        _db = db;  // Injected, thread-safe, testable!
    }
}
```

### Korzyści DI nad ręcznym Singletonem:
1. **Testowalność** – łatwo mockować zależności
2. **Loose coupling** – zależność od interfejsu, nie konkretnej klasy
3. **Lifecycle management** – container zarządza tworzeniem i niszczeniem
4. **Thread-safety** – gwarantowane przez framework
5. **Invesja kontroli** – framework decyduje o zależnościach

## 🔄 Kiedy stosować Singleton?
✅ **TAK** dla:
- Configuration managers
- Logging services
- Cache managers
- Connection pools
- Hardware interface objects

❌ **NIE** dla:
- Obiektów z wieloma odpowiedzialnościami (God Object)
- Obiektów przechowujących zmienialny stan (problemy z concurrent access)
- Obiektów trudnych do testowania

## ⚠️ Wady Singletona
1. **Global state** – ukryte zależności, trudne do testowania
2. **Tight coupling** – kod zależy od konkretnej implementacji
3. **Singleton to anti-pattern?** – w nowoczesnych aplikacjach preferuj DI

## 📝 Podsumowanie
- W C# użyj `Lazy<T>` lub static constructor
- W .NET aplikacjach **preferuj Dependency Injection** nad ręczne singletony
- `services.AddSingleton<T>()` jest lepsze niż własna implementacja
- Singleton powinien być **immutable** lub thread-safe w pełni
