# System.Threading.Channels w C# .NET

## Problem w BadExample

### BadExample.cs - Ręczne synchronizowanie
- **Ręczne lockowanie**: `lock` i `Monitor.Wait/Pulse` - podatne na błędy
- **Blokowanie wątków**: `Monitor.Wait` blokuje wątki zamiast async wait
- **Brak backpressure**: Kolejka może rosnąć w nieskończoność
- **Trudne w utrzymaniu**: Dużo boilerplate code

### BadExample2.cs - Brak backpressure
- **Memory leak risk**: Producer może zalewać consumer
- **Brak throttling**: Wszystkie tasks startują naraz
- **OutOfMemory**: Dla dużych zbiorów danych
- **CPU waste**: Busy waiting marnuje zasoby

### BadExample3.cs - Nieefektywne alternatywy
- **ConcurrentQueue**: Brak async, wymaga polling
- **BlockingCollection**: Blokuje wątki, nie async/await
- **Ręczny SemaphoreSlim**: Skomplikowany, podatny na błędy
- **Queue + lock**: Nie thread-safe bez dodatkowych zabezpieczeń

## Rozwiązanie w GoodExample

### GoodExample.cs - System.Threading.Channels
- **Channel.CreateUnbounded**: Nieograniczona pojemność
- **Channel.CreateBounded**: Bounded z backpressure
- **WriteAsync/ReadAllAsync**: Async-friendly API
- **Writer.Complete()**: Sygnalizacja zakończenia

### GoodExample2.cs - Backpressure i throttling
- **BoundedChannelFullMode.Wait**: Automatyczne backpressure
- **Throttling**: Ograniczona liczba równoczesnych workerów
- **Rate limiting**: Kontrola przepustowości
- **Metrics**: Monitoring wydajności

### GoodExample3.cs - Performance comparison
- **Benchmarks**: Porównanie Channel vs ConcurrentQueue vs BlockingCollection
- **Feature comparison**: Tabela funkcjonalności
- **Recommendations**: Kiedy używać której struktury
- **Real data**: Konkretne metryki wydajności

## System.Threading.Channels - podstawy

### Tworzenie Channels

```csharp
// UnboundedChannel - nieograniczona pojemność
var unbounded = Channel.CreateUnbounded<int>();

// BoundedChannel - ograniczona pojemność (backpressure)
var bounded = Channel.CreateBounded<int>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait, // Wait gdy pełny
    SingleReader = false,  // Wiele konsumentów
    SingleWriter = false   // Wiele producentów
});
```

### Producer pattern
```csharp
public async Task ProduceAsync(Channel<int> channel)
{
    try
    {
        for (int i = 0; i < 100; i++)
        {
            await channel.Writer.WriteAsync(i);
        }
    }
    finally
    {
        channel.Writer.Complete(); // Zawsze complete!
    }
}
```

### Consumer pattern
```csharp
public async Task ConsumeAsync(Channel<int> channel)
{
    // ReadAllAsync - async enumeration
    await foreach (var item in channel.Reader.ReadAllAsync())
    {
        await ProcessAsync(item);
    }
}
```

## BoundedChannelFullMode

| Mode | Behavior |
|------|----------|
| Wait | WriteAsync czeka gdy channel pełny (backpressure) |
| DropNewest | Usuwa najnowszy element |
| DropOldest | Usuwa najstarszy element |
| DropWrite | Odrzuca nowy element |

## Backpressure

### Problem: Fast producer, slow consumer
```csharp
// ❌ Bez backpressure
var queue = new Queue<int>();
for (int i = 0; i < 1_000_000; i++)
{
    queue.Enqueue(i); // Pamięć rośnie!
}
```

### Rozwiązanie: Bounded channel
```csharp
// ✅ Z backpressure
var channel = Channel.CreateBounded<int>(100);
for (int i = 0; i < 1_000_000; i++)
{
    await channel.Writer.WriteAsync(i); // Czeka gdy pełny!
}
```

## Throttling pattern

```csharp
public async Task ThrottledProcessing(
    List<string> items, 
    int maxConcurrency)
{
    var channel = Channel.CreateBounded<string>(maxConcurrency);

    // Producer
    var producer = Task.Run(async () =>
    {
        foreach (var item in items)
            await channel.Writer.WriteAsync(item);
        channel.Writer.Complete();
    });

    // Multiple consumers (throttled)
    var consumers = new Task[maxConcurrency];
    for (int i = 0; i < maxConcurrency; i++)
    {
        consumers[i] = ConsumeAsync(channel);
    }

    await Task.WhenAll(producer);
    await Task.WhenAll(consumers);
}
```

