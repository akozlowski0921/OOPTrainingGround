# Decorator Pattern

## 📌 Problem w Bad Example
Próba dodawania funkcjonalności przez **dziedziczenie** prowadzi do **eksplozji liczby klas**:
- 4 dodatki = 2⁴ = **16 klas**
- 5 dodatków = **32 klasy**
- 6 dodatków = **64 klasy**

To prowadzi do:
- **Niemożliwości utrzymania** – setki klas do zarządzania
- **Duplikacji kodu** – logika dodatków powtarza się w każdej klasie
- **Braku elastyczności** – niemożność dynamicznego dodawania funkcjonalności
- **Trudności w zmianach** – zmiana ceny dodatku wymaga edycji wielu klas

## ✅ Rozwiązanie: Decorator Pattern
Decorator to wzorzec strukturalny, który pozwala **dynamicznie dodawać nowe funkcjonalności do obiektów** poprzez opakowywanie ich w obiekty dekoratorów.

### Kluczowe elementy:
1. **Interfejs/klasa bazowa** (`Coffee`) – definiuje wspólny kontrakt
2. **Komponent konkretny** (`SimpleCoffee`) – podstawowa implementacja
3. **Dekorator bazowy** (`CoffeeDecorator`) – implementuje interfejs i trzyma referencję do obiektu
4. **Dekoratory konkretne** (`MilkDecorator`, `SugarDecorator`) – dodają konkretne funkcjonalności

## 🎯 Korzyści

### 1. Unikanie eksplozji klas
```typescript
// Zamiast 16 klas dla 4 dodatków, mamy tylko 4 klasy dekoratorów!
// Każda kombinacja = kompozycja dekoratorów
```

### 2. Elastyczność w runtime
```typescript
let coffee: Coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
// Dynamiczne budowanie w czasie wykonania!
```

### 3. Single Responsibility
```typescript
// Każdy dekorator odpowiada tylko za jeden dodatek:
class CaramelDecorator extends CoffeeDecorator {
    getCost(): number {
        return this.coffee.getCost() + 2;  // Tylko logika karmelu
    }
}
```

### 4. Open/Closed Principle
```typescript
// Nowy dodatek = nowa klasa dekoratora (bez modyfikacji istniejących):
class VanillaDecorator extends CoffeeDecorator {
    // Nie dotykamy innych klas!
}
```

## 🔄 Kiedy stosować?
- Chcesz **dynamicznie** dodawać funkcjonalności do obiektów
- Rozszerzanie przez dziedziczenie jest **niepraktyczne** (eksplozja klas)
- Potrzebujesz **różnych kombinacji** funkcjonalności
- Chcesz uniknąć **modyfikacji** istniejących klas

## 📦 Przykłady w praktyce
1. **Java I/O Streams**:
   ```java
   InputStream in = new BufferedInputStream(
       new FileInputStream("file.txt")
   );
   ```

2. **React Higher-Order Components**:
   ```typescript
   const EnhancedComponent = withAuth(withLogging(BaseComponent));
   ```

3. **.NET Middleware Pipeline**:
   ```csharp
   app.UseAuthentication();
   app.UseAuthorization();
   ```

## ⚠️ Uwagi
- Może prowadzić do **dużej liczby małych obiektów** w pamięci
- Debugowanie może być trudniejsze (wiele poziomów opakowań)
- W TypeScript można też użyć **mixinów** dla prostszych przypadków
- Kolejność dekoratorów **może mieć znaczenie**

## 🆚 Decorator vs. Strategy
- **Strategy** – zmienia **algorytm** obiektu
- **Decorator** – dodaje **nowe funkcjonalności** do obiektu

---

## 🎯 FAQ / INSIGHT

### Po co stosować Decorator?

**Decorator rozwiązuje problem:**
- **Eksplozji klas** – zamiast 100 klas dla wszystkich kombinacji, masz 10 dekoratorów
- **Sztywnego dziedziczenia** – funkcjonalności dodawane statycznie w compile time
- **Braku elastyczności** – niemożność zmiany funkcjonalności w runtime
- **Modyfikacji istniejących klas** – naruszenie Open/Closed Principle

