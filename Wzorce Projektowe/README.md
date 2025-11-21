# Wzorce Projektowe - GOF i Enterprise Patterns

Ten folder zawiera praktyczne przykłady wzorców projektowych (Design Patterns) z Gang of Four oraz wzorce Enterprise.

## 📂 Zawartość

### 1. Singleton
**Problem**: Potrzeba pojedynczej instancji klasy w całej aplikacji.  
**Rozwiązanie**: Singleton pattern zapewnia globalny punkt dostępu do jednej instancji.  
**Kluczowe korzyści**: Kontrola dostępu do zasobu, lazy initialization, thread-safety.

### 2. Builder
**Problem**: Tworzenie złożonych obiektów z wieloma parametrami, gdzie wiele z nich jest opcjonalnych.  
**Rozwiązanie**: Builder pattern - fluent API dla konstruowania obiektów krok po kroku.  
**Kluczowe korzyści**: Czytelny kod, immutability, validacja podczas budowania.

### 3. Adapter
**Problem**: Niekompatybilne interfejsy między klasami, legacy code integration.  
**Rozwiązanie**: Adapter konwertuje interfejs jednej klasy na oczekiwany przez klienta.  
**Kluczowe korzyści**: Reusability, integracja z legacy/third-party code, loose coupling.

### 4. Strategy
**Problem**: Wiele algorytmów do wyboru w runtime, if/switch statements.  
**Rozwiązanie**: Strategy pattern enkapsuluje algorytmy w osobnych klasach.  
**Kluczowe korzyści**: Open/Closed Principle, łatwe dodawanie nowych strategii, testowanie.

### 5. Decorator
**Problem**: Dodawanie funkcjonalności do obiektów dynamicznie bez modyfikacji klasy.  
**Rozwiązanie**: Decorator opakowuje obiekt, dodając nowe zachowanie.  
**Kluczowe korzyści**: Single Responsibility, flexible alternative to subclassing.

### 6. Observer
**Problem**: One-to-many dependency, tight coupling między obiektami.  
**Rozwiązanie**: Observer pattern - obserwatorzy subskrybują zmiany w subject.  
**Kluczowe korzyści**: Loose coupling, event-driven architecture, reactive programming.

### 7. Factory / Abstract Factory
**Problem**: Tworzenie obiektów z new keyword tworzy tight coupling.  
**Rozwiązanie**: Factory enkapsuluje logikę tworzenia obiektów, Abstract Factory dla rodzin obiektów.  
**Kluczowe korzyści**: Loose coupling, łatwe testowanie (mocking), DI integration.

### 8. Command
**Problem**: Brak możliwości undo/redo, brak historii operacji.  
**Rozwiązanie**: Command pattern enkapsuluje request jako obiekt.  
**Kluczowe korzyści**: Undo/redo, command queue, macro commands, transaction management.

### 9. Mediator
**Problem**: Komponenty znają się nawzajem, skomplikowana sieć zależności.  
**Rozwiązanie**: Mediator enkapsuluje interakcje między komponentami.  
**Kluczowe korzyści**: Loose coupling, reusable components, CQRS pattern (MediatR).

### 10. Facade
**Problem**: Złożone subsystemy, client musi znać szczegóły implementacji.  
**Rozwiązanie**: Facade dostarcza uproszczony interfejs do subsystemów.  
**Kluczowe korzyści**: Simplified interface, loose coupling, cache + external services integration.

## 🎯 Cel

Każdy przykład demonstruje:
- ❌ **BadExample**: Kod bez wzorca - tight coupling, trudny w utrzymaniu
- ✅ **GoodExample**: Implementacja wzorca - loose coupling, SOLID principles
- 📝 **Explanation.md**: Wyjaśnienie wzorca, use cases, korzyści

## 🚀 Jak używać

1. Przeczytaj BadExample - zrozum problem
2. Przeanalizuj GoodExample - zobacz implementację wzorca
3. Przeczytaj Explanation.md - zgłęb szczegóły i best practices

## 💡 Dla kogo

- **Mid → Senior developers**: Systematyzacja wiedzy o wzorcach
- **Architektów**: Przykłady praktycznego zastosowania
- **Code reviewers**: Referencja do rozwiązań architektonicznych
- **Zespoły**: Materiał do tech talks i szkoleń

## 📖 Gang of Four

**Creational Patterns** (tworzenie obiektów):
- Singleton, Builder, Factory, Abstract Factory

**Structural Patterns** (struktura klas):
- Adapter, Decorator, Facade

**Behavioral Patterns** (interakcje między obiektami):
- Strategy, Observer, Command, Mediator

### 11. CQRS (Command Query Responsibility Segregation)
**Problem:** Jeden model dla zapisu i odczytu, brak optymalizacji.  
**Rozwiązanie:** Rozdzielenie modelu zapisu (Commands) od modelu odczytu (Queries).  
**Kluczowe korzyści:** Independent scaling, performance optimization, different models for different purposes, caching strategies.

### 12. Event Sourcing
**Problem:** Brak historii zmian, niemożność odtworzenia stanu w przeszłości.  
**Rozwiązanie:** Zapisywanie wszystkich zmian jako sekwencji eventów zamiast current state.  
**Kluczowe korzyści:** Complete audit trail, temporal queries, event replay, compliance, debugging.

## 🔗 Enterprise Patterns

- **Repository Pattern** (dostęp do danych)
- **Unit of Work** (transakcje)
- **CQRS** (Command Query Responsibility Segregation) - Rozdzielenie read/write
- **Event Sourcing** - Historia zmian przez eventy
- **Mediator** (MediatR library w ASP.NET Core) - Centralizacja komunikacji

## ⚠️ Uwaga

- Nie nadużywaj wzorców - używaj gdy rozwiązują realny problem
- YAGNI (You Aren't Gonna Need It) - nie over-engineer
- Wzorce to narzędzia, nie cele same w sobie
- Priorytet: czytelny, prosty kod

## 🌟 Best Practices

✅ Używaj wzorców gdy rozwiązują problem  
✅ Prefer composition over inheritance  
✅ SOLID principles  
✅ Dependency Injection  
✅ Unit testowanie z mockami  
✅ Dokumentacja - dlaczego użyto wzorca  

❌ Nie używaj wzorca "dla wzorca"  
❌ Nie over-engineer prostych przypadków  
❌ Nie ignoruj YAGNI  
