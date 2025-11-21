# Event Sourcing Pattern

## 📌 Definicja

**Event Sourcing** to wzorzec architektoniczny, w którym **wszystkie zmiany stanu aplikacji są zapisywane jako sekwencja niezmiennych eventów**. Zamiast zapisywać tylko aktualny stan (current state), Event Sourcing zapisuje **pełną historię tego, co się wydarzyło** (what happened).

### Kluczowa zasada:
**Events = Source of Truth**  
Stan aplikacji jest **wynikiem replay** wszystkich eventów od początku.

## 🔴 Problem w BadExample

### BadExample 1: Brak historii zmian

```csharp
// ❌ BAD: Zapisujemy tylko current state
public class BankAccount
{
    public decimal Balance { get; set; } // Tylko wartość!
}

public void Deposit(decimal amount)
{
    account.Balance += amount; // ❌ Straciliśmy poprzednią wartość!
}
```

**Problemy:**
- ❌ Brak audit trail - nie wiemy kto i kiedy zmienił dane
- ❌ Nie możemy odtworzyć stanu w przeszłości
- ❌ Utrata informacji o sekwencji operacji
- ❌ Trudny debugging - "jak doszło do tego stanu?"
- ❌ Compliance issues - niektóre branże WYMAGAJĄ pełnej historii

### BadExample 2: Niemożność rekonstrukcji stanu

```csharp
// ❌ BAD: Brak historii działań użytkownika
public void AddItem(int productId)
{
    cart.Items.Add(productId);
    // ❌ Nie wiemy że użytkownik dodał i usunął ten produkt 3 razy
}
```

**Problemy:**
- ❌ Brak analityki zachowań użytkowników
- ❌ Nie możemy zobaczyć abandoned products
- ❌ Temporal queries niemożliwe
- ❌ Business insights tracone

### BadExample 3: Brak integracji z CQRS

```csharp
// ❌ BAD: Jeden model dla wszystkiego
public Order GetOrder(int id) => _orders.Find(id);
public void UpdateStatus(string status) => order.Status = status;
```

**Problemy:**
- ❌ Brak separacji read/write
- ❌ Queries ładują całe agregaty
- ❌ Brak optymalizacji
- ❌ Miss the synergy

## ✅ Rozwiązanie: Event Sourcing Pattern

### Kluczowe komponenty:

```
┌──────────────────────────────────────────────────┐
│                  COMMAND                          │
│           (Change Request)                        │
└────────────────┬─────────────────────────────────┘
                 │
                 ↓
        ┌────────────────┐
        │   AGGREGATE    │
        │  (Domain Model)│
        │                │
        │  - Execute()   │
        │  - Apply()     │
        └────────┬───────┘
                 │
                 ↓ Generates
        ┌────────────────┐
        │     EVENTS     │
        │  (What happened)│
        └────────┬───────┘
                 │
                 ↓ Append-only
        ┌────────────────┐
        │  EVENT STORE   │
        │ (Source of Truth)│
        └────────┬───────┘
                 │
                 ↓ Replay
        ┌────────────────┐
        │  CURRENT STATE │
        │  (Reconstructed)│
        └────────────────┘
```

### Implementacja:

#### 1. Domain Events (niezmienne rekordy)

```csharp
// ✅ Events = facts, immutable
public abstract class DomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public Guid AggregateId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public int Version { get; set; }
    public string UserId { get; set; } // Who made change
}

public class MoneyDepositedEvent : DomainEvent
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
}

public class MoneyWithdrawnEvent : DomainEvent
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
}
```

#### 2. Aggregate Root (buduje stan z eventów)

