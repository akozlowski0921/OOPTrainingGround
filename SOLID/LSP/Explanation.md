# LSP - Liskov Substitution Principle (Zasada Podstawienia Liskov)

## 🔴 Problem w BadExample

Klasyczna pułapka dziedziczenia: **"Pingwin jest ptakiem, więc dziedziczy po Bird"**

```csharp
public class Bird
{
    public virtual void Fly() { ... }
}

public class Penguin : Bird
{
    public override void Fly()
    {
        throw new NotImplementedException("Pingwiny nie potrafią latać!");
    }
}
```

### Dlaczego to narusza LSP?

LSP mówi:
> "Obiekty klasy potomnej powinny móc zastąpić obiekty klasy bazowej bez zmiany poprawności programu"

W praktyce:
- Jeśli kod oczekuje `Bird` i wywołuje `Fly()`, to powinien działać dla **każdego** ptaka
- `Penguin` łamie ten kontrakt rzucając wyjątek
- Musisz wiedzieć, czy masz `Penguin`, żeby uniknąć wyjątku
- To **NIE JEST** prawdziwy polimorfizm

### Konsekwencje:

1. **Kod defensywny**: Wszędzie musisz sprawdzać typ:
   ```csharp
   if (bird is Penguin)
       // nie wywołuj Fly()
   else
       bird.Fly();
   ```

2. **Niespodziewane wyjątki**: Kod kompiluje się, ale pada w runtime

3. **Niemożliwość traktowania wszystkich Birds jednakowo**

4. **Złamana abstrakcja**: Musisz znać szczegóły implementacji

## ✅ Rozwiązanie w GoodExample

### Podział na interfejsy według możliwości:

```csharp
public abstract class Bird { ... }  // Wspólne cechy
public interface IFlyable { void Fly(); }
public interface ISwimmable { void Swim(); }
```

### Implementacje:

- `Sparrow : Bird, IFlyable` ← może latać
- `Eagle : Bird, IFlyable` ← może latać
- `Penguin : Bird, ISwimmable` ← może pływać (nie latać!)
- `Duck : Bird, IFlyable, ISwimmable` ← może obydwa!

### Korzyści:

1. **Brak wyjątków**: Każda klasa implementuje tylko to, co potrafi
2. **Prawdziwy polimorfizm**: Możesz bezpiecznie używać `IFlyable` lub `ISwimmable`
3. **Zgodność z LSP**: Każda implementacja spełnia kontrakt interfejsu
4. **Kompozycja nad dziedziczeniem**: Używamy interfejsów zamiast wymuszać niepasujące dziedziczenie

## 💼 Kontekst Biznesowy

### Przykład z prawdziwego świata: System e-commerce

**BadExample** (❌):
```csharp
public class Product
{
    public virtual decimal GetShippingCost() { ... }
}

public class DigitalProduct : Product
{
    public override decimal GetShippingCost()
    {
        throw new InvalidOperationException("Produkty cyfrowe nie mają kosztów wysyłki!");
    }
}
```

Efekt: Koszyk obliczający łączny koszt wysyłki wywali się na produktach cyfrowych.

**GoodExample** (✅):
```csharp
public abstract class Product { ... }
public interface IShippable
{
    decimal GetShippingCost();
}

public class PhysicalProduct : Product, IShippable { ... }
public class DigitalProduct : Product { ... } // brak IShippable
```

Efekt: Koszyk prosi o `IShippable` - tylko fizyczne produkty uczestniczą w obliczeniach.

## 🎯 Kiedy stosować LSP?

**Zawsze** gdy używasz dziedziczenia! Zadaj sobie pytania:

1. Czy klasa potomna może **naprawdę** zastąpić klasę bazową wszędzie?
2. Czy muszę znać konkretny typ, żeby bezpiecznie użyć metody?
3. Czy klasa potomna rzuca wyjątki tam, gdzie bazowa nie rzuca?
4. Czy klasa potomna ma "puste" implementacje metod bazowych?

Jeśli odpowiedź na 2-4 to "TAK" → naruszenie LSP.

## 📏 Czerwone flagi naruszenia LSP:

- ❌ `throw new NotImplementedException()`
- ❌ Puste implementacje wirtualnych metod
- ❌ Sprawdzanie typu przed wywołaniem metody (`if (x is Penguin)`)
- ❌ Dokumentacja typu "Uwaga: nie wywołuj X na klasie Y"
- ❌ Metody bazowe, które nie mają sensu dla wszystkich potomków

## 🔧 Narzędzia zgodne z LSP:

- **Interface Segregation (ISP)**: Małe, specyficzne interfejsy
- **Composition over Inheritance**: Preferuj kompozycję
- **Strategy Pattern**: Wymienne zachowania przez interfejsy
- **Null Object Pattern**: Zamiast null, zwróć bezpieczny obiekt

## 💡 Złota zasada LSP

> "Jeśli S jest podtypem T, to obiekty typu T mogą być zastąpione obiektami typu S bez zmiany właściwości programu"

W praktyce:
- Nie osłabiaj warunków wstępnych (prekondycji)
- Nie wzmacniaj warunków końcowych (postkondycji)
- Nie rzucaj nowych typów wyjątków
- Nie usuń funkcjonalności klasy bazowej

## 🧪 Test LSP:

```csharp
// Jeśli ten kod działa:
void ProcessBird(Bird bird)
{
    bird.Fly();
}
ProcessBird(new Eagle());

// To ten też MUSI działać:
ProcessBird(new Penguin());  // ❌ BOOM! NotImplementedException
```

Jeśli drugi nie działa → naruszenie LSP.
