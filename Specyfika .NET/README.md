# Specyfika .NET - Zaawansowane wzorce i pułapki

Ten folder zawiera praktyczne przykłady specyficznych dla .NET zagadnień, które często są źródłem błędów w produkcyjnych aplikacjach.

## 📂 Zawartość

### 1. IEnumerable vs IQueryable (Entity Framework)
**Problem**: Pobieranie wszystkich danych z bazy do RAM i filtrowanie w pamięci aplikacji.  
**Rozwiązanie**: Wykorzystanie IQueryable do budowania zapytań SQL z filtrowaniem na poziomie bazy danych.  
**Kluczowe korzyści**: Optymalizacja wydajności, wykorzystanie indeksów DB, zmniejszenie zużycia pamięci.

### 2. Scoped vs Singleton (Dependency Injection)
**Problem**: Wstrzyknięcie Scoped Service (np. DbContext) bezpośrednio do Singletona (captive dependency).  
**Rozwiązanie**: Użycie IServiceScopeFactory do tworzenia scope'ów on-demand.  
**Kluczowe korzyści**: Eliminacja wycieków pamięci, rozwiązanie problemów concurrency, prawidłowe zarządzanie lifetime'ami.

### 3. Exceptions (Obsługa wyjątków)
**Problem**: Exception swallowing, `throw ex;` tracący StackTrace, generyczne wyjątki.  
**Rozwiązanie**: Użycie `throw;` do zachowania StackTrace, specyficzne typy wyjątków, custom exceptions.  
**Kluczowe korzyści**: Pełny StackTrace dla debugowania, jasna komunikacja o typie błędu, łatwiejsza diagnostyka.

### 4. IDisposable (Zarządzanie zasobami)
**Problem**: Brak wywołania Dispose() na zasobach niezarządzanych (FileStream, HttpClient, SqlConnection).  
**Rozwiązanie**: Implementacja IDisposable pattern, using statement/declaration.  
**Kluczowe korzyści**: Eliminacja wycieków pamięci, zwolnienie handle'ów systemowych, zapobieganie socket exhaustion.

### 5. Records vs Classes (C# 9+)
**Problem**: Mutable DTOs, przypadkowa modyfikacja obiektów, ręczna implementacja equality.  
**Rozwiązanie**: Użycie record dla immutability, value-based equality, with expressions.  
**Kluczowe korzyści**: Gwarancja niemutowalności, automatyczna implementacja Equals/GetHashCode, thread-safety.

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
