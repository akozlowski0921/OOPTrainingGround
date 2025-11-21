# CancellationToken - Wzorce anulowania operacji

## Czym jest CancellationToken?

`CancellationToken` to mechanizm cooperative cancellation w .NET, który pozwala na graceful cancellation długich operacji.

**Kluczowe cechy:**
- **Cooperative** - operacja musi sama sprawdzać token i reagować
- **Propagowalny** - przekazywany przez cały call stack
- **Lightweight** - struct, brak alokacji
- **Thread-safe** - może być używany z wielu wątków

## Problemy w BadExample

### 1. Ignorowanie CancellationToken
```csharp
public async Task FetchAsync(string url, CancellationToken token) {
    await Task.Delay(5000); // ❌ Nie używamy tokena
    return await _httpClient.GetStringAsync(url); // ❌ Nie używamy tokena
}
```

**Problem:**
- Operacja nie może być anulowana
- Token jest parametrem, ale martwym kodem
- Misleading API - sugeruje że możemy anulować

### 2. Brak propagacji
```csharp
public async Task ProcessAsync(CancellationToken token) {
    await StepOne(); // ❌ Nie przekazujemy tokena
    await StepTwo(); // ❌ Nie przekazujemy tokena
}
```

**Problem:** Token musi być propagowany przez cały call stack

### 3. Manual checking
```csharp
if (token.IsCancellationRequested)
    return; // ❌ Brak wyjątku
```

**Problem:**
- Boilerplate code
- Brak `OperationCanceledException`
- Caller nie wie że operacja została anulowana

### 4. Ignorowanie OperationCanceledException
```csharp
catch (OperationCanceledException) {
    Console.WriteLine("Retry..."); // ❌ Nie powinniśmy retry cancellation
}
```

**Problem:** Cancellation to nie błąd - nie robimy retry

### 5. Brak linked tokens
```csharp
var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
await operation(timeoutCts.Token);
// ❌ Nie można anulować z zewnątrz
```

**Problem:** Nie można połączyć timeout z external cancellation

## Rozwiązania w GoodExample

### 1. Zawsze przyjmuj CancellationToken
```csharp
public async Task FetchAsync(
    string url, 
    CancellationToken cancellationToken = default) // ✅ Optional parameter
{
    await Task.Delay(5000, cancellationToken);
    return await _httpClient.GetStringAsync(url, cancellationToken);
}
```

**Best practice:**
- Opcjonalny parametr z `= default`
- Zawsze ostatni parametr
- Propaguj do wszystkich async operations

### 2. ThrowIfCancellationRequested
```csharp
foreach (var item in items) {
    cancellationToken.ThrowIfCancellationRequested(); // ✅ Rzuca wyjątek
    await ProcessAsync(item, cancellationToken);
}
```

**Korzyści:**
- Automatyczny `OperationCanceledException`
- Jasny flow control
- Caller wie że operacja została anulowana

### 3. Linked tokens
```csharp
using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
    cancellationToken, 
    timeoutCts.Token);

await operation(linkedCts.Token);
```

**Użycie:**
- Timeout + external cancellation
- Multiple cancellation sources
- User cancellation + system shutdown

**Triggered when:** Którykolwiek z linked tokens jest cancelled

### 4. Prawidłowa obsługa OperationCanceledException
```csharp
try {
    await operation(cancellationToken);
}
catch (OperationCanceledException) {
    // ✅ Cleanup i re-throw
    await CleanupAsync();
    throw; // Propaguj do callera
}
```

**Kiedy catch:**
- Potrzebujesz cleanup
- Logowanie
- Konwersja na inny exception type

**Zawsze:** Re-throw lub return gracefully

### 5. Disposal CancellationTokenSource
```csharp
using var cts = new CancellationTokenSource(); // ✅ using statement

// Lub manual:
try {
    var cts = new CancellationTokenSource();
    await operation(cts.Token);
} finally {
    cts.Dispose(); // ✅ Zawsze dispose
}
```

**Dlaczego dispose?**
- Zwalnia timer resources (dla timeout)
- Unregisters callbacks
- Prevents memory leaks

### 6. Callback registration
```csharp
using var registration = cancellationToken.Register(() => {
    Console.WriteLine("Cancelled!");
    Cleanup();
});

await operation(cancellationToken);
```

**Użycie:**
- Cleanup przy cancellation
- Logging
- Notification

**Uwaga:** Dispose registration po użyciu

## Wzorce użycia

### Pattern 1: Async method z cancellation
```csharp
public async Task<T> OperationAsync(CancellationToken ct = default)
{
    ct.ThrowIfCancellationRequested();
    
    var result = await DoWorkAsync(ct);
    
    ct.ThrowIfCancellationRequested();
    
    return result;
}
```

