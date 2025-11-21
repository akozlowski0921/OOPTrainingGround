# Expression Trees i LINQ providers w C# .NET

## Problem w BadExample

### BadExample.cs - Ręczne budowanie zapytań
- **SQL Injection**: Konkatenacja stringów bez parametryzacji
- **Brak type safety**: Literówki w nazwach kolumn nie są wykrywane
- **Trudne w utrzymaniu**: Każda zmiana wymaga modyfikacji stringów
- **Brak walidacji**: Nie sprawdza poprawności składni

### BadExample2.cs - Hardcoded warunki LINQ
- **Duplikacja kodu**: Osobna metoda dla każdej kombinacji filtrów
- **Eksplozja kombinatoryczna**: Dla n filtrów potrzeba 2^n metod
- **Brak elastyczności**: Nie można łączyć filtrów w runtime
- **Trudne testowanie**: Każda kombinacja wymaga osobnego testu

### BadExample3.cs - Zły LINQ provider
- **Brak implementacji**: Provider nie tłumaczy expression trees
- **Utrata danych**: GetEnumerator zwraca pustą kolekcję
- **Brak optymalizacji**: Nie tłumaczy na natywne zapytania
- **Nie działa**: Całkowicie nieużyteczny provider

## Rozwiązanie w GoodExample

### GoodExample.cs - Analiza expression trees
- **ExpressionVisitor**: Wzorzec do traversowania drzewa wyrażeń
- **Type safety**: Wyrażenia lambda są sprawdzane w compile-time
- **Pełna analiza**: Rozpoznaje Binary, Member, Constant, Method Call expressions
- **Ekstrakcja metadanych**: Pobiera nazwy properties i wartości stałych

### GoodExample2.cs - Dynamiczne LINQ
- **Expression.Parameter**: Dynamiczne tworzenie parametrów lambda
- **Expression.Property**: Dynamiczny dostęp do properties
- **CombineWithAnd/Or**: Łączenie wielu wyrażeń w jedno
- **ParameterReplacer**: ExpressionVisitor do zamiany parametrów

### GoodExample3.cs - Custom LINQ provider
- **QueryTranslator**: Tłumaczy expression tree na SQL-like syntax
- **IQueryProvider**: Prawidłowa implementacja interfejsu
- **ExecuteEnumerable**: Faktyczne wykonanie zapytania
- **ExpressionVisitor pattern**: Do traversowania i translacji

## Kluczowe koncepty

### Expression Tree
```csharp
// Lambda expression
Func<int, bool> func = x => x > 5;

// Expression tree
Expression<Func<int, bool>> expr = x => x > 5;

// Można analizować strukturę:
// expr.Body -> BinaryExpression (>)
//   Left -> ParameterExpression (x)
//   Right -> ConstantExpression (5)
```

### Budowanie dynamicznych wyrażeń
```csharp
var parameter = Expression.Parameter(typeof(Product), "p");
var property = Expression.Property(parameter, "Price");
var constant = Expression.Constant(100m);
var comparison = Expression.GreaterThan(property, constant);
var lambda = Expression.Lambda<Func<Product, bool>>(comparison, parameter);

// Rezultat: p => p.Price > 100
```

## Porównanie

| Aspekt | Bad Example | Good Example |
|--------|-------------|--------------|
| Type Safety | Brak - string concat | Pełna - compile-time check |
| Performance | Powolne parsing | Szybkie - skompilowane |
| Maintainability | Trudne - stringi | Łatwe - kod |
| Testability | Trudne | Łatwe - expressions |
| Security | SQL Injection risk | Bezpieczne - parametryzacja |

## Kiedy używać Expression Trees

✅ **Dobre zastosowania:**
- ORM (Entity Framework) - translacja LINQ na SQL
- Dynamiczne zapytania w runtime
- Validation frameworks (FluentValidation)
- Mapping libraries (AutoMapper)
- Dynamic filtering APIs

❌ **Unikaj gdy:**
- Prosty kod wystarczy
- Performance jest super-krytyczna (expression compilation ma koszt)
- Logika jest statyczna

## LINQ Provider - jak działa

1. **Query Creation**: LINQ query tworzy expression tree
2. **Translation**: Provider tłumaczy expressions na natywny język (SQL, etc.)
3. **Execution**: Provider wykonuje natywne zapytanie
4. **Materialization**: Rezultaty są mapowane na obiekty .NET

```csharp
// 1. Query Creation
IQueryable<Product> query = dbContext.Products.Where(p => p.Price > 100);

// 2. Translation (w EF Core)
// SELECT * FROM Products WHERE Price > @p0

// 3. Execution (gdy ToList/FirstOrDefault/etc)
var results = query.ToList();
```

## Best Practices

1. **Cache compiled expressions**: Kompilacja jest kosztowna
2. **Używaj ExpressionVisitor**: Dla traversowania i modyfikacji
3. **Waliduj input**: Przed budowaniem dynamicznych expressions
4. **Consider alternatives**: Czasem proste delegates wystarczą
5. **Test thoroughly**: Expression trees mogą być skomplikowane

## Performance Tips

```csharp
// ❌ Wolne - kompilacja przy każdym wywołaniu
public bool Filter(Product p)
{
    var expr = BuildExpression();
    return expr.Compile()(p);
}

// ✅ Szybkie - cache skompilowanego wyrażenia
private static Func<Product, bool> _cachedFunc;
public bool Filter(Product p)
{
    _cachedFunc ??= BuildExpression().Compile();
    return _cachedFunc(p);
}
```

## Zaawansowane techniki

- **PredicateBuilder**: Łączenie wielu expressions
- **Expression.Invoke**: Wywoływanie innych expressions
- **ParameterReplacer**: Zamiana parametrów w expressions
- **Custom ExpressionVisitor**: Dla specyficznych transformacji

## Entity Framework i Expression Trees

Entity Framework używa expression trees do translacji LINQ na SQL:
- `IQueryable<T>` przechowuje expression tree
- Provider analizuje expressions
- Generuje zoptymalizowane SQL
- Tylko przefiltrowane dane są pobierane z bazy
