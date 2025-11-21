# Interfejsy markerowe i atrybuty customowe w C# .NET

## Problem w BadExample

### BadExample.cs - Zły ICloneable
- **Shallow copy**: `MemberwiseClone()` kopiuje tylko referencje
- **Współdzielenie obiektów**: Zmiany w klonie wpływają na oryginał
- **Pusty interfejs markerowy**: IEntity bez kontraktu jest bezużyteczny
- **Brak typu**: ICloneable zwraca `object`, nie typ generyczny

### BadExample2.cs - Walidacja bez atrybutów
- **Duplikacja kodu**: Każda klasa ma własną metodę `Validate()`
- **Brak reużywalności**: Reguły walidacji nie są współdzielone
- **Łatwo zapomnieć**: Trzeba pamiętać o wywołaniu `Validate()`
- **Trudne testowanie**: Nie można testować reguł osobno

### BadExample3.cs - Ignorowanie atrybutów
- **Stracone metadane**: Atrybuty są zdefiniowane ale nie używane
- **Brak automatyzacji**: Nie ma kodu odczytującego atrybuty
- **Marnowanie potencjału**: Informacje strukturalne są tracone
- **Ręczne zarządzanie**: Wszystko trzeba robić manualnie

## Rozwiązanie w GoodExample

### GoodExample.cs - Prawidłowy deep clone
- **IDeepCloneable<T>**: Generyczny interfejs z type safety
- **Deep copy**: Rekurencyjne kopiowanie wszystkich referencji
- **Niezależne obiekty**: Zmiany w klonie nie wpływają na oryginał
- **IEntity<TKey>**: Interfejs z kontraktem (Id, IsTransient)

### GoodExample2.cs - Custom validation attributes
- **ValidationAttribute**: Reużywalne atrybuty walidacji
- **Deklaratywne**: Walidacja widoczna przy property
- **AgeRange, StrongPassword**: Custom atrybuty dla specyficznych reguł
- **Validator.TryValidateObject**: Automatyczna walidacja

### GoodExample3.cs - Reflection i atrybuty
- **GetCustomAttribute<T>**: Odczyt atrybutów przez reflection
- **SQL Generation**: Automatyczne generowanie DDL z atrybutów
- **Documentation**: Generowanie docs z Description attributes
- **Runtime mapping**: Mapowanie obiektów na kolumny DB

## Interfejsy markerowe

### Zły marker interface (bez kontraktu)
```csharp
// ❌ Bezużyteczny
public interface IEntity { }

// Co to znaczy być "entity"? Nie wiadomo!
```

### Dobry marker interface (z kontraktem)
```csharp
// ✅ Użyteczny
public interface IEntity<TKey>
{
    TKey Id { get; set; }
    bool IsTransient();
}

// Wyraźny kontrakt - każdy entity ma ID
```

## ICloneable - problem i rozwiązanie

### Problem z ICloneable
```csharp
public interface ICloneable
{
    object Clone(); // Zwraca object - brak type safety
}

// Shallow copy przez MemberwiseClone
public object Clone() => this.MemberwiseClone(); // ❌
```

### Lepsze rozwiązanie
```csharp
public interface IDeepCloneable<T>
{
    T DeepClone(); // Zwraca T - type safe
}

// Deep copy
public T DeepClone() 
{
    return new T 
    {
        Property1 = this.Property1,
        Reference = this.Reference?.DeepClone() // Rekurencyjnie
    };
}
```

## Custom Validation Attributes

### Podstawowa struktura
```csharp
[AttributeUsage(AttributeTargets.Property)]
public class MyValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(
        object value, 
        ValidationContext validationContext)
    {
        // Logika walidacji
        if (/* valid */)
            return ValidationResult.Success;
        
        return new ValidationResult("Error message");
    }
}
```