## Porównanie mechanizmów kolejkowania

| Feature | Channel | ConcurrentQueue | BlockingCollection | Queue + lock |
|---------|---------|-----------------|--------------------| -------------|
| **Async/await** | ✅ | ❌ | ❌ | ❌ |
| **Backpressure** | ✅ (bounded) | ❌ | ✅ | ❌ |
| **Thread-safe** | ✅ | ✅ | ✅ | ❌ (manual) |
| **Completion signal** | ✅ | ❌ | ✅ | ❌ |
| **Performance** | Excellent | Good | Good | Poor |
| **Modern .NET** | ✅ | ✅ | Legacy | Legacy |

## Kiedy używać czego?

### ✅ Channel - pierwsze wybór dla:
- Async/await scenariuszy
- Producer-consumer patterns
- Pipeline processing
- Backpressure requirements
- Modern applications

### ⚠️ ConcurrentQueue - gdy:
- Prosty sync scenario
- Nie potrzebujesz completion signaling
- Zarządzasz signaling ręcznie
- Legacy codebase

### ⚠️ BlockingCollection - gdy:
- Legacy sync code
- Migracja ze starszego .NET
- Bounded queue bez async
- Compatibility requirements

### ❌ Queue + lock - UNIKAJ:
- Podatne na błędy
- Trudne w utrzymaniu
- Brak async support
- Use Channel instead!

## Advanced patterns

### Pipeline pattern
```csharp
// Stage 1: Load
var loadChannel = Channel.CreateUnbounded<string>();

// Stage 2: Transform
var transformChannel = Channel.CreateUnbounded<Data>();

// Stage 3: Save
var saveChannel = Channel.CreateUnbounded<Data>();

// Chain stages
var loader = LoadAsync(loadChannel);
var transformer = TransformAsync(loadChannel, transformChannel);
var saver = SaveAsync(transformChannel);

await Task.WhenAll(loader, transformer, saver);
```

### Fan-out pattern
```csharp
// One producer, multiple consumers
var channel = Channel.CreateBounded<int>(100);

var producer = ProduceAsync(channel);

var consumers = new Task[Environment.ProcessorCount];
for (int i = 0; i < consumers.Length; i++)
{
    consumers[i] = ConsumeAsync(channel);
}

await producer;
await Task.WhenAll(consumers);
```

### Fan-in pattern
```csharp
// Multiple producers, one consumer
var channel = Channel.CreateBounded<int>(100);

var producers = new Task[3];
for (int i = 0; i < producers.Length; i++)
{
    producers[i] = ProduceAsync(channel);
}

var consumer = ConsumeAsync(channel);

await Task.WhenAll(producers);
channel.Writer.Complete();
await consumer;
```

## Performance tips

1. **Choose appropriate capacity**: Bounded channel capacity based on memory
2. **Use SingleReader/SingleWriter**: Optimize when possible
3. **Batch operations**: Process multiple items at once
4. **Monitor metrics**: Track channel depth and throughput
5. **Proper completion**: Always call `Writer.Complete()`

## Common pitfalls

### ❌ Forgetting to complete
```csharp
// Consumer będzie czekał w nieskończoność!
await channel.Writer.WriteAsync(item);
// Brak Complete()!
```

### ❌ Not handling exceptions
```csharp
try
{
    await channel.Writer.WriteAsync(item);
}
finally
{
    channel.Writer.Complete(); // Zawsze!
}
```

### ❌ Deadlock z bounded channel
```csharp
// Producer i consumer w tym samym task
// Producer czeka na miejsce
// Consumer nigdy nie startuje
// = Deadlock!
```

## Best Practices

1. **Always complete**: Call `Writer.Complete()` in finally block
2. **Use bounded for backpressure**: Protect against memory issues
3. **Separate producer/consumer**: Different tasks for clarity
4. **Handle cancellation**: Support CancellationToken
5. **Monitor performance**: Track channel depth
6. **Choose right mode**: UnboundedChannelFullMode based on needs
7. **Test edge cases**: Empty channel, full channel, completion

## Real-world use cases

- **Web scrapers**: Crawl URLs with rate limiting
- **Data processing pipelines**: ETL workflows
- **Message queues**: In-process message bus
- **Background jobs**: Task queue with workers
- **API throttling**: Rate limiting requests
- **Stream processing**: Real-time data processing

## Conclusion

System.Threading.Channels is the modern, recommended approach for:
- Producer-consumer scenarios
- Async pipelines
- Backpressure management
- High-performance async code

It provides clean, efficient, and safe async queuing without manual synchronization!
