# IEnumerable vs IQueryable w Entity Framework

## Problem w BadExample

- **ToList() zbyt wcześnie**: Wymusza załadowanie wszystkich rekordów z bazy do pamięci RAM
- **Filtrowanie w aplikacji**: `Where()` na `IEnumerable<T>` wykonuje się w C#, nie w SQL
- **Brak optymalizacji**: Baza danych nie może użyć indeksów ani zoptymalizować zapytania
- **Zużycie zasobów**: Dla milionów rekordów grozi OutOfMemoryException

## Rozwiązanie w GoodExample

- **IQueryable<T>**: Reprezentuje zapytanie, które NIE zostało jeszcze wykonane
- **Expression Trees**: LINQ na IQueryable buduje drzewo wyrażeń, które EF tłumaczy na SQL
- **Lazy Evaluation**: Zapytanie wykonuje się dopiero przy `.ToList()`, `.FirstOrDefault()`, itp.
- **Filtrowanie w SQL**: Klauzula WHERE jest częścią zapytania SQL

## Kluczowe różnice

| Aspekt | IEnumerable<T> | IQueryable<T> |
|--------|----------------|---------------|
| Wykonanie | In-memory (C#) | Na bazie danych (SQL) |
| Optymalizacja | Brak | Wykorzystuje indeksy DB |
| Transferu danych | Wszystkie rekordy | Tylko przefiltrowane |
| Wydajność | Niska dla dużych zbiorów | Wysoka |

## Kiedy używać IQueryable

- Pracujesz z Entity Framework / Dapper
- Zbiór danych może być duży
- Chcesz paginacji, filtrowania, sortowania na DB

## Przykład wygenerowanego SQL

**BadExample**:
```sql
SELECT * FROM Products  -- pobiera wszystkie rekordy
```

**GoodExample**:
```sql
SELECT * FROM Products WHERE Price >= @minPrice  -- filtruje na bazie
```

## Zasada złotego środka

Zawsze staraj się pracować na `IQueryable<T>` tak długo, jak to możliwe i wywołuj `.ToList()` / `.FirstOrDefault()` dopiero gdy naprawdę potrzebujesz zmaterializować dane w pamięci aplikacji.