```csharp
// ✅ Aggregate - domain logic
public class BankAccount
{
    public Guid AccountId { get; private set; }
    public decimal Balance { get; private set; }
    public int Version { get; private set; }
    
    private List<DomainEvent> _uncommittedEvents = new();

    // ✅ Command handlers - generate events
    public void Deposit(decimal amount, string description, string userId)
    {
        if (amount <= 0)
            throw new InvalidOperationException("Amount must be positive");

        // ✅ Create event (not modifying state directly!)
        Apply(new MoneyDepositedEvent
        {
            AggregateId = AccountId,
            Amount = amount,
            Description = description,
            Version = Version + 1,
            UserId = userId
        });
    }

    // ✅ Apply event (for new events)
    private void Apply(DomainEvent @event)
    {
        When(@event); // Update state
        _uncommittedEvents.Add(@event); // Track for persistence
    }

    // ✅ Event handlers - rebuild state
    private void When(DomainEvent @event)
    {
        switch (@event)
        {
            case MoneyDepositedEvent e:
                Balance += e.Amount;
                Version = e.Version;
                break;
            
            case MoneyWithdrawnEvent e:
                Balance -= e.Amount;
                Version = e.Version;
                break;
        }
    }

    // ✅ Load from history
    public static BankAccount LoadFromHistory(IEnumerable<DomainEvent> history)
    {
        var account = new BankAccount();
        foreach (var @event in history)
        {
            account.When(@event); // Replay!
        }
        return account;
    }
}
```

#### 3. Event Store (append-only storage)

```csharp
// ✅ Event Store - stores events, not state
public interface IEventStore
{
    Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion);
    Task<List<DomainEvent>> GetEventsAsync(Guid aggregateId);
}

// ✅ Repository - load and save aggregates
public class BankAccountRepository
{
    private readonly IEventStore _eventStore;

    // ✅ Load aggregate by replaying events
    public async Task<BankAccount> GetByIdAsync(Guid accountId)
    {
        var events = await _eventStore.GetEventsAsync(accountId);
        if (!events.Any())
            return null;

        return BankAccount.LoadFromHistory(events); // ✅ Rebuild!
    }

    // ✅ Save aggregate by persisting new events
    public async Task SaveAsync(BankAccount account)
    {
        var newEvents = account.GetUncommittedEvents();
        if (!newEvents.Any())
            return;

        await _eventStore.SaveEventsAsync(
            account.AccountId,
            newEvents,
            account.Version - newEvents.Count()); // Optimistic concurrency

        account.MarkEventsAsCommitted();
    }
}
```

#### 4. Użycie

```csharp
// ✅ Create account
var account = BankAccount.Open("John Doe", 1000m, "user123");
await repository.SaveAsync(account); // Saves AccountOpenedEvent

// ✅ Perform operations
account = await repository.GetByIdAsync(accountId); // Replays all events
account.Deposit(500m, "Salary", "user123");
account.Withdraw(200m, "Shopping", "user123");
await repository.SaveAsync(account); // Saves new events

// ✅ View history
var events = await eventStore.GetEventsAsync(accountId);
foreach (var evt in events)
{
    Console.WriteLine($"{evt.Timestamp}: {evt.GetType().Name}");
}
```

## 🎯 Trzy praktyczne przykłady

### Przykład 1: Budowanie historii zmian obiektu

**Use Case:** System bankowy z pełną historią transakcji

**Implementacja:**
- Events: AccountOpened, MoneyDeposited, MoneyWithdrawn, AccountClosed
- Event Store: Append-only log
- Aggregate: Rebuilds balance from events
- Audit trail: Kto, kiedy, ile, dlaczego

**Korzyści:**
- ✅ Pełna historia wszystkich transakcji
- ✅ Compliance (regulatory requirements)
- ✅ Debugging - replay events to find bug
- ✅ Temporal queries - balance at any point in time

### Przykład 2: Rekonstrukcja stanu systemu z eventów (Projection)

**Use Case:** E-commerce shopping cart z analytics

**Implementacja:**
- Events: CartCreated, ItemAdded, ItemRemoved, QuantityChanged, CartCheckedOut
- Multiple projections:
  - Current State Projection (for display)
  - Analytics Projection (business insights)
  - Audit Trail Projection (compliance)
- Temporal queries: Stan koszyka 5 minut temu

**Korzyści:**
- ✅ Multiple views from same events
- ✅ Business analytics (abandoned items, conversion funnel)
- ✅ A/B testing insights
- ✅ User behavior patterns

