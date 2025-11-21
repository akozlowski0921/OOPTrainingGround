# CQRS Pattern - Command Query Responsibility Segregation

## 📌 Definicja

**CQRS (Command Query Responsibility Segregation)** to wzorzec architektoniczny, który **rozdziela operacje zapisu (Commands) od operacji odczytu (Queries)** w aplikacji. Zamiast używać jednego modelu dla wszystkich operacji, CQRS proponuje dwa oddzielne modele:

- **Write Model (Command Side)** - zoptymalizowany pod transakcyjne operacje zapisu
- **Read Model (Query Side)** - zoptymalizowany pod szybkie odczyty i prezentację danych

### Podstawowa zasada:
**Command** - zmienia stan systemu, nie zwraca danych  
**Query** - zwraca dane, nie zmienia stanu systemu

## 🔴 Problem w BadExample

### BadExample 1: Brak separacji Command/Query

```csharp
// ❌ BAD: Jeden model dla wszystkich operacji
public class ProductService
{
    // Command i Query w jednym serwisie
    public void CreateProduct(string name, decimal price) { }
    public Product GetProduct(int id) { }
    public List<Product> GetAllProducts() { }
}
```

**Problemy:**
- ❌ Jeden model danych dla zapisu i odczytu
- ❌ Brak optymalizacji dla różnych typów operacji
- ❌ Trudne skalowanie (odczyty i zapisy konkurują o zasoby)
- ❌ Queries zwracają pełne encje zamiast DTO
- ❌ Write operations muszą ładować całe obiekty

### BadExample 2: Brak separacji z Event Sourcing

```csharp
// ❌ BAD: Events i current state w tym samym modelu
public class Order
{
    public string Status { get; set; }
    public List<OrderEvent> Events { get; set; } // ❌ Redundancja!
}
```

**Problemy:**
- ❌ Events przechowywane obok current state (redundancja)
- ❌ Queries ładują wszystkie eventy nawet gdy niepotrzebne
- ❌ Brak separation of concerns
- ❌ Trudne odbudowanie stanu z eventów

### BadExample 3: Brak optymalizacji wydajnościowej

```csharp
// ❌ BAD: Jeden database dla wszystkiego
public User GetUser(int id)
{
    // ❌ Ładuje cały graf obiektów (Orders, Addresses, Profile)
    return _users.FirstOrDefault(u => u.Id == id);
}
```

**Problemy:**
- ❌ Brak cachingu dla częstych odczytów
- ❌ Ładowanie całych grafów obiektów gdy potrzeba kilku pól
- ❌ Write operations blokują reads
- ❌ Brak indexowania dla queries
- ❌ Niemożliwe niezależne skalowanie

## ✅ Rozwiązanie: CQRS Pattern

### Kluczowe komponenty:

```
┌──────────────────────────────────────────────────────────┐
│                       CLIENT                              │
└────────────┬─────────────────────────┬───────────────────┘
             │                         │
             │ Commands                │ Queries
             ↓                         ↓
    ┌────────────────┐        ┌────────────────┐
    │ COMMAND SIDE   │        │  QUERY SIDE    │
    │                │        │                │
    │ - Commands     │        │ - Queries      │
    │ - Handlers     │        │ - Handlers     │
    │ - Write Model  │        │ - Read Model   │
    │ - Validation   │        │ - Projections  │
    └────────┬───────┘        └────────┬───────┘
             │                         │
             ↓                         ↓
    ┌────────────────┐        ┌────────────────┐
    │ Write Database │        │ Read Database  │
    │ (Normalized)   │←───────│ (Denormalized) │
    └────────────────┘  Sync  └────────────────┘
```

### Implementacja:

#### 1. Write Side (Commands)

```csharp
// ✅ Command - intent to change state
public class CreateProductCommand : ICommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}

// ✅ Command Handler - processes command
public class CreateProductCommandHandler : ICommandHandler<CreateProductCommand>
{
    private readonly IProductRepository _repository;
    
    public async Task HandleAsync(CreateProductCommand command, CancellationToken ct)
    {
        // Validation
        if (string.IsNullOrEmpty(command.Name))
            throw new ArgumentException("Name required");
        
        // Business logic
        var product = new Product
        {
            Name = command.Name,
            Price = command.Price,
            StockQuantity = command.StockQuantity
        };
        
        // Persist
        await _repository.AddAsync(product, ct);
    }
}
```

