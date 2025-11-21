# IDisposable - Zarządzanie zasobami niezarządzanymi

## Problem w BadExample

### 1. Wycieki zasobów
- **FileStream**: Handle do pliku nie jest zwalniany - plik pozostaje zablokowany
- **HttpClient**: Socket exhaustion - wyczerpanie dostępnych portów sieciowych
- **SqlConnection**: Connection pool może się wyczerpać

### 2. Skutki braku Dispose

- **Memory leaks**: Pamięć niezarządzana nie jest zwalniana
- **File locks**: Pliki pozostają zablokowane dla innych procesów
- **Socket exhaustion**: `System.Net.Sockets.SocketException: Only one usage of each socket address`
- **Connection pool exhaustion**: Brak dostępnych połączeń DB

## Rozwiązanie w GoodExample

### 1. Implementacja IDisposable Pattern

```csharp
public class MyResource : IDisposable
{
    private bool _disposed = false;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this); // Ważne!
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Zwalniamy managed resources (obiekty C#)
        }
        
        // Zwalniamy unmanaged resources (handles, pointery)
        
        _disposed = true;
    }

    ~MyResource()
    {
        // Finalizer jako backup
        Dispose(false);
    }
}
```

### 2. Using statement

```csharp
// Tradycyjna składnia
using (var resource = new MyResource())
{
    // Użycie zasobu
}
// Dispose() wywoływane automatycznie, nawet przy wyjątku

// C# 8+ using declaration
using var resource = new MyResource();
// Dispose() na końcu scope'a
```

## Zasoby wymagające Dispose

| Typ | Dlaczego |
|-----|----------|
| **FileStream** | Handle do pliku, blokada systemu plików |
| **StreamReader/Writer** | Wewnętrzny FileStream |
| **SqlConnection** | Połączenie DB, connection pool |
| **HttpClient** | Socket (ale używaj shared instance!) |
| **Bitmap, Graphics** | GDI+ handles |
| **Timer** | Zasoby systemowe |
| **CancellationTokenSource** | Event handles |

## HttpClient - Specjalny przypadek

**NIE twórz nowego HttpClient przy każdym request!**

### Opcja 1: Shared static instance
```csharp
private static readonly HttpClient _client = new HttpClient();
```

### Opcja 2: IHttpClientFactory (ASP.NET Core)
```csharp
services.AddHttpClient<MyService>();
```

## Dispose vs Finalize

| Aspekt | Dispose | Finalizer (~Class) |
|--------|---------|-------------------|
| Kiedy | Deterministyczne, natychmiast | Niedeterministyczne, GC decyduje |
| Wydajność | Szybkie | Wolne (2 cykle GC) |
| Użycie | Zawsze gdy możliwe | Backup gdy Dispose nie został wywołany |

## GC.SuppressFinalize() - Dlaczego?

```csharp
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this); // Wyłącza finalizer
}
```

Powody:
1. Finalizer jest kosztowny (przedłuża życie obiektu o 1 cykl GC)
2. Jeśli Dispose() został wywołany, finalizer jest niepotrzebny
3. Optymalizacja wydajności

## Wykrywanie wycieków

### .NET Memory Profiler
- Szukaj obiektów z finalizerami w F-reachable queue
- Sprawdź czy obiekty IDisposable są prawidłowo zwalniane

### Narzędzia
- dotMemory (JetBrains)
- Visual Studio Diagnostic Tools
- PerfView

## Best practices

1. **Zawsze implementuj IDisposable dla klas z zasobami niezarządzanymi**
2. **Używaj using statement/declaration**
3. **Dla HttpClient używaj shared instance lub IHttpClientFactory**
4. **Dodaj finalizer (~Class) jako safety net**
5. **Wywołaj GC.SuppressFinalize(this) w Dispose()**
6. **Sprawdź _disposed flag przed użyciem zasobu**
7. **Nie wywołuj virtual members w Dispose(bool disposing)**

## Przykład wycieku pamięci

```csharp
// BAD - 1000 plików pozostanie otwartych!
for (int i = 0; i < 1000; i++)
{
    var logger = new BadFileLogger($"log{i}.txt");
    logger.Log("test");
    // Brak Dispose!
}

// GOOD - pliki są prawidłowo zamykane
for (int i = 0; i < 1000; i++)
{
    using var logger = new GoodFileLogger($"log{i}.txt");
    logger.Log("test");
} // Dispose() wywoływane automatycznie
```

## Async Dispose (C# 8+)

Dla operacji asynchronicznych:

```csharp
public class AsyncResource : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await FlushAsync();
        // Zwolnij zasoby
    }
}

// Użycie
await using var resource = new AsyncResource();
```

## Zasada

**Jeśli klasa używa obiektów implementujących IDisposable, sama powinna implementować IDisposable i wywoływać Dispose() na tych obiektach.**

---

## 🎯 FAQ / INSIGHT

### Po co stosować IDisposable?

**Problem bez Dispose:**
- **Memory leaks** – zasoby niezarządzane nie są zwalniane
- **Resource exhaustion** – file handles, sockets, DB connections wyczerpane
- **File locks** – pliki pozostają zablokowane
- **Socket exhaustion** – aplikacja nie może otworzyć nowych połączeń
- **Connection pool exhaustion** – brak dostępnych połączeń do bazy