### Przykład 3: Integracja Event Sourcing z CQRS

**Use Case:** Order management system

**Write Side (Event Sourcing):**
- Commands → Events → Event Store
- Append-only storage
- Full audit trail

**Read Side (CQRS):**
- Events → Projections → Read Models
- Denormalized views
- Cached, optimized queries
- Multiple read models for different purposes

**Korzyści:**
- ✅ Write side: Event history, audit, temporal queries
- ✅ Read side: Optimized queries, caching, scalability
- ✅ Independent scaling
- ✅ Perfect synergy

## 📊 Kiedy stosować Event Sourcing?

### ✅ Użyj Event Sourcing gdy:

1. **Audit trail is critical**
   - Financial systems
   - Healthcare records
   - Legal documents
   - Compliance-driven industries

2. **Temporal queries needed**
   - "What was the state at 2023-01-01?"
   - Historical analysis
   - Time travel debugging

3. **Business analytics from events**
   - User behavior patterns
   - Funnel analysis
   - A/B testing insights

4. **Complex domain logic**
   - Events capture business intent
   - Domain experts speak in events

5. **Integration with CQRS**
   - Different read models from same events
   - Optimized queries

6. **Event-driven architecture**
   - Microservices communication
   - Publish-subscribe patterns

### ❌ NIE używaj Event Sourcing gdy:

1. **Simple CRUD applications**
   - Overhead nie wart korzyści
   - State-based persistence wystarcza

2. **No audit requirements**
   - Jeśli historia nie jest ważna

3. **Small datasets**
   - Event store może być większy niż potrzeba

4. **Team not familiar**
   - Steep learning curve
   - Requires discipline

5. **Performance-critical reads**
   - Rebuilding from events może być wolne
   - (Ale można używać snapshots)

## ⚖️ Zalety i wady

### Zalety

✅ **Complete audit trail** – pełna historia kto, kiedy, co  
✅ **Temporal queries** – stan w dowolnym momencie czasu  
✅ **Event replay** – debugging, testing, migration  
✅ **Multiple projections** – różne widoki z tych samych eventów  
✅ **Event-driven architecture** – reactive systems  
✅ **Business insights** – analytics z event stream  
✅ **No data loss** – events nigdy nie są kasowane  
✅ **Compliance** – regulatory requirements  

### Wady

❌ **Complexity** – więcej kodu, bardziej złożone  
❌ **Learning curve** – zespół musi zrozumieć wzorzec  
❌ **Event versioning** – eventy muszą być backward compatible  
❌ **Storage overhead** – event store rośnie szybko  
❌ **Eventual consistency** – projekcje mogą być outdated  
❌ **Performance** – replay może być wolny (snapshots help)  
❌ **Query complexity** – ad-hoc queries trudne (CQRS helps)  

## ⚠️ Na co uważać?

### 1. **Event versioning - backward compatibility**

```csharp
// ❌ BAD: Breaking change in event
public class OrderPlacedEvent_V1
{
    public string CustomerName { get; set; }
}

public class OrderPlacedEvent_V2
{
    public int CustomerId { get; set; } // ❌ Zmiana typu!
}

// ✅ GOOD: Add new fields, keep old
public class OrderPlacedEvent
{
    public string CustomerName { get; set; } // Keep for V1
    public int? CustomerId { get; set; } // Add for V2
    public int Version { get; set; }
}

// ✅ Or use event upcasting
public class EventUpcaster
{
    public DomainEvent Upcast(DomainEvent @event)
    {
        if (@event is OrderPlacedEvent_V1 v1)
        {
            return new OrderPlacedEvent_V2
            {
                CustomerId = LookupCustomerId(v1.CustomerName)
            };
        }
        return @event;
    }
}
```

### 2. **Snapshots for performance**

