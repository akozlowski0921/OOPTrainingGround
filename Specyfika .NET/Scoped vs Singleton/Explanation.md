# Scoped vs Singleton - Zarządzanie czasem życia w DI

## Problem w BadExample

- **Captive Dependency**: Singleton trzyma referencję do Scoped service (DbContext)
- **Wycieki pamięci**: DbContext nigdy nie jest zwalniany, bo Singleton żyje przez cały czas działania aplikacji
- **Concurrency issues**: DbContext nie jest thread-safe - współbieżne zapytania mogą prowadzić do błędów
- **Stale dane**: Cache w DbContext nie jest odświeżany między requestami

## Rozwiązanie w GoodExample

- **IServiceScopeFactory**: Pozwala Singletonowi tworzyć nowe scope'y on-demand
- **Izolacja**: Każda operacja dostaje świeży DbContext
- **Automatyczne disposal**: `using` zapewnia zwolnienie zasobów
- **Thread-safety**: Każdy wątek/request ma własną instancję DbContext

## Lifetime'y w .NET DI

| Lifetime | Czas życia | Użycie |
|----------|-----------|---------|
| **Transient** | Nowa instancja przy każdym wstrzyknięciu | Lekkie, bezstanowe serwisy |
| **Scoped** | Jedna instancja na scope (request) | DbContext, Unit of Work |
| **Singleton** | Jedna instancja przez cały czas działania app | Cache, konfiguracja, logger |

## Captive Dependency - Antywzorzec

```
Singleton → Scoped    ❌ BŁĄD
Singleton → Transient ❌ BŁĄD (może być problem)
Scoped → Transient    ✅ OK
Scoped → Singleton    ✅ OK
```

## Dlaczego DbContext powinien być Scoped?

1. **Tracking**: EF śledzi zmiany w encjach - nie może tego robić przez wiele requestów
2. **Unit of Work**: DbContext reprezentuje jednostkę pracy - jeden request = jedna transakcja
3. **Connection pooling**: Połączenia DB są zwalniane po zakończeniu scope'a
4. **Thread-safety**: Każdy request/wątek dostaje własną instancję

## Alternatywne rozwiązania

### Opcja 1: Zmień Singleton na Scoped
```csharp
services.AddScoped<OrderProcessor>(); // Jeśli to możliwe
```

### Opcja 2: IServiceScopeFactory (jak w GoodExample)
```csharp
services.AddSingleton<OrderProcessor>(); // Używa IServiceScopeFactory
```

### Opcja 3: Factory Pattern
```csharp
services.AddSingleton<IDbContextFactory<AppDbContext>>();
```

## Symptomy problemu

- `InvalidOperationException`: "Cannot access a disposed object"
- Wycieki pamięci widoczne w profilerze
- Błędy concurrency w Entity Framework
- Stare dane zwracane z cache DbContext

## Zasada

**Nigdy nie wstrzykuj Scoped lub Transient serwisu bezpośrednio do Singletona.** Zamiast tego użyj `IServiceScopeFactory` lub `IServiceProvider` do tworzenia scope'ów w razie potrzeby.