#### 2. Read Side (Queries)

```csharp
// ✅ Query - request for data
public class GetProductQuery : IQuery<ProductDto>
{
    public int ProductId { get; set; }
}

// ✅ Read Model (DTO) - optimized for display
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    // Only fields needed for this view
}

// ✅ Query Handler
public class GetProductQueryHandler : IQueryHandler<GetProductQuery, ProductDto>
{
    private readonly IProductReadRepository _readRepository;
    
    public async Task<ProductDto> HandleAsync(GetProductQuery query, CancellationToken ct)
    {
        // Optimized read - only requested fields
        return await _readRepository.GetProductByIdAsync(query.ProductId, ct);
    }
}
```

#### 3. Synchronizacja Write → Read

```csharp
// ✅ Event-based synchronization
public class ProductCreatedEvent
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

// ✅ Projection - updates read model from events
public class ProductProjection
{
    private readonly IProductReadRepository _readRepository;
    
    public async Task HandleAsync(ProductCreatedEvent @event)
    {
        await _readRepository.InsertAsync(new ProductReadModel
        {
            Id = @event.ProductId,
            Name = @event.Name,
            Price = @event.Price
        });
    }
}
```

## 🎯 Trzy praktyczne przykłady

### Przykład 1: Rozdzielenie warstwy zapisu i odczytu (Basic CQRS)

**Use Case:** E-commerce system z produktami

**Write Side:**
- Normalized database schema
- ACID transactions
- Business logic validation
- Domain events

**Read Side:**
- Denormalized views
- No business logic
- Optimized indexes
- Fast lookups

**Korzyści:**
- ✅ Reads nie blokują writes
- ✅ Różne modele dla różnych potrzeb
- ✅ Możliwość osobnego skalowania
- ✅ Cache tylko na read side

### Przykład 2: CQRS z Event Sourcing

**Use Case:** System zamówień z pełną historią zmian

**Write Side:**
- Event store (append-only)
- Aggregate roots rebuilding from events
- Commands generate events
- No direct state updates

**Read Side:**
- Projections z eventów
- Multiple read models dla różnych celów
- Eventual consistency
- Możliwość odtworzenia stanu w przeszłości

**Korzyści:**
- ✅ Pełna historia zmian (audit trail)
- ✅ Temporal queries (stan w dowolnym momencie)
- ✅ Event replay dla debugging
- ✅ Multiple projections z tych samych eventów

### Przykład 3: Optymalizacja pod skalowalność i wydajność

**Use Case:** System użytkowników z wysokim ruchem

**Write Side:**
- Transactional SQL database
- Minimal writes
- Optimistic locking
- Write-through cache invalidation

**Read Side:**
- Redis cache (hot data)
- Elasticsearch (search)
- Read replicas (reports)
- Materialized views
- CDN dla statycznych danych

**Korzyści:**
- ✅ Horizontal scaling dla reads
- ✅ Różne storage technologies
- ✅ Cache dla popularnych queries
- ✅ Writes nie są obciążone przez reads

## 📊 Kiedy stosować CQRS?

### ✅ Użyj CQRS gdy:

1. **Znaczna dysproporcja read/write**
   - 90% operacji to odczyty, 10% zapisy
   - Potrzebujesz skalować reads i writes niezależnie

2. **Różne modele dla read i write**
   - Write wymaga walidacji, business logic
   - Read wymaga denormalizacji, agregacji

3. **Performance optimization**
   - Reads potrzebują caching
   - Writes potrzebują ACID transactions

4. **Complex business logic**
   - Write side ma złożoną logikę domenową
   - Read side ma proste projekcje

5. **Event Sourcing**
   - Potrzebujesz audit trail
   - Temporal queries

6. **Multiple clients z różnymi potrzebami**
   - Mobile app (lightweight DTOs)
   - Web app (rich models)
   - Reports (aggregations)

### ❌ NIE używaj CQRS gdy:

1. **Simple CRUD applications**
   - Proste formularze bez skomplikowanej logiki
   - Jeden model wystarcza

2. **Równy read/write**
   - Brak korzyści ze skalowania

3. **Mały system**
   - CQRS dodaje complexity
   - YAGNI - może jest za wcześnie

4. **Eventual consistency nieakceptowalna**
   - Jeśli musisz mieć immediate consistency
   - (Choć można mieć synchronous CQRS)