### Użycie
```csharp
public class Model
{
    [MyValidation]
    public string Property { get; set; }
}

// Automatyczna walidacja
var results = new List<ValidationResult>();
var context = new ValidationContext(model);
bool isValid = Validator.TryValidateObject(
    model, context, results, validateAllProperties: true);
```

## Odczyt atrybutów przez Reflection

### GetCustomAttribute
```csharp
// Z klasy
var attr = typeof(MyClass).GetCustomAttribute<TableAttribute>();

// Z property
var prop = typeof(MyClass).GetProperty("Name");
var colAttr = prop.GetCustomAttribute<ColumnAttribute>();

// Wszystkie atrybuty danego typu
var attrs = prop.GetCustomAttributes<ValidationAttribute>();
```

### AttributeUsage
```csharp
[AttributeUsage(
    AttributeTargets.Property,      // Gdzie można użyć
    AllowMultiple = false,           // Czy wiele na raz
    Inherited = true)]               // Czy dziedziczone
public class MyAttribute : Attribute { }
```

## Popularne atrybuty w .NET

### Data Annotations
- `[Required]` - pole wymagane
- `[StringLength]` - długość stringa
- `[Range]` - zakres wartości
- `[RegularExpression]` - regex pattern
- `[EmailAddress]` - walidacja email

### Entity Framework
- `[Table]` - nazwa tabeli
- `[Column]` - nazwa kolumny
- `[Key]` - klucz główny
- `[ForeignKey]` - klucz obcy
- `[Index]` - indeks

### ASP.NET
- `[Route]` - routing
- `[HttpGet]`, `[HttpPost]` - HTTP methods
- `[Authorize]` - autoryzacja
- `[ValidateAntiForgeryToken]` - CSRF protection

## Best Practices

### Interfejsy markerowe
1. **Dodaj kontrakt**: Interfejs powinien definiować metody/properties
2. **Używaj generics**: Dla type safety
3. **Extension methods**: Dodaj funkcjonalność przez extensions
4. **Dokumentuj**: Wyjaśnij co znaczy implementować ten interfejs

### Custom attributes
1. **Dziedzicz z Attribute**: Zawsze dziedzicz z odpowiedniej klasy bazowej
2. **AttributeUsage**: Zawsze definiuj gdzie można użyć
3. **Waliduj w konstruktorze**: Sprawdź parametry atrybutu
4. **Czytelne error messages**: Dobre komunikaty błędów
5. **Testuj osobno**: Unit testy dla każdego atrybutu

### Reflection i atrybuty
1. **Cache results**: Reflection jest kosztowny
2. **Sprawdzaj null**: Atrybut może nie istnieć
3. **Użyj GetCustomAttribute<T>**: Zamiast GetCustomAttributes
4. **Consider Source Generators**: Dla compile-time generation

## Performance considerations

### Reflection cost
```csharp
// ❌ Wolne - reflection przy każdym wywołaniu
public void Process(object obj)
{
    var attr = obj.GetType().GetCustomAttribute<MyAttribute>();
    // ...
}

// ✅ Szybkie - cache results
private static readonly ConcurrentDictionary<Type, MyAttribute> _cache = new();

public void Process(object obj)
{
    var attr = _cache.GetOrAdd(obj.GetType(), 
        t => t.GetCustomAttribute<MyAttribute>());
    // ...
}
```

## Przykłady zastosowań

### ORM (Entity Framework)
- Mapowanie klas na tabele
- Definicje kolumn i relacji
- Indeksy i constrainty

### Validation frameworks
- FluentValidation
- Data Annotations
- Custom business rules

### Serialization
- JSON.NET attributes
- XML serialization attributes
- Custom formatters

### Dependency Injection
- Service lifetime attributes
- Configuration binding
- Options pattern

## Alternatywy

### Zamiast marker interfaces
- Abstract base classes z implementacją
- Extension methods na interfejsach z metodami
- Composition over inheritance

### Zamiast reflection na atrybutach
- Source Generators (compile-time)
- T4 Templates
- Roslyn analyzers
