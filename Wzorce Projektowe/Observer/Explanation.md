# Observer Pattern

## 📌 Definicja
Observer (Obserwator) to behawioralny wzorzec projektowy, który definiuje **relację jeden-do-wielu** między obiektami. Gdy stan obiektu (Subject) się zmienia, **wszyscy jego obserwatorzy są automatycznie powiadamiani** i aktualizowani.

### Znany również jako:
- **Publish-Subscribe** (Pub-Sub)
- **Event-Listener**
- **Dependents**

## 🔴 Problem w BadExample

Bezpośrednie powiązanie między Subject a konkretnymi klasami obserwatorów:

```csharp
public class WeatherStation
{
    private PhoneDisplay _phoneDisplay;
    private WebDisplay _webDisplay;
    private TVDisplay _tvDisplay;
    
    public void UpdateTemperature(float temp)
    {
        _phoneDisplay.Update(temp);  // Tight coupling!
        _webDisplay.Update(temp);
        _tvDisplay.Update(temp);
    }
}
```

### Problemy:
❌ **Tight coupling** – WeatherStation musi znać wszystkie konkretne klasy displayów  
❌ **Naruszenie OCP** – dodanie nowego display wymaga modyfikacji WeatherStation  
❌ **Brak dynamiki** – nie można dodać/usunąć displayów w runtime  
❌ **Duplikacja** – logika powiadamiania rozrzucona po całym kodzie  
❌ **Trudne testowanie** – niemożliwe mockowanie obserwatorów  

## ✅ Rozwiązanie w GoodExample

### Struktura:

```
┌─────────────────┐        ┌──────────────────┐
│   IObserver<T>  │◄───────│  ISubject<T>     │
│   + Update(T)   │        │  + Attach()      │
└────────┬────────┘        │  + Detach()      │
         │                 │  + Notify()      │
         │                 └────────┬─────────┘
         │                          │
    ┌────┴────────┐        ┌────────┴────────┐
    │  Concrete   │        │   Concrete      │
    │  Observer   │        │   Subject       │
    └─────────────┘        └─────────────────┘
```

### Implementacja:

```csharp
// Observer interface
public interface IObserver<T>
{
    void Update(T data);
}

// Subject interface
public interface ISubject<T>
{
    void Attach(IObserver<T> observer);
    void Detach(IObserver<T> observer);
    void Notify(T data);
}

// Concrete Subject
public class WeatherStation : ISubject<WeatherData>
{
    private List<IObserver<WeatherData>> _observers = new();
    
    public void Attach(IObserver<WeatherData> observer)
    {
        _observers.Add(observer);
    }
    
    public void Detach(IObserver<WeatherData> observer)
    {
        _observers.Remove(observer);
    }
    
    public void Notify(WeatherData data)
    {
        foreach (var observer in _observers)
        {
            observer.Update(data);
        }
    }
    
    public void SetWeather(float temp, float humidity)
    {
        var data = new WeatherData(temp, humidity);
        Notify(data);  // Powiadom wszystkich!
    }
}

// Concrete Observers
public class PhoneDisplay : IObserver<WeatherData>
{
    public void Update(WeatherData data)
    {
        Console.WriteLine($"Phone: {data.Temperature}°C");
    }
}

public class WebDisplay : IObserver<WeatherData>
{
    public void Update(WeatherData data)
    {
        Console.WriteLine($"Web: {data.Temperature}°C");
    }
}
```

### Korzyści:
✅ **Loose coupling** – Subject nie zna konkretnych klas obserwatorów  
✅ **OCP compliance** – nowy observer bez modyfikacji Subject  
✅ **Dynamiczne zarządzanie** – Attach/Detach w runtime  
✅ **Reużywalność** – Subject może być używany z dowolnymi obserwatorami  
✅ **Testowalność** – łatwe mockowanie obserwatorów  

## 🎯 Po co stosować Observer?

### 1. **Rozdzielenie odpowiedzialności**
Subject odpowiada za swój stan, Observers za reakcje na zmiany.

### 2. **Broadcast communication**
Jeden event → wiele reakcji od różnych komponentów.