**Korzyści z IDisposable:**
- **Deterministyczne zwalnianie** – zasoby zwalniane natychmiast
- **Using statement** – automatyczne wywołanie Dispose
- **Resource cleanup** – gwarancja zwolnienia nawet przy exception
- **System health** – aplikacja nie wyczerpuje zasobów systemowych

### W czym pomaga IDisposable?

✅ **Eliminuje memory leaks** – zasoby niezarządzane zwalniane  
✅ **File management** – pliki prawidłowo zamykane i odblokowane  
✅ **DB connections** – connection pool nie wyczerpuje się  
✅ **Network resources** – sockets prawidłowo zamykane  
✅ **GDI+ handles** – graphics resources zwalniane  
✅ **Using statement** – automatyczny cleanup  

### ⚠️ Na co uważać?

#### 1. **HttpClient - NIE dispose przy każdym użyciu!**
```csharp
// ❌ BAD: Socket exhaustion!
for (int i = 0; i < 1000; i++)
{
    using var client = new HttpClient();  // Nowy socket!
    await client.GetAsync("https://api.example.com");
}
// Wyczerpanie portów TCP - aplikacja crash!

// ✅ GOOD: Shared static instance
private static readonly HttpClient _httpClient = new HttpClient();

for (int i = 0; i < 1000; i++)
{
    await _httpClient.GetAsync("https://api.example.com");
}

// ✅ BETTER: IHttpClientFactory (ASP.NET Core)
services.AddHttpClient<MyService>();
```

#### 2. **Double dispose protection**
```csharp
// ✅ Zawsze sprawdzaj _disposed flag
public class MyResource : IDisposable
{
    private bool _disposed = false;
    
    public void DoWork()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MyResource));
        
        // Work...
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;  // Guard against double dispose
        
        if (disposing)
        {
            // Cleanup managed resources
        }
        
        _disposed = true;
    }
}
```

#### 3. **Zapominanie o GC.SuppressFinalize**
```csharp
// ❌ BAD: Brak SuppressFinalize
public void Dispose()
{
    Dispose(true);
    // Obiekt przedłuża życie przez finalizer queue!
}

// ✅ GOOD: SuppressFinalize optymalizuje
public void Dispose()
{
    Dispose(true);
    GC.SuppressFinalize(this);  // Wyłącza finalizer
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **Brak using statement**
```csharp
// ❌ BAD: File handle leak
public void WriteLog(string message)
{
    var writer = new StreamWriter("log.txt");
    writer.WriteLine(message);
    // File pozostaje otwarty!
}

// ✅ GOOD: Using
public void WriteLog(string message)
{
    using var writer = new StreamWriter("log.txt");
    writer.WriteLine(message);
}  // Dispose automatic
```

#### 2. **Dispose w constructor**
```csharp
// ❌ BAD: Dispose w constructor
public class BadService : IDisposable
{
    public BadService()
    {
        var resource = new Resource();
        resource.Dispose();  // Przedwczesne!
        _resource = resource;  // Już disposed!
    }
}

// ✅ GOOD: Store i dispose w Dispose()
public class GoodService : IDisposable
{
    private readonly Resource _resource;
    
    public GoodService()
    {
        _resource = new Resource();
    }
    
    public void Dispose()
    {
        _resource?.Dispose();
    }
}
```

#### 3. **Async Dispose bez IAsyncDisposable**
```csharp
// ❌ BAD: Async w Dispose
public class BadAsyncResource : IDisposable
{
    public void Dispose()
    {
        FlushAsync().Wait();  // ❌ Deadlock risk!
    }
}

// ✅ GOOD: IAsyncDisposable
public class GoodAsyncResource : IAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        await FlushAsync();
        // Proper async cleanup
    }
}

// Usage:
await using var resource = new GoodAsyncResource();
```

### 💼 Kontekst biznesowy

**Scenariusz: File processing service**

**Bez proper Dispose:**
```csharp
// ❌ Bug: Files locked, service crashes after 100 files
public void ProcessFile(string path)
{
    var reader = new StreamReader(path);
    var content = reader.ReadToEnd();
    // File handle leak!
    
    ProcessContent(content);
}

// Po 100 plikach: Out of file handles
// Files locked: Cannot delete/move
// Service restart required
```

**Z proper Dispose:**
```csharp
// ✅ Reliable service
public void ProcessFile(string path)
{
    using var reader = new StreamReader(path);
    var content = reader.ReadToEnd();
    
    ProcessContent(content);
}  // File closed, handle released

// Unlimited files processed
// No locks
// Service runs 24/7 without issues
```

**Impact:**
- Service uptime: 90% → 99.9%
- File processing failures: 5% → 0%
- Support tickets: 20/month → 0
- Restart frequency: 3x/day → 0

### 📝 Podsumowanie

- **IDisposable** dla zasobów niezarządzanych (files, sockets, DB connections)
- **Using statement** zapewnia automatic cleanup
- **HttpClient** – shared instance, nie dispose przy każdym użyciu!
- **GC.SuppressFinalize** – optymalizuje performance
- **IAsyncDisposable** – dla async cleanup operations
- **Pattern:** Dispose(bool), _disposed flag, finalizer jako backup
