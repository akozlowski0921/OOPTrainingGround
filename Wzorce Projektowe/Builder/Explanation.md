# Builder Pattern

## 📌 Problem w Bad Example
Konstruktor z wieloma parametrami (tzw. **Telescoping Constructor Antipattern**) prowadzi do:
- **Nieczytelności** – trudno określić co oznacza każdy argument
- **Błędów w kolejności** – łatwo pomylić parametry tego samego typu (np. string, string, string...)
- **Braku elastyczności** – trzeba przekazać wszystkie parametry, nawet gdy nie są potrzebne
- **Niemożności walidacji** – obiekt tworzony jest od razu, bez możliwości sprawdzenia poprawności

## ✅ Rozwiązanie: Builder Pattern
Builder to wzorzec kreacyjny, który **oddziela konstrukcję złożonego obiektu od jego reprezentacji**, pozwalając na krokowe budowanie obiektu z walidacją.

### Kluczowe elementy:
1. **Klasa główna** – zawiera wszystkie pola, konstruktor prywatny
2. **Klasa Builder** – zagnieżdżona klasa z metodami ustawiającymi poszczególne pola
3. **Fluent interface** – każda metoda zwraca `this`, umożliwiając łańcuchowanie wywołań
4. **Metoda Build()** – waliduje dane i zwraca gotowy obiekt

## 🎯 Korzyści

### 1. Czytelność
```csharp
// Jasne i zrozumiałe wywołanie:
var user = new UserProfile.Builder()
    .WithName("Jan", "Kowalski")
    .WithEmail("jan@example.com")
    .Build();
```

### 2. Bezpieczeństwo
```csharp
// Walidacja w Build() zapobiega tworzeniu niepoprawnych obiektów:
public UserProfile Build()
{
    if (string.IsNullOrEmpty(_profile.Email))
        throw new InvalidOperationException("Email is required");
    return _profile;
}
```

### 3. Elastyczność
```csharp
// Podajemy tylko potrzebne dane:
var minimalUser = new UserProfile.Builder()
    .WithName("Anna", "Nowak")
    .WithEmail("anna@example.com")
    .Build();
```

### 4. Immutability
Settery w klasie głównej są `private`, więc obiekt nie może być modyfikowany po utworzeniu.

## 🔄 Kiedy stosować?
- Klasa ma **więcej niż 4-5 parametrów** w konstruktorze
- Wiele parametrów jest **opcjonalnych**
- Chcesz **walidować** dane przed utworzeniem obiektu
- Potrzebujesz **różnych reprezentacji** tego samego obiektu
- Konstrukcja obiektu wymaga **wielu kroków**

## 🏗️ Warianty
1. **Simple Builder** – jak w przykładzie powyżej
2. **Director** – dodatkowa klasa orkiestrująca proces budowania
3. **Faceted Builder** – multiple builderów dla różnych aspektów obiektu

## ⚠️ Uwagi
- Zwiększa ilość kodu (ale poprawia jego jakość!)
- W C# można użyć **object initializers** dla prostszych przypadków:
  ```csharp
  var user = new UserProfile 
  { 
      FirstName = "Jan", 
      LastName = "Kowalski" 
  };
  ```
  Jednak Builder oferuje większą kontrolę i walidację.