### 3. **Loose coupling**
Komponenty nie muszą znać się nawzajem, tylko kontrakt (interface).

### 4. **Dynamiczna konfiguracja**
Obserwatorzy mogą być dodawani/usuwani w runtime bez zmian w Subject.

## W czym pomaga?

✅ **Event-driven architecture** – reagowanie na zmiany w systemie  
✅ **UI updates** – automatyczne odświeżanie widoków przy zmianach modelu (MVC/MVVM)  
✅ **Distributed systems** – powiadamianie wielu serwisów o eventach  
✅ **Real-time updates** – ceny akcji, wyniki sportowe, powiadomienia  
✅ **Logging i monitoring** – centralne logowanie zmian  
✅ **Cache invalidation** – automatyczne czyszczenie cache przy zmianach danych  

## ⚖️ Zalety i wady

### Zalety
✅ **Open/Closed Principle** – nowi obserwatorzy bez zmian w Subject  
✅ **Runtime relationships** – dynamiczne przyłączanie obserwatorów  
✅ **Loose coupling** – Subject i Observer niezależne  
✅ **Broadcast** – jeden event, wiele odbiorców  
✅ **Separation of Concerns** – Subject zarządza stanem, Observers reakcjami  

### Wady
❌ **Unexpected updates** – obserwatorzy powiadamiani w nieprzewidywalnej kolejności  
❌ **Memory leaks** – niezdetachowane obserwatory trzymają referencje  
❌ **Performance** – wiele obserwatorów = wiele wywołań przy każdej zmianie  
❌ **Debugging** – trudne śledzenie łańcucha wywołań  
❌ **Cascade updates** – observer może zmienić Subject → kolejny notify → nieskończona pętla  

## ⚠️ Na co uważać?

### 1. **Memory leaks przez niezdetachowane observery**
```csharp
// ❌ BAD: Zapomnienie o detach
public class UserService
{
    public void ProcessUser(User user)
    {
        var notifier = new EmailNotifier();
        _userSubject.Attach(notifier);
        
        // ... operacje ...
        
        // notifier nie został detachowany!
        // _userSubject trzyma referencję → memory leak!
    }
}

// ✅ GOOD: Zawsze detach lub używaj weak references
public class UserService
{
    public void ProcessUser(User user)
    {
        var notifier = new EmailNotifier();
        _userSubject.Attach(notifier);
        
        try
        {
            // ... operacje ...
        }
        finally
        {
            _userSubject.Detach(notifier);  // Cleanup!
        }
    }
}

// ✅ BETTER: Using pattern z IDisposable
public class ObserverSubscription : IDisposable
{
    private readonly ISubject<T> _subject;
    private readonly IObserver<T> _observer;
    
    public void Dispose()
    {
        _subject.Detach(_observer);
    }
}

// Użycie:
using var subscription = subject.Subscribe(observer);
// Automatyczny detach przy wyjściu z scope!
```

### 2. **Race conditions w multi-threaded scenarios**
```csharp
// ❌ BAD: Nie thread-safe
public void Notify(T data)
{
    foreach (var observer in _observers)
    {
        observer.Update(data);  // Co jeśli inny wątek modyfikuje _observers?
    }
}

// ✅ GOOD: Thread-safe notification
private readonly object _lock = new object();

public void Notify(T data)
{
    IObserver<T>[] observersCopy;
    
    lock (_lock)
    {
        observersCopy = _observers.ToArray();  // Snapshot
    }
    
    foreach (var observer in observersCopy)
    {
        observer.Update(data);
    }
}
```

### 3. **Nieskończone pętle aktualizacji**
```csharp
// ❌ BAD: Observer zmienia Subject → notify → observer zmienia Subject...
public class BadObserver : IObserver<int>
{
    private ISubject<int> _subject;
    
    public void Update(int value)
    {
        _subject.Notify(value + 1);  // ❌ Nieskończona pętla!
    }
}

// ✅ GOOD: Observer nie modyfikuje Subject bezpośrednio
public class GoodObserver : IObserver<int>
{
    public void Update(int value)
    {
        // Tylko reakcja, bez modyfikacji Subject
        Console.WriteLine($"Received: {value}");
    }
}
```

