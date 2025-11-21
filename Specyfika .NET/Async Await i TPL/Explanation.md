# Async/Await i Task Parallel Library

## Problemy w BadExample

### 1. async void - Nieobsługiwalne wyjątki
```csharp
public async void FetchDataAsync(string url) // ❌ async void
{
    await _httpClient.GetStringAsync(url);
}
```
**Problemy:**
- Wyjątki z async void nie mogą być złapane przez wywołującego
- Crash całej aplikacji przy nieobsłużonym wyjątku
- Brak możliwości śledzenia zakończenia operacji

**Wyjątek:** Event handlery (np. button click w WPF/WinForms)

### 2. Deadlock - Blokowanie async kodu
```csharp
return task.Result; // ❌ Deadlock risk
```
**Dlaczego powstaje deadlock?**
1. `.Result` blokuje bieżący wątek czekając na Task
2. W kontekście z `SynchronizationContext` (UI, ASP.NET pre-Core) kontynuacja async próbuje wrócić na zablokowany wątek
3. Wzajemne blokowanie → deadlock

**Rozwiązanie:** Async all the way - używaj `await` w całym call stack

### 3. ConfigureAwait - Kiedy używać?
```csharp
// ❌ W library code bez ConfigureAwait
await _httpClient.GetStringAsync(url);

// ✅ W library code
await _httpClient.GetStringAsync(url).ConfigureAwait(false);
```

**Zasady:**
- **Library code:** Zawsze `.ConfigureAwait(false)` - nie potrzebujemy kontekstu
- **Application code (UI):** Bez ConfigureAwait lub `.ConfigureAwait(true)` - musimy wrócić na UI thread
- **ASP.NET Core:** Nie ma SynchronizationContext, więc ConfigureAwait ma mniejsze znaczenie

### 4. Sekwencyjne vs Równoległe wykonanie
```csharp
// ❌ Sekwencyjnie - wolno
foreach (var url in urls) {
    var result = await _httpClient.GetStringAsync(url); // Czeka na każde
}

// ✅ Równolegle - szybko
var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
var results = await Task.WhenAll(tasks);
```

**Performance:**
- 3 requesty po 1s sekwencyjnie = 3s
- 3 requesty po 1s równolegle = ~1s

### 5. Async over sync
```csharp
public async Task<int> CalculateAsync(int a, int b) {
    return a + b; // ❌ Brak await, niepotrzebny async
}
```
**Problem:** Overhead tworzenia state machine bez korzyści

**Rozwiązanie:** Usuń async/await dla synchronicznych operacji

### 6. Łapanie wyjątków bez await
```csharp
try {
    return _httpClient.GetStringAsync(url); // ❌ Bez await
} catch (HttpRequestException ex) {
    // Nigdy się nie wykona - Task jest zwracany, nie wykonany
}
```

**Rozwiązanie:** Zawsze `await` w try/catch

## Rozwiązania w GoodExample

### 1. async Task zamiast async void
```csharp
public async Task FetchDataAsync(string url) // ✅
{
    try {
        await _httpClient.GetStringAsync(url);
    } catch (HttpRequestException ex) {
        // Prawidłowa obsługa
    }
}
```

### 2. Task.WhenAll dla równoległego wykonania
```csharp
var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
var results = await Task.WhenAll(tasks);
```

**Alternatywy:**
- `Task.WhenAny` - pierwszy zakończony
- `Task.WaitAll` - synchroniczne (unikać w async code)

### 3. TPL - Task Parallel Library

**Parallel.ForEach** - CPU-bound operations:
```csharp
Parallel.ForEach(items, item => {
    var result = HeavyCalculation(item);
});
```

**Kiedy używać:**
- CPU-bound operacje (obliczenia, przetwarzanie obrazów)
- Wiele rdzeni CPU
- **NIE używać dla I/O-bound** (async/await jest lepsze)

### 4. ValueTask - Optymalizacja hot path
```csharp
public ValueTask<string> GetValueAsync(int key)
{
    if (_cache.TryGetValue(key, out var value))
        return new ValueTask<string>(value); // Synchroniczne, bez alokacji
    
    return new ValueTask<string>(FetchFromDatabaseAsync(key)); // Async
}
```

**Kiedy używać:**
- Operacje często synchroniczne (cache hit)
- Hot path z wysoką częstotliwością wywołań
- **Nie** dla zwykłych przypadków (Task wystarczy)

## Deadlock - Szczegółowy przykład

### ASP.NET (pre-Core) Deadlock:
```csharp
// Controller action - działa na ASP.NET thread z SynchronizationContext
public ActionResult Index()
{
    var result = GetDataAsync().Result; // ❌ DEADLOCK!
    return View(result);
}

async Task<string> GetDataAsync()
{
    await Task.Delay(1000);
    // Kontynuacja próbuje wrócić na ASP.NET thread
    // Ale ten wątek jest zablokowany przez .Result
    return "data";
}
```

### Jak uniknąć:
```csharp
// ✅ Async all the way
public async Task<ActionResult> Index()
{
    var result = await GetDataAsync();
    return View(result);
}
```

## Best Practices

### DO:
✅ Używaj `async Task` (nie `async void` poza event handlerami)  
✅ Async all the way - unikaj `.Result` i `.Wait()`  
✅ `ConfigureAwait(false)` w library code  
✅ `Task.WhenAll` dla równoległych operacji  
✅ `try/catch` z `await` dla obsługi wyjątków  
✅ `ValueTask` dla hot path z częstymi sync completions  

### DON'T:
❌ `async void` (chyba że event handler)  
❌ `.Result` lub `.Wait()` na async code  
❌ Mieszanie sync i async (deadlock risk)  
❌ `Task.Run` dla I/O-bound operations  
❌ Niepotrzebny `async/await` dla synchronicznych operacji  

## Performance Tips

1. **Task.WhenAll > ForEach with await** - równoległość
2. **ConfigureAwait(false)** - unika przełączania kontekstu
3. **ValueTask** - redukuje alokacje dla hot path
4. **Parallel.ForEach** - dla CPU-bound, nie I/O-bound
5. **CancellationToken** - możliwość anulowania długich operacji

## Diagnostyka

**Deadlock detection:**
- Aplikacja "zawiesiła się"
- Thread pool exhaustion
- Analiza thread dump

**Tools:**
- Visual Studio Diagnostic Tools
- dotTrace / dotMemory
- PerfView

## ASP.NET Core vs ASP.NET Framework

**ASP.NET Framework:**
- Ma SynchronizationContext
- Ryzyko deadlocków z `.Result`
- ConfigureAwait ważniejszy

**ASP.NET Core:**
- Brak SynchronizationContext
- Mniejsze ryzyko deadlocków
- ConfigureAwait mniej krytyczny (ale dobra praktyka w libraries)
