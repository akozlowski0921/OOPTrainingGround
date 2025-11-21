# Factory & Abstract Factory

## Factory Method Pattern
Defines an interface for creating objects, but lets subclasses decide which class to instantiate.

## Abstract Factory Pattern
Provides an interface for creating families of related objects without specifying their concrete classes.

## Problem
- `new` keyword tworzy tight coupling
- Trudne testowanie
- Naruszenie Open/Closed Principle przy dodawaniu nowych typów

## Rozwiązanie
- Factory enkapsuluje tworzenie obiektów
- Client zależy od abstrakcji, nie konkretnych klas
- Łatwe dodawanie nowych typów

## DI Integration
```csharp
services.AddTransient<IPaymentFactory, PaymentFactory>();
services.AddTransient<Func<PaymentType, IPayment>>(provider => ...);
```

## Kiedy używać
✅ Wiele podobnych klas do tworzenia  
✅ Logika tworzenia jest złożona  
✅ Potrzebujesz testować z mockami  
✅ Families of related objects (Abstract Factory)
