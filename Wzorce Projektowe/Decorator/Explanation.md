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
