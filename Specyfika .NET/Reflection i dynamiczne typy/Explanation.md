# Reflection i dynamiczne typy w C# .NET

## Problem w BadExample

### BadExample.cs - Wywołanie metod przez reflection
- **Brak walidacji**: Nie sprawdza czy metoda/property istnieje przed wywołaniem
- **NullReferenceException**: Crash przy próbie wywołania nieistniejącej metody
- **Brak cache**: Każde wywołanie wymaga kosztownego reflection lookup
- **Brak obsługi błędów**: Wyjątki nie są przechwytywane

### BadExample2.cs - Dynamiczne tworzenie typów
- **Brak implementacji metod**: Próba utworzenia proxy bez implementacji interfejsu
- **Memory leak**: Assembly utworzone przez `DefineDynamicAssembly` nie może być unload
- **Brak walidacji**: Nie sprawdza czy parametry są prawidłowe
- **Niepełna funkcjonalność**: Typy bez properties/metod są bezużyteczne

### BadExample3.cs - Ładowanie assembly
- **Brak możliwości unload**: `Assembly.LoadFrom()` ładuje do domyślnego kontekstu
- **Ryzyko bezpieczeństwa**: Wykonuje kod bez sandbox i walidacji
- **Brak walidacji pliku**: Nie sprawdza czy plik istnieje i jest bezpieczny
- **Memory leak**: Wielokrotne ładowanie tego samego assembly

## Rozwiązanie w GoodExample

### GoodExample.cs - Bezpieczne reflection
- **Cache dla MethodInfo/PropertyInfo**: `ConcurrentDictionary` redukuje koszt reflection
- **Try-pattern**: Metody zwracają `bool` i używają `out` parametrów
- **Pełna walidacja**: Sprawdza null, istnienie metod, zgodność parametrów
- **Exception handling**: Wszystkie błędy są przechwytywane i logowane

### GoodExample2.cs - Prawidłowe tworzenie typów
- **Pełna implementacja**: Properties z getterami/setterami, backing fields
- **IL Generation**: Używa `ILGenerator` do generowania kodu IL
- **Walidacja interfejsów**: Sprawdza czy typ jest interfejsem przed utworzeniem proxy
- **Generyczne podejście**: Można tworzyć typy z różnymi properties

### GoodExample3.cs - Bezpieczne ładowanie assembly
- **AssemblyLoadContext**: Umożliwia unload assembly przez `isCollectible: true`
- **Izolacja**: Każdy plugin w osobnym kontekście
- **Cache z WeakReference**: Pozwala GC zbierać nieużywane assembly
- **Pełna walidacja**: Sprawdza istnienie pliku, typu, konstruktora
- **IDisposable pattern**: Automatyczne czyszczenie zasobów

## Kluczowe różnice

| Aspekt | Bad Example | Good Example |
|--------|-------------|--------------|
| Performance | Powolne - bez cache | Szybkie - cache z ConcurrentDictionary |
| Bezpieczeństwo | Brak walidacji | Pełna walidacja i exception handling |
| Memory | Memory leaks | Możliwość unload z AssemblyLoadContext |
| Niezawodność | Częste crashes | Graceful degradation z Try-pattern |

## Kiedy używać Reflection

✅ **Dobre zastosowania:**
- Dependency Injection containers
- ORM (Entity Framework)
- Serializacja/deserializacja (JSON.NET)
- Plugin systems
- Testing frameworks

❌ **Unikaj reflection gdy:**
- Wydajność jest krytyczna (hot path)
- Możesz użyć generics lub interfaces
- Kod jest wykonywany bardzo często

## Performance tips

1. **Cache reflection results**: `MethodInfo`, `PropertyInfo`, `Type`
2. **Używaj `BindingFlags`**: Precyzyjnie określ co szukasz
3. **Rozważ Source Generators**: Dla performance-critical code
4. **Compiled expressions**: Szybsze niż bezpośrednie `MethodInfo.Invoke()`

## AssemblyLoadContext - kluczowe korzyści

```csharp
// Możliwość unload - zwalnia pamięć
var context = new AssemblyLoadContext("Plugin", isCollectible: true);
Assembly assembly = context.LoadFromAssemblyPath(path);
// ... użyj assembly
context.Unload(); // Zwalnia pamięć!
```

## Best Practices

1. **Zawsze waliduj**: Sprawdź czy metoda/property/typ istnieje
2. **Używaj Try-pattern**: `TryGetValue`, `TryInvoke` zamiast throw
3. **Cache rezultaty**: Reflection jest kosztowny
4. **AssemblyLoadContext**: Dla plugin systems - umożliwia unload
5. **Consider alternatives**: Source Generators, Expression Trees mogą być szybsze

## Alternatywy dla Reflection

- **Source Generators**: Compile-time code generation
- **Expression Trees**: Cached compiled delegates
- **Dynamic keyword**: Prostszy syntax, ale nadal używa reflection
- **Interfaces/Generics**: Gdy znasz typ w compile-time