**Decorator zapewnia:**
- **Kompozycję funkcjonalności** – budowanie złożonych obiektów z prostych części
- **Runtime flexibility** – dodawanie/usuwanie funkcjonalności dynamicznie
- **Reużywalność** – każdy dekorator może być użyty z dowolnym komponentem
- **Przejrzystość** – dekoratory są interchan geable, nie zmieniają interfejsu

### W czym pomaga stosowanie Decorator?

✅ **Unikanie class explosion** – N dekoratorów zamiast 2^N klas  
✅ **Dynamiczne rozszerzanie** – dodawaj funkcjonalności w runtime  
✅ **Open/Closed Principle** – rozszerzaj bez modyfikacji  
✅ **Single Responsibility** – każdy dekorator ma jedną odpowiedzialność  
✅ **Kompozycja nad dziedziczeniem** – elastyczniejsze niż hierarchia klas  
✅ **Testowanie** – każdy dekorator testowany osobno  

### ⚖️ Zalety i wady Decorator

#### Zalety
✅ **Elastyczność** – funkcjonalności dodawane/usuwane w runtime  
✅ **Brak eksplozji klas** – liniowy wzrost liczby klas zamiast wykładniczego  
✅ **Single Responsibility** – każdy dekorator robi jedną rzecz  
✅ **Open/Closed** – nowe funkcjonalności bez modyfikacji kodu  
✅ **Kompozycja** – różne kombinacje dekoratorów  
✅ **Transparent** – dekorator ma ten sam interfejs co obiekt bazowy  

#### Wady
❌ **Złożoność** – wiele małych obiektów zamiast jednego  
❌ **Trudniejszy debugging** – wielopoziomowe opakowania  
❌ **Kolejność ma znaczenie** – `Decorator1(Decorator2(obj))` ≠ `Decorator2(Decorator1(obj))`  
❌ **Identity problem** – `decorator !== originalObject`  
❌ **Performance overhead** – każde wywołanie przechodzi przez warstwę  
❌ **Konfiguracja** – trzeba ręcznie składać dekoratory  

### ⚠️ Na co uważać przy stosowaniu Decorator?

#### 1. **Kolejność dekoratorów ma znaczenie**
```typescript
// ❌ Różne wyniki w zależności od kolejności
const coffee1 = new DiscountDecorator(
    new TaxDecorator(new SimpleCoffee())
);
// Tax: (10 * 1.2) = 12, Discount: 12 * 0.9 = 10.8

const coffee2 = new TaxDecorator(
    new DiscountDecorator(new SimpleCoffee())
);
// Discount: (10 * 0.9) = 9, Tax: 9 * 1.2 = 10.8
// Inna semantyka biznesowa!

// ✅ GOOD: Zdefiniuj jasną kolejność
public class CoffeeBuilder
{
    public Coffee Build()
    {
        Coffee coffee = new SimpleCoffee();
        
        // Zawsze: additives → discount → tax
        coffee = ApplyAdditives(coffee);
        coffee = ApplyDiscount(coffee);
        coffee = ApplyTax(coffee);
        
        return coffee;
    }
}
```

#### 2. **Memory overhead przy wielu warstwach**
```csharp
// ❌ BAD: 10 warstw dekoratorów
ICoffee coffee = new SimpleCoffee();
for (int i = 0; i < 10; i++)
{
    coffee = new LoggingDecorator(coffee);  // 10 obiektów!
}

// ✅ GOOD: Rozważ Composite pattern dla wielu podobnych
public class CompositeLoggingDecorator : CoffeeDecorator
{
    private readonly List<ILogger> _loggers;
    
    public override double GetCost()
    {
        var cost = coffee.GetCost();
        
        // Jedna warstwa, wiele loggerów
        foreach (var logger in _loggers)
        {
            logger.Log($"Cost: {cost}");
        }
        
        return cost;
    }
}
```

#### 3. **Identity i equality problems**
```typescript
// ❌ Dekorator zmienia identity
const original = new SimpleCoffee();
const decorated = new MilkDecorator(original);

console.log(decorated === original);  // false
console.log(decorated instanceof SimpleCoffee);  // false!

// ✅ Jeśli identity jest ważna, użyj Proxy pattern lub udostępnij unwrap()
interface Coffee {
    getCost(): number;
    unwrap?(): Coffee;  // Dostęp do oryginału
}

class MilkDecorator implements Coffee {
    constructor(private coffee: Coffee) {}
    
    getCost(): number {
        return this.coffee.getCost() + 2;
    }
    
    unwrap(): Coffee {
        return this.coffee;
    }
}
```