```csharp
// ❌ BAD: Replay 10,000 events każdorazowo
public async Task<Order> GetByIdAsync(Guid id)
{
    var events = await _eventStore.GetEventsAsync(id); // 10k events!
    return Order.LoadFromHistory(events); // Slow!
}

// ✅ GOOD: Use snapshots
public async Task<Order> GetByIdAsync(Guid id)
{
    var snapshot = await _snapshotStore.GetSnapshotAsync(id);
    var events = await _eventStore.GetEventsAsync(id, fromVersion: snapshot?.Version ?? 0);
    
    var order = snapshot != null 
        ? Order.LoadFromSnapshot(snapshot)
        : new Order();
    
    order.LoadFromHistory(events);
    return order;
}

// ✅ Save snapshot every N events
public async Task SaveAsync(Order order)
{
    await _eventStore.SaveEventsAsync(order.AccountId, order.GetUncommittedEvents());
    
    if (order.Version % 100 == 0) // Every 100 events
    {
        await _snapshotStore.SaveSnapshotAsync(order.CreateSnapshot());
    }
}
```

### 3. **Idempotent event handlers**

```csharp
// ❌ BAD: Non-idempotent projection
public async Task HandleAsync(OrderPlacedEvent @event)
{
    await _readRepository.InsertAsync(new OrderReadModel(@event));
    // ❌ Co jeśli event zostanie przetworzony 2 razy?
}

// ✅ GOOD: Idempotent with event ID tracking
public async Task HandleAsync(OrderPlacedEvent @event)
{
    if (await _processedEvents.ExistsAsync(@event.EventId))
        return; // Already processed

    await _readRepository.InsertAsync(new OrderReadModel(@event));
    await _processedEvents.MarkAsProcessedAsync(@event.EventId);
}
```

### 4. **Optimistic concurrency**

```csharp
// ❌ BAD: No concurrency check
public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events)
{
    _events.AddRange(events); // ❌ Race condition!
}

// ✅ GOOD: Optimistic locking with version
public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<DomainEvent> events, int expectedVersion)
{
    var currentVersion = _events
        .Where(e => e.AggregateId == aggregateId)
        .Max(e => e.Version);

    if (currentVersion != expectedVersion)
        throw new ConcurrencyException("Version conflict");

    _events.AddRange(events);
}
```

### 5. **Event size and storage**

```csharp
// ❌ BAD: Storing large objects in events
public class DocumentUpdatedEvent
{
    public byte[] DocumentContent { get; set; } // ❌ 10MB file!
}

// ✅ GOOD: Store reference, not content
public class DocumentUpdatedEvent
{
    public string DocumentId { get; set; } // ✅ Reference
    public string BlobUrl { get; set; }
    public string ContentHash { get; set; } // For verification
}
```

### 6. **Event ordering**

```csharp
// ❌ BAD: Assuming global event order
foreach (var @event in allEvents)
{
    HandleEvent(@event); // ❌ Events from different aggregates!
}

// ✅ GOOD: Per-aggregate ordering
var aggregateEvents = allEvents
    .Where(e => e.AggregateId == aggregateId)
    .OrderBy(e => e.Version); // ✅ Order within aggregate

foreach (var @event in aggregateEvents)
{
    HandleEvent(@event);
}
```

## 🔄 Event Sourcing Patterns

### 1. **Basic Event Sourcing**
```
Commands → Events → Event Store
                      ↓ Replay
                  Current State
```

### 2. **Event Sourcing + Snapshots**
```
Events → Event Store
           ↓
        Snapshot (every N events)
           ↓
        Fast rebuild
```

### 3. **Event Sourcing + CQRS**
```
Commands → Events → Event Store
                      ↓ Projections
                  Read Models (multiple)
```

### 4. **Event Sourcing + Saga**
```
Event → Event Store → Saga Coordinator → Commands → Other Aggregates
```

## 💼 Kontekst biznesowy

### Przykład: Healthcare Patient Record System

**Bez Event Sourcing:**
```csharp
public class PatientRecord
{
    public string Diagnosis { get; set; } // Tylko current!
    public string Treatment { get; set; }
}
// ❌ Utraciliśmy historię zmian diagnozy
// ❌ Compliance problem - regulatory wymaga audit trail
```

**Z Event Sourcing:**

