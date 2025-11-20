# Records vs Classes - Immutability i Value-Based Equality

## Problem w BadExample

### 1. Mutability - Niezamierzone modyfikacje
```csharp
public void Process(UserDto user)
{
    user.FirstName = user.FirstName.ToUpper(); // Modyfikuje oryginał!
}
```
- Metoda zmienia obiekt przekazany przez referencję
- Wywołujący nie spodziewa się modyfikacji
- Trudne do wyśledzenia błędy w złożonych aplikacjach

### 2. Reference Equality
```csharp
var user1 = new UserDto { Id = 1, FirstName = "John" };
var user2 = new UserDto { Id = 1, FirstName = "John" };
user1 == user2; // false - porównuje referencje, nie wartości
```

### 3. Brak kopiowania
Trzeba ręcznie kopiować wszystkie pola - łatwo o błąd.

## Rozwiązanie w GoodExample

### 1. Record (C# 9+)

```csharp
public record UserDto(int Id, string FirstName, string LastName);
```

Automatycznie dostajemy:
- **Immutability**: Wszystkie właściwości są `init-only`
- **Value-based equality**: Porównanie po wartościach, nie referencjach
- **with expression**: Kopiowanie z modyfikacją
- **Deconstruction**: Rozpakowywanie do zmiennych
- **ToString()**: Czytelna reprezentacja

### 2. init accessor

```csharp
public decimal Price { get; init; } // Można ustawić tylko przy tworzeniu
```

```csharp
var order = new OrderDto { Price = 100 };
order.Price = 200; // ❌ Błąd kompilacji
```

### 3. with expression

```csharp
var original = new UserDto(1, "John", "Doe", "john@example.com");
var modified = original with { Email = "newemail@example.com" };
// original pozostaje niezmieniony
```

## Kiedy używać Record?

### Użyj Record dla:
- **DTO (Data Transfer Objects)**: API request/response
- **Value Objects**: Email, Money, Address
- **Immutable state**: Redux/state management
- **Messages/Events**: CQRS, Event Sourcing
- **Configuration objects**: Ustawienia aplikacji

### Użyj Class dla:
- **Encje z logiką**: Domain models z behavior
- **Mutable state**: Gdy rzeczywiście potrzebujesz mutacji
- **Dziedziczenie z abstrakcji**: Gdy dziedziczysz z class
- **Performance-critical**: (choć różnica jest minimalna)

## Value-Based Equality - Przykłady

### Record (automatycznie)
```csharp
var user1 = new UserDto(1, "John", "Doe", "john@example.com");
var user2 = new UserDto(1, "John", "Doe", "john@example.com");

user1 == user2; // true
user1.Equals(user2); // true
user1.GetHashCode() == user2.GetHashCode(); // true
```

### Class (ręcznie)
```csharp
public class UserDto
{
    public override bool Equals(object obj)
    {
        if (obj is not UserDto other) return false;
        return Id == other.Id && 
               FirstName == other.FirstName && 
               LastName == other.LastName &&
               Email == other.Email;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, FirstName, LastName, Email);
    }
}
```

## Record Syntax - Warianty

### 1. Positional record (najprostszy)
```csharp
public record UserDto(int Id, string Name);
```

### 2. Record z właściwościami
```csharp
public record UserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}
```

### 3. Hybrid
```csharp
public record UserDto(int Id, string Name)
{
    public string FullName => $"User: {Name}";
}
```

## with Expression - Zaawansowane użycie

```csharp
var user = new UserDto(1, "John", "Doe", "john@example.com");

// Modyfikacja jednego pola
var updated = user with { Email = "new@example.com" };

// Modyfikacja wielu pól
var admin = user with 
{ 
    FirstName = "Admin",
    Email = "admin@example.com" 
};

// Kopiowanie bez zmian
var copy = user with { };
```

## Deconstruction

```csharp
var user = new UserDto(1, "John", "Doe", "john@example.com");

// Deconstruct do zmiennych
var (id, firstName, lastName, email) = user;

// Ignorowanie niektórych wartości
var (_, firstName, _, _) = user;
```

## ToString() - Automatycznie generowany

```csharp
var user = new UserDto(1, "John", "Doe", "john@example.com");
Console.WriteLine(user);
// Output: UserDto { Id = 1, FirstName = John, LastName = Doe, Email = john@example.com }
```

## Immutability - Korzyści

1. **Thread-safety**: Immutable objects są bezpieczne wątkowo
2. **Predictability**: Obiekt nie zmienia się - łatwiejsze rozumowanie o kodzie
3. **Hashable**: Bezpieczne używanie jako klucze w Dictionary/HashSet
4. **Functional programming**: Pasuje do stylu funkcyjnego
5. **Debugging**: Łatwiejsze debugowanie - stan nie zmienia się niespodziewanie

## Record vs Struct vs Class

| Aspekt | Record (class) | Struct | Class |
|--------|---------------|--------|-------|
| Typ | Reference type | Value type | Reference type |
| Equality | Value-based | Value-based | Reference-based |
| Mutability | Immutable (default) | Może być mutable | Mutable (default) |
| Heap/Stack | Heap | Stack (zazwyczaj) | Heap |
| with | Tak | Tak (C# 10+) | Nie |

## Migracja Class → Record

```csharp
// Przed
public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Po
public record UserDto(int Id, string Name);

// Lub, jeśli chcesz zachować nazwany konstruktor
public record UserDto
{
    public int Id { get; init; }
    public string Name { get; init; }
}
```

## Record z walidacją

```csharp
public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        if (!value.Contains("@"))
            throw new ArgumentException("Invalid email");
        
        Value = value;
    }
}
```

## Performance

Records mają minimalny overhead w porównaniu do classes:
- Value-based equality jest szybko zoptymalizowana przez kompilator
- `with` expression jest efektywny - kopiuje tylko referencje do immutable fields

## Best Practice

**Dla DTOs i Value Objects domyślnie używaj `record`. Używaj `class` tylko gdy masz konkretny powód (mutability, dziedziczenie z istniejącej class).**
