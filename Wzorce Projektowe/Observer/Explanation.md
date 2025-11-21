# Observer Pattern

## Intent
Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notified and updated automatically.

## Problem w BadExample
- Tight coupling między Subject i Observers
- Subject musi znać konkretne klasy wszystkich obserwatorów
- Niemożliwe dynamiczne dodawanie/usuwanie obserwatorów
- Naruszenie Open/Closed Principle

## Rozwiązanie w GoodExample
- Loose coupling przez interfaces
- Subject tylko wywołuje `Update()` na obserwatorach
- Dynamiczne Attach/Detach obserwatorów
- Możliwość dodania nowych obserwatorów bez zmiany Subject

## Struktura
```
IObserver<T>    ISubject<T>
    ↑               ↑
    |               |
Concrete        Concrete
Observer        Subject
```

## C# Implementation
**Classic:** IObserver/ISubject interfaces  
**Native:** C# events (`event EventHandler<T>`)

## Best Practices
✅ Używaj generics dla type safety  
✅ C# events dla prostych przypadków  
✅ Detach obserwatorów aby uniknąć memory leaks  
✅ Thread-safe notify gdy multi-threading
