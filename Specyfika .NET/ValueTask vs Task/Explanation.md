# ValueTask vs Task w C# .NET

## Problem w BadExample

### BadExample.cs - Nadużywanie Task
- **Niepotrzebne alokacje**: Task dla każdego cache hit (synchroniczny rezultat)
- **Memory pressure**: Setki tysięcy alokacji Task obiektów
- **GC overhead**: Częste garbage collections
- **Performance**: Wyższe latency przez GC pauses

### BadExample2.cs - Nieprawidłowe użycie ValueTask
- **Przechowywanie w polach**: ValueTask może być użyty tylko raz
- **Wielokrotne await**: Undefined behavior przy ponownym await
- **Synchroniczne blokowanie**: `.GetAwaiter().GetResult()` może powodować deadlock
- **Niepotrzebna konwersja**: `.AsTask()` traci korzyści ValueTask

### BadExample3.cs - Brak świadomości memory pressure
- **1 milion alokacji**: Każda iteracja alokuje Task
- **Wysokie zużycie pamięci**: Dziesiątki MB dla prostych operacji
- **Częste GC**: Gen 0/1/2 collections spowalniają kod
- **Niska wydajność**: Nie wykorzystuje optimization potential

## Rozwiązanie w GoodExample

### GoodExample.cs - Prawidłowe użycie ValueTask
- **Zero alokacji dla cache hits**: `new ValueTask<T>(result)`
- **Fast path optimization**: Synchroniczne ścieżki bez alokacji
- **Slow path separation**: Async tylko gdy naprawdę potrzeba
- **Hot path friendly**: Idealne dla high-performance APIs

### GoodExample2.cs - Prawidłowy cache pattern
- **Cache przechowuje Task**: Nie ValueTask (Task można reużyć)
- **ValueTask jako wrapper**: Dla ukończonych Tasks zero alokacji
- **Jednokrotne użycie**: ValueTask jest await-owany dokładnie raz
- **ConfigureAwait support**: ValueTask wspiera wszystkie async patterns

### GoodExample3.cs - Memory pressure comparison
- **Benchmark framework**: Porównanie Task vs ValueTask
- **Metryki GC**: Monitorowanie collections i memory usage
- **Performance data**: Konkretne liczby pokazujące improvement
- **Best practices**: Kiedy używać ValueTask

## Kluczowe różnice

| Aspekt | Task<T> | ValueTask<T> |
|--------|---------|--------------|
| Typ | Reference type (class) | Value type (struct) |
| Alokacja | Zawsze na heap | Stack (lub heap dla async) |
| Reużywalność | Tak - można await wiele razy | Nie - tylko raz |
| Cache | Można przechowywać | NIE wolno przechowywać |
| Performance | Dobre | Lepsze dla hot paths |
| Memory | Wyższe zużycie | Niższe zużycie |

## Kiedy używać ValueTask

✅ **Używaj ValueTask gdy:**
- Hot path może być synchroniczny (cache, validation)
- Metoda często zwraca synchronicznie
- Performance jest krytyczna
- Chcesz zredukować memory pressure
- API wysokowydajne (ASP.NET Core middleware)

❌ **Używaj Task gdy:**
- Zawsze async operacja (I/O bez cache)
- Potrzebujesz `Task.WhenAll`, `Task.WhenAny`
- Przechowujesz w polu/property
- API publiczne dla kompatybilności

## ValueTask - zasady użycia

### ✅ DOZWOLONE:
```csharp
// 1. Await dokładnie raz
var result = await GetValueTaskAsync();

// 2. Konwersja do Task gdy potrzeba
Task<T> task = valueTask.AsTask();

// 3. ConfigureAwait
await valueTask.ConfigureAwait(false);
```

### ❌ ZABRONIONE:
```csharp
// 1. Wielokrotne await
await valueTask;
await valueTask; // BŁĄD!

// 2. Przechowywanie w polu
private ValueTask<T> _field; // BŁĄD!

// 3. Concurrent operations
var t1 = valueTask.AsTask();
var t2 = valueTask.AsTask(); // BŁĄD!
```

## Pattern: Fast path / Slow path

```csharp
public ValueTask<string> GetDataAsync(string key)
{
    // Fast path - synchroniczny rezultat
    if (_cache.TryGetValue(key, out string value))
    {
        return new ValueTask<string>(value); // Zero alokacji
    }

    // Slow path - async operacja
    return GetDataSlowPathAsync(key);
}

private async ValueTask<string> GetDataSlowPathAsync(string key)
{
    string value = await FetchFromDatabaseAsync(key);
    _cache[key] = value;
    return value;
}
```

## Performance Tips

### Memory Pressure
```csharp
// ❌ Task - 24 bytes per allocation
for (int i = 0; i < 1_000_000; i++)
{
    await GetTaskAsync(); // 24 MB allocated
}

// ✅ ValueTask - 0 bytes for sync path
for (int i = 0; i < 1_000_000; i++)
{
    await GetValueTaskAsync(); // ~0 MB if mostly sync
}
```

### Caching
```csharp
// Cache Task, zwracaj ValueTask
private readonly Dictionary<string, Task<string>> _cache;

public ValueTask<string> GetAsync(string key)
{
    if (_cache.TryGetValue(key, out var task))
    {
        // Jeśli completed - zero alokacji
        if (task.IsCompletedSuccessfully)
            return new ValueTask<string>(task.Result);
        
        // Jeśli pending - wrap w ValueTask
        return new ValueTask<string>(task);
    }
    
    return GetSlowPathAsync(key);
}
```

## ASP.NET Core i ValueTask

ASP.NET Core intensywnie używa ValueTask:
- Middleware pipeline
- Response.WriteAsync()
- Stream operations
- High-performance scenarios

```csharp
public async ValueTask InvokeAsync(HttpContext context)
{
    // Fast path - większość requests
    if (!RequiresProcessing(context))
    {
        await _next(context);
        return; // Zero extra alokacji
    }

    // Slow path - specjalne przypadki
    await ProcessSlowPathAsync(context);
}
```

## Benchmarks (typowe wyniki)

### 100,000 iteracji (90% synchronicznych):
- **Task**: ~2.4 MB alokacji, 15-20ms, 5-8 GC Gen0
- **ValueTask**: ~0.24 MB alokacji, 10-15ms, 1-2 GC Gen0

### Improvement:
- **Memory**: ~90% mniej alokacji
- **Time**: ~25-33% szybciej
- **GC**: ~70% mniej collections

## Best Practices

1. **Hot paths**: Używaj ValueTask dla często wykonywanych ścieżek
2. **Nie przechowuj**: ValueTask w polach/properties
3. **Await raz**: ValueTask może być await-owany tylko raz
4. **Cache Tasks**: Nie ValueTasks
5. **AsTask świadomie**: Tylko gdy naprawdę potrzebujesz Task API
6. **Measure**: Benchmark przed i po optymalizacji

## Kiedy NIE warto?

- Metoda zawsze async (zawsze await I/O)
- Nie ma hot path
- Performance nie jest krytyczna
- Kod publiczny API (kompatybilność)