### 4. **Wyjątki w obserwatorach**
```csharp
// ❌ BAD: Wyjątek w jednym observerze zatrzymuje notyfikację innych
public void Notify(T data)
{
    foreach (var observer in _observers)
    {
        observer.Update(data);  // Jeśli rzuci exception → kolejne nie dostaną update!
    }
}

// ✅ GOOD: Izolacja błędów
public void Notify(T data)
{
    foreach (var observer in _observers)
    {
        try
        {
            observer.Update(data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Observer update failed");
            // Kontynuuj powiadamianie innych
        }
    }
}
```

### 5. **Performance przy dużej liczbie obserwatorów**
```csharp
// Problem: 1000 obserwatorów × 100 updates/sec = 100,000 wywołań/sec

// ✅ Rozwiązanie 1: Batching
public class BatchingSubject<T>
{
    private Queue<T> _pendingUpdates = new();
    
    public void Notify(T data)
    {
        _pendingUpdates.Enqueue(data);
        
        if (_pendingUpdates.Count >= 10)  // Batch co 10 updates
        {
            FlushUpdates();
        }
    }
}

// ✅ Rozwiązanie 2: Async notifications
public async Task NotifyAsync(T data)
{
    var tasks = _observers.Select(o => Task.Run(() => o.Update(data)));
    await Task.WhenAll(tasks);
}
```

## 🔄 Kiedy stosować Observer?

### Użyj Observer gdy:
✅ **Zmiana w jednym obiekcie wymaga zmian w innych** – ale nie wiesz ile i jakie obiekty  
✅ **Chcesz broadcast events** – jeden sender, wielu receivers  
✅ **Potrzebujesz loose coupling** – sender nie powinien znać receivers  
✅ **Dynamiczne subskrypcje** – obserwatorzy mogą się zmieniać w runtime  
✅ **Event-driven architecture** – system bazuje na eventach  

### NIE używaj Observer gdy:
❌ **Prosty one-to-one callback wystarczy** – niepotrzebna złożoność  
❌ **Synchroniczna komunikacja jest wymagana** – Observer jest asynchroniczny  
❌ **Order of execution ma znaczenie** – Observer nie gwarantuje kolejności  

## 🚨 Najczęstsze pomyłki

### 1. **Używanie strong references (memory leaks)**
```csharp
// ❌ BAD
private List<IObserver<T>> _observers = new();

// ✅ GOOD: WeakReference dla długo żyjących Subjects
private List<WeakReference<IObserver<T>>> _observers = new();
```

### 2. **Zapominanie o exception handling**
```csharp
// ❌ BAD: Jeden błędny observer zatrzymuje wszystkich
Notify(data);

// ✅ GOOD: Izoluj błędy
foreach (var obs in _observers)
{
    try { obs.Update(data); }
    catch (Exception ex) { LogError(ex); }
}
```

### 3. **Modyfikacja kolekcji podczas iteracji**
```csharp
// ❌ BAD: Detach podczas Notify
public void Notify(T data)
{
    foreach (var obs in _observers)
    {
        obs.Update(data);
        _observers.Remove(obs);  // ❌ InvalidOperationException!
    }
}

// ✅ GOOD: Snapshot przed iteracją
public void Notify(T data)
{
    var snapshot = _observers.ToArray();
    foreach (var obs in snapshot)
    {
        obs.Update(data);
    }
}
```

### 4. **Brak cleanup mechanizmu**
```csharp
// ❌ BAD: Brak IDisposable
public class MyObserver : IObserver<T>
{
    public MyObserver(ISubject<T> subject)
    {
        subject.Attach(this);  // Kiedy detach???
    }
}

// ✅ GOOD: IDisposable dla cleanup
public class MyObserver : IObserver<T>, IDisposable
{
    private readonly ISubject<T> _subject;
    
    public MyObserver(ISubject<T> subject)
    {
        _subject = subject;
        _subject.Attach(this);
    }
    
    public void Dispose()
    {
        _subject.Detach(this);
    }
}
```

