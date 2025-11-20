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