## ⚖️ Zalety i wady

### Zalety

✅ **Independent scaling** – scale reads i writes osobno  
✅ **Performance optimization** – różne strategie dla read/write  
✅ **Flexibility** – różne storage technologies  
✅ **Simplified queries** – denormalized read models  
✅ **Caching** – łatwe dodanie cache na read side  
✅ **Multiple read models** – różne projekcje dla różnych celów  
✅ **Eventual consistency** – lepsze skalowanie  

### Wady

❌ **Increased complexity** – więcej kodu, więcej componentów  
❌ **Eventual consistency** – read model może być outdated  
❌ **Code duplication** – write i read models osobno  
❌ **Learning curve** – zespół musi zrozumieć wzorzec  
❌ **Synchronization** – trzeba zsynchronizować write→read  
❌ **Testing** – więcej do testowania  
❌ **Infrastructure** – może wymagać dodatkowych serwisów  

## ⚠️ Na co uważać?

### 1. **Eventual consistency - handle properly**

```csharp
// ❌ BAD: Założenie immediate consistency
public async Task<ProductDto> CreateAndGetProduct(CreateProductCommand cmd)
{
    await _commandHandler.HandleAsync(cmd);
    return await _queryHandler.HandleAsync(new GetProductQuery(cmd.ProductId));
    // ❌ Read model może jeszcze nie być zaktualizowany!
}

// ✅ GOOD: Return result from command
public async Task<ProductDto> CreateAndGetProduct(CreateProductCommand cmd)
{
    var productId = await _commandHandler.HandleAsync(cmd);
    
    // ✅ Option 1: Return from write side
    return new ProductDto { Id = productId, Name = cmd.Name };
    
    // ✅ Option 2: Poll until available
    var maxRetries = 5;
    for (int i = 0; i < maxRetries; i++)
    {
        var product = await _queryHandler.HandleAsync(new GetProductQuery(productId));
        if (product != null) return product;
        await Task.Delay(100); // Wait for projection
    }
    throw new TimeoutException("Projection not ready");
}
```

### 2. **Synchronizacja Write → Read**

```csharp
// ❌ BAD: Direct database sync
public async Task HandleAsync(CreateProductCommand cmd)
{
    await _writeRepository.AddAsync(product);
    await _readRepository.AddAsync(productDto); // ❌ Tight coupling!
}

// ✅ GOOD: Event-based sync
public async Task HandleAsync(CreateProductCommand cmd)
{
    await _writeRepository.AddAsync(product);
    await _eventBus.PublishAsync(new ProductCreatedEvent(product.Id)); // ✅ Decoupled
}

// ✅ Projection handler
public async Task HandleAsync(ProductCreatedEvent @event)
{
    await _readRepository.AddAsync(new ProductReadModel(@event));
}
```

### 3. **Over-engineering prostych przypadków**

```csharp
// ❌ BAD: CQRS for simple lookup table
public class GetCountryQuery : IQuery<CountryDto> { } // Overkill!

// ✅ GOOD: Simple repository for lookups
public interface ICountryRepository
{
    Task<List<Country>> GetAllAsync();
}
```

### 4. **Cache invalidation strategy**

```csharp
// ✅ GOOD: Invalidate cache after write
public class UpdateProductCommandHandler
{
    private readonly IProductRepository _repository;
    private readonly ICacheInvalidator _cache;
    
    public async Task HandleAsync(UpdateProductCommand cmd)
    {
        await _repository.UpdateAsync(product);
        await _cache.InvalidateAsync($"product:{cmd.ProductId}"); // ✅ Clear cache
    }
}
```

### 5. **Command validation**

```csharp
// ❌ BAD: No validation
public class CreateProductCommandHandler
{
    public async Task HandleAsync(CreateProductCommand cmd)
    {
        await _repository.AddAsync(new Product(cmd.Name)); // ❌ Boom if null!
    }
}

// ✅ GOOD: Validate commands
public class CreateProductCommandValidator
{
    public void Validate(CreateProductCommand cmd)
    {
        if (string.IsNullOrEmpty(cmd.Name))
            throw new ValidationException("Name required");
        if (cmd.Price <= 0)
            throw new ValidationException("Price must be positive");
    }
}
```

## 🔄 CQRS Variants

### 1. **Simple CQRS** (Single database)
```
Commands → Write Model → Database ← Read Model ← Queries
```
- Same database
- Different models
- Synchronous