## 💼 Kontekst biznesowy

### Przykład: System notyfikacji e-commerce

**Scenariusz:** Zamówienie zostało złożone → powiadom różne systemy

**Bez Observer:**
```csharp
public class OrderService
{
    public void PlaceOrder(Order order)
    {
        _repository.Save(order);
        _emailService.SendConfirmation(order);    // Tight coupling!
        _smsService.SendSms(order);
        _analyticsService.Track(order);
        _inventoryService.Reserve(order);
        _loyaltyService.AddPoints(order);
        // Każdy nowy system = modyfikacja OrderService!
    }
}
```

**Z Observer:**
```csharp
public class OrderService : ISubject<OrderPlacedEvent>
{
    private List<IObserver<OrderPlacedEvent>> _observers = new();
    
    public void PlaceOrder(Order order)
    {
        _repository.Save(order);
        
        var evt = new OrderPlacedEvent(order);
        Notify(evt);  // Wszyscy zainteresowani dostają event!
    }
}

// Observers (każdy w osobnym module):
public class EmailNotifier : IObserver<OrderPlacedEvent> { }
public class SmsNotifier : IObserver<OrderPlacedEvent> { }
public class AnalyticsTracker : IObserver<OrderPlacedEvent> { }
public class InventoryReserver : IObserver<OrderPlacedEvent> { }
public class LoyaltyPointsAdder : IObserver<OrderPlacedEvent> { }

// Dodanie nowego systemu = ZERO zmian w OrderService!
```

**Korzyści:**
- **Nowy system?** → Dodaj nowego Observera, zero zmian w OrderService
- **Testowanie** → Mockuj tylko potrzebne Observers
- **Skalowanie** → Dodaj więcej instancji Observers
- **Monitoring** → Dodaj MonitoringObserver bez zmian w biznesie

## 🔧 Implementacje w C#

### 1. **Classic Observer (ręczna implementacja)**
Pokazana powyżej – pełna kontrola, ale więcej kodu.

### 2. **C# Events (built-in)**
```csharp
// Subject z eventami
public class WeatherStation
{
    public event EventHandler<WeatherData> WeatherChanged;
    
    public void SetWeather(float temp)
    {
        WeatherChanged?.Invoke(this, new WeatherData(temp));
    }
}

// Observer
var station = new WeatherStation();
station.WeatherChanged += (sender, data) => 
{
    Console.WriteLine($"Weather: {data.Temperature}°C");
};
```

**Zalety:** Prosty, built-in, thread-safe  
**Wady:** Łatwo zapomnieć o `-=` (memory leak)

### 3. **Reactive Extensions (Rx.NET)**
```csharp
var subject = new Subject<WeatherData>();

// Subscribe
var subscription = subject.Subscribe(
    data => Console.WriteLine($"Weather: {data.Temperature}°C")
);

// Notify
subject.OnNext(new WeatherData(25.5f));

// Cleanup
subscription.Dispose();
```

**Zalety:** Potężne operatory (Filter, Map, Throttle), automatyczny cleanup  
**Wady:** Dodatkowa zależność (Rx.NET)

### 4. **IObservable<T> / IObserver<T> (.NET built-in)**
```csharp
public class WeatherStation : IObservable<WeatherData>
{
    private List<IObserver<WeatherData>> _observers = new();
    
    public IDisposable Subscribe(IObserver<WeatherData> observer)
    {
        _observers.Add(observer);
        return new Unsubscriber(_observers, observer);  // Auto-cleanup!
    }
}
```

**Zalety:** .NET standard, IDisposable dla cleanup  
**Wady:** Więcej boilerplate niż events

## 📝 Podsumowanie

- **Observer** definiuje relację 1-do-wielu dla automatycznego powiadamiania
- **Stosuj** dla event-driven architecture, UI updates, distributed notifications
- **Uważaj** na memory leaks (zawsze detach), race conditions, exception handling
- **W C#** używaj: events (prosto), Rx.NET (zaawansowane), lub ręczne (pełna kontrola)
- **Najczęstsze błędy:** brak cleanup, modyfikacja podczas iteracji, brak thread-safety
