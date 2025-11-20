# Obsługa wyjątków - StackTrace i dobre praktyki

## Problemy w BadExample

### 1. Exception Swallowing
```csharp
catch (Exception ex) {
    Console.WriteLine("Błąd");
    return string.Empty; // Ukrywamy problem
}
```
- Aplikacja nie wie, że wystąpił błąd
- Trudne debugowanie
- Może prowadzić do corrupted state

### 2. throw ex; - Utrata StackTrace
```csharp
catch (Exception ex) {
    throw ex; // ❌ Resetuje StackTrace do tego miejsca
}
```
**Różnica**:
- `throw ex;` - StackTrace zaczyna się od linii z `throw ex`
- `throw;` - StackTrace zachowuje oryginalne miejsce wystąpienia błędu

### 3. Generyczne wyjątki
```csharp
throw new Exception("Invalid quantity"); // ❌ Zbyt ogólne
```
Lepiej użyć specyficznych wyjątków: `ArgumentException`, `InvalidOperationException`, itp.

### 4. Pusta klauzula catch
```csharp
catch { } // ❌ Najgorsze możliwe rozwiązanie
```
Absolutnie niedopuszczalne - ukrywa wszystkie błędy.

## Rozwiązania w GoodExample

### 1. throw; zachowuje StackTrace
```csharp
catch (FileNotFoundException ex) {
    Log(ex);
    throw; // ✅ Pełny StackTrace
}
```

### 2. Specyficzne wyjątki
```csharp
throw new ArgumentOutOfRangeException(nameof(quantity), "Ilość musi być > 0");
```
Korzyści:
- Jasna komunikacja o typie problemu
- Łatwiejsze łapanie konkretnych przypadków
- Lepsze komunikaty dla użytkownika

### 3. Custom exceptions z InnerException
```csharp
throw new DatabaseException("Nie udało się zapisać", ex);
```
- Zachowuje oryginalny wyjątek w `InnerException`
- Dodaje kontekst biznesowy
- Pełny StackTrace w całym łańcuchu

## Hierarchia wyjątków .NET

```
Exception
├── SystemException
│   ├── ArgumentException
│   │   ├── ArgumentNullException
│   │   └── ArgumentOutOfRangeException
│   ├── InvalidOperationException
│   └── NullReferenceException
└── ApplicationException (rzadko używane)
```

## Kiedy łapać wyjątki?

### TAK - łap gdy:
- Możesz obsłużyć problem i aplikacja może kontynuować
- Dodajesz kontekst biznesowy (custom exception)
- Logujesz dla celów diagnostycznych
- Na granicy aplikacji (API controller, global handler)

### NIE - nie łap gdy:
- Nie możesz nic zrobić z wyjątkiem
- Chcesz tylko zalogować i rzucić dalej (można, ale z `throw;`)
- Jest to nieoczekiwany błąd programistyczny

## Best practices

1. **Używaj throw; zamiast throw ex;**
2. **Łap konkretne wyjątki, nie Exception**
3. **Waliduj argumenty wcześnie (fail-fast)**
4. **Twórz custom exceptions dla logiki biznesowej**
5. **Nigdy nie ukrywaj wyjątków bez logowania**
6. **Używaj ArgumentNullException, ArgumentException dla walidacji parametrów**

## Przykład StackTrace

**throw ex;** (BAD):
```
at BadFileProcessor.ProcessData(String data) line 25  // Zaczyna się tutaj!
at Program.Main() line 10
```

**throw;** (GOOD):
```
at SomeOtherClass.InternalMethod() line 142  // Oryginalne miejsce błędu
at BadFileProcessor.ProcessData(String data) line 25
at Program.Main() line 10
```

## Global Exception Handler (ASP.NET)

```csharp
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        // Loguj exception.Error z pełnym StackTrace
        // Zwróć odpowiedni response do klienta
    });
});
```

Dzięki temu centralizujesz obsługę błędów i zawsze masz dostęp do pełnego StackTrace.