### 2. **CQRS with Separate Databases**
```
Commands → Write DB
             ↓ (sync)
Queries ← Read DB
```
- Separate databases
- Better scaling
- Eventual consistency

### 3. **CQRS with Event Sourcing**
```
Commands → Event Store
             ↓ (projection)
Queries ← Read Models
```
- Events are source of truth
- Multiple projections
- Full audit trail

### 4. **CQRS with Message Bus**
```
Commands → Write Side → Event Bus → Projections → Read Side
```
- Fully decoupled
- Async processing
- Scalable architecture

## 💼 Kontekst biznesowy

### Przykład: E-commerce Order System

**Bez CQRS:**
```csharp
public class OrderService
{
    public Order GetOrder(int id) => _orders.Find(id); // Loads everything
    public void UpdateOrder(Order order) => _orders.Update(order); // Blocks reads
}
```

**Z CQRS:**

**Write Side:**
```csharp
// Command: Create order
var cmd = new CreateOrderCommand { CustomerId = 123, Items = items };
await _commandBus.Send(cmd);

// ✅ Optimized for writes
// ✅ Business logic validation
// ✅ Domain events published
```

**Read Side:**
```csharp
// Query: Get order summary
var query = new GetOrderSummaryQuery { OrderId = 456 };
var summary = await _queryBus.Send(query);

// ✅ Cached in Redis
// ✅ Denormalized for display
// ✅ No business logic
```

**Korzyści:**
- **Scaling:** Reads scaled on read replicas, writes on master
- **Performance:** Reads cached, writes optimized for transactions
- **Flexibility:** Different models for admin panel vs customer view
- **Analytics:** Separate read model for reporting

## 🌟 Best Practices

### DO:
✅ Start simple - CQRS on module level first  
✅ Use MediatR lub podobną bibliotekę dla command/query dispatch  
✅ Implement retry logic dla eventual consistency  
✅ Cache aggressively on read side  
✅ Use DTOs for queries (never domain entities)  
✅ Validate commands przed handlerem  
✅ Log wszystkie commands (audit trail)  
✅ Use different databases if scaling needed  
✅ Consider Event Sourcing jeśli potrzebujesz audit  

### DON'T:
❌ Don't use CQRS everywhere - tylko gdzie ma sens  
❌ Don't return data from commands - commands return void lub ID  
❌ Don't reuse write model w queries  
❌ Don't forget cache invalidation  
❌ Don't assume immediate consistency  
❌ Don't over-engineer simple CRUD  

## 🚨 Najczęstsze pomyłki

### 1. **Returning data from commands**
```csharp
// ❌ BAD
public class CreateOrderCommand : ICommand<OrderDto> { }

// ✅ GOOD
public class CreateOrderCommand : ICommand<int> { } // Returns ID only
```

### 2. **Querying immediately after command**
```csharp
// ❌ BAD: Race condition
await _commandBus.Send(createCmd);
var order = await _queryBus.Send(new GetOrderQuery(orderId)); // May fail!

// ✅ GOOD: Return from command or poll
var orderId = await _commandBus.Send(createCmd);
// Use orderId directly or poll until available
```

### 3. **Using write model in queries**
```csharp
// ❌ BAD
public OrderViewModel GetOrder(int id)
{
    var order = _dbContext.Orders.Include(o => o.Items).First(o => o.Id == id);
    return MapToViewModel(order); // Loading entire aggregate!
}

// ✅ GOOD
public OrderViewModel GetOrder(int id)
{
    return _readRepository.GetOrderViewAsync(id); // Optimized projection
}
```

## 📝 Podsumowanie

- **CQRS** rozdziela Commands (write) od Queries (read)
- **Stosuj** gdy reads >> writes, potrzebujesz scaling, różne modele
- **Korzyści:** Independent scaling, performance optimization, flexibility
- **Wady:** Complexity, eventual consistency, code duplication
- **Najczęstsze błędy:** Assumption of immediate consistency, returning data from commands, over-engineering
- **W .NET:** MediatR library, Event Sourcing frameworks (EventStore, Marten)

### Kluczowe zasady:
1. **Commands** - intent, validation, nie zwracają danych
2. **Queries** - data retrieval, projection, caching
3. **Eventual consistency** - accept i handle properly
4. **Different models** - optimized for purpose
5. **Start simple** - add complexity gdy potrzeba