### Pattern 2: Loop z cancellation
```csharp
while (!ct.IsCancellationRequested)
{
    try 
    {
        await ProcessBatchAsync(ct);
        await Task.Delay(1000, ct);
    }
    catch (OperationCanceledException)
    {
        break; // Exit gracefully
    }
}
```

### Pattern 3: Timeout pattern
```csharp
public async Task<T> WithTimeoutAsync<T>(
    Func<CancellationToken, Task<T>> operation,
    TimeSpan timeout,
    CancellationToken ct = default)
{
    using var timeoutCts = new CancellationTokenSource(timeout);
    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);
    
    try 
    {
        return await operation(linkedCts.Token);
    }
    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
    {
        throw new TimeoutException($"Operation timed out after {timeout}");
    }
}
```

### Pattern 4: Parallel operations
```csharp
var options = new ParallelOptions 
{ 
    CancellationToken = ct,
    MaxDegreeOfParallelism = 4
};

Parallel.ForEach(items, options, item => 
{
    ProcessItem(item);
    ct.ThrowIfCancellationRequested();
});
```

## CancellationToken vs CancellationTokenSource

### CancellationToken (struct)
- **Readonly view** na stan cancellation
- Przekazywany do operacji
- Lightweight (8 bytes)
- Check: `IsCancellationRequested`, `ThrowIfCancellationRequested`

### CancellationTokenSource (class)
- **Control object** - może anulować
- Tworzy CancellationToken przez property `.Token`
- Disposable - musi być disposed
- Control: `Cancel()`, `CancelAfter(timeout)`

**Zasada:** Kto tworzy CTS, ten go disposes i cancels

## Performance

### CancellationToken checking overhead
```csharp
// ❌ Za częste checking
for (int i = 0; i < 1000000; i++) {
    ct.ThrowIfCancellationRequested(); // Overhead
    DoQuickOperation();
}

// ✅ Checking co N iteracji
for (int i = 0; i < 1000000; i++) {
    if (i % 1000 == 0)
        ct.ThrowIfCancellationRequested();
    DoQuickOperation();
}
```

### Callback registration cost
```csharp
// ❌ Registration w pętli
for (int i = 0; i < 1000; i++) {
    using var reg = ct.Register(() => Cleanup());
    DoWork();
}

// ✅ Registration raz
using var reg = ct.Register(() => Cleanup());
for (int i = 0; i < 1000; i++) {
    DoWork();
}
```

## Best Practices

### DO:
✅ Zawsze przyjmuj `CancellationToken` w async methods  
✅ Używaj `= default` jako optional parameter  
✅ Propaguj token przez cały call stack  
✅ Używaj `ThrowIfCancellationRequested()`  
✅ Dispose `CancellationTokenSource`  
✅ Używaj linked tokens dla multiple sources  
✅ Re-throw `OperationCanceledException` po cleanup  

### DON'T:
❌ Ignoruj przekazany token  
❌ Catch i ignoruj `OperationCanceledException`  
❌ Używaj `IsCancellationRequested` + `return` (użyj `ThrowIfCancellationRequested`)  
❌ Zapomnij dispose `CancellationTokenSource`  
❌ Używaj `CancellationToken.None` jako default (użyj `default`)  
❌ Twórz nowy `CancellationTokenSource` bez disposing poprzedniego  

## ASP.NET Core Integration

### Controller action
```csharp
[HttpGet]
public async Task<IActionResult> GetData(CancellationToken ct)
{
    // ✅ Framework automatycznie cancels przy request abortion
    var data = await _service.GetDataAsync(ct);
    return Ok(data);
}
```

**Framework cancels gdy:**
- Client disconnects
- Request timeout
- Server shutdown

### Background service
```csharp
public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await DoWorkAsync(stoppingToken);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
```

## Testing

### Testing cancellation
```csharp
[Fact]
public async Task Should_Cancel_Operation()
{
    var cts = new CancellationTokenSource();
    var task = _service.LongRunningAsync(cts.Token);
    
    await Task.Delay(100);
    cts.Cancel();
    
    await Assert.ThrowsAsync<OperationCanceledException>(() => task);
}
```

### Testing timeout
```csharp
[Fact]
public async Task Should_Timeout()
{
    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
    
    await Assert.ThrowsAsync<OperationCanceledException>(
        () => _service.LongRunningAsync(cts.Token));
}
```

## Common Pitfalls

1. **Nie propagowanie tokena** - najczęstszy błąd
2. **Catch OperationCanceledException bez re-throw** - ukrywa cancellation
3. **Brak dispose CTS** - memory leak
4. **Używanie .Result z CT** - deadlock risk
5. **Manual checking zamiast ThrowIfCancellationRequested** - więcej kodu