#### 4. **Trudności w konfiguracji**
```csharp
// ❌ BAD: Ręczne tworzenie łańcucha
var service = new CachingDecorator(
    new LoggingDecorator(
        new RetryDecorator(
            new TimeoutDecorator(
                new RealService()
            )
        )
    )
);

// ✅ GOOD: Builder lub DI container
// Builder pattern
var service = new ServiceBuilder()
    .WithBase(new RealService())
    .WithTimeout(5000)
    .WithRetry(3)
    .WithLogging()
    .WithCaching()
    .Build();

// Lub ASP.NET Core DI
services.AddTransient<IRealService, RealService>();
services.Decorate<IRealService, TimeoutDecorator>();
services.Decorate<IRealService, RetryDecorator>();
services.Decorate<IRealService, LoggingDecorator>();
services.Decorate<IRealService, CachingDecorator>();
```

#### 5. **Stacktrace w debugowaniu**
```csharp
// Problem: Długi stacktrace przez wiele dekoratorów
CachingDecorator.GetCost()
  LoggingDecorator.GetCost()
    RetryDecorator.GetCost()
      TimeoutDecorator.GetCost()
        RealService.GetCost()

// ✅ Dodaj metadata do dekoratorów dla łatwiejszego debugowania
public abstract class NamedDecorator : CoffeeDecorator
{
    public string DecoratorName { get; protected set; }
    
    public override double GetCost()
    {
        try
        {
            return coffee.GetCost();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error in {DecoratorName}", ex);
        }
    }
}
```

### 🚨 Najczęstsze pomyłki

#### 1. **Dekorator modyfikuje interfejs bazowy**
```typescript
// ❌ BAD: Dekorator dodaje nowe metody
interface Coffee {
    getCost(): number;
}

class MilkDecorator implements Coffee {
    getCost(): number { ... }
    getMilkType(): string { ... }  // ❌ Nowa metoda!
}

// Problem: Nie można traktować jak Coffee
function printCost(coffee: Coffee) {
    console.log(coffee.getMilkType());  // ❌ Compile error!
}

// ✅ GOOD: Dekorator ma ten sam interfejs
class MilkDecorator implements Coffee {
    getCost(): number {
        // Milk type jako część implementacji, nie API
        const milkCost = this.getMilkTypeCost();
        return this.coffee.getCost() + milkCost;
    }
    
    private getMilkTypeCost(): number { ... }  // Private helper
}
```

#### 2. **Zapominanie o delegacji do obiektu bazowego**
```csharp
// ❌ BAD: Nie deleguje do coffee
public class SugarDecorator : CoffeeDecorator
{
    public override double GetCost()
    {
        return 1.5;  // ❌ Zapomniał o coffee.GetCost()!
    }
    
    public override string GetDescription()
    {
        return "With sugar";  // ❌ Zgubił oryginalny opis!
    }
}

// ✅ GOOD: Zawsze deleguj i rozszerzaj
public class SugarDecorator : CoffeeDecorator
{
    public override double GetCost()
    {
        return coffee.GetCost() + 1.5;  // ✅ Delegacja + rozszerzenie
    }
    
    public override string GetDescription()
    {
        return coffee.GetDescription() + ", with sugar";  // ✅
    }
}
```

#### 3. **Używanie Decorator tam gdzie wystarczy dziedziczenie**
```typescript
// ❌ Overkill: Tylko jedna "specjalizacja"
class ExpressoCoffee extends Coffee { }  // Wystarczy!

// Nie trzeba:
class ExpressoDecorator extends CoffeeDecorator { }

// ✅ Decorator używaj gdy:
// - Wiele kombinacji (milk + sugar + caramel...)
// - Dynamiczne dodawanie w runtime
// - Różne kombinacje dla różnych użytkowników
```

