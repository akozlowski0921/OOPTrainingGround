# Specyfika .NET - Zaawansowane wzorce i pułapki

Ten folder zawiera praktyczne przykłady specyficznych dla .NET zagadnień, które często są źródłem błędów w produkcyjnych aplikacjach.

## 📂 Zawartość

### 1. IEnumerable vs IQueryable (Entity Framework)
**Problem**: Pobieranie wszystkich danych z bazy do RAM i filtrowanie w pamięci aplikacji.  
**Rozwiązanie**: Wykorzystanie IQueryable do budowania zapytań SQL z filtrowaniem na poziomie bazy danych.  
**Kluczowe korzyści**: Optymalizacja wydajności, wykorzystanie indeksów DB, zmniejszenie zużycia pamięci.

### 2. Dependency Injection (ASP.NET Core)
**Problem**: Ręczne tworzenie zależności, tight coupling, brak testowalności, captive dependency.  
**Rozwiązanie**: Constructor injection, Service Lifetimes (Transient/Scoped/Singleton), IServiceScopeFactory, rozwiązywanie circular dependencies.  
**Kluczowe korzyści**: Loose coupling, łatwe testowanie, SOLID principles, prawidłowe zarządzanie lifetime'ami.

### 3. Scoped vs Singleton (Dependency Injection Lifetimes)
**Problem**: Wstrzyknięcie Scoped Service (np. DbContext) bezpośrednio do Singletona (captive dependency).  
**Rozwiązanie**: Użycie IServiceScopeFactory do tworzenia scope'ów on-demand.  
**Kluczowe korzyści**: Eliminacja wycieków pamięci, rozwiązanie problemów concurrency, prawidłowe zarządzanie lifetime'ami.

### 4. Exceptions (Obsługa wyjątków)
**Problem**: Exception swallowing, `throw ex;` tracący StackTrace, generyczne wyjątki.  
**Rozwiązanie**: Użycie `throw;` do zachowania StackTrace, specyficzne typy wyjątków, custom exceptions.  
**Kluczowe korzyści**: Pełny StackTrace dla debugowania, jasna komunikacja o typie błędu, łatwiejsza diagnostyka.

### 5. Testy jednostkowe w .NET
**Problem**: Testy z zależnościami zewnętrznymi, brak mockowania, słabe asserty, współdzielony state.  
**Rozwiązanie**: Moq dla mockowania, xUnit/NUnit/MSTest, FluentAssertions, Theory/TestCase, async testing.  
**Kluczowe korzyści**: Szybkie testy, izolacja, łatwe testowanie edge cases, czytelne asserty.

### 6. REST API w ASP.NET Core
**Problem**: Brak walidacji, nieprawidłowe status codes, brak dokumentacji, zwracanie encji DB, brak paginacji.  
**Rozwiązanie**: [ApiController], Data Annotations, Swagger/OpenAPI, DTOs, paginacja, middleware, versioning.  
**Kluczowe korzyści**: Prawidłowa architektura REST, bezpieczeństwo, dokumentacja, testowalność, performance.

### 7. IDisposable (Zarządzanie zasobami)
**Problem**: Brak wywołania Dispose() na zasobach niezarządzanych (FileStream, HttpClient, SqlConnection).  
**Rozwiązanie**: Implementacja IDisposable pattern, using statement/declaration.  
**Kluczowe korzyści**: Eliminacja wycieków pamięci, zwolnienie handle'ów systemowych, zapobieganie socket exhaustion.

### 8. Records vs Classes (C# 9+)
**Problem**: Mutable DTOs, przypadkowa modyfikacja obiektów, ręczna implementacja equality.  
**Rozwiązanie**: Użycie record dla immutability, value-based equality, with expressions.  
**Kluczowe korzyści**: Gwarancja niemutowalności, automatyczna implementacja Equals/GetHashCode, thread-safety.

### 9. Async/Await i TPL (Task Parallel Library)
**Problem**: Deadlocki, brak ConfigureAwait, async void, sekwencyjne wykonywanie równoległych operacji.  
**Rozwiązanie**: Async all the way, ConfigureAwait(false) w library code, Task.WhenAll dla równoległości.  
**Kluczowe korzyści**: Brak deadlocków, lepsza wydajność, prawidłowa obsługa asynchroniczności.

### 10. Serializacja JSON
**Problem**: Mieszanie System.Text.Json i Newtonsoft.Json, brak walidacji, wrażliwe dane w JSON.  
**Rozwiązanie**: Spójne użycie System.Text.Json, custom converters, JsonIgnore dla wrażliwych danych.  
**Kluczowe korzyści**: Lepsza wydajność, type safety, bezpieczeństwo danych.

### 11. Typy Referencyjne vs Wartościowe
**Problem**: Boxing/unboxing, mutable struct, duże struktury kopiowane przy wywołaniach.  
**Rozwiązanie**: Readonly struct, Span<T> dla zero-allocation slicing, generics zamiast object.  
**Kluczowe korzyści**: Brak boxing, zero allocations, lepsza wydajność.

### 12. CancellationToken
**Problem**: Ignorowanie tokena, brak propagacji, nieprawidłowa obsługa OperationCanceledException.  
**Rozwiązanie**: Propagacja przez cały call stack, linked tokens, cooperative cancellation.  
**Kluczowe korzyści**: Graceful cancellation, możliwość przerwania długich operacji, kontrola nad wykonaniem.

### 13. Options Pattern
**Problem**: Bezpośredni dostęp do IConfiguration, magic strings, brak walidacji.  
**Rozwiązanie**: Strongly-typed options classes, IOptions/IOptionsSnapshot/IOptionsMonitor, walidacja.  
**Kluczowe korzyści**: Type safety, łatwe testowanie, hot-reload configuration.

## 🎯 Cel

Każdy przykład demonstruje:
- ❌ **BadExample**: Typowy błąd lub antywzorzec
- ✅ **GoodExample**: Prawidłowe rozwiązanie z komentarzami
- 📝 **Explanation.md**: Szczegółowe wyjaśnienie problemu, rozwiązania i zasad

## 🚀 Jak używać

1. Zacznij od przeczytania BadExample - zrozum problem
2. Przeanalizuj GoodExample - zobacz prawidłowe rozwiązanie
3. Przeczytaj Explanation.md - zgłęb szczegóły i best practices

## 💡 Dla kogo

- **Mid → Senior developers**: Systematyzacja wiedzy o pułapkach .NET
- **Code reviewers**: Szybka referencja do typowych problemów
- **Zespoły**: Materiał do tech talks i szkoleń

## 🔗 Powiązane tematy

Te przykłady często występują razem w rzeczywistych aplikacjach:
- DbContext (IQueryable + Scoped + IDisposable)
- API DTOs (Records + Exceptions)
- Background services (Singleton + IServiceScopeFactory + IDisposable)

## ⚠️ Uwaga

Przykłady są uproszczone dla celów edukacyjnych. W produkcyjnych aplikacjach należy uwzględnić dodatkowe aspekty:
- Logowanie (np. Serilog, NLog)
- Monitoring (np. Application Insights)
- Testy jednostkowe i integracyjne
- Security (np. walidacja input, SQL injection prevention)