**Events:**
- PatientAdmitted
- DiagnosisAdded
- TreatmentPrescribed
- MedicationAdministered
- PatientDischarged

**Korzyści:**
- **Audit trail:** Kto, kiedy, jaką diagnozę postawił
- **Compliance:** Full history dla regulatorów
- **Temporal queries:** Stan pacjenta w dowolnym momencie
- **Analytics:** Treatment effectiveness, patterns
- **Medical research:** Historical data analysis

## 🌟 Best Practices

### DO:
✅ Make events immutable (append-only)  
✅ Store WHO made change (UserId in event)  
✅ Use snapshots for large aggregates (>100 events)  
✅ Version your events (for schema evolution)  
✅ Keep events small (store references, not large data)  
✅ Make event handlers idempotent  
✅ Use optimistic concurrency (version checking)  
✅ Consider event store as primary database  
✅ Integrate with CQRS for read optimization  

### DON'T:
❌ Don't modify or delete events (append-only!)  
❌ Don't put business logic in event handlers  
❌ Don't assume global event ordering  
❌ Don't store large binary data in events  
❌ Don't use Event Sourcing everywhere  
❌ Don't ignore event versioning  
❌ Don't forget snapshots for large streams  

## 🚨 Najczęstsze pomyłki

### 1. **Modifying existing events**
```csharp
// ❌ BAD: Mutating event
var evt = await _eventStore.GetEvent(eventId);
evt.Amount = newAmount; // ❌ Events are immutable!
await _eventStore.UpdateEvent(evt);

// ✅ GOOD: Create new event
await _eventStore.AppendEvent(new AmountCorrectedEvent
{
    OriginalEventId = eventId,
    OldAmount = oldAmount,
    NewAmount = newAmount,
    Reason = "Correction"
});
```

### 2. **Business logic in event handlers**
```csharp
// ❌ BAD: Logic in When()
private void When(MoneyDepositedEvent e)
{
    if (e.Amount > 10000)
        SendAlertToCompliance(); // ❌ Side effect!
    Balance += e.Amount;
}

// ✅ GOOD: Logic in command handler
public void Deposit(decimal amount)
{
    if (amount > 10000)
        SendAlertToCompliance(); // ✅ Before event

    Apply(new MoneyDepositedEvent { Amount = amount });
}
```

### 3. **No snapshots for large streams**
```csharp
// ❌ BAD: Always replay all events
var events = await _eventStore.GetEventsAsync(id); // 10k events!
var aggregate = Aggregate.LoadFromHistory(events); // Slow!

// ✅ GOOD: Use snapshots
var snapshot = await _snapshotStore.GetLatestAsync(id);
var events = await _eventStore.GetEventsAsync(id, snapshot.Version);
var aggregate = Aggregate.LoadFromSnapshot(snapshot);
aggregate.ApplyEvents(events);
```

### 4. **Not versioning events**
```csharp
// ❌ BAD: Changing event structure
public class OrderPlacedEvent
{
    // Old: public string CustomerName;
    public Customer Customer; // ❌ Breaking change!
}

// ✅ GOOD: Add version and keep backward compatibility
public class OrderPlacedEvent
{
    public int EventVersion { get; set; } = 2;
    public string CustomerName { get; set; } // V1
    public Customer Customer { get; set; } // V2
}
```

## 📝 Podsumowanie

- **Event Sourcing** zapisuje historię zmian jako events, nie current state
- **Events** są źródłem prawdy (source of truth), immutable
- **Stan** jest rebuilowany przez replay eventów
- **Stosuj** gdy: audit trail, temporal queries, complex domain, CQRS
- **Korzyści:** Full history, temporal queries, replay, projections, analytics
- **Wady:** Complexity, versioning, storage overhead, learning curve
- **Best practices:** Snapshots, idempotent handlers, event versioning, small events
- **Perfect match z CQRS** dla write optimization + read optimization

### Event Sourcing w .NET:
- **EventStore** - dedykowany event store database
- **Marten** - PostgreSQL-based event store
- **NEventStore** - event sourcing library
- **EventFlow** - CQRS + Event Sourcing framework