#### 4. **Circular decoration**
```csharp
// ❌ BAD: Możliwa nieskończona pętla
Coffee coffee = new SimpleCoffee();
coffee = new MilkDecorator(coffee);
coffee = new SugarDecorator(coffee);
coffee = new MilkDecorator(coffee);  // Czy to ma sens?

// ✅ Rozważ walidację lub unique decorators
public class CoffeeBuilder
{
    private HashSet<Type> _appliedDecorators = new();
    
    public CoffeeBuilder AddDecorator<T>() where T : CoffeeDecorator
    {
        if (_appliedDecorators.Contains(typeof(T)))
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} already applied");
        }
        
        _appliedDecorators.Add(typeof(T));
        // ... apply decorator
        return this;
    }
}
```

#### 5. **Decorator przechowuje mutable state**
```csharp
// ❌ BAD: Decorator ze stanem
public class CountingDecorator : CoffeeDecorator
{
    private int _callCount = 0;  // ❌ Mutable state!
    
    public override double GetCost()
    {
        _callCount++;  // Problem z concurrent access!
        return coffee.GetCost();
    }
}

// ✅ GOOD: Decorator bez stanu lub immutable
public class CountingDecorator : CoffeeDecorator
{
    private readonly ICounter _counter;  // Dependency
    
    public CountingDecorator(Coffee coffee, ICounter counter)
    {
        this.coffee = coffee;
        _counter = counter;
    }
    
    public override double GetCost()
    {
        _counter.Increment();  // External state management
        return coffee.GetCost();
    }
}
```

### 💼 Kontekst biznesowy

#### Przykład: E-commerce - konfiguracja produktów

**Bez Decorator (dziedziczenie):**
```csharp
// Potrzebujesz:
class LaptopWithRAMUpgrade { }
class LaptopWithSSDUpgrade { }
class LaptopWithRAMAndSSD { }
class LaptopWithRAMAndSSDAndGPU { }
// ... 2^N klas dla N upgrades!

// Każda zmiana ceny = modyfikacja wielu klas
```

**Z Decorator:**
```csharp
IProduct laptop = new BaseLaptop();  // $1000

// Klient dodaje upgrades dynamicznie:
laptop = new RAMUpgradeDecorator(laptop);      // +$200
laptop = new SSDUpgradeDecorator(laptop);      // +$150
laptop = new GPUUpgradeDecorator(laptop);      // +$500

Console.WriteLine(laptop.GetPrice());  // $1850

// Korzyści:
// - Zmiana ceny upgrade = zmiana w 1 klasie
// - Nowe upgrade = nowa klasa dekoratora
// - Różne kombinacje bez nowych klas
// - A/B testing: różne kombinacje dla różnych klientów
```

#### Przykład: Middleware pipeline w web apps

```csharp
// ASP.NET Core używa Decorator pattern dla middleware:
app.UseAuthentication();      // Decorator 1
app.UseAuthorization();       // Decorator 2
app.UseRateLimiting();        // Decorator 3
app.UseLogging();             // Decorator 4
app.UseCompression();         // Decorator 5

// Każdy middleware:
// 1. Dostaje request
// 2. Przetwarza (auth, logging, etc.)
// 3. Deleguje do następnego
// 4. Przetwarza response przy powrocie
```

### 🔧 Implementacje w różnych językach

#### C# - Attributes jako dekoratory
```csharp
[Authorize]  // Decorator!
[ValidateInput]  // Decorator!
[Cache(Duration = 60)]  // Decorator!
public IActionResult GetUser(int id)
{
    return Ok(_userService.GetUser(id));
}
```

#### Python - Function decorators
```python
@login_required  # Decorator
@cache(timeout=300)  # Decorator
def get_user(user_id):
    return User.get(user_id)
```

#### TypeScript - Method decorators
```typescript
class UserService {
    @Log()  // Decorator
    @Retry(3)  // Decorator
    @Timeout(5000)  // Decorator
    async getUser(id: number): Promise<User> {
        return await this.repo.findOne(id);
    }
}
```

### 📝 Podsumowanie

- **Decorator** dynamicznie dodaje funkcjonalności przez opakowanie obiektów
- **Stosuj** gdy dziedziczenie prowadzi do eksplozji klas, potrzebujesz runtime flexibility
- **Uważaj** na kolejność dekoratorów, memory overhead, trudności w debugowaniu
- **Najczęstsze błędy:** modyfikacja interfejsu, brak delegacji, circular decoration, mutable state
- **W praktyce:** middleware pipelines, I/O streams, React HOCs, attributes/annotations
